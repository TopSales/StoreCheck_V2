using Android.App;
using Android.Content.PM;
using Android.OS;

namespace StoreCheck;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
   public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
   {
      base.OnCreate(savedInstanceState, persistentState);
   }
}
