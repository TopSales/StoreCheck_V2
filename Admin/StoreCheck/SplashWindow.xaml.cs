using System;
using System.Timers;
using System.Windows;
using ZPF;
using ZPF.AT;
using ZPF.SQL;
using ZPF.WPF;
using static BackboneViewModel;

namespace StoreCheck
{
   /// <summary>
   /// Interaction logic for SplashWindow.xaml
   /// </summary>
   public partial class SplashWindow : Window
   {
      public SplashWindow()
      {
         InitializeComponent();

         BackboneViewModel.Current.InitMsgCallBack(MsgCallBack);

         //Timer t = new Timer();
         //t.Interval = 200;
         //t.Elapsed += t_Elapsed;
         //t.Start();

         System.Windows.Threading.DispatcherTimer t = new System.Windows.Threading.DispatcherTimer();
         t.Interval = TimeSpan.FromMilliseconds(200);
         t.Tick += T_Tick; 
         t.Start();
      }


      bool FatalOups = false;

      bool MsgCallBack(MessageBoxType type, string Text, string Caption = "")
      {
         bool Result = true;
         FatalOups = true;

         DoIt.OnMainThread(() =>
         {
            Result = WPFMessageBox.Show(this, type, Text, Caption);

            this.Close();
         });

         return Result;
      }


      bool t_Elapsed_Sema = false;

      private void T_Tick(object sender, EventArgs e)
      {
      //   throw new NotImplementedException();
      //}
      //void t_Elapsed(object sender, ElapsedEventArgs e)
      //{
         System.Diagnostics.Debug.WriteLine("t_Elapsed");

         (sender as Timer)?.Stop();

         if (t_Elapsed_Sema) return;

         // - - -  - - - 

         t_Elapsed_Sema = true;

         Log.WriteTimeStamp("Init MainViewModel");

         if (DB_SQL._ViewModel == null)
         {
            MainViewModel.Current.OpenDB();
         };

         MainViewModel.Current.InitDB(true);
         UserAdminViewModel.Current.LoadData(true);

         MainViewModel.Current.Load();

         AuditTrailViewModel.Current.Init(new DBAuditTrailWriter(DB_SQL._ViewModel));
         Log.InitTimeStamp();

         AuditTrailPage auditTrailPage = null;

         Application.Current.Dispatcher.Invoke(new System.Action(() =>
         {
            auditTrailPage = new AuditTrailPage( AuditTrailViewModel.Current, MainViewModel.IniFileName);
         }));

         // - - -  - - - 

         //ToDo: Log.WriteHeader(MainViewModel.DeviceInfo.APP, MainViewModel.DeviceInfo.AV, MainViewModel.DeviceInfo.OS);

         // - - -  - - - 

         Application.Current.Dispatcher.Invoke(new System.Action(() =>
         {
            MainViewModel.Current.Load();
            string fmt = Environment.CurrentDirectory + @"\Styles\{0}.xaml";

            TStrings FileNames = new TStrings();
            FileNames.Add(string.Format(fmt, MainViewModel.Current.Style));

//#if DEBUG
//            (Application.Current as App).DynamicLoadStyles(FileNames);
//#else
//            (Application.Current as App).DynamicLoadStyles(FileNames);
//#endif
            Log.WriteTimeStamp("End Splash");

            this.Hide();

            if (!FatalOups)
            {
               //var timer = new DispatcherTimer
               //  (
               //  TimeSpan.FromMinutes(1),
               //  DispatcherPriority.ApplicationIdle,// Or DispatcherPriority.SystemIdle
               //  (s, e2) =>
               //  {
               //     BackboneViewModel.Current.IncBusy();
               //     BackboneViewModel.Current.BusyTitle = "En veille ...";

               //     BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Info, "En veille ...");
               //     MainViewModel.Current.RefreshAll();

               //     BackboneViewModel.Current.DecBusy();
               //  },
               //  Application.Current.Dispatcher
               //  );

               new MainWindow().ShowDialog();

               //Init
               this.Close();
            };
         }));
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {

      }
   }
}
