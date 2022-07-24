using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

using ZPF.SQL;

namespace ZPF.WPF
{
   // https://github.com/tom-englert/DataGridExtensions


   /// <summary>
   /// Interaction logic for DataGridTools.xaml
   /// </summary>
   public partial class DataGridTools : Window
   {
      // Declare the event
      public event PropertyChangedEventHandler PropertyChanged;

      // Create the OnPropertyChanged method to raise the event
      protected void OnPropertyChanged(string name)
      {
         PropertyChangedEventHandler handler = PropertyChanged;
         if (handler != null)
         {
            handler(this, new PropertyChangedEventArgs(name));
         }
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private string _IniFileName;
      private string _DataGridName;
      private DataGrid _DataGrid;
      bool _ShowFilter = true;

      public List<DataGridToolsColumn> columns { get; set; }
      public List<NameValue> Operations { get; set; }

      public DataGridTools(string IniFileName, string DataGridName, DataGrid dataGrid)
      {
         DataContext = this;

         try
         {
            Owner = Application.Current.MainWindow;
         }
         catch
         {
         };

         InitializeComponent();

#if Excel
         btnExportXLS.Visibility = Visibility.Visible;
#else
         btnExportXLS.Visibility = Visibility.Collapsed;
#endif

         ShowFilter();

         Operations = new List<NameValue>();
         Operations.Add(new NameValue { Name = "", Value = "" });
         Operations.Add(new NameValue { Name = "<", Value = "<" });
         Operations.Add(new NameValue { Name = "<=", Value = "<=" });
         Operations.Add(new NameValue { Name = ">", Value = ">" });
         Operations.Add(new NameValue { Name = ">=", Value = ">=" });
         Operations.Add(new NameValue { Name = "=", Value = "=" });
         Operations.Add(new NameValue { Name = "<>", Value = "<>" });
         Operations.Add(new NameValue { Name = "LIKE", Value = "LIKE" });
         OnPropertyChanged("Operations");

         dgcOperation.ItemsSource = Operations;

         columns = new List<DataGridToolsColumn>();
         this.WindowStyle = WindowStyle.None;
         this.AllowsTransparency = true;

         _IniFileName = IniFileName;
         this._DataGridName = DataGridName;
         this._DataGrid = dataGrid;

         if (_DataGrid.IsLoaded)
         {
            dataGrid_Loaded(_DataGrid, null);
         }
         else
         {
            _DataGrid.Loaded += dataGrid_Loaded;
         };

         this.KeyDown += DataGridTools_KeyDown;
      }

      private void ShowFilter()
      {
         _ShowFilter = _DataGrid != null && _DataGrid.Items.SourceCollection is System.Data.DataView;

#if DEBUG
         _ShowFilter = true;
#else
         btnExportXLS.IsEnabled = _ShowFilter;
#endif

         btnExportXLS.IsEnabled = true;

         if (_ShowFilter)
         {
            this.Width = 400;

            dgcOperation.Visibility = Visibility.Visible;
            dgcColumnData.Visibility = Visibility.Visible;
            btnClear.Visibility = Visibility.Visible;
         }
         else
         {
            this.Width = 300;

            dgcOperation.Visibility = Visibility.Collapsed;
            dgcColumnData.Visibility = Visibility.Collapsed;
            btnClear.Visibility = Visibility.Visible;
         };
      }

      private void DataGridTools_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.Key == Key.Escape)
         {
            btnCancel_Click(null, null);
         };

         if (e.Key == Key.Return)
         {
            btnOK_Click(null, null);
         };
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void btnOK_Click(object sender, RoutedEventArgs e)
      {
         if (_ShowFilter)
         {
            SetFilter();
         };

         WriteDataGridColumns(true);
         DoClose();
      }

