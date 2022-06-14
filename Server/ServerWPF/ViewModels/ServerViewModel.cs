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

   public ChatServer chatServer { get; }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   public async Task<bool> ChatServer_OnChatEvent(object sender, TcpClient tcpClient, ChatCore.EventType eventType, string message = "")
   {
      bool Result = true;

      switch (eventType)
      {
         case ChatCore.EventType.Message:
            {
               // Message to operator
               //ToDo: AddMessage(message);
            }
            break;

         case ChatCore.EventType.SocketException:
            {
               //btnStartStop.Content = "Start";
               //MessageBox.Show("Server port already in use or the IP Address or server port is invalid!");
            }
            break;

         case ChatCore.EventType.ClientConnected:
            {
               //await ChatServer.Current.SendMessageToClient(tcpClient, MainViewModel.Current.Config.CurrentMessage);

               //var list = DB_SQL.Query<Spooler>("select * from Current order by CreatedOn");
               //await ChatServer.Current.SendDataToClient(tcpClient, "GetSpooler", list);
            }
            break;

         case ChatCore.EventType.File:
            {
               // file
               //ToDo: AddMessage($"Received file '{message}'");
               //System.Diagnostics.Process.Start(message);
            }
            break;

         case ChatCore.EventType.Data:
            {
               ChatData data = null;

               // Data Message
               try
               {
                  data = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatData>(message.TrimEnd(new char[] { '~' }));
               }
               catch (Exception ex)
               {
                  Log.Write(new AuditTrail(ex) { DataInType="JSON", DataIn=message.Left(1024) });

                  return false;
               };

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

                        await ChatServer.Current.SendDataToClient(tcpClient, "entry", user);
                     };
                     break;

                  case "call":
                     {
                        //Spooler current = Newtonsoft.Json.JsonConvert.DeserializeObject<Spooler>(data.Data);

                        //// - - -  - - - 

                        //if (current.FirstCall == DateTime.MinValue)
                        //{
                        //   current.FirstCall = DateTime.Now;
                        //};

                        //current.LastCall = DateTime.Now;

                        //// - - -  - - - 

                        //if (MainViewModel.Current.Config.AutoDeleteAfterCall)
                        //{
                        //   DB_SQL.QuickQuery($"delete from Current where Beeper = '{current.Beeper}'");
                        //   DB_SQL.Insert("Spooler", current);
                        //   DB_SQL.Insert("Stats", current);
                        //}
                        //else
                        //{
                        //   //DB_SQL.Update(current);
                        //   DB_SQL.QuickQuery($"update Current set FirstCall={ DB_SQL.DateTimeToSQL(current.FirstCall) } where Client='{current.Client}' and Beeper = '{current.Beeper}'");
                        //   DB_SQL.QuickQuery($"update Current set LastCall={ DB_SQL.DateTimeToSQL(current.LastCall) } where Client='{current.Client}' and Beeper = '{current.Beeper}'");
                        //   DB_SQL.QuickQuery($"update Current set Gate={current.Gate} where Client='{current.Client}' and Beeper = '{current.Beeper}'");
                        //};

                        //// - - -  - - -    

                        //await ChatServer.Current.SendDataToAllClients("call", current);

                        //SendSpoolerToAllClients();
                     };
                     break;

                  case "remove":
                     {
                        //Spooler current = Newtonsoft.Json.JsonConvert.DeserializeObject<Spooler>(data.Data);

                        //// - - -  - - - 

                        //current.DeletedOn = DateTime.Now;

                        //// - - -  - - - 

                        //DB_SQL.Insert("Spooler", current);
                        //DB_SQL.Insert("Stats", current);

                        //DB_SQL.Query<Spooler>($"delete from Current where Beeper='{current.Beeper}'");

                        //SendSpoolerToAllClients();
                     };
                     break;

                  case "get_interventions":
                     {
                        QueryParams qp = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryParams>(data.Data);

                        var list = MainViewModel.Current.GetInterventions(qp.Begin, true, qp.FKUser, null, null);

                        await ChatServer.Current.SendDataToClient(tcpClient, "get_interventions", list);
                     };
                     break;

                  case "get_stores":
                     {
                        QueryParams qp = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryParams>(data.Data);

                        var list = MainViewModel.Current.GetStores(qp.Begin);

                        await ChatServer.Current.SendDataToClient(tcpClient, "get_stores", list);
                     };
                     break;

                  case "clearspooler":
                     {
                        //DB_SQL.QuickQuery("delete from Spooler");
                        //DB_SQL.Query<Spooler>("delete from Current");

                        //SendSpoolerToAllClients();
                     };
                     break;

               };

               //AddMessage(message);
            }
            break;

         case ChatCore.EventType.ClientDisconnected:
            {
               //AddMessage(message);
               ////ToDo: Dispatcher.Invoke(() => listClients.Items.RemoveAt(_clientList.IndexOf(tcpClient)));

               //Dispatcher.Invoke(() =>
               //{
               //   listClients.ItemsSource = null;
               //   listClients.ItemsSource = ChatServer.Current.Clients;
               //});
            }
            break;
      };

      return Result;
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
      string ipAddress = MainViewModel.Current.Config.ServerIP;
      int port = int.Parse(MainViewModel.Current.Config.ServerPort);
      string clientName = MainViewModel.Current.Config.ClientName;
      int bufferSize = 1024;

      // await chatClient.CreateConnection(ipAddress, port, clientName, bufferSize);

      if (!IsServerRunning)
      {
         if (ChatCore.IsValidIpAddress(ipAddress) && ChatCore.IsValidPort(MainViewModel.Current.Config.ServerPort))
         {
            try
            {
               IPAddress.TryParse(ipAddress, out var IP);
               await chatServer.Listener(IP, port, bufferSize);

               IsServerRunning = true;
            }
            catch (Exception ex)
            {
               ZPF.AT.Log.Write(new AuditTrail(ex));

               IsServerRunning = false;

               StopServer();
            };
         }
         else
         {
            //MessageBox.Show("IP Address or server port is invalid!");
         };
      }
      else
      {
         IsServerRunning = false;

         StopServer();
      };
   }

   /// <summary>
   /// This method triggers the stop functionality
   /// </summary>
   private async void StopServer()
   {
      string ipAddress = MainViewModel.Current.Config.ServerIP;
      int port = int.Parse(MainViewModel.Current.Config.ServerPort);
      int bufferSize = 1024;

      //ToDo: AddMessage("[SERVER]: Server is closed!");

      IPAddress.TryParse(ipAddress, out var IP);
      await chatServer.Listener(IP, port, bufferSize, true);

      await chatServer.StopServer();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
