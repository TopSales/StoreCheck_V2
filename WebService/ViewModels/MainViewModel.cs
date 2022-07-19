using System;
using System.IO;
using System.Timers;
using Newtonsoft.Json;
using ZPF;
using ZPF.AT;
using ZPF.SQL;

public partial class MainViewModel : BaseViewModel
{
   public static string AppTitle = "StoreCheck_Server";

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static MainViewModel _Current = null;

   public DBSQLViewModel Connection_DB { get; private set; }
   public DBSQLViewModel Connection_AT { get; private set; }
   public DBSQLViewModel Connection_DOC { get; private set; }
   public string DataFolder { get; }

   private System.Timers.Timer timer;

   public static MainViewModel Current
   {
      get
      {
         if (_Current == null)
         {
            _Current = new MainViewModel();
         };

         return _Current;
      }

      set
      {
         _Current = value;
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public MainViewModel()
   {
      _Current = this;

      DataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\";

      System.Diagnostics.Debug.WriteLine($"Data: {DataPath}");

      // - - -  - - - 

      //AuditTrailViewModel.Current.Init(new FileAuditTrailWriter(DataPath + AppTitle + ".AuditTrail.txt"));
      AuditTrailViewModel.Current.Init(new JSONAuditTrailWriter(DataPath + AppTitle + ".AuditTrail.JSON", JSONAuditTrailWriter.FileTypes.PartialJSON));
      AuditTrailViewModel.Current.Application = "SCServ";
      AuditTrailViewModel.Current.Clean();

      // - - -  - - -

//      OpenDB();

//      AuditTrailViewModel.Current.Init(new DBAuditTrailWriter(Connection_AT));

//      // - - -  - - -

//#if DEBUG
//      Log.WriteHeader("ServerWPF", $"debug on {Environment.MachineName}", "");
//#else
//      Log.WriteHeader("ServerWPF", $"release on {Environment.MachineName}", "");
//#endif

//      try
//      {
//         var st = $"DB {Connection_DB.DbConnection.Database} - Doc {Connection_DOC.DbConnection.Database} - AT {Connection_AT.DbConnection.Database}";
//         Log.Write(ErrorLevel.Log, st);
//      }
//      catch { };

      // - - -  - - -
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   public string DataPath { get; private set; }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public Params Config { get; private set; } = new Params();

   public string LastError { get => _LastError; set => SetField(ref _LastError, value); }
   string _LastError = "";

   //public bool WebServiceOK { get; private set; } = true;

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public async void Load()
   {
      // - - - config - - -

      {
         string FileName = DataFolder + @"StoreCheck.Server.Params.json";

         if (System.IO.File.Exists(FileName))
         {
            string json = File.ReadAllText(FileName);

            var p = JsonConvert.DeserializeObject<Params>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            if (p != null)
            {
               Config = p;
            };
         }
         else
         {
            Save();
         };
      };
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   internal void Save()
   {
      // - - - config - - -

      {
         string FileName = DataFolder + @"StoreCheck.Server.Params.json";

         var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
         File.WriteAllText(FileName, json);
      };
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public string CurrentMessage
   {
      get => Config.CurrentMessage;
      set
      {
         Config.CurrentMessage = value;
         OnPropertyChanged("CurrentMessage");
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
}
