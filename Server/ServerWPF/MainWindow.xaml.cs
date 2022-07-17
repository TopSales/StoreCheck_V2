using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ZPF.Chat;

namespace _03_ChatServerWPF
{
   /// <summary>
   /// TCP chat server
   /// </summary>
   public partial class MainWindow
   {

      // Write the host messages to the console
      void OnHostMessage(string input)
      {
         PeriodicallyClearScreen();
         AddMessage(input);
      }

      int i = 0;
      void PeriodicallyClearScreen()
      {
         i++;
         if (i > 15)
         {
            ClearMessage();
            AddMessage("Press esc key to stop");
            i = 0;
         }
      }

      /// <summary>
      /// Initialize main window
      /// </summary>
      public MainWindow()
      {
         //DataContext = ServerViewModel.Current;
         InitializeComponent();

         serverIpAddress.Text = MainViewModel.Current.Config.ServerIP;
         serverPortValue.Text = MainViewModel.Current.Config.ServerPort;

         ServerViewModel.Current.chatServer = (ChatServer)ChatServer.Current;
         ServerViewModel.Current.chatServer.OnSystemMessage += ChatServer_OnSystemMessage;
         ServerViewModel.Current.chatServer.OnDataEvent += ServerViewModel.Current.ChatServer_OnDataEvent;
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         btnStartStop_Click(sender, e);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      private void ChatServer_OnSystemMessage(object sender, TcpClient tcpClient, ChatCore.EventType eventType, string message = "")
      {
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
                  btnStartStop.Content = "Start";
                  MessageBox.Show("Server port already in use or the IP Address or server port is invalid!");
               }
               break;

            case ChatCore.EventType.ClientConnected:
               {
                  AddMessage(message); // clientName

                  Dispatcher.Invoke(() =>
                  {
                     listClients.ItemsSource = null;
                     listClients.ItemsSource = ServerViewModel.Current.chatServer.Clients;
                  });
               }
               break;

            case ChatCore.EventType.File:
               {
                  // file
                  AddMessage($"Received file '{message}'");
                  System.Diagnostics.Process.Start(message);
               }
               break;

            case ChatCore.EventType.Data:
               {
                  // Data Message
                  AddMessage(message);
               }
               break;

            case ChatCore.EventType.ClientDisconnected:
               {
                  AddMessage(message);

                  Dispatcher.Invoke(() =>
                  {
                     listClients.ItemsSource = null;
                     listClients.ItemsSource = ServerViewModel.Current.chatServer.Clients;
                  });
               }
               break;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
      /// <summary>
      /// Add messages to messages ListBox
      /// </summary>
      /// <param name="message"></param>
      //private void AddMessage(string message)
      //{
      //   ZPF.AT.Log.Write(new ZPF.AT.AuditTrail
      //   {
      //      IsBusiness = false,
      //      Level = ZPF.AT.ErrorLevel.Info,
      //      Message = message,
      //   });

      //   //ToDo: Dispatcher.Invoke(() => ServerViewModel.Current.AddMessage(message));
      //}

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      /// <summary>
      /// Add messages to messages ListBox
      /// </summary>
      /// <param name="message"></param>
      private void AddMessage(string message)
      {
         Dispatcher.Invoke(() => listChats.Items.Add(message));

         System.Diagnostics.Debug.WriteLine(message);
      }

      private void ClearMessage()
      {
         Dispatcher.Invoke(() => listChats.Items.Clear());
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      /// <summary>
      /// Start server and stop server button functionality
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private async void btnStartStop_Click(object sender, RoutedEventArgs e)
      {
         if ((string)btnStartStop.Content == "Start")
         {
            if (ChatCore.IsValidIpAddress(serverIpAddress.Text) && ChatCore.IsValidPort(serverPortValue.Text))
            {
               btnStartStop.Content = "Stop";

               if (ChatCore.IsValidIpAddress(serverIpAddress.Text) && ChatCore.IsValidPort(serverPortValue.Text))
               {
                  btnStartStop.Content = "Stop";

                  var serverPort = ChatCore.ParseStringToInt(serverPortValue.Text);
                  var serverBufferSize = ChatCore.ParseStringToInt(serverBufferSizeValue.Text);
                  IPAddress.TryParse(serverIpAddress.Text, out var ipAddress);
                  await ServerViewModel.Current.chatServer.Listener(ipAddress, serverPort, serverBufferSize);
               }
               else
               {
                  MessageBox.Show("IP Address or server port is invalid!");
               }
            }
            else
            {
               MessageBox.Show("IP Address or server port is invalid!");
            };
         }
         else
         {
            btnStartStop.Content = "Start";
            StopServer();
         };
      }


      /// <summary>
      /// This method triggers the stop functionality
      /// </summary>
      private async void StopServer()
      {
         await ServerViewModel.Current.chatServer.SendMessageToAllClients("ServerDisconnectSignal");

         //Dispatcher.Invoke(() => listClients.Items.Clear());
         AddMessage("[SERVER]: Server is closed!");

         var serverPort = ChatCore.ParseStringToInt(serverPortValue.Text);
         var serverBufferSize = ChatCore.ParseStringToInt(serverBufferSizeValue.Text);
         IPAddress.TryParse(serverIpAddress.Text, out var ipAddress);
         await ServerViewModel.Current.chatServer.Listener(ipAddress, serverPort, serverBufferSize, true);

         await ServerViewModel.Current.chatServer.StopServer();
      }


      /// <summary>
      /// Method is triggered when user closes the application.
      /// This method will stop the server.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private async void CloseServerConnection(object sender, CancelEventArgs e)
      {
         await ServerViewModel.Current.chatServer.CloseConnection();
      }

      private async void listClients_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         var l = sender as ListBox;

         if (l != null)
         {
            var c = (l.SelectedItem as Client);

            await ServerViewModel.Current.chatServer.SendMessageToClient(c.Tcp, "Beuh?");
         };
      }

      private async void MenuItem_Click(object sender, RoutedEventArgs e)
      {
         OpenFileDialog openFileDialog = new OpenFileDialog();

         if (openFileDialog.ShowDialog() == true)
         {
            var c = (sender as MenuItem).DataContext as Client;

            AddMessage($"[{c.Name}]: Send file '{openFileDialog.FileName}'");

            await ServerViewModel.Current.chatServer.SendFileToClient(c.Tcp, openFileDialog.FileName);
         };
      }
   }
}
