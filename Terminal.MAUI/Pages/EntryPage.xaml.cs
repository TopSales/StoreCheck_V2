using ZPF;
using ZPF.XF.Compos;

namespace StoreCheck.Pages;

public partial class EntryPage : PageEx
{
   public EntryPage()
   {
      BindingContext = MainViewModel.Current;

      InitializeComponent();

      MainViewModel.Current.EntryMsg = "Trying to connect to the server ...";

      Title = "entry";
   }

   protected override async void OnAppearing()
   {
      base.OnAppearing();

      //ClientViewModel.Current.Connect();

      if (MainViewModel.Current.Config.FKUser > 0)
      {
         await Navigation.PopModalAsync();
      }
      else
      {
         DoIt.Delay(200, () =>
         {
            bool isOK = true;

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
               try
               {
                  ClientViewModel.Current.Entry(MainViewModel.Current.DeviceID);

                  isOK = true;
               }
               catch
               {
                  isOK = false;
               };
            }
            else
            {
               isOK = false;
            };

            if (isOK)
            {
               MainViewModel.Current.EntryMsg = "ID send ...";
            }
            else
            {
               DisplayAlert("Error", "Could not connect to the server!", "ok");
               MainViewModel.Current.EntryMsg = "Could not connect to the server!";
            };
         });
      };
   }

   private async void btnBack_Clicked(object sender, EventArgs e)
   {
      await Navigation.PopModalAsync();
   }
}
