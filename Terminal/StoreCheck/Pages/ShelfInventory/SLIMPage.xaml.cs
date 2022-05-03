using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZPF;
using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SLIMPage : Page_Base
   {
      enum Filters { done, todo, all }
      Filters Filter { get; set; } = Filters.todo;

      enum Orders { none }
      Orders Order { get; set; } = Orders.none;


      public SLIMPage()
      {
         BindingContext = ShelfInventoryViewModel.Current;

         InitializeComponent();

         Title = "SLIM";

         // - - -  - - - 

         var tiles = SetAppBarContent(new List<AppBarItem>(new AppBarItem[]
         {
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Tools_02), "filter"),
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Exit_03), "exit"),
         }), new GridLength(85, GridUnitType.Absolute));

         tiles[0].FontSize = 16;
         tiles[0].IsEnabled = false;
         tiles[0].Clicked += async (object sender, System.EventArgs e) =>
         {
            //var st = await MDDlgOnTop("#To be done ...\n * passé / en cours / futures\n * filtrable / triable");

            #region - - - Régénération - - -

            var _Filter = Filter;
            var _Order = Order;

            var s = new StackLayout
            {
               Margin = 30,
            };
            {
               var l = new Label
               {
                  Text = "filter",
                  FontAttributes = FontAttributes.Bold,
                  FontSize = 20,
                  TextColor = ColorViewModel.Current.TextColor,
               };
               s.Children.Add(l);
            };

            {
               var rb = new ZPF.XF.Compos.RadioButton
               {
                  Text = "all",
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  Checked = Filter == Filters.all,
                  TextColor = ColorViewModel.Current.TextColor,
               };
               rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
               {
                  Filter = (rb.Checked ? Filters.all : Filter);
               };

               s.Children.Add(rb);
            };

            {
               var rb = new ZPF.XF.Compos.RadioButton
               {
                  Text = "done",
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  Checked = Filter == Filters.done,
                  TextColor = ColorViewModel.Current.TextColor,
               };
               rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
               {
                  Filter = (rb.Checked ? Filters.done : Filter);
               };

               s.Children.Add(rb);
            };

            {
               var rb = new ZPF.XF.Compos.RadioButton
               {
                  Text = "to do",
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  Checked = Filter == Filters.todo,
                  TextColor = ColorViewModel.Current.TextColor,
               };
               rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
               {
                  Filter = (rb.Checked ? Filters.todo : Filter);
               };
               s.Children.Add(rb);
            };


            //{
            //   var l = new Label
            //   {
            //      Text = T("sort by"),
            //      FontAttributes = FontAttributes.Bold,
            //      FontSize = 20,
            //      TextColor = ColorViewModel.Current.TextColor,
            //      Margin = new Thickness(0, 20, 0, 0),
            //   };
            //   s.Children.Add(l);
            //};

            //{
            //   var rb = new ZPF.XF.Compos.RadioButton
            //   {
            //      Text = T("none"),
            //      FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
            //      Checked = Order == Orders.none,
            //      TextColor = ColorViewModel.Current.TextColor,
            //   };
            //   rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
            //   {
            //      Order = (rb.Checked ? Orders.none : Order);
            //   };

            //   s.Children.Add(rb);
            //};


            var Result = await GridDlgOnTop.DlgOnTop(mainGrid, s, GridDlgOnTop.OkCancelTiles());

            if (Result == T("OK"))
            {
               LoadMissingArticles();
            };

            if (Result == T("cancel"))
            {
               Order = _Order;
            };

            #endregion
         };

         tiles[1].FontSize = 16;
         tiles[1].Clicked += async (object sender, System.EventArgs e) =>
         {
            await Navigation.PopAsync();
         };

         listView.SetBinding(ListView.ItemsSourceProperty, new Binding("listIMD", BindingMode.TwoWay, source: this));

         // - - -  - - - 

         this.OnPrevious += onPrevious;
      }

      bool OnAppearing_SemaFirst = true;

      protected override void OnAppearing()
      {
         base.OnAppearing();

         if (OnAppearing_SemaFirst)
         {
            OnAppearing_SemaFirst = false;

            DoIt.OnMainThread(() =>
            {
               BackboneViewModel.Current.IncBusy();
            });

            LoadMissingArticles();

            DoIt.OnMainThread(() =>
            {
               BackboneViewModel.Current.DecBusy();
            });
         };
      }

      protected override bool OnBackButtonPressed()
      {
         Dlg();
         return true;
      }

      public void onPrevious()
      {
         Dlg();
      }

      private async void Dlg()
      {
         if (await DisplayAlertView.Display(this, mainGrid, "SLIM", "Confirmation that all the SLIM are well put on?", "yes", "no") == "yes")
         {
            var p = MainViewModel.Current.SelectedInterventionParams;

            foreach (var missingGuid in ShelfInventoryViewModel.Current.MissingArticles)
            {
               var s = p.Data.Scanns.Where(x => x.Guid == missingGuid.Guid).FirstOrDefault();

               if (s == null)
               {
                  // - - - new article - - -
                  p.Data.Scanns.Add(new Intervention_Params.Scann
                  {
                     Guid = missingGuid.Guid,
                     EAN = missingGuid.EAN,
                     //Brand = MainViewModel.Current.CurrentArticle.Brand,
                     //Libelle = MainViewModel.Current.CurrentArticle.Label_FR,
                     //Condi = MainViewModel.Current.CurrentArticle.Condi,
                     //Prix = MainViewModel.Current.CurrentArticle.Price,
                     //ToDo: State = (missingGuid.Tag == 0 ? -2 : -1),
                     // Facing 
                     // Promo
                     // QtStock
                  });
               }
               else
               {
                  //ToDo: s.State = (missingGuid.Tag == 0 ? -2 : -1);
               };
            };

            MainViewModel.Current.SaveInterv(p);

            await Navigation.PopAsync();
         }
         else
         {
            return;
         };
      }

      void LoadMissingArticles()
      {
         var ipData = MainViewModel.Current.SelectedInterventionParams;
         var iData = MainViewModel.Current.SelectedInterventionParams.Data;
         ipData.Data.TypeMag = ShelfInventoryViewModel.Current.GetTypeMag(iData);

         MainViewModel.Current.SaveInterv(ipData);

         if (ipData.Data.TypeMag == "???")
         {
            DisplayAlert("SLIM", "No SLIM management for this store.", "ok");
            return;
         };

         // - - -  - - -

         var mt = ShelfInventoryViewModel.Current.UpdateMissing(iData, ipData.Data.TypeMag);

         if (mt != null)
         {
            ShelfInventoryViewModel.Current.MissingArticles.Clear();
            foreach (var missingGuid in mt.Guids.Where(x => x.WasMissing))
            {
               var art = MainViewModel.Current.Articles.Where(x => x.Guid == missingGuid.Guid && x.Master == true).FirstOrDefault();

               var m = iData.Scanns.Where(x => x.Guid == art.Guid && x.State < 0).FirstOrDefault();

               if (m != null)
               {
                  //ToDo: art.Tag = (m.State == -1 ? 1 : 0);
               };

               ShelfInventoryViewModel.Current.MissingArticles.Add(art);
            };
         }
         else
         {
#if DEBUG
            ShelfInventoryViewModel.Current.MissingArticles.Clear();

            var art = MainViewModel.Current.Articles.Where(x => x.EAN == "4005800258886").FirstOrDefault();
            ShelfInventoryViewModel.Current.MissingArticles.Add(art);

            art = MainViewModel.Current.Articles.Where(x => x.EAN == "4005900457707").FirstOrDefault();
            ShelfInventoryViewModel.Current.MissingArticles.Add(art);

            art = MainViewModel.Current.Articles.Where(x => x.EAN == "42242239").FirstOrDefault();
            ShelfInventoryViewModel.Current.MissingArticles.Add(art);
#endif
         };

         // - - -  - - - 

         //switch (Filter)
         //{
         //   default:
         //   case Filters.all:
         //      break;

         //   case Filters.done:
         //      ShelfInventoryViewModel.Current.MissingArticles !!!! non il faut une coopi de la liste pour affichage!!!! = ShelfInventoryViewModel.Current.MissingArticles.Where(x => x.Tag == 1).ToList();
         //      break;

         //   case Filters.todo:
         //      ShelfInventoryViewModel.Current.MissingArticles = ShelfInventoryViewModel.Current.MissingArticles.Where(x => x.Tag == 0).ToList();
         //      break;
         //};

         // - - -  - - - 

         DoIt.OnMainThread(() =>
         {
            ShelfInventoryViewModel.Current.UpdateMissingArticles();

            listView.ItemsSource = null;
            listView.ItemsSource = ShelfInventoryViewModel.Current.MissingArticles;
         });
      }
   }
}
