using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace _03_ChatServerWPF
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      private void Application_Startup(object sender, StartupEventArgs e)
      {
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
