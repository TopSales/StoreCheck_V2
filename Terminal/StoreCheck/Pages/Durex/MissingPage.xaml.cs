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
   public partial class MissingPage : Page_Base
   {
      enum Filters { done, todo, all }
      Filters Filter { get; set; } = Filters.todo;

      enum Orders { none }
      Orders Order { get; set; } = Orders.none;


      public MissingPage()
      {
         BindingContext = ShelfInventoryViewModel.Current;

         InitializeComponent();

         Title = "Missing";

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
            Dlg();
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
         var p = MainViewModel.Current.SelectedInterventionParams;

         foreach (var m in MissingArticles.Where(x => x.Tag))
         {
            var a = new Durex_SKU
            {
               EAN = m.EAN,
               Name = m.Label_FR,
               ConsumerPrice = m.Price,
               Facing = -1,
               IsPresent = false,
            };

            InterventionsViewModel.Current.durexScanns.Add(a);
         };

         MainViewModel.Current.SaveInterv(p);
         await Navigation.PopAsync();
      }

      class EAN_ArticleEx : EAN_Article
      {
         public bool Tag
         {
            get; set;
         }
      }

      List<EAN_ArticleEx> MissingArticles { get; set; } = new List<EAN_ArticleEx>();

      void LoadMissingArticles()
      {
         {
#if DEBUG
            //MissingArticles.Clear();

            //var art = MainViewModel.Current.Articles.Where(x => x.EAN == "4005800258886").FirstOrDefault();
            //MissingArticles.Add(art);

            //art = MainViewModel.Current.Articles.Where(x => x.EAN == "4005900457707").FirstOrDefault();
            //MissingArticles.Add(art);

            //art = MainViewModel.Current.Articles.Where(x => x.EAN == "42242239").FirstOrDefault();
            //MissingArticles.Add(art);
#endif
         };

         // - - -  - - - 

         DoIt.OnMainThread(() =>
         {
            MissingArticles.Clear();

            try
            {
               foreach (var ean in MainViewModel.Current.durexParams.SKUsLists.First().SKUs.Where(x => x.MayBePresent == true))
               {
                  Durex_SKU missing = null;

                  try
                  {
                     missing = InterventionsViewModel.Current.durexScanns.Where(x => x.EAN == ean.EAN).FirstOrDefault();
                  }
                  catch { };

                  if (missing == null)
                  {
                     var art = new EAN_ArticleEx
                     {
                        EAN = ean.EAN,
                        Label_FR = ean.Name,
                        Price = ean.ConsumerPrice,
                        Condi = ean.Content,
                        Tag = ean.IsTagged,
                     };

                     MissingArticles.Add(art);
                  };
               };
            }
            catch { };

            listView.ItemsSource = null;
            listView.ItemsSource = MissingArticles;
         });
      }
   }
}