      private void SetFilter()
      {
         string RowFilter = "";

         try
         {
            // Build the RowFilter statement according to the user restriction 
            foreach (var c in columns.Where(x => !string.IsNullOrEmpty(x.Operation) && !string.IsNullOrEmpty(x.ColumnData)))
            {
               string ColumnData = c.ColumnData.ToString();

               if (c.Operation.ToString() == "LIKE"
                  && !ColumnData.StartsWith("%")
                  && !ColumnData.EndsWith("%"))
               {
                  ColumnData = "%" + ColumnData + "%";
               };

               // Add the "AND" operator only from the second filter condition 
               // The RowFilter get statement which simallar to the Where condition in sql query
               // For example "GroupID = '6' AND GroupName LIKE 'A%' 
               if (RowFilter == string.Empty)
               {
                  RowFilter = c.ColumnName.ToString() + " " + c.Operation.ToString() + " '" + ColumnData + "' ";
               }
               else
               {
                  RowFilter += " AND " + c.ColumnName.ToString() + " " + c.Operation.ToString() + " '" + ColumnData + "'";
               };
            };

            if (_DataGrid.Items.SourceCollection is System.Data.DataView)
            {
               ((System.Data.DataView)_DataGrid.Items.SourceCollection).RowFilter = RowFilter;
            }
            else
            {
               System.ComponentModel.ICollectionView view = System.Windows.Data.CollectionViewSource.GetDefaultView(_DataGrid.ItemsSource);

               // _DataGrid.Items.AsQueryable().Where(RowFilter, null);

               if (view.Filter == null)
               {
                  // now we add the Filter
                  view.Filter += TextFilter;
               };

               view.Refresh();
            };
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(ex.Message);
         };
      }

      public bool TextFilter(object o)
      {
         //ToDo: Write filter

         return true;
      }

      private void DoClose()
      {
         DoubleAnimation animFadeIn = new DoubleAnimation();
         animFadeIn.From = 1;
         animFadeIn.To = 0;
         animFadeIn.Duration = new Duration(TimeSpan.FromMilliseconds(200));
         this.BeginAnimation(Window.OpacityProperty, animFadeIn);

         DoIt.Delay(200, () =>
         {
            DoIt.OnMainThread(() =>
            {
               this.Hide();
            });
         });
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         //??? ReadDataGridColumns();
         DoClose();
      }

