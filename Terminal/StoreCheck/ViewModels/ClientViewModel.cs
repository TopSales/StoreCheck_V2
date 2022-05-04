using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZPF;
using ZPF.Chat;

#if !UT
#endif

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

      chatClient = new ChatClient();
      chatClient.OnChatEvent += ChatClient_OnChatEvent;

      #endregion
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -


   public ChatClient chatClient { get; }
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
               var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatData>(message.TrimEnd(new char[] { '~' }));

               switch (data.Action.ToLower())
               {
                  case "new":
                     {
                        //var s = Newtonsoft.Json.JsonConvert.DeserializeObject<Spooler>(data.Data);

                        //if (s.Client == MainViewModel.Current.Config.ClientName)
                        //{
                        //   MainViewModel.Current.LastBeeper = s.Beeper;
                        //};
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

                  case "getspooler":
                     {
                        //MainViewModel.Current.SetSpoolerList(data.Data);
                     };
                     break;

                  case "getstats":
                     {
                        //MainViewModel.Current.SetStats(data.Data);
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

               //System.Diagnostics.Debug.WriteLine("Chat(M): " + message);
            }
            break;
      };

      return Result;
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

   public async void GetStats()
   {
      await chatClient.SendDataToServer("GetStats", new BeginEnd());
   }

   public async void GetStats(DateTime begin, DateTime end)
   {
      await chatClient.SendDataToServer("GetStats", new BeginEnd { Begin = begin, End = end });
   }

   public async void ClearSpooler()
   {
      await chatClient.SendDataToServer("ClearSpooler", "");
   }

   public async void GetSpooler()
   {
      // // Get(@"/Spooler/GetCurrent");
      await chatClient.SendDataToServer("GetSpooler", "");
   }

   public async void SendMessage(string msg)
   {
      await chatClient.SendMessageToServer(msg);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public async void Connect()
   {
      string ipAddress = MainViewModel.Current.Config.ServerIP;
      int port = int.Parse(MainViewModel.Current.Config.ServerPort);
      string clientName = MainViewModel.Current.Config.ClientName;
      int bufferSize = 1024;

      await chatClient.CreateConnection(ipAddress, port, clientName, bufferSize);
   }

   public bool IsConnected()
   {
      return chatClient.IsConnected();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
