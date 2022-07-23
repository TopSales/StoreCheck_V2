using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using ZPF;
using ZPF.SQL;

public class ReferentielViewModel : BaseViewModel
{     
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static ReferentielViewModel _Current = null;

   public static ReferentielViewModel Current
   {
      get
      {
         if (_Current == null)
         {
            _Current = new ReferentielViewModel();
         };

         return _Current;
      }

      set
      {
         _Current = value;
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public ReferentielViewModel()
   {
      _Current = this;

      EditReferentielViewModel.Current.OnChanges += OnChanges;

      worker.DoWork += worker_DoWork;

      LoadData();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   private void OnChanges(string Msg)
   {
      LoadData();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public ObservableCollection<NameValue_Int64> Fournisseurs { get; set; }
   public ObservableCollection<NameValue_Int64> TauxTVA { get; set; }
   public ObservableCollection<NameValue_Int64> EmplacementsNV { get; set; }
   public ObservableCollection<NameValue_Int64> DocumentTypes { get; set; }
   public ObservableCollection<NameValue_Int64> EmplacementTypes { get; set; }
   public ObservableCollection<NameValue_Int64> FamilleTypes { get; set; }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   private readonly BackgroundWorker worker = new BackgroundWorker();

   public void LoadData()
   {
      if (worker.IsBusy)
      {
         return;
      };

      BackboneViewModel.Current.IncBusy();

      try
      {
         worker.RunWorkerAsync();
      }
      catch (Exception ex)
      {
         //Probably:
         // This BackgroundWorker is currently busy and cannot run multiple tasks concurrently.
         Debug.WriteLine(ex.Message);

         BackboneViewModel.Current.DecBusy();
      };
   }

   private void worker_DoWork(object sender, DoWorkEventArgs e)
   {
      // - - -  - - -

      DoIt.OnMainThread(() =>
      {
         Fournisseurs = new ObservableCollection<NameValue_Int64>();
         TauxTVA = new ObservableCollection<NameValue_Int64>();
         EmplacementsNV = new ObservableCollection<NameValue_Int64>();
         DocumentTypes = new ObservableCollection<NameValue_Int64>();
         FamilleTypes = new ObservableCollection<NameValue_Int64>();
         EmplacementTypes = new ObservableCollection<NameValue_Int64>();
      });

      string SQL = "";

      switch (DB_SQL._ViewModel.DBType)
      {
         case DBType.SQLite:
            SQL = "select PK as Value, trim(ifnull(Contact.Societe, '') || ' ' || ifnull(Contact.Nom, '')) as Name from Contact where IsFournisseur = 1 order by SortOrder, Contact.Societe, Contact.Nom";
            break;

         default:
         case DBType.SQLServer:
            SQL = "select PK as Value, ltrim(rtrim(concat(Societe, ' ', Contact.Nom))) as Name from Contact where IsFournisseur = 1 order by SortOrder, Contact.Societe, Contact.Nom";
            break;
      };

      foreach (var r in (DB_SQL.QuickQueryView(SQL) as DataTable).AsEnumerable())
      {
         var t = new NameValue_Int64();
         t.CopyDataRowValues(r);

         DoIt.OnMainThread(() =>
         {
            Fournisseurs.Add(t);
         });
      };
      OnPropertyChanged("Fournisseurs");

      // - - -  - - -

      foreach (var r in (DB_SQL.QuickQueryView("select PK as Value, [Nom] as Name from TauxTVA order by [Nom]") as DataTable).AsEnumerable())
      {
         var t = new NameValue_Int64();
         t.CopyDataRowValues(r);

         DoIt.OnMainThread(() =>
         {
            TauxTVA.Add(t);
         });
      };
      OnPropertyChanged("TauxTVA");

      // - - -  - - -

      foreach (var r in (DB_SQL.QuickQueryView("select PK as Value, [NomLong] as Name from Emplacement where Emplacement.IsInventaire=1 order by NomLong,Nom") as DataTable).AsEnumerable())
      {
         var t = new NameValue_Int64();
         t.CopyDataRowValues(r);

         DoIt.OnMainThread(() =>
         {
            EmplacementsNV.Add(t);
         });
      };
      OnPropertyChanged("EmplacementsNV");

      // - - -  - - -

      foreach (var r in (DB_SQL.QuickQueryView("select PK as Value, [Nom] as Name from DocumentType order by [Nom]") as DataTable).AsEnumerable())
      {
         var t = new NameValue_Int64();
         t.CopyDataRowValues(r);

         DoIt.OnMainThread(() =>
         {
            DocumentTypes.Add(t);
         });
      };
      OnPropertyChanged("DocumentTypes");

      // - - -  - - -

      foreach (var r in (DB_SQL.QuickQueryView("select PK as Value, [Nom] as Name from FamilleType order by [Nom]") as DataTable).AsEnumerable())
      {
         var t = new NameValue_Int64();
         t.CopyDataRowValues(r);

         DoIt.OnMainThread(() =>
         {
            FamilleTypes.Add(t);
         });
      };
      OnPropertyChanged("FamilleTypes");

      // - - -  - - -

      foreach (var r in (DB_SQL.QuickQueryView("select PK as Value, [Nom] as Name from EmplacementType order by [Nom]") as DataTable).AsEnumerable())
      {
         var t = new NameValue_Int64();
         t.CopyDataRowValues(r);

         DoIt.OnMainThread(() =>
         {
            EmplacementTypes.Add(t);
         });
      };
      OnPropertyChanged("EmplacementTypes");

      // - - -  - - -

      BackboneViewModel.Current.DecBusy();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
}

