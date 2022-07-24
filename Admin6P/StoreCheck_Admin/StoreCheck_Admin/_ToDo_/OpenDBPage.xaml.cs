using System;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ZPF.SQL;
using static BackboneViewModel;

namespace ZPF.WPF
{
   /// <summary>
   /// Interaction logic for OpenDBPage.xaml
   /// </summary>
   public partial class OpenDBPage : Page, INotifyPropertyChanged
   {
      /// <summary>
      /// MainViewModel.Current.DataPath + MainViewModel.AppTitle + ".DBList.json"
      /// </summary>
      public static string DBListFileName { get; set; } = "DBList.json";

      public static DBType[] DBTypes { get; set; } = new SQL.DBType[] { SQL.DBType.Firebird, SQL.DBType.PostgreSQL, SQL.DBType.SQLite, SQL.DBType.SQLServer, SQL.DBType.MySQL };

      public OpenDBPage()
      {
         
         InitializeComponent();
         DataContext = DBViewModel.Current;
         DBViewModel.Current.LoadList(DBListFileName);

         dataGrid.ItemsSource = DBViewModel.Current.PrevConnections;

         cbDBType.ComboBox.ItemsSource = Enum.GetValues(typeof(DBType));

         cbDBType.ComboBox.ItemsSource = DBTypes;
         cbDBType.ComboBox.SelectionChanged += ComboBox_SelectionChanged;

         tbPassword.ShowPassword(true);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         switch ((DBType)(cbDBType.ComboBox.SelectedItem))
         {
            case DBType.OleDB:
            case DBType.SQLite:
               tbServer.IsEnabled = false;
               tileGetFile.Visibility = Visibility.Visible;
               break;

            default:
               tbServer.IsEnabled = true;
               tileGetFile.Visibility = Visibility.Collapsed;
               break;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public event PropertyChangedEventHandler PropertyChanged;

      protected void OnPropertyChanged(string propertyName)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      DBConnectionParams _Connection = new DBConnectionParams();
      public DBConnectionParams Connection { get => _Connection; set => _Connection = value; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         ListViewSelection(sender);
      }

      private void ListViewSelection(object sender)
      {
         var dg = (sender as DataGrid);

         if (dg.SelectedItem != null)
         {
            if (DBSQL_Helper.OpenDB((dg.SelectedItem as DBConnectionParams)) == null)
            {
               if (DBViewModel.Current.Connection != null)
               {
                  BackboneViewModel.Current.MessageBox(MessageBoxType.Error, DBViewModel.Current.Connection.LastError);
                  return;
               };

               if (!string.IsNullOrEmpty(DB_SQL._ViewModel.LastError))
               {
                  BackboneViewModel.Current.MessageBox(MessageBoxType.Error, DB_SQL._ViewModel.LastError);
               };

               return;
            };

            dg.UnselectAllCells();
            dg.UnselectAll();

            DoOpen();
         };
      }

      private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         var dg = (sender as DataGrid);

         if (dg.SelectedItem != null)
         {
            this.Connection = (dg.SelectedItem as DBViewModel.ConnectionParams_List).ToConnectionParams();
            cbDBType.SelectedValue = this.Connection.DBType;

            OnPropertyChanged("Connection");
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void btnOpen_Click(object sender, RoutedEventArgs e)
      {
         if (cbDBType.ComboBox.SelectedItem == null)
         {
            return;
         };

         Connection.DBType = (DBType)(cbDBType.ComboBox.SelectedItem);

         dataGrid.ItemsSource = null;
         DoOpen();

         Binding binding = new Binding();
         binding.Path = new PropertyPath("PrevConnections");
         binding.Source = DBViewModel.Current;  // view model

         dataGrid.SetBinding(DataGrid.ItemsSourceProperty, binding);
      }

      private void DoOpen()
      {
         if (DBSQL_Helper.OpenDB(Connection, true, true) == null)
         {
            if (DBViewModel.Current.Connection != null)
            {
            };

            if (!string.IsNullOrEmpty(DB_SQL._ViewModel.LastError))
            {
               BackboneViewModel.Current.MessageBox(MessageBoxType.Error, DB_SQL._ViewModel.LastError);
            };
         }
         else
         {
            if (!string.IsNullOrEmpty(DB_SQL._ViewModel.LastError))
            {
               BackboneViewModel.Current.MessageBox(MessageBoxType.Error, DB_SQL._ViewModel.LastError);
            };

            OnPropertyChanged("Connection");
         };
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         Connection = new DBConnectionParams();
         OnPropertyChanged("Connection");
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void page_Unloaded(object sender, RoutedEventArgs e)
      {
         DBViewModel.Current.SaveList(DBListFileName);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void delConnection_Click(object sender, RoutedEventArgs e)
      {
         var con = (((sender as MenuItem).DataContext as DataGrid).SelectedItem as DBViewModel.ConnectionParams_List);

         dataGrid.UnselectAllCells();
         dataGrid.UnselectAll();
         dataGrid.SelectedItem = null;

         DBViewModel.Current.RemoveCon(con);

         dataGrid.ItemsSource = null;
         dataGrid.ItemsSource = DBViewModel.Current.PrevConnections;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void tileGetFile_Click(object sender, RoutedEventArgs e)
      {
         // Configure open file dialog box
         Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
         dlg.Title = "database ...";
         dlg.DefaultExt = ".db3";                        // Default file extension
         dlg.Filter = "SQLite (.db3)|*.db3|All files (.*)|*.*"; // Filter files by extension 

#if DEBUG
         string BaseDir = @"C:\ProgramData\";

         dlg.InitialDirectory = BaseDir;
#endif
         dlg.FileName = Connection.DBase;

         // Show open file dialog box
         Nullable<bool> result = dlg.ShowDialog();

         // Process open file dialog box results 
         if (result == true)
         {
            Connection.DBase = dlg.FileName;
         };
      }

      private void metroAnimatedTabControl_Unloaded(object sender, RoutedEventArgs e)
      {

      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void copyConnection_Click(object sender, RoutedEventArgs e)
      {
         var con = (((sender as MenuItem).DataContext as DataGrid).SelectedItem as DBViewModel.ConnectionParams_List);

         var json = JsonSerializer.Serialize(con, new JsonSerializerOptions { WriteIndented = true });

         System.Windows.Clipboard.SetText(json);
      }

      private void pasteConnection_Click(object sender, RoutedEventArgs e)
      {
         if (System.Windows.Clipboard.ContainsText())
         {
            var json = System.Windows.Clipboard.GetText(TextDataFormat.Text);

            try
            {
               var connectionParams = JsonSerializer.Deserialize<DBViewModel.ConnectionParams_List>(json);

               DBViewModel.Current.PrevConnections.Insert(0, connectionParams);

               DBSQL_Helper.OpenDB(DBViewModel.Current.PrevConnections[0]);

               DoOpen();
            }
            catch
            {
            };

         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}
