using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microcharts;
using ZPF;
using ZPF.WPF;
using ZPF.XLS;

namespace StoreCheck.Pages
{
   /// <summary>
   /// Interaction logic for DashBoardPage.xaml
   /// </summary>
   public partial class DashBoardPage : Page
   {
      public DashBoardPage()
      {
         DataContext = MainViewModel.Current;

         InitializeComponent();

         {
            //labelChart.Content = // I18nViewModel.Current.T("Répartition des articles (qté) par familles");
            labelChart.Content = ("Répartition des articles (qté) par familles");

            //chartFamilles.Visibility = Visibility.Visible;
            //chartEmplacements.Visibility = Visibility.Collapsed;

            labelDataGrid.Content = "Articles en alerte";

            // - - -  - - - 

            chartView.Visibility = Visibility.Visible;

            chartView.Chart = new DonutChart();
            chartView.Chart.LabelTextSize = 12;
            chartView.Chart.MinValue = -1000;
            chartView.Chart.MaxValue = 1000;

            chartView.Chart.Entries = GenerateEntries();
         };
      }

      string[] colors = new string[]{
         "#99737373", "#99f1595f", "#9979c36a", "#99599ad3",
         "#99f9a65a", "#999e66ab", "#99cd7058", "#99d77fb3" };

      private IEnumerable<ChartEntry> GenerateEntries()
      {
         List<ChartEntry> entries = new List<ChartEntry>();

         // StockCalc.DashDonatFamilles
         //if (StockCalcViewModel.Current.DashDonatFamilles.Count > 0)
         //{
         //   foreach (var i in StockCalcViewModel.Current.DashDonatFamilles)
         //   {
         //      var entry = new ChartEntry((float)i.Value)
         //      {
         //         Label = i.Name,
         //         XValue = entries.Count,
         //         ValueLabel = i.Value.ToString(),
         //         Color = ZPF.Color.Parse(colors[entries.Count % 7])
         //      };

         //      entries.Add(entry);
         //   };
         //};

         //var chart = new Microcharts.PieChart()
         //{
         //   Entries = entries,
         //   LabelTextSize = 24,
         //   IsAnimated = false,
         //   Radius = 0.8F,
         //};

         return entries;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void ListViewArticles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         if (MainViewModel.Current.DblClickToSelect)
         {
            ListViewArticlesSelection(sender);
         };
      }

      private void ListViewArticlesSelection(object sender)
      {
         var dg = (sender as DataGrid);

         if (dg.SelectedItem != null)
         {
            //if (dg.SelectedItem is Articles_View)
            //{
            //   ItemsViewModel.Current.SetArticle((dg.SelectedItem as Articles_View).PK);
            //}
            //else
            //{
            //   ItemsViewModel.Current.SetArticle((dg.SelectedItem as Stock_View).PK);
            //};

            MenuViewModel.Instance.NavigateMenu("ART");

            dg.UnselectAllCells();
            dg.UnselectAll();
         };
      }

      private void ListViewArticles_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         if (!MainViewModel.Current.DblClickToSelect)
         {
            ListViewArticlesSelection(sender);
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void DataGrid_LoadingRow_1(object sender, DataGridRowEventArgs e)
      {
         //ItemsViewModel.Current.SetDGRowColor(sender, e);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void ListViewMVT_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         if (MainViewModel.Current.DblClickToSelect)
         {
            ListViewMVTSelection(sender);
         };
      }

      private void ListViewMVTSelection(object sender)
      {
         var dg = (sender as DataGrid);

         if (dg.SelectedItem != null)
         {
            //ItemsViewModel.Current.SetArticle((dg.SelectedItem as StockMVT_View).FKArticle);
            MenuViewModel.Instance.NavigateMenu("ART");

            dg.UnselectAllCells();
            dg.UnselectAll();
         };
      }

      private void ListViewMVT_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         if (!MainViewModel.Current.DblClickToSelect)
         {
            ListViewMVTSelection(sender);
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void btnPrintMVT_Click(object sender, RoutedEventArgs e)
      {
         //ExcelHelper.ExportXLS(null, VMLocator.Stock.GetStockMVT7View(), System.IO.Path.GetTempFileName() + ".xlsx", true,
         //   (MainViewModel.Current.IsDemo ? MainViewModel.MaxArticles : -1));
      }

      private void btnPrintAlerte_Click(object sender, RoutedEventArgs e)
      {
         //ExcelHelper.ExportXLS(null, VMLocator.Items.GetArticlesAlertView(), System.IO.Path.GetTempFileName() + ".xlsx", true,
         //   (MainViewModel.Current.IsDemo ? MainViewModel.MaxArticles : -1));
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void dgMVT_Loaded(object sender, RoutedEventArgs e)
      {
         var dataGrid = sender as DataGrid;
         new DataGridTools(MainViewModel.IniFileName, "Dashboard_MVT", dataGrid);
      }

      private void dgAlertes_Loaded(object sender, RoutedEventArgs e)
      {
         var dataGrid = sender as DataGrid;
         new DataGridTools(MainViewModel.IniFileName, "Dashboard_Alertes", dataGrid);
      }

      private void dgSurLeQuai_Loaded(object sender, RoutedEventArgs e)
      {
         var dataGrid = sender as DataGrid;
         new DataGridTools(MainViewModel.IniFileName, "Dashboard_SurLeQuai", dataGrid);
      }

      private void Tile_Click(object sender, RoutedEventArgs e)
      {
         var dlg = new InputComboBox();
         dlg.label.Content = "Veuillez sélectionner le mois";
         var dt = DateTime.Now.AddMonths(-12);
         List<NameValue> list = new List<NameValue>();

         for (int i = 0; i < 12; i++)
         {
            if (dt > new DateTime(2018, 4, 1))
            {
               list.Add(new NameValue
               {
                  Name = dt.ToString("MMMM yyyy"),
                  Tag = dt,
               });
            };

            dt = dt.AddMonths(1);
         };

#if DEBUG
         list.Add(new NameValue
         {
            Name = dt.ToString("MMMM yyyy"),
            Tag = dt,
         });
#else
         if (DateTime.Now.AddDays(1).Date == dt )
         {
            list.Add(new NameValue
            {
               Name = dt.ToString("MMMM yyyy"),
               Tag = dt,
            });
         };
#endif

         dlg.comboBox.ItemsSource = list;
         dlg.comboBox.DisplayMemberPath = "Name";
         dlg.comboBox.SelectedValuePath = "Tag";
         dlg.comboBox.SelectedItem = list.Last();

         if (dlg.ShowDialog() == true)
         {
            var nv = dlg.comboBox.SelectedItem as NameValue;

            //EuromatViewModel.Current.Export((DateTime)(nv.Tag));
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

      private void Grid_Loaded(object sender, RoutedEventArgs e)
      {
         //dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
         //dispatcherTimer.Interval = TimeSpan.FromSeconds(90);
         //dispatcherTimer.Start();
      }

      private void dispatcherTimer_Tick(object sender, EventArgs e)
      {
         MainViewModel.Current.RefreshAll();
         dispatcherTimer.Start();
      }

      private void Grid_Unloaded(object sender, RoutedEventArgs e)
      {
         dispatcherTimer.Stop();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}
