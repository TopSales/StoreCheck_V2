using System.Diagnostics;
using System.Text;
using ZPF;
using ZPF.AT;
using ZPF.Chat;

public class ClientViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static ClientViewModel _Current = null;

   public static ClientViewModel Current
   {
      get
      {
         if (_Current == null)
         {
            _Current = new ClientViewModel();
         };

         return _Current;
      }

      set
      {
         _Current = value;
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public ClientViewModel()
   {
      _Current = this;

      //ChatCore.DataFolder = System.IO.Path.GetTempPath();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   int i = 0;
   void PeriodicallyClearScreen()
   {
      i++;
      if (i > 15)
      {
         ClearMessage();
         //AddMessage("Press esc key to stop");
         i = 0;
      }
   }

   /// <summary>
   /// Add messages to messages ListBox
   /// </summary>
   /// <param name="message"></param>
   private void AddMessage(string message)
   {
      //Dispatcher.Invoke(() => listChats.Items.Add(message));

      System.Diagnostics.Debug.WriteLine(message);
   }

   private void ClearMessage()
   {
      //Dispatcher.Invoke(() => listChats.Items.Clear());
   }

   public INavigation Navigation { get; internal set; }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   public async void OnChatData(ChatData data)
   {
      switch (data.Action.ToLower())
      {
         case "get_interventions":
            {
               MainViewModel.Current.SetInterventions(data.GetData());
               MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
               MainViewModel.Current.EntryMsg = $"Sync Interventions ({MainViewModel.Current.Interventions.Count}) OK ...";
            };
            break;

         case "get_stores":
            {
               MainViewModel.Current.SetStores(data.GetData());
               MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Stores);
               MainViewModel.Current.EntryMsg = $"Sync Stores ({MainViewModel.Current.Stores.Count}) OK ...";
            };
            break;
      };

   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
   public async Task<string> SendDataToServer(string action, object data)
   {
      if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
      {
         Log.Write("Chat", "SendDataToServer: " + action);

         try
         {
            var chatData = ChatCore.SerializeData(data);
            chatData.Action = action;

            var json = await wPost_ChatData(@"/SendDataToServer", chatData);

            return json;
         }
         catch (Exception ex)
         {
            Log.Write(new AuditTrail(ex));
         };

         return null;
      }
      else
      {
         return null;
      };
   }


   public static async Task<string> wPost_ChatData(string Function, ChatData chatData, string basicAuth = "")
   {
      wsHelper.LastError = "";
      wsHelper.LastData = "";

      Uri uri = wsHelper.CalcURI(Function);

      if (uri == null)
      {
         return null;
      };

      try
      {
#if __WASM__
#else
         if (!string.IsNullOrEmpty(basicAuth))
         {
            byte[] byteArray = Encoding.ASCII.GetBytes(basicAuth);
            wsHelper._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
         };
#endif

         var json = ChatData.Serialize(chatData);
         var content = new StringContent(json, Encoding.UTF8, "application/json");
         var result = await wsHelper._httpClient.PutAsync(uri, content); //or PostAsync for POST

         result.EnsureSuccessStatusCode();

         wsHelper.LastData = await result.Content.ReadAsStringAsync();
         return wsHelper.LastData;
      }
      catch (Exception ex)
      {
         Log.Write(new AuditTrail(ex)
         {
            Application = "wsHelper.wPost_ChatData",
            //DataOut = json,
         });

         wsHelper.LastError = ex.Message;
         Debug.WriteLine(ex.Message);

         return null;
      };
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   public async void Entry(string deviceID)
   {
      if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
      {
         string json = await SendDataToServer("entry", deviceID);

         if (!string.IsNullOrEmpty(json))
         {
            var chatData = ChatCore.DeserializeChatData(json);

            if (chatData != null && chatData.Action == "entry")
            {
               User_CE user = Newtonsoft.Json.JsonConvert.DeserializeObject<User_CE>(chatData.GetData());

               if (user != null)
               {
                  // - - - ? erase old data - - -              

                  if (MainViewModel.Current.Config.Login != user.Login)
                  {
                     MainViewModel.Current.Config.LastSynchro = DateTime.MinValue;

                     MainViewModel.Current.Interventions.Clear();
                     MainViewModel.Current.Documents.Clear();

                     // - - - clean photo folder - - -

                     var folder = ZPF.XF.FileIO.CleanPath(MainViewModel.Current.DataFolder + @"/Photos/");

                     if (!System.IO.Directory.Exists(folder))
                     {
                        System.IO.Directory.CreateDirectory(folder);
                     };

                     var files = System.IO.Directory.GetFiles(folder);

                     foreach (var file in files)
                     {
                        try
                        {
                           System.IO.File.Delete(file);
                        }
                        catch { };
                     };
                  };

                  // - - -  - - - 

                  MainViewModel.Current.EntryMsg = "ID checked ...";

                  MainViewModel.Current.Config.IsLogged = true;
                  MainViewModel.Current.Config.FKUser = user.PK;
                  MainViewModel.Current.Config.Login = user.Login;
                  MainViewModel.Current.Save();

                  MainViewModel.Current.EntryMsg = $"Hello '{user.Login}'\nsyncing datas ...";

                  // - - -  - - - 

                  var queryParams = new QueryParams() { FKUser = MainViewModel.Current.Config.FKUser };

                  {
                     json = await SendDataToServer("get_interventions", queryParams);

                     if (!string.IsNullOrEmpty(json))
                     {
                        chatData = ChatCore.DeserializeChatData(json);
                        this.OnChatData(chatData);
                     };
                  }

                  {
                     json = await SendDataToServer("get_stores", queryParams);

                     if (!string.IsNullOrEmpty(json))
                     {
                        chatData = ChatCore.DeserializeChatData(json);
                        this.OnChatData(chatData);
                     };
                  };
               }
               else
               {
                  MainViewModel.Current.EntryMsg = "ID couldn't be verified ...";
               };
            };
         };
      }
      else
      {
         //return null;
      };
   }


   //public async void NewDevice(Spooler spooler)
   //{
   //   // wPost(@"/Spooler/Add", s);
   //   await chatClient.SendDataToServer("new", spooler);
   //}

   //public async void CallDevice(Spooler spooler)
   //{
   //   await chatClient.SendDataToServer("call", spooler);
   //}

   //public async void RemoveDevice(Spooler spooler)
   //{
   //   // string json = await wGet($@"/Spooler/Remove/{s.Beeper}");
   //   await chatClient.SendDataToServer("remove", spooler);
   //}

   //public async void GetStats()
   //{
   //   await chatClient.SendDataToServer("GetStats", new QueryParams());
   //}

   //public async void GetStats(DateTime begin, DateTime end)
   //{
   //   await chatClient.SendDataToServer("GetStats", new QueryParams { Begin = begin, End = end });
   //}

   //public async void ClearSpooler()
   //{
   //   await chatClient.SendDataToServer("ClearSpooler", "");
   //}

   //public async void GetSpooler()
   //{
   //   // // Get(@"/Spooler/GetCurrent");
   //   await chatClient.SendDataToServer("GetSpooler", "");
   //}

   //public async void SendMessage(string msg)
   //{
   //   await chatClient.SendMessageToServer(msg);
   //}

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //public async void Connect()
   //{
   //   string ipAddress = MainViewModel.Current.Config.ServerIP;
   //   int port = int.Parse(MainViewModel.Current.Config.ServerPort);
   //   string clientName = MainViewModel.Current.Config.ClientName;
   //   int bufferSize = 1024;

   //   await chatClient.CreateConnection(ipAddress, port, clientName, bufferSize);

   //   //if (chatClient == null)
   //   //{
   //   //   chatClient = new ZPF.Chat.ChatClient(OnHostMessage, OnDataMessage, ipAddress, port);
   //   //   chatClient.StartClient();
   //   //};
   //}

   //public bool IsConnected()
   //{
   //   return chatClient.IsConnected();
   //}

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
