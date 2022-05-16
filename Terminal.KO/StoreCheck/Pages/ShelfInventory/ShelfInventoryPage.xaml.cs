using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZPF;
using ZPF.AT;
using ZPF.Calc.XF;
using ZPF.MCE;
using ZPF.XF;
using static UnitechViewModel;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ShelfInventoryPage : Page_Base
   {

      public ShelfInventoryPage()
      {
         BindingContext = MainViewModel.Current;
         Title = "shelf inventory";

         InitializeComponent();

         slSymb.BindingContext = UnitechViewModel.Current;

         // - - -  - - - 

         SetAppBarContent();

         // - - -  - - - 

         var s = MainViewModel.Current.SelectedInterventionParams.Data.Scanns.LastOrDefault();
         if (s != null)
         {
            lNbScan.Text = MainViewModel.Current.SelectedInterventionParams.Data.Scanns.Count().ToString();

            MainViewModel.Current.PrevArticle.FromEAN(MainViewModel.Current.Articles.Where(x => x.EAN == s.EAN).LastOrDefault());

            if (MainViewModel.Current.PrevArticle != null)
            {
               MainViewModel.Current.PrevArticle.Price = s.Price;
            }
            else
            {
               MainViewModel.Current.PrevArticle.FromEAN(new EAN_Article
               {
                  Brand = s.Brand,
                  Condi = s.Condi,

                  EAN = s.EAN,
                  Label_FR = s.Name,
                  Price = s.Price,
                  //ToDo: Promo = s.Promo,
               });
            };
         }
         else
         {
            MainViewModel.Current.PrevArticle = null;
            lNbScan.Text = "";
         };

         MainViewModel.Current.UpdatePrevArticle();

         // - - -  - - - 

         UnitechViewModel.Current.Data = "";
         UnitechViewModel.Current.Symbology = Symbologies.None;
         UnitechViewModel.Current.nSymbology = 0;
         UnitechViewModel.Current.Length = 0;

         // - - -  - - - 

         MainViewModel.Current.CurrentArticle = null;
         MainViewModel.Current.UpdateCurrentArticle();

         DoIt.Delay(100, () =>
         {
            DoIt.OnMainThread(() =>
            {
               //#if SCAN_WEDGE
               entry.Unfocused += Entry_Unfocused;
               //#endif
               entry.Focus();
            });
         });

         this.OnPrevious += onPrevious;
      }

      CalculatorView calView = null;


      View AddKeyboard()
      {
         try
         {
            if (calView == null)
            {
               //calView = new CalculatorView(this, XFHelper.GetFromResources(this, "StoreCheck.Pages.ShelfInventory.InputPagePromo.xml"));
               calView = new CalculatorView(this, XFHelper.GetFromResources(this, "StoreCheck.Pages.ShelfInventory.InputPage.xml"));

               calView.Margin = new Thickness(20, 10, 20, 10);
               calView.MaxLength = 8;
               slFacing.IsVisible = false;
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
         };

         return null;
      }

      async void CalView_OnKeyPressed(object sender, CalculatorView.OnKeyPressedEventArgs e)
      {
         if (e.Key == "RET" || e.Key == "Promo")
         {
            if (calView.CalcHelper.Value.dValue >= 0)
            {
               //if (MainViewModel.Current.SelectedInterventionParams.PriceMax > 0 && (decimal)(calView.CalcHelper.Value.dValue) > MainViewModel.Current.SelectedInterventionParams.PriceMax)
               //{
               //   await DisplayAlert("Price", $"The price exceeds the limits (max {MainViewModel.Current.SelectedInterventionParams.PriceMax:n2}) set for this intervention.", "ok");
               //}
               //else 
               {
                  if (MainViewModel.Current.CurrentArticle != null)
                  {
                     MainViewModel.Current.CurrentArticle.Price = (decimal)calView.CalcHelper.Value.dValue;
                     MainViewModel.Current.CurrentArticle.Facing = decimal.Parse(Facing);
                     MainViewModel.Current.UpdateCurrentArticle();

                     slFacing.IsVisible = false;
                     calView.IsVisible = false;

                     // EditEtatEmplacementView.Display(this, mainGrid, CallBack);
                     InterventionsViewModel.Current.AddToScann(new Intervention_Params.Scann
                     {
                        Guid = MainViewModel.Current.CurrentArticle.Guid,
                        EAN = MainViewModel.Current.CurrentArticle.EAN,
                        Name = MainViewModel.Current.CurrentArticle.Name,
                        QTReplaced = MainViewModel.Current.CurrentArticle.QTReplaced,
                        //Brand = MainViewModel.Current.CurrentArticle.Brand,
                        //Libelle = MainViewModel.Current.CurrentArticle.Label_FR,
                        //Condi = MainViewModel.Current.CurrentArticle.Condi,
                        Price = MainViewModel.Current.CurrentArticle.Price,
                        State = 1,
                        Facing = MainViewModel.Current.CurrentArticle.Facing,
                        Promo = (e.Key == "Promo"),
                        // QtStock
                     });
                  }
                  else
                  {
                     slFacing.IsVisible = false;
                     calView.IsVisible = false;
                  };

                  CallBack();
               };
            };
         };
      }

      public void CallBack()
      {
         DoIt.Delay(100, () =>
         {
            DoIt.OnMainThread(async () =>
            {
               slBarCode.IsVisible = true;
               entry.Text = "";

               //#if SCAN_WEDGE
               entry.Unfocused += Entry_Unfocused;
               //#endif

               if (calView == null)
               {
                  slScroll.Children.Add(AddKeyboard());
               };

               entry.Focus();

               {
                  var mce = new MCEPage();
                  mce.Input = MainViewModel.Current.durexParams.Input;
                  mce.Output = Output;
                  mce.OnUpdateOutput += Scan_OnUpdateOutput;

                  await Navigation.PushAsync(mce);
               };
            });
         });
      }

      string Output = "";

      void Scan_OnUpdateOutput(object sender, MCEViewModel mce)
      {
         slFacing.IsVisible = false;
         InterventionsViewModel.Current.SaveMCE(mce.ControlValues.Text);
         MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Interventions);

         //DependencyService.Get<IScanner>().OpenScanner();
      }

      private void Entry_Unfocused(object sender, FocusEventArgs e)
      {
         entry.Focus();
      }

      public bool OnScann(string data, int length, Symbologies symbology, byte[] rawData)
      {
         try
         {
            //DependencyService.Get<IScanner>().CloseScanner();

            entry.Unfocused -= Entry_Unfocused;
            entry.Unfocus();

            var mem = GC.GetTotalMemory(false);

#if DEBUG

            if (MainViewModel.Current.Memory != -1)
            {
               Log.Write(new AuditTrail
               {
                  Level = ErrorLevel.Log,
                  Tag = "MEM",
                  Message = $"--- (OnScann) base {MainViewModel.Current.Memory:N0} current {mem:N0} => {MainViewModel.Current.Memory - mem:N0} bytes " + new string('-', 20),
               });
            };

            MainViewModel.Current.Memory = mem;
#endif


#if !SCAN_WEDGE
            slBarCode.IsVisible = false;

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
               Xamarin.Essentials.Vibration.Vibrate(100);
            };
#endif

            lNbScan.Text = InterventionsViewModel.Current.ScannCount().ToString();

            Durex_SKU a = null;

            try
            {
               a = InterventionsViewModel.Current.GetArt(data);
            }
            catch { };

            if (a == null)
            {
               DoIt.OnMainThread(() =>
               {

                  MainViewModel.Current.CurrentArticle = new Intervention_Params.Scann
                  {
                     EAN = data,
                  };

                  if (calView != null)
                  {
                     slScroll.Children.Remove(calView);
                     calView = null;
                  };

                  // new produit
                  NewArticleView.Display(this, mainGrid, CallBack);

                  return;
               });
            }
            else if (a.ToReplace && !string.IsNullOrEmpty(a.ReplacementEAN))
            {
               DoIt.OnMainThread(() =>
               {
                  MainViewModel.Current.CurrentArticle = new Intervention_Params.Scann
                  {
                     EAN = data,
                     Name = a.Name,
                  };

                  MainViewModel.Current.ReplacementArticle = new Intervention_Params.Scann
                  {
                     EAN = a.ReplacementEAN,
                     Name = MainViewModel.Current.durexParams.SKUsLists.First().SKUs.Where(x => x.EAN == a.ReplacementEAN).FirstOrDefault().Name,
                  };

                  if (calView != null)
                  {
                     slScroll.Children.Remove(calView);
                     calView = null;
                  };

                  // Replace produit
                  ReplaceArticleView.Display(this, mainGrid, CallBack);

                  return;
               });
            }
            else
            {
               //MainViewModel.Current.CurrentArticle = a;

               MainViewModel.Current.CurrentArticle = new Intervention_Params.Scann
               {
                  Condi = a.Content,
                  EAN = a.EAN,
                  Name = a.Name,
                  Price = a.ConsumerPrice,
               };

               MainViewModel.Current.UpdateCurrentArticle();

               MainViewModel.Current.CurrentData = data;

               //ToDo: var f = MainViewModel.Current.Families.Where(x => x.PK == a.FKFamily).FirstOrDefault();
               //ToDo: MainViewModel.Current.CurrentFamily = f;

               // - - -  - - - 

               if (MainViewModel.Current.CurrentData == MainViewModel.Current.PrevData || MainViewModel.Current.SelectedInterventionParams.Data.Scanns.Where(x => x.EAN == data).Count() > 0)
               {
                  DoIt.OnMainThread(async () =>
                  {
                     if (await DisplayAlertView.Display(this, mainGrid, "Error Code", "Already scanned. Do you want to replace the previous reading?", "ok", "cancel") == "ok")
                     {
                        // replace
                        MainViewModel.Current.SelectedInterventionParams.Data.Scanns.RemoveAll(x => x.EAN == data);
                     }
                     else
                     {
                        // nope
                     };

                     rowDefinition.Height = new GridLength(0, GridUnitType.Absolute);
                     slPreviousLayout.IsVisible = false;
                     ResetFacing();


                     calView.CalcHelper.Clear();
                     calView.IsVisible = true;
                     await scrollView.ScrollToAsync(0, 300, true);
                  });

                  return true;
               }
               else
               {
                  rowDefinition.Height = new GridLength(0, GridUnitType.Absolute);
                  slPreviousLayout.IsVisible = false;
                  ResetFacing();

                  calView.CalcHelper.Clear();
                  calView.IsVisible = true;
                  scrollView.ScrollToAsync(0, 300, true);
               };

               MainViewModel.Current.LogMemory("end1 OnScann");

               return true;
            };
         }
         catch (Exception ex)
         {
            var infos = new System.Collections.Generic.Dictionary<string, string>
                  {
                     { "Exception", ex.Message },
                     { "StackTrace", ex.StackTrace }
                  };

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("ShelfInventoryPage.OnScann", infos);
         };

         // - - -  - - - 

         MainViewModel.Current.LogMemory("end2 OnScann");
         return true;
      }

      private void ResetFacing()
      {
         slFacing.IsVisible = true;
         Facing = "-1";
         if (LastButton != null)
         {
            LastButton.Checked = false;
            LastButton = null;
         }
      }

      private void itemAppearing(object sender, ItemVisibilityEventArgs e)
      {
         var listView = (sender as ListView);

         var lastItem = UnitechViewModel.Current.LastScans.LastOrDefault();
         listView.ScrollTo(lastItem, ScrollToPosition.MakeVisible, true);
      }

      public void onPrevious()
      {
         OnBackButtonPressed();
      }

      protected override bool OnBackButtonPressed()
      {
         if (entry.IsFocused)
         {
         };

         entry.Unfocused -= Entry_Unfocused;
         entry.Unfocus();

         UnitechViewModel.Current.OnScann -= OnScann;
         UnitechViewModel.Current.Scanner_Disable(OnScann);

         return base.OnBackButtonPressed();
      }

      protected override void OnAppearing()
      {
         UnitechViewModel.Current.Scanner_Enable(OnScann);
         UnitechViewModel.Current.OnScann += OnScann;

         DoIt.OnMainThread(() =>
         {
            //#if SCAN_WEDGE
            entry.Unfocused += Entry_Unfocused;
            //#endif
            entry.Focus();
         });

         base.OnAppearing();

         if (calView == null)
         {
            slScroll.Children.Add(AddKeyboard());
         };
      }

      protected override void OnDisappearing()
      {
         entry.Unfocused -= Entry_Unfocused;
         entry.Unfocus();

         UnitechViewModel.Current.OnScann -= OnScann;
         UnitechViewModel.Current.Scanner_Disable(OnScann);

         base.OnDisappearing();
      }

      private void entry_Completed(object sender, EventArgs e)
      {
         var entry = sender as Entry;

         if (entry != null && entry.Text != null)
         {
            var st = entry.Text.Trim();

            if (!string.IsNullOrEmpty(st))
            {
               entry.Unfocused -= Entry_Unfocused;
               slBarCode.IsVisible = false;
               UnitechViewModel.Current.NewBarcode(st, st.Length, 0, null);
            };
         };
      }

      private void entry_TextChanged(object sender, TextChangedEventArgs e)
      {
         var entry = sender as Entry;
         entry.Unfocused -= Entry_Unfocused;

         UnitechViewModel.Current.Length = entry.Text.Length;
      }

      string Facing = "-1";
      ZPF.XF.Compos.RadioButton LastButton = null;

      private void RadioButton_CheckedChanged(object sender, EventArgs e)
      {
         LastButton = sender as ZPF.XF.Compos.RadioButton;

         if (LastButton.Checked) Facing = LastButton.CommandParameter as string;
      }

      async void btnEOFScan_Clicked(System.Object sender, System.EventArgs e)
      {
         OnBackButtonPressed();

         await Navigation.PopAsync();
      }
   }
}
