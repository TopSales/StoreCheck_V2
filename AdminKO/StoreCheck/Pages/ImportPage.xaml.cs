//using OpenXML.Silverlight.Spreadsheet;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using OpenXML.Silverlight.Spreadsheet;
using OpenXML.Silverlight.Spreadsheet.Parts;
using ZPF;
using ZPF.AT;
using ZPF.SQL;
using ZPF.XLS;
using static BackboneViewModel;

namespace StoreCheck.Pages
{
   /// <summary>
   /// Interaction logic for ImportPage.xaml
   /// </summary>
   public partial class ImportPage : Page
   {
      public ImportPage()
      {
         InitializeComponent();

         // - - -  - - - 

         // I18nViewModel.Current.T(this);

#if TDB
         DB_SQL.QuickQuery("delete from ImportLog");
         StockViewModel.Current.LogImportLog();
#endif
      }

      private void btnImportStock_Click(object sender, RoutedEventArgs e)
      {
         if (btnReplace.IsChecked == true)
         {
            if (!UserViewModel.Current.CheckRights("Import.Stock.R"))
            {
               BackboneViewModel.Current.MessageBox(MessageBoxType.Warning, "Droits insuffisants");
               return;
            };
         }
         else
         {
            if (!UserViewModel.Current.CheckRights("Import.Stock.A"))
            {
               BackboneViewModel.Current.MessageBox(MessageBoxType.Warning, "Droits insuffisants");
               return;
            };
         };

         if (btnReplace.IsChecked == true)
         {
            if (!BackboneViewModel.Current.MessageBox(MessageBoxType.Confirmation, "Etes-vous sur de vouloir remplacer les données ?"))
            {
               return;
            };
         };

         // Configure open file dialog box
         Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
         dlg.Title = "Import stock ...";
         //dlg.IniFileName = "Document";                 // Default file name
         dlg.DefaultExt = ".xlsx";                       // Default file extension
         dlg.Filter = "Excel (.xlsx)|*.xlsx";            // Filter files by extension 

#if DEBUG
         string BaseDir = @"D:\Software\Projects\PIMS\Doc\";

         dlg.InitialDirectory = BaseDir;
         dlg.FileName = @"Import.01.xlsx";
#endif

         // Show open file dialog box
         Nullable<bool> result = dlg.ShowDialog();

         // Process open file dialog box results 
         if (result == true)
         {
            // Open document 
            string FileName = dlg.FileName;

            BackboneViewModel.Current.BusyTitle = "Importation ...";
            BackboneViewModel.Current.IncBusy();
            ImportStock(FileName);
            BackboneViewModel.Current.DecBusy();

            {
               BackboneViewModel.Current.MessageBox(MessageBoxType.Info, "Importation terminée.");
            };

            //StockViewModel.Current.LoadData();
            //ItemsViewModel.Current.LoadData();
         };
      }

      public void ImportStock(string FileName)
      {
         //Todo: deplacer dans le ViewModel

         Log.InitTimeStamp();
         DbTransaction Transaction = null;

         // ...
      }

      //private int ImportXLSEmpl(TStrings record, string Ref, string Name)
      //{
      //   int Result = -1;

      //   Ref = record[Ref];

      //   Result = DB_SQL.QuickQueryInt(string.Format("select PK from Emplacement where Ref='{0}'", Ref));

      //   if (Result < 1)
      //   {
      //      string Nom = record[Name];

      //      if (string.IsNullOrEmpty(Ref))
      //      {
      //         Result = DB_SQL.QuickQueryInt(string.Format("select PK from Emplacement where Nom='{0}'", DB_SQL.StringToSQL(DB_SQL._ViewModel.DBType, Nom)));

      //         if (Result < 1)
      //         {
      //            switch (DB_SQL._ViewModel.DBType)
      //            {
      //               case DBType.SQLite:
      //                  Ref = string.Format("{0}{1}", "E", DB_SQL.QuickQueryInt("select seq from sqlite_sequence WHERE name = 'Emplacement'") + 1);
      //                  break;

      //               default:
      //               case DBType.SQLServer:
      //                  Ref = string.Format("{0}{1}", "E", DB_SQL.QuickQueryInt("SELECT IDENT_CURRENT('Emplacement')") + 1);
      //                  break;
      //            };
      //         }
      //         else
      //         {
      //            return Result;
      //         };
      //      };


