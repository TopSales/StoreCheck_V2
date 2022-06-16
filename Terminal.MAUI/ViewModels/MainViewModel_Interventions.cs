using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using ZPF;
using ZPF.AT;
using ZPF.Chat;

public partial class MainViewModel : BaseViewModel
{
   public Intervention_Params SelectedInterventionParams { get => _SelectedInterventionParams; set => _SelectedInterventionParams = value; }

   Intervention_Params _SelectedInterventionParams = null;

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public List<Intervention_CE> Interventions { get; private set; } = new List<Intervention_CE>();

   // - - -  - - - 

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

   // - - -  - - - 

   public void SetLocalInterventions(List<Intervention_CE> list)
   {
      Interventions = list;

      OnPropertyChanged("Interventions");
   }

   // - - -  - - - 

   public string SetInterventions(string json)
   {
      string Result = "ok";

      if (json == null)
      {
         // oups ...
         return "JSON = null";
      };

      var list = JsonConvert.DeserializeObject<Intervention_CE[]>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
      if (list != null)
      {
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

         OnPropertyChanged("Interventions");
      };

      return Result;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public void SaveInterv(Intervention_Params p)
   {
      SelectedIntervention.Parameters = null;

      string json = System.Text.Json.JsonSerializer.Serialize(p);

      string extractPath = ZPF.XF.FileIO.CleanPath(MainViewModel.Current.DataFolder + @"Interventions\");

      if (!System.IO.Directory.Exists(extractPath))
      {
         System.IO.Directory.CreateDirectory(extractPath);
      };

      System.IO.File.WriteAllText(ZPF.XF.FileIO.CleanPath($@"{MainViewModel.Current.DataFolder}Interventions\{MainViewModel.Current.SelectedIntervention.PK}.json"), json);

      MainViewModel.Current.SelectedIntervention.SyncOn = DateTime.MinValue;
      MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
   }
   
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public Durex_Params durexParams { get; internal set; }
   public PhotoAuKM_Params PhotoAuKM { get; internal set; }
   public QCM_woMenu_Params QCM_woMenuParams { get; internal set; }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
