using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AppCenter.Distribute;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZPF;
using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class StoreListPage : Page_Base
   {
      public ObservableCollection<string> Items { get; set; }

      public StoreListPage()
      {
         this.BindingContext = MainViewModel.Current;

         InitializeComponent();

         Title = "stores";

         // - - -  - - - 

         var tiles = SetAppBarContent(new List<AppBarItem>(new AppBarItem[]
         {
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Tools_02), "sort/filter"),
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Reload), "sync"),
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Delete), "cancel"),
         }), new GridLength(85, GridUnitType.Absolute));

         tiles[0].FontSize = 16;
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
                  Text = "already visited",
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  Checked = Filter == Filters.past,
                  TextColor = ColorViewModel.Current.TextColor,
               };
               rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
               {
                  Filter = (rb.Checked ? Filters.past : Filter);
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


            {
               var l = new Label
               {
                  Text = T("sort by"),
                  FontAttributes = FontAttributes.Bold,
                  FontSize = 20,
                  TextColor = ColorViewModel.Current.TextColor,
                  Margin = new Thickness(0, 20, 0, 0),
               };
               s.Children.Add(l);
            };

            {
               var rb = new ZPF.XF.Compos.RadioButton
               {
                  Text = T("by distance"),
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  Checked = Order == Orders.byDistance,
                  TextColor = ColorViewModel.Current.TextColor,
               };
               rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
               {
                  Order = (rb.Checked ? Orders.byDistance : Order);
               };

               s.Children.Add(rb);
            };

            {
               var rb = new ZPF.XF.Compos.RadioButton
               {
                  Text = T("by zone"),
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  Checked = Order == Orders.byCP,
                  TextColor = ColorViewModel.Current.TextColor,
               };
               rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
               {
                  Order = (rb.Checked ? Orders.byZone : Order);
               };
               s.Children.Add(rb);
            };

            {
               var rb = new ZPF.XF.Compos.RadioButton
               {
                  Text = T("by CP"),
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  Checked = Order == Orders.byCP,
                  TextColor = ColorViewModel.Current.TextColor,
               };
               rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
               {
                  Order = (rb.Checked ? Orders.byCP : Order);
               };
               s.Children.Add(rb);
            };

            var Result = await GridDlgOnTop.DlgOnTop(mainGrid, s, GridDlgOnTop.OkCancelTiles());

            if (Result == T("OK"))
            {
               UpdateData();
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
            listView_Refreshing(null, null);
         };

         tiles[2].FontSize = 16;
         tiles[2].Clicked += async (object sender, System.EventArgs e) =>
         {
            await Navigation.PopAsync();
         };

         listView.SetBinding(ListView.ItemsSourceProperty, new Binding("listIMD", BindingMode.TwoWay, source: this));
      }

      bool OnAppearing_Sema = true;

      protected override void OnAppearing()
      {
         base.OnAppearing();

         if (OnAppearing_Sema)
         {
            OnAppearing_Sema = false;
            UpdateData();
         };
      }

      public class InterMagDist
      {
         public Intervention_CE Inter { get; set; }
         public Store_CE Mag { get; set; }

         public double Dist { get; set; }
         public Xamarin.Forms.Color BackgroundColor { get; set; } = Xamarin.Forms.Color.AntiqueWhite;
         public int IntTotal { get; set; }
         public int IntTodo { get; set; }
      };

      enum Filters { past, todo, all }
      Filters Filter { get; set; } = Filters.todo;

      enum Orders { byDistance, byZone, byCP }
      Orders Order { get; set; } = Orders.byDistance;

      public ObservableCollection<InterMagDist> listIMD { get; set; } = new ObservableCollection<InterMagDist>();

      bool UpdateData_Sema = true;

      private async void UpdateData()
      {
         if (!UpdateData_Sema)
         {
            return;
         };

         UpdateData_Sema = false;

         BackboneViewModel.Current.IncBusy();

         Location loc = null;

         try
         {
            loc = await Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync();
         }
         catch (Exception ex)
         {
            var infos = new System.Collections.Generic.Dictionary<string, string>
                  {
                     { "Exception", ex.Message },
                     { "StackTrace", ex.StackTrace }
                  };

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Geolocation.GetLastKnownLocationAsync", infos);
         };

         if (loc == null)
         {
            //BackboneViewModel.Current.Silent = true;

            //await DisplayAlertT("Localisation", "We are having difficulty locating your phone.", "ok");

            //BackboneViewModel.Current.Silent = false;

            try
            {
               loc = await Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync();
            }
            catch (Exception ex)
            {
               var infos = new System.Collections.Generic.Dictionary<string, string>
                  {
                     { "Exception", ex.Message },
                     { "StackTrace", ex.StackTrace }
                  };

               Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Geolocation.GetLastKnownLocationAsync", infos);
            };
         };

         var mags = MainViewModel.Current.Interventions.GroupBy(x => x.FKStore).Select(x => x.FirstOrDefault());

         try
         {
            var list = mags.Join
               (MainViewModel.Current.Stores,
                  i => i.FKStore,
                  m => m.PK,
                  (i, m) => new InterMagDist
                  {
                     Inter = i,
                     Mag = m,
                     Dist = (loc == null ? 0 : Xamarin.Essentials.Location.CalculateDistance(loc, new Xamarin.Essentials.Location { Latitude = m.Latitude, Longitude = m.Longitude }, Xamarin.Essentials.DistanceUnits.Kilometers)),
                     BackgroundColor = Xamarin.Forms.Color.AntiqueWhite,
                  }).ToList();


            switch (Filter)
            {
               default:
               case Filters.all:
                  list = list.OrderBy(x => x.Inter.UpdatedOn).ToList();
                  break;

               case Filters.past:
                  list = list.Where(x => x.Inter.IsClosed == true).OrderBy(x => x.Inter.UpdatedOn).ToList();
                  break;

               case Filters.todo:
                  list = list.Where(x => x.Inter.IsClosed != true ).OrderBy(x => x.Inter.UpdatedOn).ToList();
                  break;
            };

            foreach (var mag in list)
            {
               var tmp = MainViewModel.Current.Interventions.Where(x => x.FKStore == mag.Mag.PK);

               mag.IntTotal = tmp.Count();
               mag.IntTodo = tmp.Where(x => x.DateEndIntervention == DateTime.MinValue).Count();

               if (mag.IntTodo == 0)
               {
                  mag.BackgroundColor = Xamarin.Forms.Color.Green;
               }
               else if (mag.IntTodo < mag.IntTotal)
               {
                  mag.BackgroundColor = Xamarin.Forms.Color.LightGreen;
               };
            };

            switch (Order)
            {
               default:
               case Orders.byDistance:
                  list = list.OrderBy(x => x.Dist).ToList();
                  break;

               case Orders.byZone:
                  list = list.OrderBy(x => x.Mag.Zone).ToList();
                  break;

               case Orders.byCP:
                  list = list.OrderBy(x => x.Mag.PC).ToList();
                  break;
            };


            listIMD.Clear();
            foreach (var i in list)
            {
               listIMD.Add(i);
            };
         }
         catch (Exception ex)
         {
            await DisplayAlertT("Error (1)", ex.Message, "ok");
         };

         BackboneViewModel.Current.DecBusy();

         UpdateData_Sema = true;
      }

      private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
      {
         if (e.SelectedItem != null)
         {
            var i = e.SelectedItem as InterMagDist;

            MainViewModel.Current.SelectedStore = i.Mag;
            MainViewModel.Current.SelectedIntervention = i.Inter;

            Navigation.PushAsync(new StorePage());

            OnAppearing_Sema = true;
         };
      }

      private async void listView_Refreshing(object sender, System.EventArgs e)
      {
         if (string.IsNullOrEmpty(MainViewModel.Current.Config.Login))
         {
         }
         else
         {
            if (MainViewModel.Current.IsInternetAccessAvailable)
            {
               BackboneViewModel.Current.IncBusy();
               await SyncViewModel.Current.SyncDataWithWeb(MainViewModel.Current.Config.Login);
               BackboneViewModel.Current.DecBusy();

               listView.IsRefreshing = false;

               // - - -  - - - 

               Distribute.CheckForUpdate();
            }
            else
            {
               listView.IsRefreshing = false;

               await DisplayAlertT("Oups ...", "You need access to the internet ...", "ok");
            };
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -    
   }
}
