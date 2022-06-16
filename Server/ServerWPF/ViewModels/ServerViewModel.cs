using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
//using System.Windows.Threading;
using ZPF;
using ZPF.AT;
using ZPF.Chat;
using ZPF.SQL;

#if !UT
#endif

public class ServerViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static ServerViewModel _Current = null;

   public static ServerViewModel Current
   {
      get
      {
         if (_Current == null)
         {
            _Current = new ServerViewModel();
         };

         return _Current;
      }

      set
      {
         _Current = value;
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public ServerViewModel()
   {
      _Current = this;

      #region - - - chat server - - -

      ChatCore.DataFolder = System.IO.Path.GetTempPath();

      //chatServer = (ChatServer)ChatServer.Current;
      //chatServer.OnChatEvent += ChatServer_OnChatEvent;

      #endregion
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   //public async Task<bool> ChatServer_OnChatEvent(object sender, TcpClient tcpClient, ChatCore.EventType eventType, string message = "")
   public void OnDataMessage(ChatServer chatServer, ChatData data)
   {
      bool Result = true;

      switch (data.Action.ToLower())
      {
         case "entry":
            {
               var DeviceID = Newtonsoft.Json.JsonConvert.DeserializeObject<String>(data.Data);

               // dc6ac87fb3e9a5c3 -> ME Droid
               // S-1-5-21-1273532890-4014729352-2326173191 -> ME SB3
               // S-1-5-21-952733102-2235713207-1786640839 -> ME SB2

               // cf918dd52379e32a -> ChM PA760 Unitech
               // S-1-5-21-1027867382-3233639938-3375810606 -> ChM SB3

               switch (DeviceID)
               {
                  // - - - ME - - -
                  case "S-1-5-21-952733102-2235713207-1786640839":
                  case "S-1-5-21-1273532890-4014729352-2326173191":
                     DeviceID = "dc6ac87fb3e9a5c3"; break;

                  // - - - ChM - - -
                  case "S-1-5-21-1027867382-3233639938-3375810606":
                     DeviceID = "cf918dd52379e32a"; break;
               };

               var user = DB_SQL.QueryFirst<UserAccount>(MainViewModel.Current.Connection_DB, $"select * from UserAccount where UserAccount.TerminalID='{DeviceID}'");

               if (user != null)
               {
                  user.Password = "";
               };

               chatServer.SendDataToClient("entry", user);
            };
            break;

         case "get_interventions":
            {
               QueryParams qp = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryParams>(data.Data);

               var list = MainViewModel.Current.GetInterventions(qp.Begin, true, qp.FKUser, null, null);

               chatServer.SendDataToClient("get_interventions", list);
            };
            break;

         case "get_stores":
            {
               QueryParams qp = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryParams>(data.Data);

               var list = MainViewModel.Current.GetStores(qp.Begin);

               chatServer.SendDataToClient("get_stores", list);
            };
            break;
      };

      //return Result;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   private async void SendSpoolerToAllClients()
   {
      //var list = DB_SQL.Query<Spooler>("select * from Current order by CreatedOn");

      //await ChatServer.Current.SendDataToAllClients("GetSpooler", list);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public bool IsServerRunning = false;
   public async void StartStop()
   {
      //string ipAddress = MainViewModel.Current.Config.ServerIP;
      //int port = int.Parse(MainViewModel.Current.Config.ServerPort);
      //string clientName = MainViewModel.Current.Config.ClientName;
      //int bufferSize = 1024;

      //// await chatClient.CreateConnection(ipAddress, port, clientName, bufferSize);

      //if (!IsServerRunning)
      //{
      //   if (ChatCore.IsValidIpAddress(ipAddress) && ChatCore.IsValidPort(MainViewModel.Current.Config.ServerPort))
      //   {
      //      try
      //      {
      //         IPAddress.TryParse(ipAddress, out var IP);
      //         await chatServer.Listener(IP, port, bufferSize);

      //         IsServerRunning = true;
      //      }
      //      catch (Exception ex)
      //      {
      //         ZPF.AT.Log.Write(new AuditTrail(ex));

      //         IsServerRunning = false;

      //         StopServer();
      //      };
      //   }
      //   else
      //   {
      //      //MessageBox.Show("IP Address or server port is invalid!");
      //   };
      //}
      //else
      //{
      //   IsServerRunning = false;

      //   StopServer();
      //};
   }

   /// <summary>
   /// This method triggers the stop functionality
   /// </summary>
   private void StopServer()
   {
      //string ipAddress = MainViewModel.Current.Config.ServerIP;
      //int port = int.Parse(MainViewModel.Current.Config.ServerPort);
      //int bufferSize = 1024;

      ////ToDo: AddMessage("[SERVER]: Server is closed!");

      //IPAddress.TryParse(ipAddress, out var IP);
      //await chatServer.Listener(IP, port, bufferSize, true);

      //await chatServer.StopServer();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
