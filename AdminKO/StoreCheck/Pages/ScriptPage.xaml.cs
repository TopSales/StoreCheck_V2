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

namespace ZPF
{
   /// <summary>
   /// Interaction logic for GetStartedPage.xaml
   /// </summary>
   public partial class ScriptPage : Page
   {
      public ScriptPage()
      {
         InitializeComponent();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void MenuItem_CopyRAW_Click(object sender, RoutedEventArgs e)
      {
         //Clipboard.SetText(((sender as MenuItem).DataContext as ImportLine).raw);
      }

      private void MenuItem_CopyText_Click(object sender, RoutedEventArgs e)
      {
         //Clipboard.SetText(PDF2DataViewModel.Instance.Text);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void btnSearch_Click(object sender, RoutedEventArgs e)
      {
         //int Ind = 1;

         //int SearchNo = -1;

         //int.TryParse(PDF2DataViewModel.Instance.SearchNo, out SearchNo);

         //foreach (var l in PDF2DataViewModel.Instance.ImportedLines)
         //{
         //   string st = l.raw + l.Tag;

         //   if (!string.IsNullOrEmpty(PDF2DataViewModel.Instance.SearchText) && st.IndexOf(PDF2DataViewModel.Instance.SearchText) > -1)
         //   {
         //      dgData.SelectedItem = l;
         //      dgData.ScrollIntoView(l);

         //      return;
         //   };

         //   if (Ind == SearchNo)
         //   {
         //      dgData.SelectedItem = l;
         //      dgData.ScrollIntoView(l);

         //      return;
         //   };

         //   Ind++;
         //};

         MessageBox.Show("Oups ... not found");
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void btnSaveAs_Click(object sender, RoutedEventArgs e)
      {
         // Configure save file dialog box
         Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
         dlg.Title = "Save project ...";
         //dlg.IniFileName = "Document";              // Default file name
         dlg.DefaultExt = ".proj";                    // Default file extension
         dlg.Filter = "Project (.proj)|*.proj";       // Filter files by extension 
         // dlg.InitialDirectory = MainViewModel.Current.ProjetPath;

#if DEBUG
         //     dlg.FileName = MainViewModel.Instance.SelectedProjet.FileName;
#endif

         // Show open file dialog box
         Nullable<bool> result = dlg.ShowDialog();

         // Process open file dialog box results 
         if (result == true)
         {
            // MainViewModel.Current.SelectedProjet.SaveToFile(dlg.FileName);
         };
      }

      private void btnSave_Click(object sender, RoutedEventArgs e)
      {
         // MainViewModel.Current.SelectedProjet.SaveToFile( MainViewModel.Current.SelectedProjet.TemplateFileName);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void btnLoad_Click(object sender, RoutedEventArgs e)
      {
         // Configure open file dialog box
         Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
         dlg.Title = "Open project ...";
         //dlg.IniFileName = "Document";              // Default file name
         dlg.DefaultExt = ".proj";                    // Default file extension
         dlg.Filter = "Project (.proj)|*.proj";       // Filter files by extension 
         //dlg.InitialDirectory = MainViewModel.Current.ProjetPath;

#if DEBUG
         //     dlg.FileName = MainViewModel.Instance.SelectedProjet.FileName;
#endif

         // Show open file dialog box
         Nullable<bool> result = dlg.ShowDialog();

         // Process open file dialog box results 
         if (result == true)
         {
            // MainViewModel.Current.SelectedProjet = Projet.LoadFromFile(dlg.FileName);
         };
      }

      private void MetroTabItem_GotFocus(object sender, RoutedEventArgs e)
      {
//         codeEditor.Text = MainViewModel.Current.SelectedProjet.Code;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
      {

      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
