using ZPF;
using ZPF.MCE;
using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class MCEPage : PageEx
   {
      MCEViewModel mce = null;

      public MCEPage()
      {
         Title = "QCM";

         InitializeComponent();

         SetAppBarContent();

         mce = new MCEViewModel(new XFMCEEngine());
         mce.OnButtonPressed += Mce_OnButtonPressed;
         mce.OnMessage += Mce_OnMessage;
         mce.OnFormOwnerDesign += Mce_OnFormOwnerDesign;
      }

      private void Mce_OnFormOwnerDesign(MCEViewModel sender, object panelHost, long CurrentID)
      {
         if (CurrentID == 10 && mce.ControlValues["Questionnaire"] == "SaladBar")
         {
            var sl = panelHost as StackLayout;
            var g = new Grid();
            g.Margin = new Thickness(10);
            sl.Children.Add(g);

            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var days = new string[] { "", "lundi", "mardi", "mercredi", "jeudi", "vendredi", "samedi", "dimanche" };
            var slices = new string[] { "matin", "aprèsm.", "nocturne" };

            int i = 0;
            foreach (var d in days)
            {
               g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

               {
                  var l = new Label { Text = d };
                  g.Add(l, 0, i);
               }

               int ii = 1;
               foreach (var s in slices)
               {
                  if (i == 0)
                  {
                     var l = new Label { Text = s, HorizontalTextAlignment = TextAlignment.Center };
                     g.Add(l, ii, i);
                  }
                  else
                  {
                     var l = new CheckBox { AutomationId = $"Presence.{d.Left(3)}.{s.Left(2)}", HorizontalOptions = LayoutOptions.Center };
                     g.Add(l, ii, i);
                  };

                  ii++;
               };

               i++;
            };
         }
         else
         {
            DoIt.OnMainThread(async () =>
            {
               await scrollView.ScrollToAsync(0, 0, true);
            });
         };
      }

      private void Mce_OnMessage(MCEViewModel sender, string Message)
      {
         DoIt.OnMainThread(async () =>
         {
            await DisplayAlert("StoreCheck", Message, "ok");
         });
      }


      public delegate void OnUpdateOutputHandler(object sender, MCEViewModel mce);
      public OnUpdateOutputHandler OnUpdateOutput { get; set; }

      private void Mce_OnButtonPressed(MCEViewModel sender, long ID, long targetID, MCEViewModel.ButtonTypes btnType)
      {
         switch (MainViewModel.Current.SelectedInterventionParams.FKActionType)
         {
            case (long)FKActionTypes.BeforeAfter:
            case (long)FKActionTypes.PhotoAuKM:
               {
                  if (targetID == -1 && btnType == MCEViewModel.ButtonTypes.Other)
                  {
                     if (ID == 12 || ID == 13)
                     {
                        btnType = MCEViewModel.ButtonTypes.OK;
                     };
                  };
               }
               break;
         };

         if (btnType == MCEViewModel.ButtonTypes.Cancel && targetID < 1)
         {
            DoIt.OnMainThread(async () =>
            {
               //await DisplayAlert("MCE", "Game over ...", "ok");
               await Navigation.PopAsync();
            });

            return;
         };

         if (btnType == MCEViewModel.ButtonTypes.OK && targetID < 1)
         {
            DoIt.OnMainThread(async () =>
            {
               mce.UpdateControlValues(slHost);
               Output = mce.ControlValues.Text;

               if (OnUpdateOutput != null)
               {
                  OnUpdateOutput(this, mce);
               };

               await Navigation.PopAsync();
            });

            return;
         };

         if (btnType == MCEViewModel.ButtonTypes.Other)
         {
            mce.UpdateControlValues(slHost);
            InterventionsViewModel.Current.SaveMCE(mce.ControlValues.Text);

            var item = mce.Items.Where(x => x.ID == ID).FirstOrDefault();

            DoIt.OnMainThread(async () =>
            {
               switch (item.Format)
               {
                  case "Photos":
                     var photosPage = new PhotosPage("","");
                     photosPage.Reset();
                     photosPage.SubTitle = item.Caption;
                     photosPage.Comment = item.Name;
                     photosPage.LoadData();
                     await Navigation.PushAsync(photosPage);
                     break;

                  case "Inventory":
                     await Navigation.PushAsync(new InventoryPage());
                     break;

                  case "Signature":
                     await Navigation.PushAsync(new SignaturePage());
                     break;

                  default:
                     mce.Exec(targetID, slHost);
                     break;
               };
            });

            return;
         };
      }


      bool OnAppearing_First = true;

      public string Input { get; set; } = "";
      public string Output { get; set; } = "";

      protected override async void OnAppearing()
      {
         base.OnAppearing();

         try
         {
            if (OnAppearing_First)
            {
               OnAppearing_First = false;

               if (mce.SetJSON(Input))
               {
                  //if ((CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == "NL" ? "NL" : "FR") == "NL")
                  //{
                  //   mce.FromNL();
                  //};

                  TStrings values = new TStrings();
                  values.Text = (string.IsNullOrEmpty(Output) ? "" : Output);

                  // - - -  - - - 

                  if (values.Text.Trim() == "")
                  {
                     //ToDo: values["Contact.Nom"] = MainViewModel.Current.SelectedStore.Interlocuteur;
                     values["Contact.Tel"] = MainViewModel.Current.SelectedStore.Phone;
                     values["Contact.eMail"] = MainViewModel.Current.SelectedStore.Mail;
                  };

                  // - - -  - - - 

                  //mce.ControlValues.Text = mce.ControlValues.Text + Environment.NewLine + values.Text;
                  mce.ControlValues.Text = values.Text;
                  mce.Exec(slHost);
               }
               else
               {
                  await DisplayAlert("StoreCheck", "Error while loading the questionnaire!", "ok");
               };
            };
         }
         catch (Exception ex)
         {
            await DisplayAlert("StoreCheck", "Error while loading the questionnaire!" + Environment.NewLine + ex.Message, "ok");
         };
      }

      protected override bool OnBackButtonPressed()
      {
         DoIt.OnMainThread(() =>
        {
           mce.UpdateControlValues(slHost);

           InterventionsViewModel.Current.SaveMCE(mce.ControlValues.Text);
        });

         return base.OnBackButtonPressed();
      }
   }
}
