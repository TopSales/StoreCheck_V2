﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

      public static int MaxArticles = 50;
      public static int MaxMVT = 1000;

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

         Articles = new ObservableCollection<Article>();

         // - - -  - - -

         //OK DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\";
         DataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\";
         IniFileName = DataPath + IniFileName;

         AuditTrailViewModel.Current.Init(new FileAuditTrailWriter(DataPath + AppTitle + ".AuditTrail.txt"));
         AuditTrailViewModel.Current.Clean();

         // - - -  - - -

         DB_SQL.ToUniversalTime = true;

         //ToDo: MSSQL
         //DB_SQL.DoTransactions = false;

         // OpenInitDB();

         try
         {
            TStrings bat = new TStrings();
            bat.Add(string.Format(@"@{0}{1}.exe %1 %2 %3 %4 %5", System.AppDomain.CurrentDomain.BaseDirectory, AppTitle));

            bat.SaveToFile(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\WindowsApps\StoreCheck.bat", System.Text.Encoding.ASCII);
         }
         catch (Exception ex)
         {
            Log.Write(ErrorLevel.Error, ex);
            BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Error, "Mise en place de la ligne de commande:" + Environment.NewLine + ex.Message);
         };

         // - - -  - - -

         Log.Write(ErrorLevel.Log, "MainViewModel(end)");
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public void OpenDB()
      {
         if (DB_SQL._ViewModel == null || !DB_SQL._ViewModel.CheckConnection())
         {
            string dbFileName = DataPath + AppTitle + ".db3";

            DBConnectionParams connectionParams = new DBConnectionParams()
            {
               DBType = ZPF.SQL.DBType.SQLite,
               Name = "StoreCheck",
               DBase = dbFileName,
            };

            DB_SQL._ViewModel = DBSQL_Helper.OpenDB(connectionParams, false);
            DBViewModel.Current.Connection = DB_SQL._ViewModel;

            CleanAuditTrail();

            if (DBViewModel.Current.DBType == DBType.SQLite)
            {
               DB_SQL.QuickQuery("VACUUM");
            };
         };
      }

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

         //SampleData = SampleViewModel.Current.CreateDBList(SampleData);

         //if (SampleData)
         //{
         //   if (DB_SQL._ViewModel.DBType == DBType.SQLServer)
         //   {
         //      SampleViewModel.Current.DropTables();
         //   };

         //   SampleViewModel.Current.CreateTables();
         //   SampleViewModel.Current.SampleData();
         //}
         //else
         //{
         //   DB_SQL._ViewModel.LastError = "";
         //};

         Log.Write(ErrorLevel.Log, "MainViewModel.InitDB(end)");

         if (UserViewModel.Current.Connection == null)
         {
            UserViewModel.Current.Init(DB_SQL._ViewModel);
         };

         // Updates
         if (DB_SQL._ViewModel.DBType == DBType.SQLite)
         {
            try
            {
               DB_SQL.QuickQuery("Drop table UserSession;");
               DB_SQL.QuickQuery(UserSession.SQLCreate_SQLite);


               DB_SQL.QuickQuery("ALTER TABLE User_RoleAdd ADD COLUMN Expiration datetime;");

               DB_SQL._ViewModel.LastError = "";
            }
            catch
            {

            };
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

      private string _LogoPath;
      public string LogoPath
      {
         get { return _LogoPath; }
         set { SetField(ref _LogoPath, value); }
      }

      private string _Footer1;
      public string Footer1
      {
         get { return _Footer1; }
         set { SetField(ref _Footer1, value); }
      }

      private string _Footer2;
      public string Footer2
      {
         get { return _Footer2; }
         set { SetField(ref _Footer2, value); }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private string _LabelArticles = "*";
      public string LabelArticles
      {
         get { return _LabelArticles; }
         set { SetField(ref _LabelArticles, value); }
      }

      private string _LabelEmplacements = "*";
      public string LabelEmplacements
      {
         get { return _LabelEmplacements; }
         set { SetField(ref _LabelEmplacements, value); }
      }

      private string _LabelEmplacementsAvecArt = "*";
      public string LabelEmplacementsAvecArt
      {
         get { return _LabelEmplacementsAvecArt; }
         set { SetField(ref _LabelEmplacementsAvecArt, value); }
      }

      private string _LabelBons = "*";
      public string LabelBons
      {
         get { return _LabelBons; }
         set { SetField(ref _LabelBons, value); }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public string Style = "Black";

      public bool ServerAutoStart { get; set; }

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
         IniFile.WriteString("Local", "LogoPath", LogoPath);
         IniFile.WriteString("General", "Footer1", Footer1);
         IniFile.WriteString("General", "Footer2", Footer2);
         IniFile.WriteBool("General", "AutoLogin", AutoLogin);
         IniFile.WriteString("Local", "Style", Style);
         //IniFile.WriteString("General", "Dump", Jupiter.Current.Dump);
         IniFile.WriteBool("General", "ExitOnValidation", ExitOnValidation);

         IniFile.WriteString("Label", "LabelArticles", LabelArticles);
         IniFile.WriteString("Label", "LabelEmplacements", LabelEmplacements);
         IniFile.WriteString("Label", "LabelEmplacementsAvecArt", LabelEmplacementsAvecArt);
         IniFile.WriteString("Label", "LabelBons", LabelBons);

         IniFile.WriteString("Stock", "LastMVT", LastMVT);

         IniFile.WriteString("Local", "ImportPath", ImportPath);
         IniFile.WriteString("Local", "ArchivPath", ArchivPath);
         //IniFile.WriteBool("Import", "AutoCreateArticle", StockViewModel.Current.AutoCreateArticle);

         BonE_Prefix = (BonE_Prefix == "" ? "*" : BonE_Prefix);
         BonE_Desc = (BonE_Desc == "" ? "*" : BonE_Desc);

         BonS_Prefix = (BonS_Prefix == "" ? "*" : BonS_Prefix);
         BonS_Desc = (BonS_Desc == "" ? "*" : BonS_Desc);

         IniFile.WriteString("Bons", "BonE_Prefix", BonE_Prefix);
         IniFile.WriteString("Bons", "BonE_Titre", BonE_Titre);
         IniFile.WriteString("Bons", "BonE_Desc", BonE_Desc);

         IniFile.WriteString("Bons", "BonS_Prefix", BonS_Prefix);
         IniFile.WriteString("Bons", "BonS_Titre", BonS_Titre);
         IniFile.WriteString("Bons", "BonS_Desc", BonS_Desc);

         IniFile.WriteBool("Bons", "ReplaceWithBon", ReplaceWithBon);

         IniFile.WriteBool("Local", "AutoStart", ServerAutoStart);

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

         LogoPath = "";
         Footer1 = "ZPF - 7, Rue Laurent de Lavoisier - F-91410 Dourdan - +33 1 60 81 64 10";
         Footer2 = "SAS au capital de 76 224.51 € - N° SIRET: 399 027 861 00034";

         LogoPath = IniFile.ReadString("Local", "LogoPath", LogoPath);
         Footer1 = IniFile.ReadString("General", "Footer1", Footer1);
         Footer2 = IniFile.ReadString("General", "Footer2", Footer2);

         AutoLogin = IniFile.ReadBool("General", "AutoLogin", false);
         Style = IniFile.ReadString("Local", "Style", Style);
         //Jupiter.Current.Dump = IniFile.ReadString("General", "Dump", Jupiter.Current.Dump);
         ExitOnValidation = IniFile.ReadBool("General", "ExitOnValidation", true);

         LabelArticles = IniFile.ReadString("Label", "LabelArticles", LabelArticles);
         LabelEmplacements = IniFile.ReadString("Label", "LabelEmplacements", LabelEmplacements);
         LabelEmplacementsAvecArt = IniFile.ReadString("Label", "LabelEmplacementsAvecArt", LabelEmplacementsAvecArt);
         LabelBons = IniFile.ReadString("Label", "LabelBons", LabelBons);

         LastMVT = IniFile.ReadString("Stock", "LastMVT", "7D");

         ImportPath = IniFile.ReadString("Local", "ImportPath", "");
         ArchivPath = IniFile.ReadString("Local", "ArchivPath", "");
         //StockViewModel.Current.AutoCreateArticle = IniFile.ReadBool("Import", "AutoCreateArticle", false);

         SetIniBon(IniFile);

         ServerAutoStart = IniFile.ReadBool("Local", "AutoStart", false);

         Debug.WriteLine("*** IniFile loaded");

         return true;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  
   }
}