      private void btnClear_Click(object sender, RoutedEventArgs e)
      {
         foreach (var c in columns)
         {
            c.Operation = "";
            c.ColumnData = "";
         };

         OnPropertyChanged("columns");

         dataGrid.ItemsSource = null;
         dataGrid.ItemsSource = columns;
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void btnReset_Click(object sender, RoutedEventArgs e)
      {
         int Ind = 0;

         foreach (var c in _DataGrid.Columns)
         {
            c.Width = DataGridLength.Auto;
            c.DisplayIndex = Ind;
            c.Visibility = Visibility.Visible;
            c.SortDirection = null;

            Ind++;
         };

         WriteDataGridColumns(true);

         // - - -  - - - 

         TIniFile IniFile = new TIniFile(_IniFileName);

         try
         {
            IniFile.WriteString("DataGridColumns", _DataGridName, "");

            IniFile.UpdateFile();
         }
         catch
         {
         };

         // - - -  - - - 

         DoClose();
      }

      private void btnSave_Click(object sender, RoutedEventArgs e)
      {
         WriteDataGridColumns(true);
         DoClose();
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void DataGrid_CellGotFocus(object sender, RoutedEventArgs e)
      {
         // Lookup for the source to be DataGridCell
         if (e.OriginalSource.GetType() == typeof(DataGridCell))
         {
            // Starts the Edit on the row;
            DataGrid grd = (DataGrid)sender;
            grd.BeginEdit(e);

            Control control = GetFirstChildByType<Control>(e.OriginalSource as DataGridCell);
            if (control != null)
            {
               control.Focus();

               if (control is CheckBox)
               {
                  (control as CheckBox).IsChecked = !(control as CheckBox).IsChecked;
               };
            }
         }
      }

      private T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
      {
         for (int i = 0; i < VisualTreeHelper.GetChildrenCount(prop); i++)
         {
            DependencyObject child = VisualTreeHelper.GetChild((prop), i) as DependencyObject;
            if (child == null)
               continue;

            T castedProp = child as T;
            if (castedProp != null)
               return castedProp;

            castedProp = GetFirstChildByType<T>(child);

            if (castedProp != null)
               return castedProp;
         }
         return null;
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      public void WriteDataGridColumns(bool Update = false)
      {
         // - - - DataGrid.Columns  - - - 

         if (Update)
         {
            int Ind = 0;
            foreach (var c in columns)
            {
               _DataGrid.Columns[Ind].Visibility = (c.Visibility ? Visibility.Visible : Visibility.Collapsed);

               Ind++;
            };
         };

         if (columns.Count != _DataGrid.Columns.Count)
         {
            columns = new List<DataGridToolsColumn>();
            foreach (var c in _DataGrid.Columns)
            {
               // Debug.WriteLine("{0} {1} {2} {3} {4}", c.Header, c.Width, c.DisplayIndex, c.Visibility, c.SortDirection);

               columns.Add(new DataGridToolsColumn
               {
                  Header = (string)c.Header,
                  ColumnName = c.SortMemberPath,
                  Width = (c.Width == DataGridLength.Auto ? -1 : c.Width.Value),
                  DisplayIndex = c.DisplayIndex,
                  Visibility = (c.Visibility == Visibility.Visible),
                  SortDirection = c.SortDirection,
               });
            };
         }
         else
         {
            foreach (var c in _DataGrid.Columns)
            {
               var col = columns.Where(x => x.ColumnName == c.SortMemberPath).FirstOrDefault();

               if (col != null)
               {
                  col.Header = (string)c.Header;
                  col.Width = (c.Width == DataGridLength.Auto ? -1 : c.Width.Value);
                  col.DisplayIndex = c.DisplayIndex;
                  col.Visibility = (c.Visibility == Visibility.Visible);
                  col.SortDirection = c.SortDirection;
               };
            };
         };

         dataGrid.ItemsSource = columns;
         //OnPropertyChanged("columns");

         if (!Update)
         {
            return;
         };

         if (!string.IsNullOrEmpty(_IniFileName) && !string.IsNullOrEmpty(_DataGridName))
         {
            List<DataGridToolsColumn> cols = new List<DataGridToolsColumn>();

            foreach (var c in columns)
            {
               // Debug.WriteLine("{0} {1} {2} {3} {4}", c.Header, c.Width, c.DisplayIndex, c.Visibility, c.SortDirection);

               var col = new DataGridToolsColumn();

               col.Width = (c.Width == DataGridLength.Auto ? -1 : c.Width);
               col.DisplayIndex = c.DisplayIndex;
               col.Visibility = c.Visibility;
               col.SortDirection = c.SortDirection;

               cols.Add(col);
            };

            string JSON = JsonSerializer.Serialize(cols);

            TIniFile IniFile = new TIniFile(_IniFileName);

            try
            {
               IniFile.WriteString("DataGridColumns", _DataGridName, JSON);

               IniFile.UpdateFile();
            }
            catch
            {
               //on E: Exception do
               //{
               //   MessageDlg('Erreur fatale (WriteFormPos): '  + #13 + #10
               //               + E.Message, mtError, [mbOk], 0);
               //   Application.Terminate;
               //};
            };
         };
      }

      public void ReadDataGridColumns()
      {
         if (!string.IsNullOrEmpty(_IniFileName) && !string.IsNullOrEmpty(_DataGridName))
         {
            TIniFile IniFile = new TIniFile(_IniFileName);

            string JSON = IniFile.ReadString("DataGridColumns", _DataGridName, "");

            try
            {
               List<DataGridToolsColumn> cols = JsonSerializer.Deserialize<List<DataGridToolsColumn>>(JSON);

               if (cols != null)
               {
                  int Ind = 0;

                  foreach (var c in cols)
                  {
                     // Debug.WriteLine("{0} {1} {2} {3} {4}", c.Header, c.Width, c.DisplayIndex, c.Visibility, c.SortDirection);

                     var col = _DataGrid.Columns[Ind];

                     col.Width = (c.Width == 1.0 ? DataGridLength.Auto : c.Width);
                     col.DisplayIndex = c.DisplayIndex;
                     col.Visibility = (c.Visibility ? Visibility.Visible : Visibility.Hidden);
                     col.SortDirection = c.SortDirection;

                     Ind++;
                  };
               };
            }
            catch (Exception ex)
            {
               Debug.WriteLine(ex.Message);

               IniFile.WriteString("DataGridColumns", _DataGridName, "");
               IniFile.UpdateFile();
            };
         };
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void dataGrid_Loaded(object sender, RoutedEventArgs e)
      {
         var dataGrid = (DataGrid)sender;

         // if (dataGrid.Visibility != Visibility.Visible) return;
         if (!dataGrid.IsVisible) return;

         var border = (Border)VisualTreeHelper.GetChild(dataGrid, 0);
         var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
         var grid = (Grid)VisualTreeHelper.GetChild(scrollViewer, 0);

         var b0 = (Button)VisualTreeHelper.GetChild(grid, 0);

         // https://github.com/christophano/FontAwesomeDotNet
         // https://github.com/charri/Font-Awesome-WPF/blob/master/README-WPF.md

         //var img = new Image();
         //ToDo: ImageAwesome.CreateImageSource(FontAwesomeIcon.Gear, Brushes.White);
         //img.Source = ImageAwesome.CreateImageSource(FontAwesomeIcon.Gear, Brushes.White);

         var img = new Image();
         img.Source = ZPF.Fonts.IF.GetImageSource(ZPF.Fonts.IF.Settings_09, Brushes.White);

         grid.Children.RemoveAt(0);
         grid.Children.Insert(0, new Button
         {
            BorderThickness = new Thickness(0),
            Content = img,
            Width = 15,
         });

         dataGrid.RowHeaderWidth = 15;

         var button = (Button)VisualTreeHelper.GetChild(grid, 0);

         button.Command = null;
         button.Click += Button_Click;

         ReadDataGridColumns();
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         WriteDataGridColumns();

         var btn = sender as Button;

         // Get absolute location on screen of upper left corner of button
         Point locationFromScreen = btn.PointToScreen(new Point(0, 0));

         // Transform screen point to WPF device independent point
         PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
         System.Windows.Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);

         this.Top = targetPoints.Y;
         this.Left = targetPoints.X;

         //Point relativePoint = btn.PointFromScreen(new Point(0, 0));

         //this.Top = Math.Max(Math.Abs(relativePoint.Y) - 50, 0);
         //this.Left = Math.Max(Math.Abs(relativePoint.X) - 50, 0);
         this.Top = this.Top - 50;
         this.Left = this.Left - 50;

         //if (this.Top + this.Height > Application.Current.MainWindow.Top + Application.Current.MainWindow.Height)
         //{
         //   this.Top = (Application.Current.MainWindow.Top + Application.Current.MainWindow.Height) - this.Height - 68;
         //};

         ShowFilter();

         try
         {
            DoubleAnimation animFadeIn = new DoubleAnimation();
            animFadeIn.From = 0;
            animFadeIn.To = 1;
            animFadeIn.Duration = new Duration(TimeSpan.FromMilliseconds(700));
            this.BeginAnimation(Window.OpacityProperty, animFadeIn);

            this.ShowDialog();
         }
         catch
         {
            this.Hide();
            this.Opacity = 1;

            try
            {
               this.ShowDialog();
            }
            catch
            {
               new DataGridTools(_IniFileName, _DataGridName, _DataGrid);
            };
         };
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void btnExportXLS_Click(object sender, RoutedEventArgs e)
      {
         
         List<ColDef> Fields = new List<ColDef>();

         foreach (var c in _DataGrid.Columns.OrderBy(x => x.DisplayIndex))
         {
            if (c.Visibility == Visibility.Visible)
            {
               Fields.Add(new ColDef { Name = c.SortMemberPath, Header = (string)c.Header });
            };
         };

         string FileName = System.IO.Path.GetTempFileName() + ".xlsx";

         if (_DataGrid.Items is System.Windows.Controls.ItemCollection)
         {
            DataTable dt = new DataTable();
            dt.Clear();

            foreach (var c in _DataGrid.Columns)
            {
               dt.Columns.Add(c.SortMemberPath);
            };

            foreach (var item in _DataGrid.Items)
            {
               DataRow row = dt.NewRow();

               row = DB_SQL.ObjToDataRow(item, row);

               dt.Rows.Add(row);
            };

#if Excel
            ZPF.ExcelHelper.ExportXLS(null, dt, FileName, true, -1, Fields);

            DoClose();
#endif
         };


         if (_DataGrid.Items.SourceCollection is System.Data.DataView)
         {
            var DataView = ((System.Data.DataView)_DataGrid.Items.SourceCollection);

#if Excel
            ZPF.ExcelHelper.ExportXLS(null, DataView.Table, FileName, true, -1, Fields);

            DoClose();
#endif
         };
      }

      private void btnExportPDF_Click(object sender, RoutedEventArgs e)
      {
         DoClose();
      }

      private void window_Loaded(object sender, RoutedEventArgs e)
      {
      }

      private void dataGrid_TextInput(object sender, TextCompositionEventArgs e)
      {

      }

      private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         var tb = (sender as TextBox);
         var c = (dataGrid.SelectedItem as DataGridToolsColumn);

         if (!string.IsNullOrEmpty(tb.Text))
         {

            if (string.IsNullOrEmpty(c.Operation))
            {
               c.Operation = "=";
            };
         };

         c.ColumnData = tb.Text;
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void window_MouseDown(object sender, MouseButtonEventArgs e)
      {
         if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         var dataGrid = sender as DataGrid;
         var item = dataGrid.SelectedItem as DataGridToolsColumn;

         Clipboard.SetData(DataFormats.Text, (Object)item.ColumnName);
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   }
}