      //      if (!string.IsNullOrEmpty(Ref) && !string.IsNullOrEmpty(Nom))
      //      {
      //         DB_SQL.QuickQuery(string.Format("insert into Emplacement (Ref, BarCode, Nom, FKEmplacement, FKTypeEmplacement, FKSite, Level) values ('{0}','{0}','{1}',0,0,0,1)", Ref, DB_SQL.StringToSQL(DB_SQL._ViewModel.DBType, Nom)));

      //         Result = DB_SQL.QuickQueryInt(string.Format("select PK from Emplacement where Ref='{0}'", Ref));
      //      };
      //   };

      //   return Result;
      //}

      //private int ImportXLSFourn(TStrings record, string Ref, string Name)
      //{
      //   int Result = -1;

      //   Ref = record[Ref];

      //   Result = DB_SQL.QuickQueryInt(string.Format("select PK from Contact where Ref='{0}'", Ref));

      //   if (Result < 1)
      //   {
      //      string Nom = record[Name];

      //      if (string.IsNullOrEmpty(Ref))
      //      {
      //         Result = DB_SQL.QuickQueryInt(string.Format("select PK from Contact where Societe='{0}'", DB_SQL.StringToSQL(DB_SQL._ViewModel.DBType, Nom)));

      //         if (Result < 1)
      //         {
      //            switch (DB_SQL._ViewModel.DBType)
      //            {
      //               case DBType.SQLite:
      //                  Ref = string.Format("{0}{1}", "F", DB_SQL.QuickQueryInt("select seq from sqlite_sequence WHERE name = 'Contact'") + 1);
      //                  break;

      //               default:
      //               case DBType.SQLServer:
      //                  Ref = string.Format("{0}{1}", "F", DB_SQL.QuickQueryInt("SELECT IDENT_CURRENT('Contact')") + 1);
      //                  break;
      //            };
      //         }
      //         else
      //         {
      //            return Result;
      //         };
      //      };

      //      if (!string.IsNullOrEmpty(Ref) && !string.IsNullOrEmpty(Nom))
      //      {
      //         DB_SQL.QuickQuery(string.Format("insert into Contact (Ref, Societe, IsFournisseur, FKContact) values ('{0}','{1}',1,0)", Ref, DB_SQL.StringToSQL(DB_SQL._ViewModel.DBType, Nom)));

      //         Result = DB_SQL.QuickQueryInt(string.Format("select PK from Contact where Ref='{0}'", Ref));
      //      };
      //   };

      //   return Result;
      //}

      private int ImportXLS(string TableName, TStrings record, string Ref, string Name)
      {
         int Result = -1;

         Ref = record[Ref];

         Result = DB_SQL.QuickQueryInt(string.Format("select PK from {0} where Ref='{1}'", TableName, Ref));

         if (Result < 1)
         {
            string Nom = record[Name];

            if (string.IsNullOrEmpty(Ref))
            {
               Result = DB_SQL.QuickQueryInt(string.Format("select PK from {0} where Nom='{1}'", TableName, DB_SQL.StringToSQL(DB_SQL._ViewModel.DBType, Nom)));

               if (Result < 1)
               {
                  switch (DB_SQL._ViewModel.DBType)
                  {
                     case DBType.SQLite:
                        Ref = string.Format("{0}{1}", "F", DB_SQL.QuickQueryInt($"select seq from sqlite_sequence WHERE name = '{TableName}'") + 1);
                        break;

                     default:
                     case DBType.SQLServer:
                        Ref = string.Format("{0}{1}", "F", DB_SQL.QuickQueryInt($"SELECT IDENT_CURRENT('{TableName}')") + 1);
                        break;
                  };
               }
               else
               {
                  return Result;
               };
            };


            if (!string.IsNullOrEmpty(Ref) && !string.IsNullOrEmpty(Nom))
            {
               DB_SQL.QuickQuery(string.Format("insert into {0} (Ref, Nom) values ('{1}','{2}')", TableName, Ref, DB_SQL.StringToSQL(DB_SQL._ViewModel.DBType, Nom)));

               Result = DB_SQL.QuickQueryInt(string.Format("select PK from {0} where Ref='{1}'", TableName, Ref));
            };
         };

         return Result;
      }

#if TDB
      private void btnImportTerminal_Click(object sender, RoutedEventArgs e)
      {
         if (MainViewModel.Current.ImportFiles != null)
         {
            StockViewModel.Current.ImportPal(MainViewModel.Current.ImportFiles);
            BackboneViewModel.Current.MessageBox(MessageBoxType.Info, "Importation terminée.");
         }
         else
         {
            BackboneViewModel.Current.MessageBox(MessageBoxType.Warning, "Il n’y a pas de fichiers d’importation.");
         };
      }
#endif

