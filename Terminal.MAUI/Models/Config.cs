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

      public long UserFK { get => _UserFK; set { _UserFK = value; SetUserID(); } }
      long _UserFK = -1;

      public DateTime LastSynchro { get; set; } = DateTime.MinValue;

      // - - -  - - - 

      void SetUserID()
      {
         // Microsoft.AppCenter.AppCenter.SetUserId($"{Login}={UserFK}");
      }

      #region - - - server - - -
      public string ClientName { get; internal set; } = "Client";

#if DEBUG
      public string ServerIP
      {
         get
         {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
               return "69.10.45.253";
            }
            else
            {
               return "127.0.0.1";
            };
         }
      }

      public string ServerPort { get; set; } = "9000";
#else
      public string ServerIP { get; set; } = "69.10.45.253";
      public string ServerPort { get; set; } = "9000";
#endif

      #endregion
   }
}
