using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StoreCheck.WPF
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      System.Threading.Mutex myMutex;

      private void Application_Startup(object sender, StartupEventArgs e)
      {
         bool aIsNewInstance = false;
         myMutex = new System.Threading.Mutex(true, "StoreCheck-WPF", out aIsNewInstance);
         if (!aIsNewInstance)
         {
            MessageBox.Show("An instance is already running...");
            App.Current.Shutdown();
         };

         //AppCenter.Start("07dea5b5-a644-478f-b441-7ece686cc1c6", typeof(Analytics), typeof(Crashes));

         // - - -  - - - 

         AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

         // - - -  - - - 
      }

      void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
      {
         Exception ex = e.ExceptionObject as Exception;

         // - - -  - - - 

         //e.IsTerminating = false;

         try
         {
            string FileName = MainViewModel.Current.DataFolder + "LastWords.txt";
            var at = new ZPF.AT.AuditTrail(ex, ZPF.AT.AuditTrail.TextFormat.TxtEx);

            if (System.IO.File.Exists(FileName))
            {
               System.IO.File.Delete(FileName);
            };

            System.IO.File.WriteAllText(FileName, at.DataOut);
         }
         catch { };
      }
   }
}
