using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ZPF.AT;

namespace ZPF.Chat
{
   public partial class ChatCore
   {
/*
      public const string EndOfFrame = "¤EOF~";

      public const string SystemMessageTag = "¤MSG~";
      public const string ClientConnectTag = "¤CONNECT~";
      public const string ClientDisconnectTag = "¤CONNECT_CLIENT~";
      public const string ServerDisconnectTag = "¤DISCONNECTED_SERVER~";
      public const string FileTag = "¤FILE~";

      //public const string DataSignal = "¤DATA~";
      //public const string ClientDisconnectSignal = "¤DISCONNECTED_CLIENT~";

      public static string DataFolder { get; set; }
*/

      public enum EventType
      {
         /// <summary>
         /// Chat client event
         /// </summary>
         ConnectionFailed,

         /// <summary>
         /// Chat client event
         /// </summary>
         SendConnectionDataFailed,

         /// <summary>
         /// Chat client event
         /// </summary>
         IOException,

         /// <summary>
         /// Chat server event
         /// </summary>
         SocketException,

         /// <summary>
         /// 
         /// </summary>
         ClientConnected,

         /// <summary>
         /// Chat client/server event
         /// </summary>
         ClientDisconnected,

         /// <summary>
         /// Chat client event
         /// </summary>
         ServerDisconnected,

         /// <summary>
         /// Chat client event
         /// </summary>
         Data,

         /// <summary>
         /// Chat server event - Message from server to operatorData
         /// </summary>
         Message,

         /// <summary>
         /// 
         /// </summary>
         File,
      }
/*
      public delegate void OnSystemMessageEventHandler(object sender, TcpClient tcpClient, EventType eventType, string message = "");
      public delegate void OnDataEventHandler(object sender, TcpClient tcpClient, ChatData data);

      /// <summary>
      /// Parse string to integer
      /// </summary>
      /// <param name="stringVal"></param>
      /// <returns>
      /// Returns integer
      /// </returns>
      public static int ParseStringToInt(string stringVal)
      {
         int.TryParse(stringVal, out var intVal);
         return intVal;
      }

      /// <summary>
      /// Validates IP Address
      /// </summary>
      /// <param name="ipAddress"></param>
      /// <returns>
      /// True if IP Address is valid
      /// </returns>
      public static bool IsValidIpAddress(string ipAddress)
      {
         return IPAddress.TryParse(ipAddress, out var ip);
      }

      /// <summary>
      /// Validates Server port
      /// </summary>
      /// <param name="port"></param>
      /// <returns>
      /// True if server port is valid
      /// </returns>
      public static bool IsValidPort(string port)
      {
         const int maxPortNumber = 65535;
         const int minPortNumber = 0;

         return (port.All(char.IsDigit) && (ParseStringToInt(port) > minPortNumber) &&
                 (ParseStringToInt(port) <= maxPortNumber));
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public static string DownloadFile(string DataFolder, string json)
      {
         ChatFile model = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatFile>(json);

         return DownloadFile(DataFolder, model);
      }

      public static string DownloadFile(string DataFolder, ChatFile model)
      {
         // Depending on if you want the byte array or a memory stream, you can use the below. 
         var dataByteArray = Convert.FromBase64String(model.FileData);

         // When creating a stream, you need to reset the position, without it you will see that you always write files with a 0 byte length. 
         var imageDataStream = new MemoryStream(dataByteArray);
         imageDataStream.Position = 0;

         // Go and do something with the actual data.

         var FileName = (DataFolder + @"\" + model.FileName).Replace(@"\\", @"\");

         if (System.IO.File.Exists(FileName))
         {
            System.IO.File.Delete(FileName);
         };

         System.IO.File.WriteAllBytes(FileName, dataByteArray);

         return FileName;
      }
*/
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public static ChatData SerializeData(object data)
      {
         if (data == null)
         {
            return null;
         }
         else
         {
            var chatData = new ChatData
            {
               Name = data.GetType().Name,
               Data = Newtonsoft.Json.JsonConvert.SerializeObject(data),
            };

            if (chatData.Data.Length > 1024)
            {
               var zip = ZIPHelper.Zip(chatData.Data);
               var st = Convert.ToBase64String(zip);

               if (st.Length < chatData.Data.Length)
               {
                  chatData.Data = st;
                  chatData.IsZipped = true;
               };
            };

            return chatData;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public static ChatData DeserializeChatData(string message)
      {
         if (message == null)
         {
            return null;
         };

         try
         {
            ChatData chatData = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatData>(message.TrimEnd(new char[] { '~' }));

            if (chatData != null && chatData.IsZipped)
            {
               var zip = Convert.FromBase64String(chatData.Data);
               chatData.Data = ZIPHelper.Unzip(zip);
               chatData.IsZipped = false;
            };

            return chatData;
         }
         catch (Exception ex)
         {
            Log.Write(new AuditTrail(ex) { DataIn = message });
            return null;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
/*
      public static string UploadFileJSON(string FileName)
      {
         var chatFile = UploadFile(FileName);

         var json = Newtonsoft.Json.JsonConvert.SerializeObject(chatFile);

         return json;
      }

      public static ChatFile UploadFile(string FileName)
      {
         var dataByteArray = System.IO.File.ReadAllBytes(FileName);

         return new ChatFile
         {
            FileName = System.IO.Path.GetFileName(FileName),
            FileData = Convert.ToBase64String(dataByteArray),
         };
      }
*/
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
