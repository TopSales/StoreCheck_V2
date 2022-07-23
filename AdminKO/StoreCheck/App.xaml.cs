using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using StoreCheck.Pages;
using ZPF;
using ZPF.AT;
using ZPF.SQL;

namespace StoreCheck
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      [DllImport("Kernel32.dll")]
      public static extern bool AttachConsole(int processId);

      private readonly int OK = 0;
      private readonly int WrongNumberArguments = 10;
      private readonly int FileNotFound = 20;


      System.Threading.Mutex myMutex;

      private void Application_Startup(object sender, StartupEventArgs e)
      {
         bool aIsNewInstance = false;
         myMutex = new System.Threading.Mutex(true, "StoreCheck-WPF", out aIsNewInstance);
         if (!aIsNewInstance)
         {
            MessageBox.Show("Already an instance is running...");
            App.Current.Shutdown();
         };

         // - - -  - - - 

         AppCenter.Start("f56b216d-4570-41ba-a2ae-77bed4efaa1e", typeof(Analytics), typeof(Crashes));
         
         AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

         // - - -  - - - 

         string[] args = Environment.GetCommandLineArgs();
         TStrings cmd = new TStrings();

         for (int index = 1; index < args.Length; index++)
         {
            cmd.Add(args[index]);
         }

         if (cmd.Count > 0)
         {
            AttachConsole(-1);
         };

         if (!string.IsNullOrEmpty(cmd["/?"]))
         {
            Console.WriteLine("");
            Console.WriteLine("   StoreCheck /Ver");
            Console.WriteLine("   StoreCheck /IsDemo");
            Console.WriteLine("   StoreCheck /DemoData");
            Console.WriteLine("   StoreCheck /ImportTerminal [LogFileName]");
            Console.WriteLine("   StoreCheck /ImportStock 'FileName'");

            this.Shutdown(OK);
         }

         if (!string.IsNullOrEmpty(cmd["/exit"]))
         {
            this.Shutdown(OK);
         }

         if (!string.IsNullOrEmpty(cmd["/Ver"]))
         {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var v = assembly.GetName().Version;
            string AV = v.ToString();

#if DEBUG
            AV += " (debug)";
#endif
            Console.WriteLine("");
            Console.WriteLine("StoreCheck - V{0}", AV);

            this.Shutdown(OK);
         };

         if (!string.IsNullOrEmpty(cmd["/DemoData"]))
         {
            //SampleViewModel.Current.DeleteAll();
            MainViewModel.Current.InitDB(true);

            this.Shutdown(OK);
         };

#if TDB
         if (!string.IsNullOrEmpty(cmd["/ImportTerminal"]))
         {
            MainViewModel.Current.Load();
            StockViewModel.Current.ImportPal(MainViewModel.Current.ImportFiles);

            if (cmd.Count > 1 && !string.IsNullOrEmpty(FindFileName(cmd)))
            {
               ZPF.WWW.Excel.ExportXLS(null, DB_SQL.QuickQueryView("select * from ImportLog") as DataTable, FindFileName(cmd) + ".xlsx", false,
            (MainViewModel.Current.IsDemo ? MainViewModel.MaxArticles : -1));
            };

            this.Shutdown(OK);
         };
#endif

         if (!string.IsNullOrEmpty(cmd["/ImportStock"]))
         {
            if (cmd.Count < 2)
            {
               Console.WriteLine("");
               Console.WriteLine("Wrong number of arguments:");
               Console.WriteLine("   StoreCheck /ImportStock 'FileName'");

               this.Shutdown(WrongNumberArguments);
               return;
            };

            string FileName = FindFileName(cmd);

            if (System.IO.File.Exists(FileName))
            {
               new ImportPage().ImportStock(FileName);
            }
            else
            {
               Console.WriteLine("");
               Console.WriteLine("File ({0}) not found.", FileName);
               this.Shutdown(FileNotFound);
               return;
            };

            this.Shutdown(OK);
         };
      }

      private string FindFileName(TStrings cmd)
      {
         string Result = "";

         for (int i = 0; i < cmd.Count; i++)
         {
            string st = cmd[i].Trim();

            if (!st.StartsWith("/"))
            {
               if (st.StartsWith("\"") && st.EndsWith("\""))
               {
                  Result = st.Substring(1, st.Length - 2);
               }
               else
               {
                  Result = st;
               };
            };
         };

         return Result;
      }

      internal void DynamicLoadStyles(TStrings FileNames)
      {
         if (!File.Exists(FileNames[0]))
         {
            Log.Write(ZPF.AT.ErrorLevel.Info, "DynamicLoadStyles: File: " + FileNames[0] + " does not exist.");
            return;
         };

         Resources.BeginInit();

         try
         {
            // Clear any previous dictionaries loaded
            Resources.MergedDictionaries.Clear();

            for (int i = 0; i < FileNames.Count; i++)
            {
               if (File.Exists(FileNames[i]))
               {
                  using (FileStream fs = new FileStream(FileNames[i], FileMode.Open))
                  {
                     // Read in ResourceDictionary File
                     ResourceDictionary dic = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(fs);

                     // Add in newly loaded Resource Dictionary
                     Resources.MergedDictionaries.Add(dic);
                  }
               }
               else
               {
                  if (Debugger.IsAttached)
                  {
                     MessageBox.Show("DynamicLoadStyles: File: " + FileNames[i] + " does not exist. Please re-enter the name.");
                  }

                  Log.Write(ZPF.AT.ErrorLevel.Info, "DynamicLoadStyles: File: " + FileNames[i] + " does not exist.");
               }
            }
         }
         finally
         {
            Resources.EndInit();
         };
      }

      void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
      {
         Exception ex = e.ExceptionObject as Exception;

         Log.Write(new AuditTrail
         {
            Level = ErrorLevel.Critical,
            Tag = "UnhandledException",
            Message = ex.Message + " (1)",
            DataOut = ex.StackTrace + Environment.NewLine + Environment.NewLine + ex.Source
         });

         if (ex.Message.StartsWith("Could not load file or assembly 'Interop.BarTender"))
         {
            if (e.IsTerminating)
            {
               MessageBox.Show(ex.Message, "BarTender", MessageBoxButton.OK, MessageBoxImage.Error);
            };
         }
         else
         {
            MessageBox.Show(ex.Message, "Uncaught Thread Exception", MessageBoxButton.OK, MessageBoxImage.Error);
         };

         // - - -  - - - 

         if (DB_SQL._ViewModel.CurrentTransaction != null)
         {
            try
            {
               DB_SQL.Commit(DB_SQL._ViewModel.CurrentTransaction);
               Log.Write(ErrorLevel.Critical, "Pending transaction on exception exit !!!");
            }
            catch { };
         };

         DB_SQL._ViewModel.Close();
      }

      private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
      {
         Exception ex = e.Exception;

         Log.Write(new AuditTrail
         {
            Level = ErrorLevel.Critical,
            Tag = "UnhandledException",
            Message = ex.Message + " (2)",
            DataOut = ex.StackTrace + Environment.NewLine + Environment.NewLine + ex.Source
         });

         if (false)
         {
            //Handling the exception within the UnhandledExcpeiton handler.
            MessageBox.Show(e.Exception.Message, "Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
         }
         else
         {
            //If you do not set e.Handled to true, the application will close due to crash.
            MessageBox.Show("Application is going to close!", "Uncaught Exception");
            e.Handled = false;
         }

         // - - -  - - - 

         if (DB_SQL._ViewModel.CurrentTransaction != null)
         {
            try
            {
               DB_SQL.Commit(DB_SQL._ViewModel.CurrentTransaction);
               Log.Write(ErrorLevel.Critical, "Pending transaction on exception exit !!!");
            }
            catch { };
         };

         DB_SQL._ViewModel.Close();
      }

   }
}
