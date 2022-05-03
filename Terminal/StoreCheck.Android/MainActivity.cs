using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;

namespace StoreCheck.Droid
{
   // https://docs.microsoft.com/en-us/xamarin/android/user-interface/splash-screen

   [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
   public class SplashActivity : AppCompatActivity
   {
      static readonly string TAG = "X:" + typeof(SplashActivity).Name;

      public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
      {
         base.OnCreate(savedInstanceState, persistentState);

         //// - - -  - - - 

         //if (Xamarin.Forms.Device.Idiom == Xamarin.Forms.TargetIdiom.Phone || Xamarin.Forms.Device.Idiom == Xamarin.Forms.TargetIdiom.Unsupported)
         //{
         //   // layout views vertically
         //   this.RequestedOrientation = ScreenOrientation.Portrait;
         //}
         //else
         //{
         //   // layout views horizontally
         //   this.RequestedOrientation = ScreenOrientation.Sensor;
         //};

         // - - -  - - - 
      }

      // Launches the startup task
      protected override void OnResume()
      {
         base.OnResume();
         Task startupWork = new Task(() => { Startup(); });
         startupWork.Start();
      }

      // Startup background work that happens behind the splash screen
      void Startup()
      {
         StartActivity(new Android.Content.Intent(Application.Context, typeof(MainActivity)));
      }
   }

   [Activity(Label = "StoreCheck", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
   public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
   {
      protected override void OnCreate(Bundle savedInstanceState)
      {
         TabLayoutResource = Resource.Layout.Tabbar;
         ToolbarResource = Resource.Layout.Toolbar;

         base.OnCreate(savedInstanceState);

         // - - -  - - - 

         if (Xamarin.Forms.Device.Idiom == Xamarin.Forms.TargetIdiom.Phone || Xamarin.Forms.Device.Idiom == Xamarin.Forms.TargetIdiom.Unsupported)
         // if (size < 5)
         {
            // layout views vertically
            this.RequestedOrientation = ScreenOrientation.Portrait;
         }
         else
         {
            // layout views horizontally
            this.RequestedOrientation = ScreenOrientation.Sensor;
         }

         // - - -  - - - 

         FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
         Xamarin.Essentials.Platform.Init(this, savedInstanceState);
         global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

         // - - -  - - - 

         LoadApplication(new App());
      }

      protected override void OnResume()
      {
         base.OnResume();
      }

      protected override void OnPause()
      {
         base.OnPause();
      }

      public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
      {
         Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

         base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
      }
   }
}
