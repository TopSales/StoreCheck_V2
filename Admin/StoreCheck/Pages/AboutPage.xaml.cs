using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
using ZPF;
using ZPF.AT;

namespace StoreCheck.Pages
{
   /// <summary>
   /// Interaction logic for AboutPage.xaml
   /// </summary>
   public partial class AboutPage : Page
   {
      public static string ResourceName = "StoreCheck.ChangeLog.PC.md";

      public AboutPage()
      {
         InitializeComponent();

         {
            //lbVersion.Content = AnalyticsHelper.DeviceInfo.AV;
            lbVersion.Content = $"{VersionInfo.sVersion} ({VersionInfo.BuildOn})";
         };

         using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))
         using (StreamReader reader = new StreamReader(stream))
         {
            TStrings text = new TStrings();
            tbText.TextEx = reader.ReadToEnd();
            // tbText.TextEx = "<body BGColor='FFADD8E6'><br/><br/><b>Echec</b></body>";
         };
      }

      private void Label_Support(object sender, object e)
      {
         System.Diagnostics.Process process = new System.Diagnostics.Process();
         process.StartInfo.FileName = "mailto:StoreCheck@ZPF.fr?subject=StoreCheck";
         process.Start();
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         //¤ DialogResult = true;
      }

/*
      private void SendTrace_Click(object sender, RoutedEventArgs e)
      {
         VMLocator.Backbone.IncBusy();

         //Send Trace
         System.Net.Mail.MailMessage Mail = new System.Net.Mail.MailMessage();

         System.Net.Mail.SmtpClient SmtpServer = null;

         //if (false)
         //{
         //   SmtpServer = new System.Net.Mail.SmtpClient("smtp.Gloups.Name");
         //   SmtpServer.Credentials = new System.Net.NetworkCredential("EuroGold@Gloups.Name", "MossIsTheBoss");
         //}
         //else
         //{
         //   SmtpServer = new System.Net.Mail.SmtpClient("smtp.Audixis.com");
         //   SmtpServer.Credentials = new System.Net.NetworkCredential("StoreCheck@Audixis.com", "Audixis78870");
         //};

         SmtpServer.Port = 25;

         Mail.From = new MailAddress("StoreCheck@ZPF.fr");
         Mail.To.Add("Support@ZPF.fr");
         Mail.Subject = "Panic mail from StoreCheck - " + DateTime.Now.ToString("dd/MM/yy HH:mm:ss");
         Mail.Body = Mail.Subject + " Version : " + lbVersion.Content;

         // System.Net.Mail.Attachment Attachment = new System.Net.Mail.Attachment(@AuditTrail.FileName);
         System.Net.Mail.Attachment Attachment = new System.Net.Mail.Attachment(AuditTrailViewModel.Current.FileName);

         Mail.Attachments.Add(Attachment);

         try
         {
            SmtpServer.Send(Mail);

            // Clear Audit Trail file
            AuditTrailViewModel.Current.Clear();

            VMLocator.Backbone.DecBusy();
         }
         catch (Exception ex)
         {
            VMLocator.Backbone.DecBusy();

            MessageBox.Show(ex.Message);
         };

         //¤ DialogResult = true;
      }
*/
      private void wwwZPF(object sender, object e)
      {
         System.Diagnostics.Process process = new System.Diagnostics.Process();
         process.StartInfo.FileName = "http://www.ZPF.fr";
         process.Start();
      }

      private void wwwStoreCheck(object sender, object e)
      {
         System.Diagnostics.Process process = new System.Diagnostics.Process();
         process.StartInfo.FileName = "https://www.ZPF.fr/logiciels/894-StoreCheck-2017-logiciel-de-gestion-de-stock.html";
         process.Start();
      }
   }
}
