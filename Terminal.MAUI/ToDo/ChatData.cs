

using System.Runtime.InteropServices.ComTypes;
using System;
using Newtonsoft.Json;

namespace ZPF.Chat
{
   public class ChatData
   {
      public string Action { get; set; }
      public bool HasMessages { get; set; } = false;
      public bool IsZipped { get; set; } = false;

      /// <summary>
      /// Data type name of serialised data 
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// Data weather string or JSON
      /// </summary>
      public string Data
      {
         //get => (IsZipped ? ZIPHelper.Unzip(Convert.FromBase64String(_Data)) : _Data);
         get => _Data;
         set => _Data = value;
      }
      string _Data = "";

      //<t>ExtractJSONFromZIP()

      // - - - tools  - - - 
      public string Serialize()
      {
         return JsonConvert.SerializeObject(this);
      }

      public static string Serialize(ChatData data)
      {
         return JsonConvert.SerializeObject(data);
      }

      public static ChatData Deserialize(string json)
      {
         try
         { 
            return JsonConvert.DeserializeObject<ChatData>(json);
         }
         catch 
         { 
            return null; 
         };
      }

      public string GetData()
      {
         if (IsZipped)
         {
            var zip = Convert.FromBase64String(Data);
            return ZIPHelper.Unzip(zip);
         }
         else
         {
            return Data;
         };
      }
   }
}
