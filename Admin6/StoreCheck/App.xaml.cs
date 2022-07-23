using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
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
               // MainViewModel.Current.InitDB(true);

                this.Shutdown(OK);
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
