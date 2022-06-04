using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ZPF;
using static Intervention_Params;

public class InterventionsViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static InterventionsViewModel _Current = null;

   public static InterventionsViewModel Current
   {
      get
      {
         if (_Current == null)
         {
            _Current = new InterventionsViewModel();
         };

         return _Current;
      }

      set
      {
         _Current = value;
      }
   }

   public List<Durex_SKU> durexScanns { get; set; }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public InterventionsViewModel()
   {
      _Current = this;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   /// <summary>
   /// Start intervention
   /// </summary>
   public async Task<bool> Start(decimal? km)
   {
      if (MainViewModel.Current.SelectedIntervention.DateEndIntervention == DateTime.MinValue)
      {
         MainViewModel.Current.SelectedIntervention.DateBeginIntervention = DateTime.Now;

         if (km != null)
         {
            MainViewModel.Current.SelectedIntervention.NbKM = (decimal)km;
         };
         MainViewModel.Current.SelectedIntervention.UpdatedOn = DateTime.Now;
         MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.MinValue;

         MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);

         //wsHelper._httpClient.Dispose();
         //wsHelper._httpClient = null;
         //MainViewModel.Current.InitWS();

         var tmpInter = MainViewModel.Current.SelectedIntervention.Copy();
         tmpInter.Parameters = "";
         tmpInter.Input = "";

         // send begin 
         if (MainViewModel.Current.IsInternetAccessAvailable &&
            await SyncViewModel.Current.UploadInter(tmpInter) == "ok")
         {
            MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.Now;
         };

         MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
         return true;
      }
      else
      {
         MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
         return false;
      };
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   /// <summary>
   /// Terminates a intervention
   /// </summary>
   public async void Stop()
   {
      MainViewModel.Current.SelectedIntervention.DateEndIntervention = DateTime.Now;
      MainViewModel.Current.SelectedIntervention.IsClosed = true;
      MainViewModel.Current.SelectedIntervention.UpdatedOn = DateTime.Now;
      MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.MinValue;

      MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);

      //wsHelper._httpClient.Dispose();
      //wsHelper._httpClient = null;
      //MainViewModel.Current.InitWS();

      var tmpInter = MainViewModel.Current.SelectedIntervention.Copy();
      tmpInter.Parameters = "";
      tmpInter.Input = "";

      // send validation "stop"
      if (MainViewModel.Current.IsInternetAccessAvailable &&
            await SyncViewModel.Current.UploadInter(tmpInter) == "ok")
      {
         MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.Now;
      };

      MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   internal void SaveMCE(string mce)
   {
      if (string.IsNullOrEmpty(mce))
      {
         return;
      };

      FKActionTypes ActionType = (FKActionTypes)(MainViewModel.Current.SelectedIntervention.FKActionType);

      switch (ActionType)
      {

         case FKActionTypes.Durex:
            var s = durexScanns.Last();
            if (s != null)
            {
               s.Output = mce;
            };

            MainViewModel.Current.SelectedIntervention.Output = JsonSerializer.Serialize(durexScanns);
            MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.MinValue;

            MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
            break;

         case FKActionTypes.QCM_woMenu:
            MainViewModel.Current.SelectedIntervention.Output = mce;
            MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.MinValue;

            MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
            break;

            
         case FKActionTypes.BeforeAfter:
         case FKActionTypes.PhotoAuKM:
         default:
            MainViewModel.Current.SelectedIntervention.Output = mce;
            MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.MinValue;

            MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
            break;
      };

   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public void AddToScann(Intervention_Params.Scann scann)
   {
      FKActionTypes ActionType = (FKActionTypes)(MainViewModel.Current.SelectedIntervention.FKActionType);

      switch (ActionType)
      {
         case FKActionTypes.BeforeAfter:
         case FKActionTypes.PhotoAuKM:
            // should not happen ...
            System.Diagnostics.Debugger.Break();
            return;

         case FKActionTypes.Durex:

            durexScanns.Add(new Durex_SKU
            {
               EAN = scann.EAN,
               Content = scann.Condi,
               RealPrice = scann.Price,
               Facing = scann.Facing,
               Name = scann.Name,
               QTReplaced = MainViewModel.Current.CurrentArticle.QTReplaced,
            });

            MainViewModel.Current.SelectedIntervention.Output = JsonSerializer.Serialize(durexScanns);
            MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.MinValue;

            MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
            break;

         case FKActionTypes.QCM_woMenu:
            // no scann
            break;

         default:
            var p = MainViewModel.Current.SelectedInterventionParams;

            p.Data.Scanns.Add(scann);
            MainViewModel.Current.SaveInterv(p);
            break;
      };

      // - - -  - - -

      MainViewModel.Current.PrevData = MainViewModel.Current.CurrentData;
      MainViewModel.Current.PrevFamily = MainViewModel.Current.CurrentFamily;
      MainViewModel.Current.PrevArticle = MainViewModel.Current.CurrentArticle;
      MainViewModel.Current.UpdatePrevArticle();

      MainViewModel.Current.CurrentArticle = null;
      MainViewModel.Current.UpdateCurrentArticle();

      UnitechViewModel.Current.Data = "";
      UnitechViewModel.Current.Symbology = UnitechViewModel.Symbologies.Unknown;
      UnitechViewModel.Current.Length = 0;

      System.GC.Collect();
   }

   public int ScannCount()
   {
      FKActionTypes ActionType = (FKActionTypes)(MainViewModel.Current.SelectedIntervention.FKActionType);

      switch (ActionType)
      {
         case FKActionTypes.BeforeAfter:
         case FKActionTypes.PhotoAuKM:
            return -1;

         case FKActionTypes.Durex:
            return durexScanns.Count();

         default:
            var p = MainViewModel.Current.SelectedInterventionParams;
            return p.Data.Scanns.Count();
      };
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   internal void LoadInterventionParams()
   {
      FKActionTypes ActionType = (FKActionTypes)(MainViewModel.Current.SelectedIntervention.FKActionType);

      switch (ActionType)
      {
         case FKActionTypes.BeforeAfter:
            {
               MainViewModel.Current.SelectedInterventionParams = new Intervention_Params();
               MainViewModel.Current.SelectedInterventionParams.Data = new Intervention_Params.TData();
               //MainViewModel.Current.SelectedInterventionParams.Data.Scanns = new List<Intervention_Params.Scann>();

               MainViewModel.Current.SelectedInterventionParams.FKActionType = (long)FKActionTypes.BeforeAfter;

               // - - -  - - -

               if (string.IsNullOrEmpty(MainViewModel.Current.SelectedIntervention.Parameters))
               {
                  MainViewModel.Current.BeforeAfter = new BeforeAfter_Params();
               }
               else
               {
                  MainViewModel.Current.BeforeAfter = JsonSerializer.Deserialize<BeforeAfter_Params>(MainViewModel.Current.SelectedIntervention.Parameters);
               };

               MainViewModel.Current.BeforeAfter.Input = MainViewModel.Current.SelectedIntervention.Input;
            };
            break;

         case FKActionTypes.PhotoAuKM:
            {
               MainViewModel.Current.SelectedInterventionParams = new Intervention_Params();
               MainViewModel.Current.SelectedInterventionParams.Data = new Intervention_Params.TData();
               //MainViewModel.Current.SelectedInterventionParams.Data.Scanns = new List<Intervention_Params.Scann>();

               MainViewModel.Current.SelectedInterventionParams.FKActionType = (long)FKActionTypes.PhotoAuKM;

               // - - -  - - -

               if (string.IsNullOrEmpty(MainViewModel.Current.SelectedIntervention.Parameters))
               {
                  MainViewModel.Current.PhotoAuKM = new PhotoAuKM_Params();
               }
               else
               {
                  MainViewModel.Current.PhotoAuKM = JsonSerializer.Deserialize<PhotoAuKM_Params>(MainViewModel.Current.SelectedIntervention.Parameters);
               };

               MainViewModel.Current.BeforeAfter.Input = MainViewModel.Current.SelectedIntervention.Input;
            };
            break;

         case FKActionTypes.Durex:
            {
               MainViewModel.Current.SelectedInterventionParams = new Intervention_Params();
               MainViewModel.Current.SelectedInterventionParams.Data = new Intervention_Params.TData();
               MainViewModel.Current.SelectedInterventionParams.Data.Scanns = new List<Intervention_Params.Scann>();

               MainViewModel.Current.SelectedInterventionParams.FKActionType = (long)FKActionTypes.Durex;

               // - - -  - - -

               if (string.IsNullOrEmpty(MainViewModel.Current.SelectedIntervention.Parameters))
               {
                  MainViewModel.Current.durexParams = new Durex_Params();
               }
               else
               {
                  MainViewModel.Current.durexParams = JsonSerializer.Deserialize<Durex_Params>(MainViewModel.Current.SelectedIntervention.Parameters);
               };

               if (string.IsNullOrEmpty(MainViewModel.Current.SelectedIntervention.Output))
               {
                  durexScanns = new List<Durex_SKU>();
               }
               else
               {
                  durexScanns = JsonSerializer.Deserialize<List<Durex_SKU>>(MainViewModel.Current.SelectedIntervention.Output);
               };

               MainViewModel.Current.durexParams.Input = MainViewModel.Current.SelectedIntervention.Input;

               #region old
               //MainViewModel.Current.durexParams.SKUsLists.Add(new Durex_Params.StoreTypeList());
               //MainViewModel.Current.durexParams.SKUsLists.First().SKUs.Clear();
               //MainViewModel.Current.durexParams.SKUsLists.First().SKUs.Add(new Durex_SKU { EAN = "5038483225400", Name = "DUREX Love 6", ConsumerPrice = 5.99m, Mandatory = true });
               //MainViewModel.Current.durexParams.SKUsLists.First().SKUs.Add(new Durex_SKU { EAN = "5052197023848", Name = "DUREX NUDE Latex Free 10", ConsumerPrice = 14.99m, Mandatory = true });
               //MainViewModel.Current.durexParams.SKUsLists.First().SKUs.Add(new Durex_SKU { EAN = "5410036306703", Name = "DUREX NUDE Classic 10", ConsumerPrice = 14.99m, Mandatory = true });
               //MainViewModel.Current.durexParams.SKUsLists.First().SKUs.Add(new Durex_SKU { EAN = "3059948002413", Name = "DUREX Perfect Gliss 50ml", ConsumerPrice = 13.59m, Mandatory = true, ToReplace = true, ReplacementEAN = "3059948002406" });
               //MainViewModel.Current.durexParams.SKUsLists.First().SKUs.Add(new Durex_SKU { EAN = "3059948002406", Name = "DUREX Perfect Gliss 100ml", ConsumerPrice = 15.59m });

               //MainViewModel.Current.durexParams.PhotoConcurence = "Veet";
               //MainViewModel.Current.durexParams.PhotoConcurence_NB = 2;
               //MainViewModel.Current.durexParams.PhotoConcurence_DocComment = "Veet";

               //MainViewModel.Current.durexParams.PhotoBefore = "Durex on arrival";
               //MainViewModel.Current.durexParams.PhotoBefore_NB = 2;
               //MainViewModel.Current.durexParams.PhotoBefore_DocComment = "Arrival";

               //MainViewModel.Current.durexParams.PhotoAfter = "Durex on departure";
               //MainViewModel.Current.durexParams.PhotoAfter_NB = 2;
               //MainViewModel.Current.durexParams.PhotoAfter_DocComment = "Departure";

               //MainViewModel.Current.durexParams.Photo_DocComment = "Replacement";

               // les skus
               //MainViewModel.Current.durexParams.Input = @"{
               //   ""Forms"": [
               //      {
               //         ""ID"": 10,
               //         ""Caption"": ""Durex"",
               //         ""PrevCaption"": "" "",
               //         ""PrevID"": 10,
               //         ""NextCaption"": ""OK"",
               //         ""NextID"": -1,
               //         ""Scroll"": true
               //      }
               //   ],
               //   ""Items"": [
               //      {
               //         ""ID"": 1000,
               //         ""FormID"": 10,
               //         ""Caption"": ""Emplacement du produit:"",
               //         ""Type"": """",
               //         ""Mandatory"": false
               //      },
               //      {
               //         ""ID"": 1002,
               //         ""FormID"": 10,
               //         ""Caption"": ""rayon"",
               //         ""Type"": ""Radio"",
               //         ""Name"": ""Emplacement"",
               //         ""Value"": ""R"",
               //         ""Mandatory"": false
               //      },
               //      {
               //   ""ID"": 1004,
               //         ""FormID"": 10,
               //         ""Caption"": ""caisse"",
               //         ""Type"": ""Radio"",
               //         ""Name"": ""Emplacement"",
               //         ""Value"": ""10"",
               //         ""Mandatory"": false
               //      },
               //      {
               //   ""ID"": 1006,
               //         ""FormID"": 10,
               //         ""Caption"": ""vitrine"",
               //         ""Type"": ""Radio"",
               //         ""Name"": ""Emplacement"",
               //         ""Value"": ""15"",
               //         ""Mandatory"": false
               //      },
               //      {
               //   ""ID"": 1008,
               //         ""FormID"": 10,
               //         ""Caption"": ""autre"",
               //         ""Type"": ""Radio"",
               //         ""Name"": ""Emplacement"",
               //         ""Value"": ""20"",
               //         ""Mandatory"": false
               //      },
               //      {
               //   ""ID"": 1010,
               //         ""FormID"": 10,
               //         ""Caption"": ""Antivol sur le produit ?"",
               //         ""Type"": """",
               //         ""Mandatory"": false
               //      },
               //      {
               //   ""ID"": 1012,
               //         ""FormID"": 10,
               //         ""Caption"": ""oui|non"",
               //         ""Type"": ""YesNo"",
               //         ""Name"": ""Antivol"",
               //         ""Value"": ""01|00"",
               //         ""Mandatory"": false
               //      }
               //   ]
               //}
               //";
               #endregion
            };
            break;


         case FKActionTypes.QCM_woMenu:
            {
               MainViewModel.Current.SelectedInterventionParams = new Intervention_Params();
               MainViewModel.Current.SelectedInterventionParams.Data = new Intervention_Params.TData();
               MainViewModel.Current.SelectedInterventionParams.Data.Scanns = new List<Intervention_Params.Scann>();

               MainViewModel.Current.SelectedInterventionParams.FKActionType = (long)FKActionTypes.QCM_woMenu;

               // - - -  - - -

               if (string.IsNullOrEmpty(MainViewModel.Current.SelectedIntervention.Parameters))
               {
                  MainViewModel.Current.QCM_woMenuParams = new QCM_woMenu_Params();
               }
               else
               {
                  MainViewModel.Current.QCM_woMenuParams = JsonSerializer.Deserialize<QCM_woMenu_Params>(MainViewModel.Current.SelectedIntervention.Parameters);
               };

               //MainViewModel.Current.QCM_woMenuParams.Input = MainViewModel.Current.SelectedIntervention.Input;
            };
            break;
            

         default:
            if (!string.IsNullOrEmpty(MainViewModel.Current.SelectedIntervention.Parameters))
            {
               // Bugfix #109
               MainViewModel.Current.SelectedInterventionParams = MainViewModel.Current.SelectedIntervention.Params;
            }
            else
            {
               MainViewModel.Current.SelectedInterventionParams = null;

               try
               {
                  var json = System.IO.File.ReadAllText(ZPF.XF.FileIO.CleanPath($@"{MainViewModel.Current.DataFolder}Interventions\{MainViewModel.Current.SelectedIntervention.PK}.json"));
                  MainViewModel.Current.SelectedInterventionParams = JsonSerializer.Deserialize<Intervention_Params>(json);
               }
               catch { };

               if (MainViewModel.Current.SelectedInterventionParams == null)
               {
                  MainViewModel.Current.SelectedInterventionParams = new Intervention_Params();
               };
            };
            break;
      };
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -


   internal Durex_SKU GetArt(string data)
   {
      FKActionTypes ActionType = (FKActionTypes)(MainViewModel.Current.SelectedIntervention.FKActionType);

      switch (ActionType)
      {
         case FKActionTypes.BeforeAfter:
         case FKActionTypes.PhotoAuKM:
            // should not happen ...
            System.Diagnostics.Debugger.Break();
            return null;

         case FKActionTypes.Durex:
            return MainViewModel.Current.durexParams.SKUsLists.First().SKUs.Where(x => x.EAN == data).FirstOrDefault();

         default:
            //var a = MainViewModel.Current.Articles.Where(x => x.EAN == data).FirstOrDefault();

            //if (a == null)
            //{
            //   var s = MainViewModel.Current.SelectedInterventionParams.Data.Scanns.Where(x => x.EAN == data).LastOrDefault();

            //   if (s != null)
            //   {
            //      a = new EAN_Article
            //      {
            //         EAN = s.EAN,
            //         Brand = s.Brand,
            //         Condi = s.Condi,
            //         Label_FR = s.Name,
            //         Price = s.Price,
            //      };
            //   };
            //};
            //return a;
            return null;
      };

   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
