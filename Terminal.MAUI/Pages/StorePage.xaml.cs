namespace StoreCheck.Pages;
using Microsoft.Maui.Controls;

using System.Collections.ObjectModel;
using ZPF;
using ZPF.XF.Compos;

public partial class StorePage : PageEx
{
   public ObservableCollection<Intervention_CE> Items { get; set; } = new ObservableCollection<Intervention_CE>();

   public StorePage()
   {
      this.BindingContext = MainViewModel.Current;

      InitializeComponent();

      Title = "store";

      listView.SetBinding(ListView.ItemsSourceProperty, new Binding("Items", BindingMode.TwoWay, source: this));

      // - - -  - - - 

      var tiles = SetAppBarContent(new List<AppBarItem>(new AppBarItem[]
      {
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Tools_02), @"sort/filter"),
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Delete), @"cancel"),
      }));

      tiles[0].FontSize = 18;
      tiles[0].Clicked += async (object sender, System.EventArgs e) =>
      {
         #region - - - Régénération - - -

         var _Filter = Filter;

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
               Text = "current",
               FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
               Checked = Filter == Filters.current,
               TextColor = ColorViewModel.Current.TextColor,
            };
            rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
            {
               Filter = (rb.Checked ? Filters.current : Filter);
            };
            s.Children.Add(rb);
         };

         {
            var rb = new ZPF.XF.Compos.RadioButton
            {
               Text = "future",
               FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
               Checked = Filter == Filters.future,
               TextColor = ColorViewModel.Current.TextColor,
            };
            rb.CheckedChanged += (object sender2, System.EventArgs e2) =>
            {
               Filter = (rb.Checked ? Filters.future : Filter);
            };
            s.Children.Add(rb);
         };

         var Result = await GridDlgOnTop.DlgOnTop(mainGrid, s, GridDlgOnTop.OkCancelTiles());

         if (Result == "OK")
         {
            UpdateData();
         };

         if (Result == "cancel")
         {
            Filter = _Filter;
         };

         #endregion
      };

      tiles[1].FontSize = 18;
      tiles[1].Clicked += async (object sender, System.EventArgs e) =>
      {
         await Navigation.PopModalAsync();
      };
   }

   enum Filters { past, current, future, all }
   Filters Filter { get; set; } = Filters.all;


   protected override void OnAppearing()
   {
      base.OnAppearing();

      UpdateData();
   }

   private void UpdateData()
   {
      var list = MainViewModel.Current.Interventions.Where(x => x.FKStore == MainViewModel.Current.SelectedStore.PK);

      switch (Filter)
      {
         default:
         case Filters.all:
            list = list.OrderBy(x => x.UpdatedOn);
            break;

         case Filters.past:
            list = list.Where(x => x.IsClosed == true).OrderBy(x => x.UpdatedOn);
            break;

         case Filters.current:
            list = list.Where(x => x.DateBeginIntervention != DateTime.MinValue && x.DateEndIntervention == DateTime.MinValue).OrderBy(x => x.UpdatedOn);
            break;

         case Filters.future:
            list = list.Where(x => x.DateBeginIntervention == DateTime.MinValue).OrderBy(x => x.UpdatedOn);
            break;
      };

      Items.Clear();
      foreach (var i in list)
      {
         Items.Add(i);
      };
   }

   private void infosClicked(object sender, System.EventArgs e)
   {
      Navigation.PushAsync(new ContactPage(MainViewModel.Current.SelectedStore));
   }

   private async void naviClicked(object sender, System.EventArgs e)
   {
      var posD = await Geolocation.GetLastKnownLocationAsync();
      var posA = new Location(MainViewModel.Current.SelectedStore.Latitude, MainViewModel.Current.SelectedStore.Longitude);
      var adresse = MainViewModel.Current.SelectedStore.Address;
      var CP = MainViewModel.Current.SelectedStore.PC;
      var Ville = MainViewModel.Current.SelectedStore.City;


      if (DeviceInfo.Platform == DevicePlatform.Android)
      {
         // https://developers.google.com/maps/documentation/urls/guide

         // opens the 'task chooser' so the user can pick Maps, Chrome or other mapping app
         // Device.OpenUri(new Uri("http://maps.google.com/?daddr=San+Francisco,+CA&saddr=Mountain+View"));

         //string URI = string.Format("http://maps.google.com/?saddr=My+Location&daddr={2} {3} {4}", posD.Latitude.ToString().Replace(",", "."), posD.Longitude.ToString().Replace(",", "."), adresse, CP, Ville);
         string URI = string.Format("http://maps.google.com/?saddr=My+Location&daddr={0},{1}", posA.Latitude.ToString().Replace(",", "."), posA.Longitude.ToString().Replace(",", "."));
         //Device.OpenUri(new Uri(URI));
         await Launcher.OpenAsync(new Uri(URI));
      }
      else if (DeviceInfo.Platform == DevicePlatform.UWP)
      {
         // https://msdn.microsoft.com/en-us/library/mt219704.aspx
         // Device.OpenUri(new Uri("bingmaps:?rtp=adr.394 Pacific Ave San Francisco CA~adr.One Microsoft Way Redmond WA 98052"));

         //string URI = string.Format("bingmaps:?rtp=pos.{0}_{1}~adr.{2} {3} {4}", posD.Latitude, posD.Longitude, adresse, CP, Ville);
         string URI = string.Format("bingmaps:?rtp=pos.{0}_{1}~pos.{2}_{3}", posD.Latitude.ToString().Replace(",", "."), posD.Longitude.ToString().Replace(",", "."), posA.Latitude.ToString().Replace(",", "."), posA.Longitude.ToString().Replace(",", "."));
         // string URI = string.Format("bingmaps:?rtp=adr.{2} {3} {4}", pos.Latitude, pos.Longitude, adresse, CP, Ville);
         //Device.OpenUri(new Uri(URI));
         await Launcher.OpenAsync(new Uri(URI));
      }
   }

   private async void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
   {
      if (e.SelectedItem != null)
      {
         var i = e.SelectedItem as Intervention_CE;

         MainViewModel.Current.SelectedIntervention = i;

         if (MainViewModel.Current.SelectedIntervention.IsClosed)
         {
            await DisplayAlert("Error", "You can't access a validated intervention!", "ok");
         }
         else
         {
            await Navigation.PushAsync(new InterventionPage());
         };

         listView.SelectedItem = null;
      };

   }
}
