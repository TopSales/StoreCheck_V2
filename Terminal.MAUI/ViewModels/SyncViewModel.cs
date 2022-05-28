
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZPF.AT;
using ZPF.SQL;

namespace ZPF
{
   public class SyncViewModel : BaseViewModel
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private static SyncViewModel _Current = null;

      public static SyncViewModel Current
      {
         get
         {
            if (_Current == null)
            {
               _Current = new SyncViewModel();
            };

            return _Current;
         }

         set
         {
            _Current = value;
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public SyncViewModel()
      {
         _Current = this;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public string LastError { get; set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }


   //class old
   //{
   //   public string LastError { get; set; }
   //   public async Task<T> wsHelperwGet<T>(string Function, [CallerMemberName] string memberName = "", bool Zipped = false)
   //   {
   //      DateTime dt = DateTime.Now;

   //      var Result = await wsHelper.wGet<T>(Function, Zipped: Zipped);

   //      TimeSpan ts = DateTime.Now - dt;

   //      if (Result == null)
   //      {
   //         Log.Write(ErrorLevel.Error, $"{memberName} - {Function} - {wsHelper.LastError} ");

   //         return default(T);
   //      }
   //      else
   //      {
   //         if (Result is Array)
   //         {
   //            Log.Write(ErrorLevel.Log, $"{memberName} - {Function} - dwnl {(Result as Array).Length} items in {ts.TotalSeconds} sec");
   //         }
   //         else
   //         {
   //            Log.Write(ErrorLevel.Log, $"{memberName} - {Function} - dwnl in {ts.TotalSeconds} sec");
   //         };
   //      };

   //      return Result;
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   public async Task<string> UploadInter(Intervention_CE tmpInter)
   //   {
   //      var json = JsonSerializer.Serialize(tmpInter);

   //      if (json == null)
   //      {
   //         return "ko";
   //      };

   //      string FileName = System.IO.Path.GetTempFileName();

   //      System.IO.File.WriteAllText(FileName, json);

   //      string URL = $"{MainViewModel.wsServer}Interventions/UpdateViaDoc/{MainViewModel.Current.Config.UserFK}/{tmpInter.PK}";

   //      {
   //         var content = new MultipartFormDataContent();

   //         using (var stream = new StreamReader(FileName).BaseStream)
   //         {
   //            var streamContent = new StreamContent(stream);

   //            streamContent.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse("form-data");
   //            streamContent.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("name", "contentFile"));
   //            streamContent.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("filename", "\"" + FileName + "\""));

   //            content.Add(streamContent);

   //            try
   //            {
   //               var r = await wsHelper._httpClient.PostAsync(URL, content);
   //               var wsResult = await r.Content.ReadAsStringAsync();

   //               if (!r.IsSuccessStatusCode)
   //               {
   //                  Debug.WriteLine(r.ReasonPhrase);
   //                  return "ko";
   //               };
   //            }
   //            catch (Exception ex)
   //            {
   //               Debug.WriteLine(ex.Message);

   //               return "ko";
   //            };
   //         };
   //      };

   //      return "ok";
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   public async Task<bool> wsHelperwGetDownload(string Function, string FilePath, [CallerMemberName] string memberName = "")
   //   {
   //      DateTime dt = DateTime.Now;

   //      var Result = await wsHelper.wGetDownload(Function, FilePath);

   //      TimeSpan ts = DateTime.Now - dt;

   //      if (Result)
   //      {
   //         Log.Write(ErrorLevel.Log, $"{memberName} - {Function} - dwnl in {ts.TotalSeconds} sec");
   //      }
   //      else
   //      {
   //         Log.Write(ErrorLevel.Error, $"{memberName} - {Function} - {wsHelper.LastError} ");
   //      };

   //      return Result;
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   public async Task<bool> SyncStores()
   //   {
   //      LastError = "";

   //      //var list = await wsHelperwGet<Store_CE[]>($@"/Store/List/{MainViewModel.Current.Config.LastSynchro.ToString("dd.MM.yyyy HH:mm")}", Zipped: true);
   //      var list = await wsHelperwGet<Store_CE[]>($@"/Store/List/{DateTime.MinValue.ToString("dd.MM.yyyy HH:mm")}", Zipped: true);

   //      if (list != null)
   //      {
   //         MainViewModel.Current.SetLocalStores(list.ToList());
   //         return true;
   //      }
   //      else
   //      {
   //         LastError = "SyncStores: Problem when communicating with the server:" + Environment.NewLine + LastError + Environment.NewLine + wsHelper.LastError;
   //         BackboneViewModel.Current.MessageBox(LastError);
   //         return false;
   //      };
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   public async Task<ChunkInfo> InitSyncArticlesEAN()
   //   {
   //      LastError = "";

   //      var st = await wsHelper.wGet($@"/ArticlesEAN/Init");

   //      if (st != null)
   //      {
   //         return JsonSerializer.Deserialize<ChunkInfo>(st, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
   //      }
   //      else
   //      {
   //         LastError = "InitSyncArticlesEAN: Problem when communicating with the server:" + Environment.NewLine + LastError + Environment.NewLine + wsHelper.LastError;
   //         BackboneViewModel.Current.MessageBox(LastError);
   //         return null;
   //      };
   //   }

   //   public async Task<bool> SyncArticlesEAN(int chunk, int RowsPPage)
   //   {
   //      LastError = "";

   //      var list = await wsHelperwGet<EAN_Article[]>($@"/ArticlesEAN/GetChunk/{chunk}/{RowsPPage}", Zipped: true);

   //      if (list != null)
   //      {
   //         if (chunk == 0)
   //         {
   //            EANViewModel.Current.SetLocalArticlesEAN(list.ToList());
   //         }
   //         else
   //         {
   //            EANViewModel.Current.AddLocalArticlesEAN(list.ToList());
   //         };

   //         return true;
   //      }
   //      else
   //      {
   //         LastError = "SyncArticlesEAN: Problem when communicating with the server:" + Environment.NewLine + LastError + Environment.NewLine + wsHelper.LastError;
   //         BackboneViewModel.Current.MessageBox(LastError);
   //         return false;
   //      };
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   public async Task<bool> UploadData()
   //   {
   //      bool Result = false;

   //      Result = Result || await UploadDocuments();
   //      Result = Result || await UploadInterventions();

   //      return Result;
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   public bool HasUnsyncedData()
   //   {
   //      bool Result = false;

   //      // Documents
   //      Result = Result || MainViewModel.Current.Documents.Where(x => x.SyncOn == DateTime.MinValue).Count() > 0;

   //      // Interventions
   //      Result = Result || MainViewModel.Current.Interventions.Where(x => x.SyncOn == DateTime.MinValue).Count() > 0;

   //      return Result;
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   public async Task<bool> UploadDocuments()
   //   {
   //      var l = MainViewModel.Current.Documents.Where(x => x.SyncOn == DateTime.MinValue).ToList();

   //      foreach (var doc in l)
   //      {
   //         BackboneViewModel.Current.BusyTitle = $"upload ({l.IndexOf(doc)}/{l.Count}) ...";

   //         if (await MainViewModel.Current.UploadDoc(doc.FullPath, global::Document.InternalDocumentTypes.image, doc.ExtType, doc.ExtRef, doc.Title, doc.Comment, doc.GUID))
   //         {
   //            doc.SyncOn = DateTime.Now;
   //            MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Documents);
   //         }
   //         else
   //         {
   //            Log.Write(ErrorLevel.Error, $"{MethodBase.GetCurrentMethod().DeclaringType.FullName} - {wsHelper.LastError} ");

   //            LastError = "SyncDocuments: Problem when communicating with the server:" + Environment.NewLine + LastError + Environment.NewLine + wsHelper.LastError;

   //            BackboneViewModel.Current.BusyTitle = "working ...";
   //            BackboneViewModel.Current.MessageBox(LastError);
   //            return false;
   //         };
   //      };

   //      BackboneViewModel.Current.BusyTitle = "working ...";

   //      return true;
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   public async Task<bool> SyncArticles()
   //   {
   //      LastError = "";

   //      //var list = await wsHelperwGet<EAN_Article[]>($@"/Articles/List/{MainViewModel.Current.Config.LastSynchro.ToString("dd.MM.yyyy HH:mm")}", Zipped: true);
   //      var list = await wsHelperwGet<EAN_Article[]>($@"/Articles/List/{DateTime.MinValue.ToString("dd.MM.yyyy HH:mm")}", Zipped: true);

   //      if (list != null)
   //      {
   //         MainViewModel.Current.SetLocalArticles(list.ToList());
   //         return true;
   //      }
   //      else
   //      {
   //         LastError = "SyncArticles: Problem when communicating with the server:" + Environment.NewLine + LastError + Environment.NewLine + wsHelper.LastError;
   //         BackboneViewModel.Current.MessageBox(LastError);
   //         return false;
   //      };
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   public async Task<bool> SyncAuditTrail(string FKUser)
   //   {
   //      LastError = "";

   //      // - - - sync AuditTrail - - - 

   //      // https://github.com/ZeProgFactory/StoreCheck/issues/108
   //      string FileName = System.IO.Path.GetTempFileName(); // MainViewModel.Current.DataFolder + "AuditTrail.csv";

   //      AuditTrailViewModel.Current.LoadAuditTrail(false, ZPF.AT.AuditTrailViewModel.Current.MaxLines);

   //      AboutPage.ListToCSV(AuditTrailViewModel.Current.AuditTrail.ToList(), FileName);

   //      // - - - sync AuditTrail - - - 

   //      if (await MainViewModel.Current.UploadDoc(FileName, global::Document.InternalDocumentTypes.doc, "User", MainViewModel.Current.Config.UserFK.ToString(), "AuditTrail.csv", "", Guid.NewGuid().ToString()))
   //      {
   //         AuditTrailViewModel.Current.Clean();

   //         try
   //         {
   //            System.IO.File.Delete(FileName);
   //         }
   //         catch { };
   //      };

   //      // - - -  - - - 

   //      return true;
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   public async Task<bool> SyncDocuments(string FKUser)
   //   {
   //      LastError = "";

   //      // - - - sync unsynced files - - - 

   //      await UploadDocuments();

   //      // - - -  - - - 

   //      try
   //      {
   //         foreach (var inter in MainViewModel.Current.Interventions)
   //         {
   //            BackboneViewModel.Current.BusyTitle = $"working ({MainViewModel.Current.Interventions.IndexOf(inter)}/{MainViewModel.Current.Interventions.Count}) ...";

   //            var list = await wsHelperwGet<Document_CE[]>($@"/Documents/List/{MainViewModel.Current.Config.LastSynchro.ToString("dd.MM.yyyy HH:mm")}/{inter.PK}");
   //            Log.Write( ErrorLevel.Info, $"/Documents/List/{MainViewModel.Current.Config.LastSynchro.ToString("dd.MM.yyyy HH:mm")}/{inter.PK} - {wsHelper.LastTripDuration.TotalSeconds} s - {wsHelper.LastReceivedDataSize} bytes" );

   //            if (list != null)
   //            {
   //               MainViewModel.Current.SetDocuments(list.ToList());
   //               //return true;
   //            }
   //            else
   //            {
   //               LastError = "DocList: Problem when communicating with the server:" 
   //                  + Environment.NewLine 
   //                  + LastError
   //                  + Environment.NewLine
   //                  + Environment.NewLine
   //                  + "Please try again in a few minutes ...";

   //               BackboneViewModel.Current.MessageBox(LastError);
   //               return false;
   //            };
   //         };

   //         BackboneViewModel.Current.BusyTitle = "working ...";

   //         return true;
   //      }
   //      catch (Exception ex)
   //      {
   //         Log.Write(new AuditTrail(ex) { Level = ErrorLevel.Error, Message = $"{MethodBase.GetCurrentMethod().DeclaringType.FullName} - {ex.Message}" });
   //         Debug.WriteLine(ex.Message);
   //         return false;
   //      };
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   public async Task<bool> SyncInterventions(string FKUser)
   //   {
   //      LastError = "";

   //      // - - - sync unsynced files - - - 

   //      if (MainViewModel.Current.Interventions != null)
   //      {
   //         await UploadInterventions();
   //      };

   //      // - - -  - - - 

   //      try
   //      {
   //         //var list = await wsHelperwGet<Intervention_CE[]>($@"/Interventions/ListInt/{MainViewModel.Current.Config.LastSynchro.ToString("dd.MM.yyyy HH:mm")}/{FKUser}", Zipped: true);
   //         //var list = await wsHelperwGet<Intervention_CE[]>($@"/Interventions/ListInt/{DateTime.MinValue.ToString("dd.MM.yyyy HH:mm")}/{FKUser}", Zipped: true);
   //         var list = await wsHelperwGet<Intervention_CE[]>($@"/Interventions/ListIntNP/{DateTime.MinValue.ToString("dd.MM.yyyy HH:mm")}/{FKUser}", Zipped: true);

   //         if (list != null)
   //         {
   //            MainViewModel.Current.SetInterventions(list.ToList());
   //            //return true;
   //         }
   //         else
   //         {
   //            LastError = "ListInter 1: Problem when communicating with the server:" + Environment.NewLine + LastError;
   //            BackboneViewModel.Current.MessageBox(LastError);
   //            return false;
   //         };
   //      }
   //      catch (Exception ex)
   //      {
   //         Log.Write(new AuditTrail(ex) { Level = ErrorLevel.Error, Message = $"{MethodBase.GetCurrentMethod().DeclaringType.FullName} - {ex.Message}" });
   //         LastError = "ListInter 2: Problem when communicating with the server:" + Environment.NewLine + ex.Message;
   //         BackboneViewModel.Current.MessageBox(LastError);
   //         return false;
   //      };

   //      // - - - download ZIP - - -
   //      // http://localhost:52158/Expertises/GetSiteDB?USR=ME&Ref=91470-ZPF-Limours

   //      //BackboneViewModel.Current.BusyTitle = "Download params ...";
   //      //Log.Write("SiteViewModel.SetSelectedSite", BackboneViewModel.Current.BusyTitle);

   //      //var list = await wsHelperwGet<Intervention_CE[]>($@"/Interventions/ListIntNP/{DateTime.MinValue.ToString("dd.MM.yyyy HH:mm")}/{FKUser}", Zipped: true);
   //      string URL = string.Format($@"Interventions/ListIntParams/{DateTime.MinValue.ToString("dd.MM.yyyy HH:mm")}/{FKUser}");

   //      try
   //      {
   //         using (var httpClient = new HttpClient())
   //         {
   //            string BasicAuth = "StoreCheck:ZPF";
   //            string zipPath = MainViewModel.Current.DataFolder + "Interventions.zip";
   //            string extractPath = ZPF.XF.Basics.Current.FileIO.CleanPath(MainViewModel.Current.DataFolder + @"Interventions\");

   //            if (!System.IO.Directory.Exists(extractPath))
   //            {
   //               System.IO.Directory.CreateDirectory(extractPath);
   //            };

   //            if (!string.IsNullOrEmpty(BasicAuth))
   //            {
   //               byte[] bArray = Encoding.ASCII.GetBytes(BasicAuth);
   //               httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(bArray));
   //            };

   //            //ToDo: timemout ...
   //            httpClient.Timeout = TimeSpan.FromSeconds(60);
   //            byte[] byteArray = await httpClient.GetByteArrayAsync(MainViewModel.wsServer + URL);

   //            ZPF.XF.Basics.Current.FileIO.WriteStream(new MemoryStream(byteArray), zipPath);

   //            // - - - clean the folder - - - 

   //            try
   //            {
   //               System.IO.Directory.Delete(extractPath, true);

   //               if (!System.IO.Directory.Exists(extractPath))
   //               {
   //                  System.IO.Directory.CreateDirectory(extractPath);
   //               };
   //            }
   //            catch { }

   //            // - - -  - - - 

   //            ZipFile.ExtractToDirectory(zipPath, extractPath);
   //         };
   //      }
   //      catch (Exception ex)
   //      {
   //         Log.Write(new AuditTrail(ex) { Level = ErrorLevel.Error, Message = $"{MethodBase.GetCurrentMethod().DeclaringType.FullName} - {ex.Message}" });
   //         LastError = "Problem when communicating with the server:" + Environment.NewLine + ex.Message;
   //         BackboneViewModel.Current.MessageBox(LastError);

   //         return false;
   //      };

   //      return true;
   //   }


   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   private async Task<bool> UploadInterventions()
   //   {
   //      var l = MainViewModel.Current.Interventions.Where(x => x.SyncOn == DateTime.MinValue).ToList();

   //      foreach (var interv in l)
   //      {
   //         //ToDo: ??? wsHelper.wPost
   //         wsHelper._httpClient.Dispose();
   //         wsHelper._httpClient = null;
   //         MainViewModel.Current.InitWS();

   //         Intervention_CE tmpInter = interv.Copy();

   //         if (tmpInter != null)
   //         {
   //            tmpInter.Parameters = "";
   //            tmpInter.Input = "";

   //            //synchro
   //            //var r = await wsHelper.wPost_String(@"/Interventions/Update", JsonSerializer.Serialize(tmpInter));
   //            var r = await SyncViewModel.Current.UploadInter(tmpInter);

   //            if (r == "ok")
   //            {
   //               interv.SyncOn = DateTime.Now;
   //               MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
   //            }
   //            else
   //            {
   //               Log.Write(ErrorLevel.Error, $"{MethodBase.GetCurrentMethod().DeclaringType.FullName} - {wsHelper.LastError} ");

   //               LastError = "SyncInterventions: Problem when communicating with the server:" + Environment.NewLine + LastError + Environment.NewLine + wsHelper.LastError;
   //               BackboneViewModel.Current.MessageBox(LastError);
   //               return false;
   //            };
   //         }
   //         else
   //         {
   //            // oups ...
   //         };
   //      };

   //      return true;
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //   public async Task SyncDataWithWeb(string login)
   //   {
   //      var dt = DateTime.Now;

   //      Log.Write(ErrorLevel.Info, $"SyncDataWithWeb({login})");

   //      LastError = "";

   //      // - - -  - - - 

   //      if (string.IsNullOrEmpty(SyncViewModel.Current.LastError)) await SyncAuditTrail(MainViewModel.Current.Config.UserFK.ToString());

   //      DoIt.OnMainThread(() =>
   //      {
   //         if (string.IsNullOrEmpty(SyncViewModel.Current.LastError))
   //         {
   //            BackboneViewModel.Current.BusySubTitle = $"AuditTrail OK \n";
   //         }
   //         else
   //         {
   //            return;
   //         };
   //      });

   //      // - - -  - - - 

   //      if (string.IsNullOrEmpty(SyncViewModel.Current.LastError)) await SyncInterventions(MainViewModel.Current.Config.UserFK.ToString());

   //      DoIt.OnMainThread(() =>
   //      {
   //         if (string.IsNullOrEmpty(SyncViewModel.Current.LastError))
   //         {
   //            BackboneViewModel.Current.BusySubTitle = $"Interventions {MainViewModel.Current.Interventions.Count} \n";
   //         }
   //         else
   //         {
   //            return;
   //         };
   //      });

   //      // - - -  - - - 

   //      if (string.IsNullOrEmpty(SyncViewModel.Current.LastError)) await SyncDocuments(MainViewModel.Current.Config.UserFK.ToString());

   //      DoIt.OnMainThread(() =>
   //      {
   //         if (string.IsNullOrEmpty(SyncViewModel.Current.LastError))
   //         {
   //            BackboneViewModel.Current.BusySubTitle = $"Documents {MainViewModel.Current.Documents.Count} \n";
   //         }
   //         else
   //         {
   //            return;
   //         };
   //      });

   //      // - - -  - - - 

   //      if (string.IsNullOrEmpty(SyncViewModel.Current.LastError)) await SyncStores();

   //      DoIt.OnMainThread(() =>
   //      {
   //         if (string.IsNullOrEmpty(SyncViewModel.Current.LastError))
   //         {
   //            BackboneViewModel.Current.BusySubTitle = $"Stores {MainViewModel.Current.Stores.Count} \n";
   //         }
   //         else
   //         {
   //            return;
   //         };
   //      });

   //      // - - -  - - - 

   //      if (string.IsNullOrEmpty(SyncViewModel.Current.LastError))
   //      {
   //         DoIt.OnMainThread(() =>
   //         {
   //            BackboneViewModel.Current.BusySubTitle = $"Save local data ... \n";
   //         });

   //         var dt2 = DateTime.Now;
   //         MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.all);

   //         MainViewModel.Current.Config.LastSynchro = DateTime.Now;
   //         MainViewModel.Current.SaveLocalConfig();

   //         Log.Write(new AuditTrail
   //         {
   //            Level = ErrorLevel.Log,
   //            Tag = "ws",
   //            Message = $"Save in {(DateTime.Now - dt2).TotalSeconds} sec",
   //         });
   //      }
   //      else
   //      {
   //         return;
   //      };

   //      // - - -  - - - 

   //      Log.Write(new AuditTrail
   //      {
   //         Level = ErrorLevel.Log,
   //         Tag = "ws",
   //         Message = $"Sync in {(DateTime.Now - dt).TotalSeconds} sec",
   //      });
   //   }

   //   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   //}

}
