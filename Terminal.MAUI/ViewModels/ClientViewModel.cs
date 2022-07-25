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

      #region - - - chat client - - -

      //ChatCore.DataFolder = System.IO.Path.GetTempPath();

      //chatClient = new ChatClient();
      //chatClient.OnSystemMessage += ChatClient_OnSystemMessage1;
      //chatClient.OnDataEvent += ChatClient_OnDataEvent;

      //Connect();

      #endregion
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   //ChatClient chatClient = null;

   // Write the host messages to the console
   private void ChatClient_OnSystemMessage1(object sender, System.Net.Sockets.TcpClient tcpClient, ChatCore.EventType eventType, string message = "")
   {
      PeriodicallyClearScreen();
      AddMessage(message);
   }

   private void ChatClient_OnDataEvent(object sender, System.Net.Sockets.TcpClient tcpClient, ChatData data)
   {
      PeriodicallyClearScreen();
      //AddMessage($"{data.Action} [{data.Data}]");

      OnDataMessage(null, data);
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

   public async void OnDataMessage(object chatClient, ChatData data)
   {
      switch (data.Action.ToLower())
      {
         case "entry":
            {
               if (data.Data == null)
               {
                  MainViewModel.Current.EntryMsg = $"Call your admin\n\n[{MainViewModel.Current.DeviceID}]";
               }
               else
               {
                  var u = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(data.Data);

                  if (u != null)
                  {
                     AuditTrailViewModel.Current.TerminalID = u.TerminalID;
                     AuditTrailViewModel.Current.FKUser = u.PK.ToString();

                     MainViewModel.Current.Config.FKUser = u.PK;
                     MainViewModel.Current.Config.Login = u.Login;
                     MainViewModel.Current.Save();

                     MainViewModel.Current.EntryMsg = $"Hello Mr '{u.Login}' ...";

                     //GC.Collect();
                     //await chatClient.SendDataToServer("get_interventions", new QueryParams { FKUser = MainViewModel.Current.Config.FKUser, Begin = MainViewModel.Current.Config.LastSync });
                     //GC.Collect();
                     //await chatClient.SendDataToServer("get_stores", new QueryParams { FKUser = MainViewModel.Current.Config.FKUser, Begin = MainViewModel.Current.Config.LastSync });
                     //GC.Collect();

                     //chatClient.SendDataToServer("get_interventions", new QueryParams { FKUser = MainViewModel.Current.Config.FKUser, Begin = MainViewModel.Current.Config.LastSync });
                     //chatClient.SendDataToServer("get_stores", new QueryParams { FKUser = MainViewModel.Current.Config.FKUser, Begin = MainViewModel.Current.Config.LastSync });
                  };
               };
            };
            break;

         case "call":
            {
               //var s = Newtonsoft.Json.JsonConvert.DeserializeObject<Spooler>(data.Data);

               //if (s.Client == MainViewModel.Current.Config.ClientName)
               //{
               //   string buffer = "";

               //   // - - - beeper - - -
               //   if (false)
               //   {
               //      buffer = (char)0x01 + s.Beeper + (char)0x03;
               //      DependencyService.Get<ISyscallHelper>().Write2SerialPort(byte.Parse(MainViewModel.Current.Config.ComPort), buffer);
               //   };

               //   // - - - pager - - -
               //   if (true)
               //   {
               //      string beeper = s.Beeper;
               //      string gate = s.Gate.ToString();
               //      string text = "Gate";

               //      buffer = beeper.PadLeft(4, '0'); // beeper
               //      buffer += gate.PadLeft(3, '0'); // gate
               //      buffer += text.ToUpper().PadLeft(10, ' '); // text

               //      buffer = (char)0x01 + buffer + (char)0x03;

               //      // fucking ugly ... but, it works ...
               //      DependencyService.Get<ISyscallHelper>().Write2SerialPort(byte.Parse(MainViewModel.Current.Config.ComPort), buffer);
               //      DependencyService.Get<ISyscallHelper>().Write2SerialPort(byte.Parse(MainViewModel.Current.Config.ComPort), buffer);
               //   };
               //};
            };
            break;

         case "get_interventions":
            {
               MainViewModel.Current.SetInterventions(ZIPHelper.Unzip(Convert.FromBase64String(data.Data)));
               MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
            };
            break;

         case "get_stores":
            {
               MainViewModel.Current.SetStores(ZIPHelper.Unzip(Convert.FromBase64String(data.Data)));
               MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Stores);
            };
            break;
      };

   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
   public async Task<string> SendDataToServer(string action, object data)
   {
      if (Connectivity.Current.NetworkAccess == Microsoft.Maui.Networking.NetworkAccess.Internet)
      {
         Log.Write("Chat", "SendDataToServer: " + action);

         try
         {
            var chatData = ChatCore.SerializeData(data);
            chatData.Action = action;

            Uri uri = wsHelper.CalcURI($@"/SendDataToServer/");
            var json = await wsHelper.wPost_Stream(uri, chatData);

            //string message = chatData.Serialize() + EndOfFrame;

            //var clientMessageByteArray = Encoding.Unicode.GetBytes(message);
            //await _networkStream.WriteAsync(clientMessageByteArray, 0, clientMessageByteArray.Length);

            //return json;                                                                                   
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

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   public async void Entry(string deviceID)
   {
      if (Connectivity.Current.NetworkAccess == Microsoft.Maui.Networking.NetworkAccess.Internet)
      {
         //var json = await wsHelper.wGet(string.Format("/User/Login/{0}/{1}", username.Text, UserViewModel.Current.Salt(username.Text, password.Text)));
         //var json = await wsHelper.wPost_String($@"/User/Login/{WebUtility.UrlEncode(username.Text)}/{WebUtility.UrlEncode(username.Text)}", UserViewModel.Current.Salt(username.Text, password.Text));

         string json =  await SendDataToServer("entry", deviceID);

         int PK = -1;
         //try
         //{
         //   PK = int.Parse(json);
         //}
         //catch { };

         //if (PK > 0)
         //{
         //   //DisplayAlert("PK", PK.ToString(), "ok");

         //   // - - - ? erase old data - - - 

         //   if (MainViewModel.Current.Config.Login != username.Text)
         //   {
         //      MainViewModel.Current.Config.LastSynchro = DateTime.MinValue;

         //      MainViewModel.Current.Interventions.Clear();
         //      MainViewModel.Current.Documents.Clear();

         //      // - - - clean photo folder - - -

         //      var folder = ZPF.XF.Basics.Current.FileIO.CleanPath(MainViewModel.Current.DataFolder + @"/Photos/");

         //      if (!System.IO.Directory.Exists(folder))
         //      {
         //         System.IO.Directory.CreateDirectory(folder);
         //      };

         //      var files = System.IO.Directory.GetFiles(folder);

         //      foreach (var file in files)
         //      {
         //         try
         //         {
         //            System.IO.File.Delete(file);
         //         }
         //         catch { };
         //      };
         //   };

         //   // - - - remember login status - - - 
         //   MainViewModel.Current.Config.IsLogged = true;
         //   MainViewModel.Current.Config.Login = username.Text;
         //   MainViewModel.Current.Config.UserFK = PK;
         //   MainViewModel.Current.Save();

         //   MainViewModel.Current.Download(username.Text);
         //   BackboneViewModel.Current.DecBusy();

         //   await Navigation.PopModalAsync();
         //}
         //else
         //{
         //   DoIt.OnMainThread(() =>
         //   {
         //      BackboneViewModel.Current.DecBusy();

         //      parent.DisplayAlert("Validation Error", "Wrong Username and/or Password", "Re-try");
         //   });
         //};
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
