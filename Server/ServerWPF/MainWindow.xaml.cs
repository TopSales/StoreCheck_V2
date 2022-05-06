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
      ChatServer chatServer = null;

      /// <summary>
      /// Initialize main window
      /// </summary>
      public MainWindow()
      {
         DataContext = ChatServer.Current;
         InitializeComponent();

         serverIpAddress.Text = MainViewModel.Current.Config.ServerIP;
         serverPortValue.Text = MainViewModel.Current.Config.ServerPort;

         chatServer = (ChatServer)ChatServer.Current;
         chatServer.OnChatEvent += ChatServer_OnChatEvent;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      private async Task<bool> ChatServer_OnChatEvent(object sender, TcpClient tcpClient, ChatCore.EventType eventType, string message = "")
      {
         bool Result = true;

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
                     listClients.ItemsSource = ChatServer.Current.Clients;
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
                     listClients.ItemsSource = ChatServer.Current.Clients;
                  });
               }
               break;
         };

         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      /// <summary>
      /// Add messages to messages ListBox
      /// </summary>
      /// <param name="message"></param>
      private void AddMessage(string message)
      {
         Dispatcher.Invoke(() => listChats.Items.Add(message));
      }

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

               var serverPort = ChatCore.ParseStringToInt(serverPortValue.Text);
               var serverBufferSize = ChatCore.ParseStringToInt(serverBufferSizeValue.Text);
               IPAddress.TryParse(serverIpAddress.Text, out var ipAddress);
               await chatServer.Listener(ipAddress, serverPort, serverBufferSize);
            }
            else
            {
               MessageBox.Show("IP Address or server port is invalid!");
            }
         }
         else
         {
            btnStartStop.Content = "Start";
            StopServer();
         }
      }


      /// <summary>
      /// This method triggers the stop functionality
      /// </summary>
      private async void StopServer()
      {
         await chatServer.SendStatusToAllClients(ChatCore.ServerDisconnectSignal);

         //Dispatcher.Invoke(() => listClients.Items.Clear());
         AddMessage("[SERVER]: Server is closed!");

         var serverPort = ChatCore.ParseStringToInt(serverPortValue.Text);
         var serverBufferSize = ChatCore.ParseStringToInt(serverBufferSizeValue.Text);
         IPAddress.TryParse(serverIpAddress.Text, out var ipAddress);
         await chatServer.Listener(ipAddress, serverPort, serverBufferSize, true);

         await chatServer.StopServer();
      }


      /// <summary>
      /// Method is triggered when user closes the application.
      /// This method will stop the server.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private async void CloseServerConnection(object sender, CancelEventArgs e)
      {
         await chatServer.CloseConnection();
      }

      private async void listClients_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         var l = sender as ListBox;

         if (l != null)
         {
            var c = (l.SelectedItem as Client);

            await chatServer.SendMessageToClient(c.Tcp, "Beuh?");
         };
      }

      private async void MenuItem_Click(object sender, RoutedEventArgs e)
      {
         OpenFileDialog openFileDialog = new OpenFileDialog();

         if (openFileDialog.ShowDialog() == true)
         {
            var c = (sender as MenuItem).DataContext as Client;

            AddMessage($"[{c.Name}]: Send file '{openFileDialog.FileName}'");

            await chatServer.SendFileToClient(c.Tcp, openFileDialog.FileName);
         };
      }
   }
}
