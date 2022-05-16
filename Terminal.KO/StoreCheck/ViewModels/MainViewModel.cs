using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using ZPF;
using ZPF.AT;

public class MainViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static MainViewModel _Current = null;

   public string DataFolder { get; }

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

   //#if DEBUG
   //   //public static string wsServer = "http://ws.StoreCheck.tech/StoreCheck/";
   //   //public static string wsServer = "http://localhost:6200/StoreCheck/";
   //   //public static string wsServer = "http://ws.StoreCheck.pro/StoreCheck/";

   //   public static string wsServer = "https://wsstorecheck.diplodocus.dev/StoreCheck/";
   //#else
   //   //public static string wsServer = "http://ws.StoreCheck.pro/StoreCheck/";

   //   public static string wsServer = "https://wsstorecheck.diplodocus.dev/StoreCheck/";
   //#endif

#if DEBUG
   public static string wsServer = "https://wsstorecheck.diplodocus.dev/StoreCheck/";
   // public static string wsServer = "http://localhost:6200/StoreCheck/";

   public static string wsServerDoc = "https://wsstorecheckdoc.diplodocus.dev/StoreCheck/";
#else
   public static string wsServer = "https://wsstorecheck.diplodocus.dev/StoreCheck/";
   public static string wsServerDoc = "https://wsstorecheckdoc.diplodocus.dev/StoreCheck/";