      private void btnExcelLog_Click(object sender, RoutedEventArgs e)
      {
         ExcelHelper.ExportXLS(null, DB_SQL.QuickQueryView("select * from ImportLog") as DataTable, System.IO.Path.GetTempFileName() + ".xlsx", true,-1);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
      private void btnExcelStock_Click(object sender, RoutedEventArgs e)
      {
         //ExcelHelper.ExportXLS(null, VMLocator.Stock.GetStockView(true), System.IO.Path.GetTempFileName() + ".xlsx", true,
         //   (MainViewModel.Current.IsDemo ? MainViewModel.MaxArticles : -1));
      }

      private void btnJSONStock_Click(object sender, RoutedEventArgs e)
      {
         // Configure open file dialog box
         Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

         dlg.Title = "Stock";
         dlg.FileName = string.Format("Stock.{0}.json", DateTime.Now.ToString("yyMMdd HHmmss"));
         dlg.Filter = "JSON document (.json)|*.json";
         dlg.DefaultExt = ".json";
         //dlg.InitialDirectory = ReportingViewModel.Instance.InitialDirectory;

         // Show open file dialog box
         Nullable<bool> result = dlg.ShowDialog(Application.Current.MainWindow);

         // Process open file dialog box results 
         if (result == true)
         {
            if (System.IO.File.Exists(dlg.FileName))
            {
               System.IO.File.Delete(dlg.FileName);
            };

            TStrings file = new TStrings();

            BackboneViewModel.Current.BusyTitle = "Exportation ...";
            BackboneViewModel.Current.IncBusy();

            //var list = DB_SQL.Query<Stock_View>(VMLocator.Stock.GetStockViewSQL(true));

            //if (MainViewModel.Current.IsDemo)
            //{
            //   file.Text = JsonSerializer.Serialize(list.Take(MainViewModel.MaxArticles));
            //}
            //else
            //{
            //   file.Text = JsonSerializer.Serialize(list);
            //};

            BackboneViewModel.Current.DecBusy();

            file.SaveToFile(dlg.FileName);
         };
      }

      private void btnXMLStock_Click(object sender, RoutedEventArgs e)
      {
         // Configure open file dialog box
         Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

         dlg.Title = "Stock";
         dlg.FileName = string.Format("Stock.{0}.xml", DateTime.Now.ToString("yyMMdd HHmmss"));
         dlg.Filter = "XML document (.xml)|*.xml";
         dlg.DefaultExt = ".xml";
         //dlg.InitialDirectory = ReportingViewModel.Instance.InitialDirectory;

         // Show open file dialog box
         Nullable<bool> result = dlg.ShowDialog(Application.Current.MainWindow);

         // Process open file dialog box results 
         if (result == true)
         {
            if (System.IO.File.Exists(dlg.FileName))
            {
               System.IO.File.Delete(dlg.FileName);
            };

            TStrings file = new TStrings();

            BackboneViewModel.Current.BusyTitle = "Exportation ...";
            BackboneViewModel.Current.IncBusy();
            //var list = DB_SQL.Query<Stock_View>(VMLocator.Stock.GetStockViewSQL(true));

            //if (MainViewModel.Current.IsDemo)
            //{
            //   file.Text = Serialize(list.Take(MainViewModel.MaxArticles)).ToString();
            //}
            //else
            //{
            //   file.Text = Serialize(list).ToString();
            //};

            //for (int i = file.Count - 1; i > 0; i--)
            //{
            //   string st = file[i];

            //   if (
            //      st.Contains("<PK>")
            //      || st.Contains("<FKEmplacement>")
            //      || st.Contains("<FKArticle>")
            //      )
            //   {
            //      file.Delete(i);
            //   };
            //};

            BackboneViewModel.Current.DecBusy();

            file.SaveToFile(dlg.FileName);
         };
      }

      public static StringWriter Serialize(object o)
      {
         var xs = new XmlSerializer(o.GetType());
         var xml = new StringWriter();
         xs.Serialize(xml, o);

         return xml;
      }

      /// <summary>
      /// Use it like this:
      ///    var deserializedDictionaries = Deserialize<List<CustomDictionary>>(myXML);
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="xml"></param>
      /// <returns></returns>
      public static T Deserialize<T>(string xml)
      {
         var xs = new XmlSerializer(typeof(T));
         return (T)xs.Deserialize(new StringReader(xml));
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
