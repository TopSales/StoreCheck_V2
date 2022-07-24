using Microsoft.Data.Sqlite;

using System;
using System.Collections.Generic;
using System.Linq;
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
using ZPF.SQL;
using static BackboneViewModel;

namespace StoreCheck.Pages
{
   /// <summary>
   /// Interaction logic for DatabasePage.xaml
   /// </summary>
   public partial class DatabasePage : Page
   {
      public DatabasePage()
      {
         InitializeComponent();

         // I18nViewModel.Current.T(this);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void btnCleanData_Click(object sender, RoutedEventArgs e)
      {
         if (UserViewModel.Current.CheckRights("DBase.Clean"))
         {
            if (BackboneViewModel.Current.MessageBox(MessageBoxType.Confirmation, "Effacer toutes les données de la base?"))
            {
               //ToDo: Clean 

               MainViewModel.Current.RefreshAll();
            };
         };
      }

      private void btnImportData_Click(object sender, RoutedEventArgs e)
      {
         MenuViewModel.Instance.NavigateMenu("IMP_EXP");
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
      private void btnBackup_Click(object sender, RoutedEventArgs e)
      {
         if (DBViewModel.Current.Connection.DBType == DBType.SQLite)
         {
            // Configure open file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Title = "Backup database ...";
            dlg.FileName = $"StoreCheck.{DateTime.Now.ToString("yyyyMMdd hhmm")}.db3";                 // Default file name
            dlg.DefaultExt = ".db3";                        // Default file extension
            dlg.Filter = "SQLite (.db3)|*.db3|All files (.*)|*.*"; // Filter files by extension 

#if DEBUG
            string BaseDir = @"C:\ProgramData\";

            dlg.InitialDirectory = BaseDir;
#endif

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            string FileName = "";

            // Process open file dialog box results 
            if (result == true)
            {
               FileName = dlg.FileName;

               try
               {
                  using (var destination = new SqliteConnection(string.Format(@"Data Source={0};", FileName)))
                  {
                     destination.Open();
                     ((SqliteConnection)(DBViewModel.Current.Connection.DbConnection)).BackupDatabase(destination);
                  };

                  BackboneViewModel.Current.MessageBox("Backup ok");
               }
               catch (Exception ex)
               {
                  BackboneViewModel.Current.MessageBox(ex.Message);
               };
            };
         }
         else
         {
            BackboneViewModel.Current.MessageBox("Seulement des bases locales peuvent être sauvegardées par le programme.");
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
