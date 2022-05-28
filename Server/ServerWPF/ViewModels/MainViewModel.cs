using System;
using System.IO;
using System.Timers;
using Newtonsoft.Json;
using ZPF;
using ZPF.AT;
using ZPF.SQL;

public class MainViewModel : BaseViewModel
{
   public static string AppTitle = "StoreCheck_Server";

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static MainViewModel _Current = null;

   public DBSQLViewModel Connection_DB { get; private set; }
   public DBSQLViewModel Connection_AT { get; private set; }
   public DBSQLViewModel Connection_DOC { get; private set; }
   public string DataFolder { get; }

   private Timer timer;

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

      OpenDB();

#if !DEBUG
      AuditTrailViewModel.Current.Init(new DBAuditTrailWriter(Connection_AT));
      AuditTrailViewModel.Current.Application = "SCAdmin";
#endif
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   public string DataPath { get; private set; }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   public bool OpenDB()
   {
#if DEBUG
      // - - - DEV - - -
      if (Connection_DB == null || !Connection_DB.CheckConnection())
      {
         //Connection_DB = SmarterASPViewModel.Current.OpenStoreCheckDev();
         Connection_DB = SmarterASPViewModel.Current.OpenStoreCheckMaquette();
      };

      if (Connection_AT == null || !Connection_AT.CheckConnection())
      {
         Connection_AT = SmarterASPViewModel.Current.OpenAuditTrailDev();

         // - - -  - - - 

         #region Create table & co 

         string SQL = "";

         switch (Connection_AT.DBType)
         {
            case DBType.SQLServer: SQL = AuditTrail.PostScript_MSSQL; break;

            case DBType.SQLite: SQL = AuditTrail.PostScript_SQLite; break;

            case DBType.PostgreSQL: SQL = AuditTrail.PostScript_PGSQL; break;

            case DBType.MySQL: SQL = AuditTrail.PostScript_MySQL; break;
         };

         // - - -  - - - 

         DB_SQL.CreateTable(Connection_AT, typeof(AuditTrail), SQL, "");
         DB_SQL.CreateTable(Connection_AT, typeof(AuditTrail_App), SQL, "");

         #endregion
      };

      if (Connection_DOC == null || !Connection_DOC.CheckConnection())
      {
         Connection_DOC = SmarterASPViewModel.Current.OpenBaseDocDev();
      };
#else
      // - - - PROD - - -
      if (Connection_DB == null || !Connection_DB.CheckConnection())
      {
         Connection_DB = SmarterASPViewModel.Current.OpenStoreCheckProd();
      };

                if (Connection_AT == null || !Connection_AT.CheckConnection())
                {
                    Connection_AT = SmarterASPViewModel.Current.OpenAuditTrailProd();

                    // - - -  - - - 

      #region Create table & co 

                    string SQL = "";

                    switch (Connection_AT.DBType)
                    {
                        case DBType.SQLServer: SQL = AuditTrail.PostScript_MSSQL; break;

                        case DBType.SQLite: SQL = AuditTrail.PostScript_SQLite; break;

                        case DBType.PostgreSQL: SQL = AuditTrail.PostScript_PGSQL; break;

                        case DBType.MySQL: SQL = AuditTrail.PostScript_MySQL; break;
                    };

                    // - - -  - - - 

                    DB_SQL.CreateTable(Connection_AT, typeof(AuditTrail), SQL, "");
                    DB_SQL.CreateTable(Connection_AT, typeof(AuditTrail_App), SQL, "");

      #endregion

                    // - - -  - - - 

                    AuditTrailViewModel.Current.Init(new DBAuditTrailWriter(Connection_AT));
                    AuditTrailViewModel.Current.Application = "SCAdmin";
                };

                if (Connection_DOC == null || !Connection_DOC.CheckConnection())
                {
                    Connection_DOC = SmarterASPViewModel.Current.OpenBaseDocProd();

                    // - - -  - - - 

      #region Create table & co 

                    string SQL = "";

                    switch (Connection_DOC.DBType)
                    {
                        case DBType.SQLServer: SQL = Document.SQLPostCreate_MSSQL; break;

                        case DBType.SQLite: SQL = Document.SQLPostCreate_SQLite; break;

                        case DBType.PostgreSQL: SQL = Document.SQLPostCreate_PGSQL; break;

                        case DBType.MySQL: SQL = Document.SQLPostCreate_MySQL; break;
                    };

                    // - - -  - - - 

                    DB_SQL.CreateTable(Connection_DOC, typeof(Document), SQL, "");
                    DB_SQL.CreateTrigger_OnUpdated(Connection_DOC, "Document", "UpdatedOn");
                    DB_SQL.CreateTrigger_OnCreated(Connection_DOC, "Document", "CreatedOn");

      #endregion

            };
#endif

      //Log.Write(new AuditTrail
      //{
      //    Level = ErrorLevel.Log,
      //    Tag = "API",
      //    Message = request.Path,
      //    TerminalID = request.HttpContext.Connection.RemoteIpAddress.ToString(),
      //    TerminalIP = request.HttpContext.Connection.RemoteIpAddress.ToString(),
      //});

      return true;
   }

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
