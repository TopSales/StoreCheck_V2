using System;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Xamarin.Forms;
using ZPF.XF;

[assembly: ExportFont("Montserrat.ttf", Alias = "Montserrat")]

namespace StoreCheck
{
   public partial class App : Application
   {
      public App()
      {
         InitializeComponent();

         //MainPage = new SplashScreen();
         MainPage = new NavigationPage(new _HomePage());
      }

      protected override void OnStart()
      {
         // Distribute uses a public distribution group
         Distribute.UpdateTrack = UpdateTrack.Public | UpdateTrack.Private;

         // In this example OnReleaseAvailable is a method name in same class
         Distribute.ReleaseAvailable = OnReleaseAvailable;

         //Distribute.SetEnabledForDebuggableBuild(true);

         AppCenter.Start("android=3c340387-fd1a-447e-ae8e-b41f513fdcce;" +
                  "uwp=c30790ae-8a77-4308-90e1-4436874ca3de;" +
                  "ios={Your iOS App secret here}",
                  typeof(Analytics), typeof(Crashes), typeof(Distribute));

         Analytics.TrackEvent("App.OnStart()");
      }

      protected override void OnSleep()
      {
         //ZPF.AT.Log.Write(ZPF.AT.ErrorLevel.Log, "App.OnSleep()");

         MainViewModel.Current.SaveLocalConfig();

         Analytics.TrackEvent("App.OnSleep()");
      }

      protected override void OnResume()
      {
         //ZPF.AT.Log.Write(ZPF.AT.ErrorLevel.Log, "App.OnResume()");
         Analytics.TrackEvent("App.OnResume()");
      }

      /// <summary>
      /// https://docs.microsoft.com/en-us/appcenter/sdk/distribute/xamarin
      /// replaces the SDK dialog with a custom one:
      /// </summary>
      /// <param name="releaseDetails"></param>
      /// <returns></returns>
      bool OnReleaseAvailable(ReleaseDetails releaseDetails)
      {
         // Look at releaseDetails public properties to get version information, release notes text or release notes URL
         string versionName = releaseDetails.ShortVersion;
         string versionCodeOrBuildNumber = releaseDetails.Version;
         string releaseNotes = releaseDetails.ReleaseNotes;
         Uri releaseNotesUrl = releaseDetails.ReleaseNotesUrl;

         // custom dialog
         var title = "Version " + versionName + " available!";
         Task answer;

         // On mandatory update, user cannot postpone
         if (releaseDetails.MandatoryUpdate)
         {
            answer = Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install");
         }
         else
         {
            answer = Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install", "Maybe tomorrow...");
         }
         answer.ContinueWith((task) =>
         {
            // If mandatory or if answer was positive
            if (releaseDetails.MandatoryUpdate || (task as Task<bool>).Result)
            {
               // Notify SDK that user selected update
               Distribute.NotifyUpdateAction(UpdateAction.Update);
            }
            else
            {
               // Notify SDK that user selected postpone (for 1 day)
               // Note that this method call is ignored by the SDK if the update is mandatory
               Distribute.NotifyUpdateAction(UpdateAction.Postpone);
            }
         });

         // Return true if you are using your own dialog, false otherwise
         return true;
      }
   }
}

