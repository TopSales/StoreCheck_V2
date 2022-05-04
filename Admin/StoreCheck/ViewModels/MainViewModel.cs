using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using OpenXML.Silverlight.Spreadsheet;
using ZPF.AT;
using ZPF.SQL;

namespace ZPF
{
   public partial class MainViewModel : BaseViewModel
   {
      public static string AppTitle = "StoreCheck";
      public static string IniFileName = "StoreCheck.ini";

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private static MainViewModel _Instance = null;

      public static MainViewModel Current
      {
         get
         {
            if (_Instance == null)
            {
               _Instance = new MainViewModel();
            };

            return _Instance;
         }

         set
         {
            _Instance = value;
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public void RefreshAll()
      {
         //StockViewModel.Current.LoadData();
         //ItemsViewModel.Current.LoadData();

         DoIt.Delay(500, () =>
         {
            ReferentielViewModel.Current.LoadData();
         });
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

      /// <summary>
      /// Load splitter position (via element width) ...
      /// </summary>
      /// <param name="Section">'Window name'</param>
      /// <param name="Ident">Splitter name</param>
      /// <param name="Default">Default value</param>
      /// <returns></returns>
      public double LoadSplitter(string Section, string Ident, double Default)
      {
         TIniFile IniFile = new TIniFile(IniFileName);

#if PCL
         await IniFile.LoadValues();
#else
         IniFile.LoadValues();
#endif

         return double.Parse(IniFile.ReadString(Section, Ident, Default.ToString()));
      }

      /// <summary>
      /// Saves splitter position (via element width) ...
      /// </summary>
      /// <param name="Section">'Window name'</param>
      /// <param name="Ident">Splitter name</param>
      /// <param name="Value"></param>
      public void SaveSplitter(string Section, string Ident, double Value)
      {
         TIniFile IniFile = new TIniFile(IniFileName);

#if PCL
         await IniFile.LoadValues();
#else
         IniFile.LoadValues();
#endif

         IniFile.WriteString(Section, Ident, Value.ToString());

         IniFile.UpdateFile();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private Printer _PrinterSettings = new Printer();
      public Printer PrinterSettings
      {
         get { return _PrinterSettings; }
         set { SetField(ref _PrinterSettings, value); }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public MainViewModel()
      {
         _Instance = this;

         DblClickToSelect = true;

         // - - -  - - - 

         DataFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\ZPF\";

         if (!Directory.Exists(DataFolder))
         {
            Directory.CreateDirectory(DataFolder);
         };

         // - - -  - - - 

         OpenDB();
         UpdateDashboard();

         // - - -  - - -

         DB_SQL.ToUniversalTime = true;

         // - - -  - - -

         Log.Write(ErrorLevel.Log, "MainViewModel(end)");
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public DBSQLViewModel Connection_DB { get; private set; }
      public DBSQLViewModel Connection_AT { get; private set; }
      public DBSQLViewModel Connection_DOC { get; private set; }

      public string DataFolder { get; internal set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
      public bool OpenDB()
      {
#if DEBUG
         // - - - DEV - - -
         if (Connection_DB == null || !Connection_DB.CheckConnection())
         {
            Connection_DB = SmarterASPViewModel.Current.OpenStoreCheckMaquette();// SmarterASPViewModel.Current.OpenStoreCheckDev();
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

            // - - -  - - - 

            AuditTrailViewModel.Current.Init(new DBAuditTrailWriter(Connection_AT));
            AuditTrailViewModel.Current.Application = "SCAdmin";
         };

         if (Connection_DOC == null || !Connection_DOC.CheckConnection())
         {
            Connection_DOC = SmarterASPViewModel.Current.OpenBaseDocDev();
            // - - -  - - - 

            #region Create table & co 

            //string SQL = "";

            //switch (Connection_DOC.DBType)
            //{
            //    case DBType.SQLServer: SQL = Document.SQLPostCreate_MSSQL; break;

            //    case DBType.SQLite: SQL = Document.SQLPostCreate_SQLite; break;

            //    case DBType.PostgreSQL: SQL = Document.SQLPostCreate_PGSQL; break;

            //    case DBType.MySQL: SQL = Document.SQLPostCreate_MySQL; break;
            //};

            //// - - -  - - - 

            //DB_SQL.CreateTable(Connection_DOC, typeof(Document), SQL, "");
            //DB_SQL.CreateTrigger_OnUpdated(Connection_DOC, "Document", "UpdatedOn");
            //DB_SQL.CreateTrigger_OnCreated(Connection_DOC, "Document", "CreatedOn");

            #endregion
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

      public void InitDB(bool SampleData = false)
      {
#if DEBUG
         if (DB_SQL._ViewModel.DBType == DBType.SQLServer)
         {
            if (Debugger.IsAttached)
            {
               // SampleViewModel.Current.DropTables();
            };
         };
#endif

         Log.Write(ErrorLevel.Log, "MainViewModel.InitDB(end)");

         if (UserViewModel.Current.Connection == null)
         {
            UserViewModel.Current.Init(Connection_DB);
         };

         if (!string.IsNullOrEmpty(DB_SQL._ViewModel.LastError))
         {
            BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Error, DB_SQL._ViewModel.LastError);
         };

         if (UserViewModel.Current.CurrentUser == null)
         {
            UserViewModel.Current.CurrentUser = new UserAccount() { Login = "(Demo)" };
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public void UpdatePrinterSettings()
      {
         OnPropertyChanged("PrinterSettings");
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public void LoadEditReferentiel()
      {
         Log.Write(ErrorLevel.Log, "MainViewModel.LoadReferentiel()");

         EditReferentielViewModel.Current.SelectedTable = null;

         EditReferentielViewModel.Current.CheckRights = false;
         EditReferentielViewModel.Current.Tables.Clear();

         // - - -  - - - 

         EditReferentielViewModel.Current.Tables.Add(new NameValue() { Name = "Chain", Value = "Chain" });
         EditReferentielViewModel.Current.Tables.Last().Tag = new TStrings();
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Name=Name,64");

         // - - -  - - - 

         EditReferentielViewModel.Current.Tables.Add(new NameValue() { Name = "SubChain", Value = "SubChain" });
         EditReferentielViewModel.Current.Tables.Last().Tag = new TStrings();
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Name=Name,64");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("FKChain=_Chain");
         {
            TStrings NameValue = new TStrings();
            NameValue = DB_SQL.QuickQueryList("Select Name as Name, PK as Value from Chain order by Name");
            NameValue.Insert(0, "");
            (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).SetObject((EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Count - 1, NameValue);
         }

         // - - -  - - - 

   //      EditReferentielViewModel.Current.Tables.Add(new NameValue() { Name = "Actions", Value = "Action" });
   //      EditReferentielViewModel.Current.Tables.Last().Tag = new TStrings();
   //      (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Nom=Nom,128");
   //      (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("FKClient=_Client");
   //      {
   //         TStrings NameValue = new TStrings();
   //         NameValue = DB_SQL.QuickQueryList("Select Nom as Name, PK as Value from Client order by Nom");
   //         NameValue.Insert(0, "");
   //         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).SetObject((EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Count - 1, NameValue);
   //      }
   //(EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("FKQuestionnaire=_Questionnaire");
   //      {
   //         TStrings NameValue = new TStrings();
   //         NameValue = DB_SQL.QuickQueryList("Select Nom as Name, PK as Value from Questionnaire order by Nom");
   //         NameValue.Insert(0, "");
   //         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).SetObject((EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Count - 1, NameValue);
   //      }
   //(EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Observations=Observations,4096");
   //      (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Duration=Duration");

         // - - -  - - - 

         EditReferentielViewModel.Current.TablesRefresh();

         // - - -  - - -  

         Log.Write(ErrorLevel.Log, "MainViewModel.LoadReferentiel(end)");

         /*
         EditReferentielViewModel.Current.Tables.Add(new NameValue() { Name = "Contacts", Value = "Contact" });
         EditReferentielViewModel.Current.Tables.Last().Tag = new TStrings();
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Ref=Ref");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("SSCC=SSCC,32");

         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Societe=Societe,64");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Fonction=Fonction,64");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Civilite=Civilité");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Nom=Nom,64");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Adresse=Adresse,512");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("CP=CP,10");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Ville=Ville,64");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Pays=Pays,64");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Telephone=Téléphone,32");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Mail=Mail,128");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Web=Web,128");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Observations=Observations,1024");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("IsClient=Client");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("IsFournisseur=Fournisseur");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("SortOrder=Ordre");

         // - - -  - - - 

         EditReferentielViewModel.Current.Tables.Add(new NameValue() { Name = "Taux TVA", Value = "TauxTVA" });
         EditReferentielViewModel.Current.Tables.Last().Tag = new TStrings();
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Ref=Ref");

         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Nom=Nom,64");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Taux=Taux");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Description=Description,1024");

         // - - -  - - - 

         EditReferentielViewModel.Current.Tables.Add(new NameValue() { Name = "Justification Litige", Value = "Justification" });
         EditReferentielViewModel.Current.Tables.Last().Tag = new TStrings();
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Nom=Nom,64");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Description=Description,1024");

         // - - -  - - - 

         EditReferentielViewModel.Current.Tables.Add(new NameValue() { Name = "Types de documents", Value = "DocumentType" });
         EditReferentielViewModel.Current.Tables.Last().Tag = new TStrings();
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Ref=Ref");

         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Nom=Nom,64");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Description=Description,1024");

         // - - -  - - - 

         EditReferentielViewModel.Current.Tables.Add(new NameValue() { Name = "Types de familles", Value = "FamilleType" });
         EditReferentielViewModel.Current.Tables.Last().Tag = new TStrings();
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Ref=Ref");

         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Nom=Nom,64");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Description=Description,1024");

         // - - -  - - - 

         EditReferentielViewModel.Current.Tables.Add(new NameValue() { Name = "Types d'emplacements", Value = "EmplacementType" });
         EditReferentielViewModel.Current.Tables.Last().Tag = new TStrings();
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Ref=Ref");

         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Nom=Nom,64");
         (EditReferentielViewModel.Current.Tables.Last().Tag as TStrings).Add("Description=Description,1024");

         EditReferentielViewModel.Current.TablesRefresh();
         */
         // - - -  - - - 

         Log.Write(ErrorLevel.Log, "MainViewModel.LoadReferentiel(end)");
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private ObservableCollection<Article> Articles { get; set; }
      public Article CurrentArticle { get; private set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private string _ImportPath;
      public string ImportPath
      {
         get { return _ImportPath; }
         set { SetField(ref _ImportPath, value); }
      }

      private string _ArchivPath;
      public string ArchivPath
      {
         get { return _ArchivPath; }
         set { SetField(ref _ArchivPath, value); }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public System.IO.FileInfo[] ImportFiles
      {
         get
         {
            try
            {
               return new System.IO.DirectoryInfo(ImportPath).GetFiles("*.csv");
            }
            catch
            {
               return null;
            };
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private string _Title = "";
      public string Title
      {
         get { return _Title; }
         set
         {
            if (SetField(ref _Title, value))
            {
               DoIt.OnMainThread(() =>
               {
                  OnPropertyChanged();
               });
            };
         }
      }

      public string DataPath { get; private set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public string Style = "Black";

      public string MasterPwd { get => _MasterPwd; set => _MasterPwd = value; }
      private string _MasterPwd = "0815";

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public bool DirectPrint { get; set; }
      public bool AutoLogin { get; set; }
      public bool DblClickToSelect { get; set; }
      public string LastMVT { get; set; }
      public bool ExitOnValidation { get; set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public bool Save()
      {
         TIniFile IniFile = new TIniFile(IniFileName);

         IniFile.WriteBool("General", "IsDebug", IsDebug);
         IniFile.WriteString("Local", "Printer", JsonSerializer.Serialize(PrinterSettings));
         IniFile.WriteBool("General", "AutoLogin", AutoLogin);
         IniFile.WriteString("Local", "Style", Style);
         //IniFile.WriteString("General", "Dump", Jupiter.Current.Dump);
         IniFile.WriteBool("General", "ExitOnValidation", ExitOnValidation);

         IniFile.WriteString("Stock", "LastMVT", LastMVT);

         IniFile.WriteString("Local", "ImportPath", ImportPath);
         IniFile.WriteString("Local", "ArchivPath", ArchivPath);

         try
         {
            IniFile.UpdateFile();
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);
         };

         // - - - Save Init to DB  - - - 

         IniFile.EraseSection("Login");
         IniFile.EraseSection("Local");
         IniFile.EraseSection("HintsShown");
         IniFile.EraseSection("DataGridColumns");
         IniFile.EraseSection("");

         // - - -  - - - 

         if (DB_SQL._ViewModel != null)
         {
            WriteIniToDB(IniFile);
         };

         // - - -  - - - 

         return true;
      }

      public bool Load()
      {
         Log.Write(ErrorLevel.Log, "MainViewModel.Load()");

         TIniFile IniFile = new TIniFile(IniFileName);

#if PCL
         await IniFile.LoadValues();
#else
         IniFile.LoadValues();
#endif

         // - - -  - - - 

         LoadIniFromDB(IniFile);

         // - - -  - - - 

         IsDebug = IniFile.ReadBool("General", "Debug", true);

         var st = IniFile.ReadString("Local", "Printer", "");

         if (string.IsNullOrEmpty(st))
         {
            LabelPrinterViewModel.Current.PrinterSettings = new Printer();
         }
         else
         {
            LabelPrinterViewModel.Current.PrinterSettings = JsonSerializer.Deserialize<Printer>(st);
         };

         if (LabelPrinterViewModel.Current.PrinterSettings == null)
         {
            LabelPrinterViewModel.Current.PrinterSettings = new Printer();
         }

         AutoLogin = IniFile.ReadBool("General", "AutoLogin", false);
         Style = IniFile.ReadString("Local", "Style", Style);
         ExitOnValidation = IniFile.ReadBool("General", "ExitOnValidation", true);

         ImportPath = IniFile.ReadString("Local", "ImportPath", "");
         ArchivPath = IniFile.ReadString("Local", "ArchivPath", "");

         SetIniBon(IniFile);

         Debug.WriteLine("*** IniFile loaded");

         return true;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

      public List<AuditTrail> atDashboard1 { get; set; } = new List<AuditTrail>();
      public List<AuditTrail> atDashboard2 { get; set; } = new List<AuditTrail>();


      public void UpdateDashboard()
      {
         string SQL = "";

         SQL = @"
select top 100 
  	AuditTrail.PK, AuditTrail.TimeStamp, AuditTrail.TimeStampApp, AuditTrail.TimeStampDB, AuditTrail.Level, AuditTrail.Parent, AuditTrail.IsBusiness, AuditTrail.Tag, AuditTrail.Application, AuditTrail.Source, AuditTrail.Message, AuditTrail.Ticks, AuditTrail.DataIn, AuditTrail.DataInType, AuditTrail.DataOut, AuditTrail.DataOutType, AuditTrail.TerminalID, AuditTrail.TerminalIP, AuditTrail.FKUser, AuditTrail.ItemID, AuditTrail.ItemType
from 
	AuditTrail
order by PK desc  
";

         var list = DB_SQL.Query<AuditTrail>(Connection_AT, SQL);
         atDashboard1.Clear();
         atDashboard2.Clear();

         foreach (AuditTrail auditTrail in list)
         {
            atDashboard1.Add(auditTrail);
            atDashboard2.Add(auditTrail);
         };

         OnPropertyChanged("atDashboard1");
         OnPropertyChanged("atDashboard2");
      }
   }
}
