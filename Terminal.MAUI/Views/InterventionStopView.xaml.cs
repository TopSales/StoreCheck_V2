using StoreCheck.Pages;
using ZPF;
using ZPF.XF.Compos;

namespace StoreCheck.Views
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class InterventionStopView : ContentView
   {
      public InterventionStopView()
      {
         InitializeComponent();
      }

      internal static async void Display(ContentPage parent, Grid mainGrid)
      {
         switch (MainViewModel.Current.SelectedInterventionParams.FKActionType)
         {
            case (long)FKActionTypes.QCM_woMenu:
               break;

            default:
               {
                  if (MainViewModel.Current.SelectedIntervention.DateBeginIntervention == DateTime.MinValue)
                  {
                     await parent.DisplayAlert("Erreur", "One cannot close an intervention that has not been started ...", "ok");
                     return;
                  };
               };
               break;
         };

         // - - -  - - - 

         var view = new InterventionStopView();

         var tiles = new List<AppBarItem>(new AppBarItem[]
         {
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01), "ok"),
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Delete), "cancel"),
         });

         tiles[0].Clicked += async (object s, EventArgs ea) =>
         {
            BackboneViewModel.Current.IncBusy();

            InterventionsViewModel.Current.Stop();

            (parent as InterventionPage).UpdateTiles();

            InterventionsViewModel.Current.Stop();

            MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);
            await SyncViewModel.Current.SyncDataWithWeb(MainViewModel.Current.Config.Login);

            BackboneViewModel.Current.DecBusy();

            // - - -  - - - 

            //ToDo: Distribute.CheckForUpdate();

            // - - -  - - - 

            await parent.Navigation.PopModalAsync();
         };

         tiles[1].IsCancel = true;

         await GridDlgOnTop.DlgOnTop(mainGrid, view, tiles, 500);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
