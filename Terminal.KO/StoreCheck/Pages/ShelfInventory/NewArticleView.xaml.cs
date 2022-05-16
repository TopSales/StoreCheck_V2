using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZPF.AT;
using ZPF.Calc.XF;
using ZPF.XF;
using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class NewArticleView : ContentView
   {
      public NewArticleView()
      {
         InitializeComponent();

         var brands = MainViewModel.Current.Articles.Select(x => x.Brand).ToArray().Distinct().ToList();
         brands.Sort();
         brands.Insert(0, "<unknown>");
         pBrand.ItemsSource = brands;

         calView = (CalculatorView)AddKeyboard();
         stackLayout.Children.Add(calView);
         calView.IsVisible = true;
      }

      // - - -  - - - 

      public bool ShowBrand { get; set; } = false;
      public bool ShowContent { get; set; } = false;
      public bool ShowFacing { get; set; } = true;
      public bool ShowPrice { get; set; } = false;

      // - - -  - - - 

      CalculatorView calView = null;
      private static ContentPage _Parent;
      private static Grid _MainGrid;

      View AddKeyboard()
      {
         try
         {
            if (calView == null)
            {
               //calView = new CalculatorView(_Parent, XFHelper.GetFromResources(this, "StoreCheck.Pages.ShelfInventory.InputPagePromo.xml"));
               calView = new CalculatorView(_Parent, XFHelper.GetFromResources(this, "StoreCheck.Pages.ShelfInventory.InputPage.xml"));
               calView.Margin = new Thickness(5, 10, 5, 10);
               calView.MaxLength = 8;
               calView.IsVisible = false;

               calView.DisplayValueTemplate =
                       new DataTemplate(() =>
                       {
                          var s = new Grid
                          {
                             BackgroundColor = ColorViewModel.Current.BackgroundColor50,
                             Margin = new Thickness(0, 0, 0, 10),
                          };
                          s.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                          s.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                          {
                             var l = new Label
                             {
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 24,
                                TextColor = ColorViewModel.Current.TextColor,
                                HorizontalTextAlignment = TextAlignment.End,
                                Margin = new Thickness(10, 0, 10, 0),
                                Text = "price"
                             };

                             s.Children.Add(l, 0, 0);
                          };

                          {
                             var l = new Label
                             {
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 24,
                                TextColor = ColorViewModel.Current.TextColor,
                                HorizontalTextAlignment = TextAlignment.End,
                                Margin = new Thickness(0, 0, 10, 0),
                             };
                             l.SetBinding(Label.TextProperty, "Value.FormatedValue", BindingMode.OneWay);

                             s.Children.Add(l, 1, 0);
                          };

                          // - - -  - - -

                          // Return an assembled ViewCell.
                          var v = new ViewCell
                          {
                             View = s
                          };

                          return v;
                       });

               try
               {
                  calView.CalcHelper.DisplayFormat = "2";
                  calView.CalcHelper.Print();
                  //calView.CalcHelper.Value.dValue = 815;
                  calView.CalcHelper.DisplayValue();

                  calView.OnKeyPressed += CalView_OnKeyPressed;
               }
               catch { };

               return calView;
            };
         }
         catch (Exception ex)
         {
            //Microsoft.AppCenter.Crashes.Crashes.TrackError(ex);
         }

         return null;
      }

      async void CalView_OnKeyPressed(object sender, CalculatorView.OnKeyPressedEventArgs e)
      {
         if (e.Key == "RET" || e.Key == "Promo")
         {
            if (calView.CalcHelper.Value.dValue >= 0)
            {
               if (MainViewModel.Current.SelectedInterventionParams.PriceMax > 0 && (decimal)(calView.CalcHelper.Value.dValue) > MainViewModel.Current.SelectedInterventionParams.PriceMax)
               {
                  await _Parent.DisplayAlert("Price", $"The price exceeds the limits (max {MainViewModel.Current.SelectedInterventionParams.PriceMax:n2}) set for this intervention.", "ok");
               }
               else
               {
                  MainViewModel.Current.CurrentArticle.Name = eLabel.Text;
                  MainViewModel.Current.CurrentArticle.Brand = (string)(pBrand.SelectedItem);
                  MainViewModel.Current.CurrentArticle.Condi = eContent.Text;
                  MainViewModel.Current.CurrentArticle.Facing = decimal.Parse(Facing);
                  MainViewModel.Current.CurrentArticle.Price = (decimal)calView.CalcHelper.Value.dValue;
                  //ToDo: MainViewModel.Current.CurrentArticle.Promo = (e.Key == "Promo");

                  MainViewModel.Current.UpdateCurrentArticle();

                  GridDlgOnTop.RemoveFromTop(_MainGrid);

                  // https://github.com/ZeProgFactory/StoreCheck/issues/98
                  try
                  {
                     GridDlgOnTop._Is_MDDlgOnTop_Terminated.SetResult("OK");
                  }
                  catch (Exception ex)
                  {
                     Log.Write(new AuditTrail(ex));
                  };
               };
            };
         };
      }

      public static async void Display(ContentPage parent, Grid mainGrid, System.Action callBack)
      {
         _Parent = parent;
         _MainGrid = mainGrid;

         var view = new NewArticleView();

         GridDlgOnTop.CustomPadding = new Thickness(5, 25, 5, 25);
         var res = await GridDlgOnTop.DlgOnTop(mainGrid, view, null, 500, GridDlgOnTop.MarginWidths.custom, view.PostInit);

         if (res == "OK")
         {
            // EditEtatEmplacementView.Display(parent, mainGrid, callBack);

            InterventionsViewModel.Current.AddToScann(new Intervention_Params.Scann
            {
               //Guid = MainViewModel.Current.CurrentArticle.Guid,
               EAN = MainViewModel.Current.CurrentArticle.EAN,
               Brand = MainViewModel.Current.CurrentArticle.Brand,
               Name = MainViewModel.Current.CurrentArticle.Name,
               Condi = MainViewModel.Current.CurrentArticle.Condi,
               Price = MainViewModel.Current.CurrentArticle.Price,
               State = 1,
               Facing = MainViewModel.Current.CurrentArticle.Facing,

               //ToDo: Promo = MainViewModel.Current.CurrentArticle.Promo,
               // QtStock
            });

            callBack();
         }
         else
         {
            UnitechViewModel.Current.Data = "";
            UnitechViewModel.Current.Symbology = UnitechViewModel.Symbologies.Unknown;
            UnitechViewModel.Current.Length = 0;
         };
      }

      public void PostInit()
      {
         List<View> list = XFHelper.GetAllChildren(this.Parent);
         var tile = list.Where(x => x is Tile).FirstOrDefault() as Tile;

         if (tile != null) tile.IsEnabled = false;
      }

      string Facing = "-1";

      private void RadioButton_CheckedChanged(object sender, EventArgs e)
      {
         var rb = sender as ZPF.XF.Compos.RadioButton;
         if (rb.Checked) Facing = rb.CommandParameter as string;
      }
   }
}
