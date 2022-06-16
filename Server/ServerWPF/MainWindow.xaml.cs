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
      ChatServer BLL = null;

      // Write the host messages to the console
      void OnHostMessage(string input)
      {
         PeriodicallyClearScreen();
         AddMessage(input);
      }

      // Write the host messages to the console
      void OnDataMessage(Object sender, ChatData data)
      {
         PeriodicallyClearScreen();
         AddMessage($"{data.Action} [{data.Data}]");

         ServerViewModel.Current.OnDataMessage(sender as ChatServer, data);
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
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         btnStartStop_Click(sender, e);
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
         //Dispatcher.Invoke(() => listChats.Items.Add(message));

         System.Diagnostics.Debug.WriteLine(message);
      }

      private void ClearMessage()
      {
         //Dispatcher.Invoke(() => listChats.Items.Clear());
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

               var serverPort = ChatCore.ParseStringToInt(serverPortValue.Text);
               var ipAddress = serverIpAddress.Text.Trim();
               //await chatServer.Listener(ipAddress, serverPort);


               if (BLL == null)
               {
                  BLL = new ZPF.Chat.ChatServer(OnHostMessage, OnDataMessage, ipAddress, serverPort);

                  if (BLL == null)
                  {
                  };
               };
               BLL.StartServer();
            }
            else
            {
               MessageBox.Show("IP Address or server port is invalid!");
            };
         }
         else
         {
            btnStartStop.Content = "Start";
            BLL.StopServer();
         };
      }


      /// <summary>
      /// This method triggers the stop functionality
      /// </summary>
      private async void StopServer()
      {
         //await chatServer.SendStatusToAllClients(ChatCore.ServerDisconnectSignal);

         ////Dispatcher.Invoke(() => listClients.Items.Clear());
         //AddMessage("[SERVER]: Server is closed!");

         //var serverPort = ChatCore.ParseStringToInt(serverPortValue.Text);
         //var serverBufferSize = ChatCore.ParseStringToInt(serverBufferSizeValue.Text);
         //IPAddress.TryParse(serverIpAddress.Text, out var ipAddress);
         //await chatServer.Listener(ipAddress, serverPort, serverBufferSize, true);

         //await chatServer.StopServer();
      }


      /// <summary>
      /// Method is triggered when user closes the application.
      /// This method will stop the server.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private async void CloseServerConnection(object sender, CancelEventArgs e)
      {
         //ToDo: await chatServer.CloseConnection();
      }
   }
}
