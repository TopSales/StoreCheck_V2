using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Threading;
using ZPF;
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

      Debug.WriteLine("ICI ICI ICI");

      switch (eventType)
      {
         case ChatCore.EventType.Message:
            {
               // Message to operator
               AddMessage(message);
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
               AddMessage($"Received file '{message}'");
               //System.Diagnostics.Process.Start(message);
            }
            break;

         case ChatCore.EventType.Data:
            {
               // Data Message
               var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatData>(message.TrimEnd(new char[] { '~' }));

               switch (data.Action.ToLower())
               {
                  case "entry":
                     {
                        var DeviceID = Newtonsoft.Json.JsonConvert.DeserializeObject<String>(data.Data);

                        long nb = 0;
                        //var nb = DB_SQL.QuickQueryInt64("select max(CAST(beeper as INT)) from Current") + 1;

                        if (nb == 0)
                        {
                           await ChatServer.Current.SendDataToClient(tcpClient, "entry", null);
                        }
                        else
                        {
                           await ChatServer.Current.SendDataToClient(tcpClient, "entry", "*");
                        };
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

                  case "getspooler":
                     {
                        //var list = DB_SQL.Query<Spooler>("select * from Current order by CreatedOn");

                        //await ChatServer.Current.SendDataToClient(tcpClient, "GetSpooler", list);
                     };
                     break;

                  case "clearspooler":
                     {
                        //DB_SQL.QuickQuery("delete from Spooler");
                        //DB_SQL.Query<Spooler>("delete from Current");

                        //SendSpoolerToAllClients();
                     };
                     break;

                  case "getstats":
                     {
                        //                        BeginEnd be = Newtonsoft.Json.JsonConvert.DeserializeObject<BeginEnd>(data.Data);

                        //                        //ToDo GetStats DateTime begin, DateTime end

                        //                        //string SQL = $@"select * from Stats where CreatedOn>={DB_SQL.DateTimeToSQL(be.Begin)} and CreatedOn<={DB_SQL.DateTimeToSQL(be.End)}";
                        //                        //var list = DB_SQL.Query<Spooler>(SQL);

                        //                        var SQL = $@"
                        //select 
                        //   *, 
                        //   json_extract(Description, '$.Company') as Company,
                        //   json_extract(Description, '$.Carrier') as Carrier, 
                        //   json_extract(Description, '$.TypeOfMerchandise') as TypeOfMerchandise 
                        //from Stats
                        //where json_valid(Description) and CreatedOn>={DB_SQL.DateTimeToSQL(be.Begin)} and CreatedOn<={DB_SQL.DateTimeToSQL(be.End)}
                        //order by FirstCall
                        //";

                        //                        var list = DB_SQL.QuickQueryView( SQL) as DataTable;

                        //                        await ChatServer.Current.SendDataToClient(tcpClient, "getstats", list);
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
            IPAddress.TryParse(ipAddress, out var IP);
            await chatServer.Listener(IP, port, bufferSize);

            IsServerRunning = true;
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

      AddMessage("[SERVER]: Server is closed!");

      IPAddress.TryParse(ipAddress, out var IP);
      await chatServer.Listener(IP, port, bufferSize, true);

      await chatServer.StopServer();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   public List<NameValue> Log { get; set; } = new List<NameValue>();

   /// <summary>
   /// Add messages to messages ListBox
   /// </summary>
   /// <param name="message"></param>
   public void AddMessage(string message)
   {
      Debug.WriteLine(message);
      Log.Insert(0, new NameValue { Value = message });
      //Dispatcher.Invoke(() => listChats.Items.Add(message));
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
