using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using ZPF;
using ZPF.XF.Compos;

namespace StoreCheck.Pages;

public partial class StoreListPage : PageEx
{
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
         new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Exit_03), "exit"),
      }));

      tiles[0].FontSize = 16;
      tiles[0].Clicked += async (object sender, System.EventArgs e) =>
      {
         //var st = await MDDlgOnTop("#To be done ...\n * passé / en cours / futures\n * filtrable / triable");

         #region - - - Régénération - - -

         var _Filter = Filter;
         var _Order = Order;

         var s2 = new StackLayout
         {
            Margin = 20,
         };
         {
            var s = new StackLayout
            {
               Margin = 20,
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
                  HorizontalOptions = LayoutOptions.Start,
                  Margin = new Thickness(60, 0, 0, 0),
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
                  HorizontalOptions = LayoutOptions.Start,
                  Margin = new Thickness(60, 0, 0, 0),
               };
               rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
               {
                  Filter = (rb.Checked ? Filters.past : Filter);
               };

               s.Children.Add(rb);
               s2.Children.Add(s);
            };

            {
               var rb = new ZPF.XF.Compos.RadioButton
               {
                  Text = "to do",
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  Checked = Filter == Filters.todo,
                  TextColor = ColorViewModel.Current.TextColor,
                  HorizontalOptions = LayoutOptions.Start,
                  Margin = new Thickness(60, 0, 0, 0),
               };
               rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
               {
                  Filter = (rb.Checked ? Filters.todo : Filter);
               };
               s.Children.Add(rb);
            };
         };

         {
            var s = new StackLayout
            {
               Margin = new Thickness(20, 0, 20, 20),
            };
            {
               var l = new Label
               {
                  Text = "sort by",
                  FontAttributes = FontAttributes.Bold,
                  FontSize = 20,
                  TextColor = ColorViewModel.Current.TextColor,
                  Margin = new Thickness(0, 20, 0, 0),
               };
               s.Children.Add(l);
            };

            {
               var rb = new ZPF.XF.Compos.Tile
               {
                  IconChar = ZPF.Fonts.IF.Check_Mark_01,
                  Text = "",
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  TextColor = ColorViewModel.Current.TextColor,
                  HorizontalOptions = LayoutOptions.Start,
                  Margin = new Thickness(60, 0, 0, 0),
                  HeightRequest = 22,
                  WidthRequest = 22,
               };

               s.Children.Add(rb);
            };


            {
               var rb = new ZPF.XF.Compos.CheckBoxZPF
               {
                  Text = "by distance",
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  Checked = Order == Orders.byDistance,
                  TextColor = ColorViewModel.Current.TextColor,
                  HorizontalOptions = LayoutOptions.Start,
                  Margin = new Thickness(60, 0, 0, 0),
                  BackgroundColor = Microsoft.Maui.Graphics.Colors.Bisque,
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
                  Text = "by distance",
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  Checked = Order == Orders.byDistance,
                  TextColor = ColorViewModel.Current.TextColor,
                  HorizontalOptions = LayoutOptions.Start,
                  Margin = new Thickness(60, 0, 0, 0),
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
                  Text = "by zone",
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  Checked = Order == Orders.byCP,
                  TextColor = ColorViewModel.Current.TextColor,
                  HorizontalOptions = LayoutOptions.Start,
                  Margin = new Thickness(60, 0, 0, 0),
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
                  Text = "by CP",
                  FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                  Checked = Order == Orders.byCP,
                  TextColor = ColorViewModel.Current.TextColor,
                  HorizontalOptions = LayoutOptions.Start,
                  Margin = new Thickness(60, 0, 0, 0),
               };
               rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
               {
                  Order = (rb.Checked ? Orders.byCP : Order);
               };
               s.Children.Add(rb);
            };
            s2.Children.Add(s);
         };

         var Result = await GridDlgOnTop.DlgOnTop(mainGrid, s2, GridDlgOnTop.OkCancelTiles());

         if (Result == "OK")
         {
            UpdateData();
         };

         if (Result == "cancel")
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
         await Navigation.PopModalAsync();
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
      public Microsoft.Maui.Graphics.Color BackgroundColor { get; set; } = Microsoft.Maui.Graphics.Colors.AntiqueWhite;
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
         loc = await Microsoft.Maui.Devices.Sensors.Geolocation.GetLastKnownLocationAsync();
      }
      catch (Exception ex)
      {
         //   var infos = new System.Collections.Generic.Dictionary<string, string>
         //            {
         //               { "Exception", ex.Message },
         //               { "StackTrace", ex.StackTrace }
         //            };

         //   Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Geolocation.GetLastKnownLocationAsync", infos);
      };

      if (loc == null)
      {
         //BackboneViewModel.Current.Silent = true;

         //await DisplayAlertT("Localisation", "We are having difficulty locating your phone.", "ok");

         //BackboneViewModel.Current.Silent = false;

         try
         {
            loc = await Microsoft.Maui.Devices.Sensors.Geolocation.GetLastKnownLocationAsync();
         }
         catch (Exception ex)
         {
            //   var infos = new System.Collections.Generic.Dictionary<string, string>
            //         {
            //            { "Exception", ex.Message },
            //            { "StackTrace", ex.StackTrace }
            //         };

            //   Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Geolocation.GetLastKnownLocationAsync", infos);
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
                  Dist = (loc == null || m.Latitude == 0 || m.Longitude == 0 ? 0 : Location.CalculateDistance(loc, new Location { Latitude = m.Latitude, Longitude = m.Longitude }, DistanceUnits.Kilometers)),
                  BackgroundColor = Microsoft.Maui.Graphics.Colors.AntiqueWhite,
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
               list = list.Where(x => x.Inter.IsClosed != true).OrderBy(x => x.Inter.UpdatedOn).ToList();
               break;
         };

         foreach (var mag in list)
         {
            var tmp = MainViewModel.Current.Interventions.Where(x => x.FKStore == mag.Mag.PK);

            mag.IntTotal = tmp.Count();
            mag.IntTodo = tmp.Where(x => x.DateEndIntervention == DateTime.MinValue).Count();

            if (mag.IntTodo == 0)
            {
               mag.BackgroundColor = Microsoft.Maui.Graphics.Colors.Green;
            }
            else if (mag.IntTodo < mag.IntTotal)
            {
               mag.BackgroundColor = Microsoft.Maui.Graphics.Colors.LightGreen;
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

         OnPropertyChanged("listIMD");
      }
      catch (Exception ex)
      {
         await DisplayAlert("Error (1)", ex.Message, "ok");
      };

      BackboneViewModel.Current.DecBusy();

      UpdateData_Sema = true;
   }

   private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
   {
   }

   private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
   {
      if (e.Item != null)
      {
         var i = e.Item as InterMagDist;

         MainViewModel.Current.SelectedStore = i.Mag;
         MainViewModel.Current.SelectedIntervention = i.Inter;

         Navigation.PushModalAsync(new StorePage());

         OnAppearing_Sema = true;
      };
   }

   private async void listView_Refreshing(object sender, System.EventArgs e)
   {
      if (MainViewModel.Current.Config.FKUser < 0)
      {
      }
      else
      {
         if (MainViewModel.Current.IsInternetAccessAvailable)
         {
            BackboneViewModel.Current.IncBusy();

            ClientViewModel.Current.Entry(MainViewModel.Current.DeviceID);

            BackboneViewModel.Current.DecBusy();

            listView.IsRefreshing = false;

            // - - -  - - - 

            //ToDo: Distribute.CheckForUpdate();
         }
         else
         {
            listView.IsRefreshing = false;

            await DisplayAlert("Oups ...", "You need access to the internet ...", "ok");
         };
      };
   }


   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -   
}
