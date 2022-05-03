using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Params
{
   public string CurrentMessage { get; set; } = "En maintennance ...";

   #region - - - server - - -
   public string ClientName { get; internal set; } = "Client";
   public string ServerIP { get; set; } = "127.0.0.1";
   public string ServerPort { get; set; } = "9000";
   #endregion
}

