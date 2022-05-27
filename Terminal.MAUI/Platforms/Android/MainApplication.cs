using Android.App;
using Android.Runtime;

namespace StoreCheck;

[Application]
public class MainApplication : MauiApplication
{
   public MainApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
   {
      MainViewModel.Current.DeviceID = GetComputerId();
   }

   private string GetComputerId()
   {
      return Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
   }

   protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
