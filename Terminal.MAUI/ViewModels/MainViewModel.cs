using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using Newtonsoft.Json;
using ZPF;
using ZPF.AT;

public partial class MainViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static MainViewModel _Current = null;

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

   public Config Config { get; private set; } = new Config();

   public string DeviceID
   {
      get => _DeviceID;
      set { SetField(ref _DeviceID, value); AuditTrailViewModel.Current.TerminalID = _DeviceID; }
   }
   string _DeviceID = "";

   public string EntryMsg { get => _EntryMsg; set => SetField(ref _EntryMsg, value); }
   string _EntryMsg = "";

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public string DataFolder { get; private set; }

   public bool IsInternetAccessAvailable 
   { 
      get
      {
         NetworkAccess accessType = Connectivity.Current.NetworkAccess;

         // Connection to internet is available
         return (accessType == NetworkAccess.Internet);
      } 
   }

   public MainViewModel()
   {
      DataFolder = FileSystem.AppDataDirectory + @"\";

      System.Diagnostics.Debug.WriteLine($"Data: {DataFolder}");

      AuditTrailViewModel.Current.Init(new JSONAuditTrailWriter(DataFolder + "AuditTrail.json", JSONAuditTrailWriter.FileTypes.PartialJSON));
      AuditTrailViewModel.Current.Application = "SC_Term";
      AuditTrailViewModel.Current.Clean();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public void Load()
   {
      string FileName = DataFolder + @"StoreCheck.Config.json";

      if (System.IO.File.Exists(FileName))
      {
         string json = File.ReadAllText(FileName);

         var c = JsonConvert.DeserializeObject<Config>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
         if (c != null)
         {
            Config = c;
         };
      }
      else
      {
         Save();
      };
   }

   public void Save()
   {
      string FileName = DataFolder + @"StoreCheck.Config.json";

      var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
      File.WriteAllText(FileName, json);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public void LoadLocalDB()
   {
      string LoadJson(string FileName)
      {
         return System.IO.File.ReadAllText(DataFolder + FileName + ".json");
      };

      #region - - - Interventions - - -

      DoIt.OnMainThread(() =>
      {
         BackboneViewModel.Current.BusySubTitle = $"Load Interventions ...";
      });

      var e = System.Text.Json.JsonSerializer.Deserialize<Intervention_CE[]>(LoadJson("Interventions"));
      if (e != null) MainViewModel.Current.SetLocalInterventions(e.ToList());

      #endregion


      #region - - - Stores - - -

      DoIt.OnMainThread(() =>
      {
         BackboneViewModel.Current.BusySubTitle = $"Load Stores ...";
      });

      try
      {
         var m = System.Text.Json.JsonSerializer.Deserialize<Store_CE[]>(LoadJson("Stores"));
         if (m != null) MainViewModel.Current.SetLocalStores(m.ToList());
      }
      catch (Exception ex)
      {
         Log.Write(new AuditTrail(ex));
      };

      #endregion


      //DoIt.OnMainThread(() =>
      //{
      //   BackboneViewModel.Current.BusySubTitle = $"Load Documents ...";
      //});
      //var d = System.Text.Json.JsonSerializer.Deserialize<Document_CE[]>(LoadJson("Documents"));
      //if (d != null) MainViewModel.Current.SetLocalDocuments(d.ToList());

      //DoIt.OnMainThread(() =>
      //{
      //   BackboneViewModel.Current.BusySubTitle = $"Load Articles ...";
      //});
      //var a = JsonSerializer.Deserialize<EAN_Article[]>(LoadJson("Articles"));
      //if (a != null) MainViewModel.Current.SetLocalArticles(a.ToList());

      //DoIt.OnMainThread(() =>
      //{
      //   BackboneViewModel.Current.BusySubTitle = $"Load Articles EAN ...";
      //});
      //var ae = JsonSerializer.Deserialize<EAN_Article[]>(LoadJson("ArticlesEAN"));
      //if (ae != null) MainViewModel.Current.SetLocalArticlesEAN(ae.ToList());

      //ToDo: Load Families ...
      //DoIt.OnMainThread(() =>
      //{
      //   BackboneViewModel.Current.BusySubTitle = $"Load Families ...";
      //});
      //var f = JsonSerializer.Deserialize<EAN_Family[]>(LoadJson("Families"));
      //if (f != null) MainViewModel.Current.SetLocalFamilies(f.ToList());
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public enum DBRange { all, Documents, Interventions, Stores, Articles, Families }

   public void SaveLocalDB(DBRange saveDBRange = DBRange.all)
   {
      // ToDo: ? SemaPhore 

      void SaveListe(string FileName, object list)
      {
         string json = "";

         try
         {
            json = System.Text.Json.JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true, });
         }
         catch (Exception ex)
         {
            json = ex.Message;
         };

         File.WriteAllText(DataFolder + FileName + ".json", json);
      }

      if (saveDBRange == DBRange.all || saveDBRange == DBRange.Interventions) SaveListe("Interventions", Interventions);
      if (saveDBRange == DBRange.all || saveDBRange == DBRange.Stores) SaveListe("Stores", Stores);

      //if (saveDBRange == DBRange.all || saveDBRange == DBRange.Documents) SaveListe("Documents", Documents);
      ////ToDo: if (saveDBRange == DBRange.all || saveDBRange == DBRange.Articles) SaveListe("ArticlesEAN", ArticlesEAN);
      //if (saveDBRange == DBRange.all || saveDBRange == DBRange.Articles) SaveListe("Articles", Articles);
      //if (saveDBRange == DBRange.all || saveDBRange == DBRange.Families) SaveListe("Families", Families);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
