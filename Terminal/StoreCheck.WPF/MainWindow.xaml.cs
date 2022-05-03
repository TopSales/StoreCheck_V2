using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;
using ZPF;
using ZPF.AT;

namespace StoreCheck.WPF
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : FormsApplicationPage
   {
#if DEBUG
      bool KioskMode = false;
#else
      bool KioskMode = true;
#endif

      public MainWindow()
      {
         InitializeComponent();

         // - - - config - - -

         Params Config = null;

         {
            string DataFolder = DataFolder = @"C:\StoreCheck\Data\";

            if (!System.IO.Directory.Exists(DataFolder))
            {
               System.IO.Directory.CreateDirectory(DataFolder);
            };

            string FileName = DataFolder + @"StoreCheck.Params.json";

            if (System.IO.File.Exists(FileName))
            {
               string json = File.ReadAllText(FileName);

               var p = JsonConvert.DeserializeObject<Params>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
               if (p != null)
               {
                  Config = p;
               };
            }
            else
            {
            };
         };

         // - - - spooler - - -

         var insertQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");

         ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
         insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
         insertWatcher.Start();

         ZPF.Compos.XF.WPF.ButtonExRenderer.Initialize();

         Forms.Init();
         LoadApplication(new StoreCheck.App());
      }

      private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
      {
         try
         {
            var x = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Removable && d.VolumeLabel.ToUpper() == "StoreCheck").FirstOrDefault();

            if (x != null)
            {
               DoIt.OnMainThread(() =>
               {
                  this.Topmost = false;
                  //this.WindowState = WindowState.Maximized;
                  this.WindowStyle = WindowStyle.SingleBorderWindow;
                  this.ResizeMode = ResizeMode.CanResize;

                  // - - -  - - - 

                  Process.Start(System.IO.Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe"));
               });
            };
         }
         catch { };
      }

      private void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
      {
      }

      //private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
      //{
      //   WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");

      //   ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
      //   insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
      //   insertWatcher.Start();

      //   WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
      //   ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
      //   removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
      //   removeWatcher.Start();

      //   // Do something while waiting for events
      //   System.Threading.Thread.Sleep(20000000);
      //}
   }
}
