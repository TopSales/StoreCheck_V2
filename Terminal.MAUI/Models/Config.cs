using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPF
{
   public class Config : BaseViewModel
   {
      public String Language { get; set; } = "DE";
      public TStrings HintsShown { get; set; } = new TStrings();
      public string LastPage { get; set; }
      public bool IsLogged { get; set; } = false;

      public string Login { get => _Login; set { _Login = value; SetUserID(); } }
      string _Login = "";

      public long FKUser { get => _FKUser; set { _FKUser = value; SetUserID(); } }
      long _FKUser = -1;

      public DateTime LastSync { get; set; } = DateTime.MinValue;

      // - - -  - - - 

      void SetUserID()
      {
         // Microsoft.AppCenter.AppCenter.SetUserId($"{Login}={FKUser}");
      }

      #region - - - server - - -
      public string ClientName { get; internal set; } = "Client";

      public const string ServerIP1 = "69.10.45.253"; // vps.Diplodocus.dev
      public const string ServerIP2 = "130.180.212.46"; // Limours.ZPF.fr

#if DEBUG
      public string ServerIP
      {
         get
         {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
               return ServerIP1;
            }
            else
            {
               return "127.0.0.1";
               //return ServerIP1;
            };
         }
      }


      public string ServerPort { get; set; } = "8080";
#else
      public string ServerIP { get; set; } = ServerIP1;
      public string ServerPort { get; set; } = "9000";
#endif

      #endregion
   }
}
