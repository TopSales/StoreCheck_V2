using System;

namespace ZPF
{
   public class Params : BaseViewModel
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
         Microsoft.AppCenter.AppCenter.SetUserId( $"{Login}={UserFK}");
      }
   }
}
