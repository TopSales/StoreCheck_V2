using Xamarin.Forms.Xaml;
using ZPF;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class EntryPage : Page_Base
   {
      public EntryPage()
      {
         BindingContext = MainViewModel.Current;

         InitializeComponent();

         MainViewModel.Current.EntryMsg = "Trying to connect to the server ..."; 

         Title = "entry";
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();

         ClientViewModel.Current.Connect();

         DoIt.Delay(100, () =>
         {
            bool isOK = true;

            if (ClientViewModel.Current.IsConnected())
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

            if( isOK)
            {
               DoIt.OnMainThread(() =>
               {
                  MainViewModel.Current.EntryMsg = "ID send ...";
               });
            }
            else
            {
               DoIt.OnMainThread(() =>
               {
                  DisplayAlert("Error", "Not connected to server!", "ok");
                  MainViewModel.Current.EntryMsg = "Not connected to server!";
               });
            };
         });
      }
   }
}
