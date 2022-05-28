using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using ZPF;
using ZPF.AT;

public partial class MainViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public List<Store_CE> Stores { get; private set; } = new List<Store_CE>();

   // - - -  - - - 

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

            //ToDo: StoresViewModel.Current.LoadStoreParams();
         };
      }
   }
   Store_CE _SelectedStore = null;

   // - - -  - - - 

   public void SetLocalStores(List<Store_CE> list)
   {
      Stores = list;

      OnPropertyChanged("Stores");
   }

   // - - -  - - - 

   public string SetStores(string json)
   {
      string Result = "ok";

      if (json == null)
      {
         // oups ...
         return "JSON = null";
      };

      var list = JsonConvert.DeserializeObject<Store_CE[]>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
      if (list != null)
      {
         foreach (var store in list)
         {
            var d = Stores.Where(x => x.PK == store.PK).FirstOrDefault();

            if (d != null)
            {
               if (store.UpdatedOn > d.UpdatedOn)
               {
                  Stores.Remove(d);
                  Stores.Add(store);
               };
            }
            else
            {
               Stores.Add(store);
            };
         };

         OnPropertyChanged("Stores");
      };

      return Result;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
