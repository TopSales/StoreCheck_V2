using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using ZPF;
using ZPF.SQL;

public class MainViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static MainViewModel _Current = null;

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
      //Ikea.InitMsg();

      // - - -  - - -

      DataFolder = System.IO.Path.GetDirectoryName(Environment.CurrentDirectory) + @"\Data\";

      System.Diagnostics.Debug.WriteLine($"Data: {DataFolder}");

      if (!Directory.Exists(DataFolder))
      {
         Directory.CreateDirectory(DataFolder);
      };

      // - - -  - - -

      //timer = new Timer();
      //timer.Interval = 1000 * 5;
      //timer.Elapsed += Timer_Elapsed;
      //timer.Start();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //bool IsFirst_Timer_Elapsed = true;

   //private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
   //{
   //   timer.Stop();

   //   WebServiceOK = ClientViewModel.Current.IsConnected();
   //   OnPropertyChanged("WebServiceOK");

   //   if (!WebServiceOK)
   //   {
   //      ClientViewModel.Current.Connect();
   //   };

   //   if (Config.IsServer && !ServerViewModel.Current.IsServerRunning)
   //   {
   //      OpenDB();
   //      ServerViewModel.Current.StartStop();
   //   };

   //   timer.Start();
   //}

   //internal void StopTimer()
   //{
   //   timer.Stop();
   //}

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

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public DBSQLViewModel Connection { get; private set; }

   public void OpenDB()
   {
      string Server = @"";
      string DBase = DataFolder + @"Syscall.SQLite.db3";
      string User = "";
      string Password = "";

      string ConnectionString = DB_SQL.GenConnectionString(DBType.SQLite, Server, DBase, User, Password);

      if (string.IsNullOrEmpty(ConnectionString))
      {
         MainViewModel.Current.Connection.LastError = "No ConnectionString ...";
         return;
      };

      Connection = new DBSQLViewModel(new MSSQLiteEngine());
      DB_SQL._ViewModel = Connection;
      Connection.Open(ConnectionString, true);
      //Log.Write("", $"{ConnectionString} {(Connection.Open(ConnectionString, true) ? "OK" : "KO")}");

      //ToDo: CleanAuditTrail();

      //DB_SQL.CreateTable(typeof(Spooler));
      //DB_SQL.CreateTable(typeof(Spooler), "Stats");
      //DB_SQL.CreateTable(typeof(Current));

      if (Connection.DBType == DBType.SQLite)
      {
         DB_SQL.QuickQuery("VACUUM");
      };

      {
         DateTime dt = DateTime.Now.AddYears(-2);

         DB_SQL.QuickQuery($"delete from Spooler where DATE(CreatedOn) <= {DB_SQL.DateTimeToSQL(DB_SQL._ViewModel.DBType, dt)}");

         //// Sodexo.Safran
         //dt = DateTime.Now.AddHours(-4);
         //DB_SQL.QuickQuery($"delete from Current where DATE(CreatedOn) <= {DB_SQL.DateTimeToSQL(DB_SQL._ViewModel.DBType, dt)}");
      };
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
}
