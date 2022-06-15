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

      ChatCore.DataFolder = System.IO.Path.GetTempPath();

      Connect();
      //chatClient = new ChatClient();
      //chatClient.OnChatEvent += ChatClient_OnChatEvent;

      #endregion
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   ChatClient chatClient = null;

   // Write the host messages to the console
   void OnHostMessage(string input)
   {
      PeriodicallyClearScreen();
      AddMessage(input);
   }

   // Write the host messages to the console
   void OnDataMessage(ChatData data)
   {
      PeriodicallyClearScreen();
      AddMessage($"{data.Action} [{data.Data}]");
   }

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

   private async Task<bool> ChatClient_OnChatEvent(object sender, ChatCore.EventType eventType, string message = "")
   {
      bool Result = true;

      switch (eventType)
      {
         case ChatCore.EventType.ConnectionFailed:
            {
               //AddMessage("[CLIENT]: ❌ Could not create a connection with the server.");
            }
            break;

         case ChatCore.EventType.SendConnectionDataFailed:
            {
               //MessageBox.Show("❌ Can't connect to the server, try again later!", "Client");
            }
            break;

         case ChatCore.EventType.IOException:
            {
               //AddMessage("[SERVER]: ❌ Connection with server is lost!");
            }
            break;

         case ChatCore.EventType.ClientDisconnected:
            {
               //AddMessage("[CLIENT]: ❌ Disconnected!");
            }
            break;

         case ChatCore.EventType.ServerDisconnect:
            {
            }
            break;

         case ChatCore.EventType.Data:
            {
               // Data Message
               ChatData data = ChatCore.DeserializeChatData(message);

               if (data == null)
               {
                  return false;
               };

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

                              chatClient.SendDataToServer("get_interventions", new QueryParams { FKUser = MainViewModel.Current.Config.FKUser, Begin = MainViewModel.Current.Config.LastSync });
                              chatClient.SendDataToServer("get_stores", new QueryParams { FKUser = MainViewModel.Current.Config.FKUser, Begin = MainViewModel.Current.Config.LastSync });
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
                        MainViewModel.Current.SetInterventions(data.Data);
                        MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
                     };
                     break;

                  case "get_stores":
                     {
                        MainViewModel.Current.SetStores(data.Data);
                        MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Stores);
                     };
                     break;
               };

               // private async Task<bool> SetSpoolerList(string json)
               //ToDo: SetStats()
               // System.Diagnostics.Debug.WriteLine("Chat(D): " + message);
            }
            break;

         case ChatCore.EventType.Message:
            {
               // Message
               //MainViewModel.Current.CurrentMessage = message;
               //MainViewModel.Current.Save();

               System.Diagnostics.Debug.WriteLine("Chat(M): " + message);
            }
            break;
      };

      return Result;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   public async void Entry(string deviceID)
   {
      ClientViewModel.Current.Connect();

      chatClient.SendDataToServer("entry", deviceID);
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

   public void GetStats()
   {
      chatClient.SendDataToServer("GetStats", new QueryParams());
   }

   public void GetStats(DateTime begin, DateTime end)
   {
      chatClient.SendDataToServer("GetStats", new QueryParams { Begin = begin, End = end });
   }

   public void ClearSpooler()
   {
      chatClient.SendDataToServer("ClearSpooler", "");
   }

   public void GetSpooler()
   {
      // // Get(@"/Spooler/GetCurrent");
      chatClient.SendDataToServer("GetSpooler", "");
   }

   public void SendMessage(string msg)
   {
      chatClient.SendMessageToServer(msg);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public void Connect()
   {
      string ipAddress = MainViewModel.Current.Config.ServerIP;
      int port = int.Parse(MainViewModel.Current.Config.ServerPort);
      string clientName = MainViewModel.Current.Config.ClientName;
      //int bufferSize = 1024;

      //await chatClient.CreateConnection(ipAddress, port, clientName, bufferSize);
      if (chatClient == null)
      {
         chatClient = new ZPF.Chat.ChatClient(OnHostMessage, OnDataMessage, ipAddress, port);
         chatClient.StartClient();
      };
   }

   public bool IsConnected()
   {
      return chatClient.IsConnected();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