#endif

   public MainViewModel()
   {
      _Current = this;

      // - - -  - - -

      DataFolder = ZPF.XF.Basics.Current.GetDataDirectory();

      if (Device.RuntimePlatform == Device.macOS || Device.RuntimePlatform == Device.WPF)
      {
         DataFolder += @"\StoreCheck\";
         DataFolder = ZPF.XF.Basics.Current.FileIO.CleanPath(DataFolder);

         if (!System.IO.Directory.Exists(DataFolder))
         {
            System.IO.Directory.CreateDirectory(DataFolder);
         };
      };

      Debug.WriteLine($"Data: {DataFolder}");

      // - - -  - - - 

      ZPF.AT.AuditTrailViewModel.Current.Init(new ZPF.AT.JSONAuditTrailWriter(DataFolder + "AuditTrail.json", JSONAuditTrailWriter.FileTypes.PartialJSON));
      ZPF.AT.Log.WriteHeader("StoreCheck", VersionInfo.Current.sVersion, $"");
      //ZPF.AT.Log.Write(ZPF.AT.ErrorLevel.Log, $"{DeviceInfo.Manufacturer} - {DeviceInfo.Model} / {DeviceInfo.Platform} {DeviceInfo.VersionString} ");
      ZPF.AT.Log.Write(ZPF.AT.ErrorLevel.Log, "MainViewModel()");

      // - - -  - - - 

      //var mem = GC.GetTotalMemory(true);
      //Log.Write(new AuditTrail
      //{
      //   Level = ErrorLevel.Log,
      //   Tag = "MEM",
      //   Message = $"{mem:N0} bytes currently allocated in managed memory",
      //});

      ZPF.AT.AuditTrailViewModel.Current.MaxLines = 100;
      ZPF.AT.AuditTrailViewModel.Current.Clean();

      // - - -  - - - 

      Analytics.TrackEvent("MainViewModel()");

      // - - -  - - - 

      InitWS();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public void SaveInterv(Intervention_Params p)
   {
      SelectedIntervention.Parameters = null;

      var json = JsonSerializer.Serialize(p);

      string extractPath = ZPF.XF.Basics.Current.FileIO.CleanPath(MainViewModel.Current.DataFolder + @"Interventions\");

      if (!System.IO.Directory.Exists(extractPath))
      {
         System.IO.Directory.CreateDirectory(extractPath);
      };

      System.IO.File.WriteAllText(ZPF.XF.Basics.Current.FileIO.CleanPath($@"{MainViewModel.Current.DataFolder}Interventions\{MainViewModel.Current.SelectedIntervention.PK}.json"), json);

      MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.MinValue;
      MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   internal void UpdatePrevArticle()
   {
      OnPropertyChanged("PrevArticle");
   }

   public void UpdateCurrentArticle()
   {
      OnPropertyChanged("CurrentArticle");
   }

   public void InitWS()
   {
      wsHelper.Init();
      wsHelper.wsServer = wsServer;

      string BasicAuth = "StoreCheck:ZPF";

      if (!string.IsNullOrEmpty(BasicAuth))
      {
         byte[] byteArray = Encoding.ASCII.GetBytes(BasicAuth);
         wsHelper._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
      };

      wsHelper._httpClient.Timeout = TimeSpan.FromSeconds(60);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   bool _IsInternetAccessAvailable = false;

   public bool IsInternetAccessAvailable
   {
      get
      {
         _IsInternetAccessAvailable = ZPF.XF.Basics.Current.Network.IsInternetAccessAvailable();

         return _IsInternetAccessAvailable;
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public Params Config { get; private set; } = new Params();

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   /// <summary>
   /// Loads config, ini file, JSON, ...   
   /// </summary>
   /// <returns></returns>
   public bool LoadLocalConfig()
   {
      // - - - config - - -

      string FileName = DataFolder + @"StoreCheck.Params.json";
      Debug.WriteLine("load" + FileName);

      if (System.IO.File.Exists(FileName))
      {
         string json = File.ReadAllText(FileName);
         Debug.WriteLine(json);

         var p = JsonSerializer.Deserialize<Params>(json);
         if (p != null)
         {
            Config = p;
         };

         var p2 = System.Text.Json.JsonSerializer.Deserialize<Params>(json);
         if (p2 != null)
         {
            Config = p2;
         };
      }
      else
      {
         SaveLocalConfig();
      };

      OnPropertyChanged("Config");

      // - - -  - - -

      return true;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   /// <summary>
   /// Saves config ...
   /// </summary>
   public void SaveLocalConfig()
   {
      // - - -  - - -

      string FileName = DataFolder + @"StoreCheck.Params.json";
      Debug.WriteLine("write" + FileName);
      var json = System.Text.Json.JsonSerializer.Serialize(Config);
      Debug.WriteLine("write" + json);
      File.WriteAllText(FileName, json);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   /// <summary>
   /// Save application state and stop any background activity
   /// </summary>
   public void SaveApplicationState()
   {
      SaveLocalConfig();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public void SaveHints(TStrings Hints)
   {
      Config.HintsShown = Hints;

      SaveLocalConfig();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public static string GetFromResources(object sender, string resourceName)
   {
      var assembly = sender.GetType().GetTypeInfo().Assembly;
      using (Stream stream = assembly.GetManifestResourceStream(resourceName))
      {
         using (var reader = new StreamReader(stream))
         {
            return reader.ReadToEnd();
         }
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   internal bool Download(string login, bool Update = false)
   {
      Config.Login = login;

      // - - - Download referentiel - - -

      if (Update)
      {
         BackboneViewModel.Current.BusyTitle = "Init data ...";
      }
      else
      {
         BackboneViewModel.Current.BusyTitle = "Update data ...";
      };

      BackboneViewModel.Current.IncBusy();

      DoIt.OnBackground(async () =>
      {
         await SyncViewModel.Current.SyncDataWithWeb(login);

         DoIt.OnMainThread(() =>
         {
            BackboneViewModel.Current.DecBusy();
         });
      });

      return true;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public void LoadLocalDB()
   {
      string LoadJson(string FileName)
      {
         return System.IO.File.ReadAllText(DataFolder + FileName + ".json");
      };

      DoIt.OnMainThread(() =>
      {
         BackboneViewModel.Current.BusySubTitle = $"Load Interventions ...";
      });

      var e = JsonSerializer.Deserialize<Intervention_CE[]>(LoadJson("Interventions"));
      if (e != null) MainViewModel.Current.SetLocalInterventions(e.ToList());

      DoIt.OnMainThread(() =>
      {
         BackboneViewModel.Current.BusySubTitle = $"Load Documents ...";
      });
      var d = JsonSerializer.Deserialize<Document_CE[]>(LoadJson("Documents"));
      if (d != null) MainViewModel.Current.SetLocalDocuments(d.ToList());

      DoIt.OnMainThread(() =>
      {
         BackboneViewModel.Current.BusySubTitle = $"Load Stores ...";
      });
      var m = JsonSerializer.Deserialize<Store_CE[]>(LoadJson("Stores"));
      if (m != null) MainViewModel.Current.SetLocalStores(m.ToList());

      DoIt.OnMainThread(() =>
      {
         BackboneViewModel.Current.BusySubTitle = $"Load Articles ...";
      });
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
            json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true, });
         }
         catch (Exception ex)
         {
            json = ex.Message;
         };

         File.WriteAllText(DataFolder + FileName + ".json", json);
      }

      if (saveDBRange == DBRange.all || saveDBRange == DBRange.Documents) SaveListe("Documents", Documents);
      if (saveDBRange == DBRange.all || saveDBRange == DBRange.Interventions) SaveListe("Interventions", Interventions);
      if (saveDBRange == DBRange.all || saveDBRange == DBRange.Stores) SaveListe("Stores", Stores);
      //ToDo: if (saveDBRange == DBRange.all || saveDBRange == DBRange.Articles) SaveListe("ArticlesEAN", ArticlesEAN);
      if (saveDBRange == DBRange.all || saveDBRange == DBRange.Articles) SaveListe("Articles", Articles);
      if (saveDBRange == DBRange.all || saveDBRange == DBRange.Families) SaveListe("Families", Families);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public List<Document_CE> Documents { get; private set; } = new List<Document_CE>();

   public Document_CE SelectedDocument { get => _SelectedDocument; set => SetField(ref _SelectedDocument, value); }
   Document_CE _SelectedDocument = null;

   internal void SetLocalDocuments(List<Document_CE> list)
   {
      Documents = list;

      OnPropertyChanged("Documents");
   }

   internal void SetDocuments(List<Document_CE> list)
   {
      // - - - remove previously deleted items - - - 

      //{
      //   var l = Documents.Where(x => x.Actif == false && x.SyncOn != DateTime.MinValue && (DateTime.Now - x.UpdatedOn).TotalDays > 10).ToList();

      //   foreach (var doc in l)
      //   {
      //      //ToDo: delete photo "file"
      //      if (System.IO.File.Exists(doc.FullPath))
      //      {
      //         System.IO.File.Delete(doc.FullPath);
      //      };

      //      Documents.Remove(doc);
      //   };
      //};

      // - - - sync unsynced files - - - 

      //{
      //   var l = Documents.Where(x => x.SyncOn == DateTime.MinValue).ToList();

      //   foreach (var doc in l)
      //   {
      //      if (doc.Actif == false)
      //      {
      //         //ToDo: delete photo "file" ???
      //      }
      //      else
      //      {
      //         DoIt.OnBackground(async () =>
      //         {
      //            // - - - sync photo "file" - - - 

      //            if (await MainViewModel.Current.UploadDoc(doc.FullPath, global::Document.InternalDocumentTypes.Signature, doc.ExtType, doc.ExtRef, doc.Title, doc.GUID))
      //            {
      //               doc.SyncOn = DateTime.Now;
      //               MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Documents);
      //            }
      //            else
      //            {
      //               // Oups 
      //            };
      //         });
      //      };
      //   };
      //};

      //// - - -  - - - 

      //foreach (var doc in list)
      //{
      //   var d = Documents.Where(x => x.GUID == doc.GUID).FirstOrDefault();

      //   if (d != null)
      //   {
      //      //ToDo: Update
      //      //Documents.Remove(d);
      //      //Documents.Add(doc);
      //   }
      //   else
      //   {
      //      Documents.Add(doc);

      //      // ToDo: DownloadDoc
      //      //if (doc.InternalDocType != Document_CE.InternalDocumentTypes.FilePath)
      //      //{
      //      //   DoIt.OnBackground(async () =>
      //      //   {
      //      //      // - - - sync photo "file" - - - 

      //      //      //      if (await MainViewModel.Current.DownloadDoc(doc))
      //      //      //      {
      //      doc.SyncOn = DateTime.Now;
      //      MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Documents);
      //      //      //      }
      //      //      //      else
      //      //      //      {
      //      //      //         // Oups 
      //      //      //         Documents.Remove(doc);
      //      //      //      };
      //      //   });
      //      //};
      //   };
      //};

      // - - -  - - - 

      {
         foreach (var doc in list)
         {
            var d = Documents.Where(x => x.GUID == doc.GUID).FirstOrDefault();

            if (d != null)
            {
               if (doc.UpdatedOn > d.UpdatedOn)
               {
                  Documents.Remove(d);
                  doc.SyncOn = DateTime.Now;
                  Documents.Add(doc);
               };
            }
            else
            {
               doc.SyncOn = DateTime.Now;
               Documents.Add(doc);
            };
         };
      };

      // - - -  - - - 

      OnPropertyChanged("Documents");
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public List<Intervention_CE> Interventions { get; private set; } = new List<Intervention_CE>();

   public Intervention_Params SelectedInterventionParams { get => _SelectedInterventionParams; set => _SelectedInterventionParams = value; }
   Intervention_Params _SelectedInterventionParams = null;

   public Durex_Params durexParams { get; set; } = new Durex_Params();
   public QCM_woMenu_Params QCM_woMenuParams { get; set; } = new QCM_woMenu_Params();

   public BeforeAfter_Params BeforeAfter { get; set; } = new BeforeAfter_Params();
   public PhotoAuKM_Params PhotoAuKM { get; set; } = new PhotoAuKM_Params();

   public Intervention_CE SelectedIntervention
   {
      get => _SelectedIntervention;
      set
      {
         if (SetField(ref _SelectedIntervention, value))
         {
            Log.Write(new AuditTrail
            {
               Level = ErrorLevel.Log,
               Tag = "DATA",
               Message = $"SelectedIntervention PK={_SelectedIntervention.PK} ({_SelectedIntervention.ToString()})",
            });

            InterventionsViewModel.Current.LoadInterventionParams();
         };
      }
   }

   Intervention_CE _SelectedIntervention = null;

   internal void SetLocalInterventions(List<Intervention_CE> list)
   {
      Interventions = list;

      OnPropertyChanged("Interventions");
   }

   internal void SetInterventions(List<Intervention_CE> list)
   {
      // - - -  - - - 

      foreach (var interv in list)
      {
         var d = Interventions.Where(x => x.PK == interv.PK).FirstOrDefault();

         if (d != null)
         {
            if (interv.UpdatedOn > d.UpdatedOn)
            {
               Interventions.Remove(d);
               interv.SyncOn = DateTime.Now;
               Interventions.Add(interv);
            };
         }
         else
         {
            interv.SyncOn = DateTime.Now;
            Interventions.Add(interv);
         };
      };

      // - - -  - - - 

      OnPropertyChanged("Interventions");
   }

   internal void SetIntervention(string Ref)
   {
      SelectedIntervention = Interventions.Where(x => x.Ref == Ref).FirstOrDefault();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public List<Store_CE> Stores { get; private set; } = new List<Store_CE>();

   public Store_CE SelectedStore
   {
      get => _SelectedStore;
      set
      {
         if (SetField(ref _SelectedStore, value))
         {
            Log.Write(new AuditTrail
            {
               Level = ErrorLevel.Log,
               Tag = "DATA",
               Message = $"SelectedStore PK={_SelectedStore.PK} ({_SelectedStore.ToString()})",
            });
         };
      }
   }
   Store_CE _SelectedStore = null;

   internal void SetLocalStores(List<Store_CE> list)
   {
      Stores = list;

      OnPropertyChanged("Stores");
   }

   internal void SetStores(List<Store_CE> list)
   {
      foreach (var mag in list)
      {
         var d = Stores.Where(x => x.PK == mag.PK).FirstOrDefault();

         if (d != null)
         {
            if (mag.UpdatedOn > d.UpdatedOn)
            {
               Stores.Remove(d);
               Stores.Add(mag);
            };
         }
         else
         {
            Stores.Add(mag);
         };
      };

      OnPropertyChanged("Stores");
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public List<EAN_Article> Articles { get; private set; } = new List<EAN_Article>();

   public EAN_Article SelectedArticle { get => _SelectedArticle; set => SetField(ref _SelectedArticle, value); }
   EAN_Article _SelectedArticle = null;

   internal void SetLocalArticles(List<EAN_Article> list)
   {
      Articles = list;

      OnPropertyChanged("Articles");
   }

   internal void SetArticles(List<EAN_Article> list)
   {
      foreach (var mag in list)
      {
         var d = Articles.Where(x => x.PK == mag.PK).FirstOrDefault();

         if (d != null)
         {
            if (mag.UpdatedOn > d.UpdatedOn)
            {
               Articles.Remove(d);
               Articles.Add(mag);
            };
         }
         else
         {
            Articles.Add(mag);
         };
      };

      OnPropertyChanged("Articles");
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public List<EAN_Family> Families { get; private set; } = new List<EAN_Family>();

   public EAN_Family SelectedFamily { get => _SelectedFamily; set => SetField(ref _SelectedFamily, value); }
   EAN_Family _SelectedFamily = null;

   public bool TestUnitaires { get; internal set; }
   public Intervention_Params.Scann CurrentArticle { get; internal set; }
   public EAN_Family CurrentFamily { get; internal set; }
   public EAN_Family PrevFamily { get; internal set; }
   public Intervention_Params.Scann PrevArticle { get; internal set; }
   public string PrevData { get; internal set; }
   public string CurrentData { get; internal set; }
   public Intervention_Params.Scann ReplacementArticle { get; internal set; }
   public string DeviceID { get; set; } = "BidulSurLaRoute";

   public string EntryMsg { get => _EntryMsg; set => SetField(ref _EntryMsg, value); }
   string _EntryMsg = "";

   //internal void SetLocalFamilies(List<EAN_Family> list)
   //{
   //   Families = list;

   //   OnPropertyChanged("Families");
   //}

   //internal void SetFamilies(List<EAN_Family> list)
   //{
   //   foreach (var mag in list)
   //   {
   //      var d = Families.Where(x => x.PK == mag.PK).FirstOrDefault();

   //      if (d != null)
   //      {
   //         if (mag.UpdatedOn > d.UpdatedOn)
   //         {
   //            Families.Remove(d);
   //            Families.Add(mag);
   //         };
   //      }
   //      else
   //      {
   //         Families.Add(mag);
   //      };
   //   };

   //   OnPropertyChanged("Families");
   //}

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public async Task<bool> UploadDoc(string FileName, Document.InternalDocumentTypes DocType, string refType, string extRef, string title, string comment, string GUID)
   {
      refType = (string.IsNullOrEmpty(refType) ? "*" : refType);
      extRef = (string.IsNullOrEmpty(extRef) ? "*" : extRef);
      comment = (string.IsNullOrEmpty(comment) ? " " : comment);


      string URL = string.Format(@"{0}Docs/Up/{1}/{2}/{3}/{4}/{5}/{6}/{7}",
         wsServerDoc, System.IO.Path.GetFileName(FileName), DocType, refType, extRef, title, comment, GUID);

      {
         var content = new MultipartFormDataContent();

         FileName = ZPF.XF.Basics.Current.FileIO.CleanPath(FileName);
         using (var stream = new StreamReader(FileName).BaseStream)
         {
            var streamContent = new StreamContent(stream);

            streamContent.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse("form-data");
            streamContent.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("name", "contentFile"));
            streamContent.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("filename", "\"" + FileName + "\""));

            content.Add(streamContent);

            try
            {
               var r = await wsHelper._httpClient.PostAsync(URL, content);
               var wsResult = await r.Content.ReadAsStringAsync();

               if (!r.IsSuccessStatusCode)
               {
                  Debug.WriteLine(r.ReasonPhrase);
                  return false;
               };
            }
            catch (Exception ex)
            {
               Debug.WriteLine(ex.Message);

               return false;
            };
         };
      };

      return true;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public async Task<bool> DownloadDoc(Document_CE doc)
   {
      if (doc.InternalDocType != Document.InternalDocumentTypes.FilePath)
      {
         if (!System.IO.Directory.Exists(MainViewModel.Current.DataFolder + @"/Photos/"))
         {
            System.IO.Directory.CreateDirectory(MainViewModel.Current.DataFolder + @"/Photos/");
         };

         doc.FullPath = MainViewModel.Current.DataFolder + @"/Photos/" + doc.FileName;
         doc.FullPath = ZPF.XF.Basics.Current.FileIO.CleanPath(doc.FullPath);

         return await SyncViewModel.Current.wsHelperwGetDownload(string.Format("/DownloadDoc/{0}/{1}", doc.InternalDocType, doc.GUID), doc.FullPath);
      };

      return false;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public long Memory = -1;

   public void LogMemory(string txt)
   {
      var mem = GC.GetTotalMemory(false);

#if DEBUG
      if (Memory != -1)
      {
         Log.Write(new AuditTrail
         {
            Level = ErrorLevel.Log,
            Tag = "Mem",
            Message = $"({txt}) base {Memory:N0} current {mem:N0} => {Memory - mem:N0} bytes ",
         });
      };
#endif
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
