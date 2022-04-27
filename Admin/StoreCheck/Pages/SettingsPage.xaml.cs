//using Seagull.BarTender.Print;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ZPF;
using ZPF.SQL;
using ZPF.WPF;
using ZPF.WPF.Compos;
using static BackboneViewModel;

namespace StoreCheck
{
   /// <summary>
   /// Interaction logic for SettingsPage.xaml
   /// </summary>
   public partial class SettingsPage : Page
   {
      private string _PrevStyle;

      public SettingsPage()
      {
         DataContext = MainViewModel.Current;

         InitializeComponent();

         MainViewModel.Current.Load();
         _PrevStyle = MainViewModel.Current.Style;

         // - - -  - - - 

         cbTemplate.Items.Clear();

         //ToDo .Net 6
         //string fmt = Environment.CurrentDirectory + @"\Styles\{0}.xaml";

         //TStrings FileNames = new TStrings();
         //IEnumerable<string> files = Directory.EnumerateFiles(Environment.CurrentDirectory + @"\Styles\", "*.xaml");

         //int Ind = -1;
         //foreach (var fn in files)
         //{
         //   FileNames.Add(fn);

         //   string st = System.IO.Path.GetFileNameWithoutExtension(fn);
         //   Ind = cbTemplate.Items.Add(st);

         //   if (st == MainViewModel.Current.Style)
         //   {
         //      cbTemplate.SelectedIndex = Ind;
         //   }
         //};

         // - - -  - - - 

         cbLastMVT.Items.Clear();

         cbLastMVT.Items.Add(new NameValue { Name = "Semaine", Value = "7D" });
         cbLastMVT.Items.Add(new NameValue { Name = "Mois", Value = "1M" });

         cbLastMVT.SelectedIndex = (MainViewModel.Current.LastMVT == "1M" ? 1 : 0);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void btnOK_Click(object sender, RoutedEventArgs e)
      {
         MainViewModel.Current.Style = cbTemplate.SelectedItem as string;
         MainViewModel.Current.LastMVT = (cbLastMVT.SelectedItem as NameValue).Value;

         MainViewModel.Current.Save();

         if (_PrevStyle != MainViewModel.Current.Style)
         {
            BackboneViewModel.Current.MessageBox(MessageBoxType.Info, "Pour profiter entièrement de l'application du nouveau style vous devez redémarrer l'application.");
         };

         MenuViewModel.Instance.NavigateMenu("STYLE");
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         MainViewModel.Current.Load();
         MenuViewModel.Instance.NavigateMenu("HOME");
      }

      private void btnImportPath_Click(object sender, RoutedEventArgs e)
      {
         Avalon.Windows.Dialogs.FolderBrowserDialog dlg = new Avalon.Windows.Dialogs.FolderBrowserDialog();
         dlg.BrowseShares = true;
         dlg.RootPath = (DataContext as MainViewModel).ImportPath;

#if DEBUG
         //string BaseDir = @"D:\Software\Projects\PIMS\Doc\";

         //dlg.InitialDirectory = BaseDir;
         //dlg.FileName = @"Splash.png";
#endif

         // Show open file dialog box
         Nullable<bool> result = dlg.ShowDialog();

         // Process open file dialog box results 
         if (result == true)
         {
            (DataContext as MainViewModel).ImportPath = dlg.SelectedPath;
         };
      }

      private void btnArchivPath_Click(object sender, RoutedEventArgs e)
      {
         Avalon.Windows.Dialogs.FolderBrowserDialog dlg = new Avalon.Windows.Dialogs.FolderBrowserDialog();
         dlg.BrowseShares = true;
         dlg.RootPath = (DataContext as MainViewModel).ArchivPath;

#if DEBUG
         //string BaseDir = @"D:\Software\Projects\PIMS\Doc\";

         //dlg.InitialDirectory = BaseDir;
         //dlg.FileName = @"Splash.png";
#endif

         // Show open file dialog box
         Nullable<bool> result = dlg.ShowDialog();

         // Process open file dialog box results 
         if (result == true)
         {
            (DataContext as MainViewModel).ArchivPath = dlg.SelectedPath;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void btnLoadImage_Click(object sender, RoutedEventArgs e)
      {
         // Configure open file dialog box
         Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
         dlg.Title = "Image ...";
         //dlg.IniFileName = "Document";                 // Default file name
         dlg.DefaultExt = ".png";                        // Default file extension
         dlg.Filter = "PNG (.png)|*.png|JPG (.jpg)|*.jpg|All files (.*)|*.*"; // Filter files by extension 

#if DEBUG
         string BaseDir = @"D:\Software\Projects\PIMS\Doc\";

         dlg.InitialDirectory = BaseDir;
         dlg.FileName = @"Splash.png";
#endif

         // Show open file dialog box
         Nullable<bool> result = dlg.ShowDialog();

         // Process open file dialog box results 
         if (result == true)
         {
            MainViewModel.Current.LogoPath = dlg.FileName;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         MainViewModel.Current.PrinterSettings.Landscape = !MainViewModel.Current.PrinterSettings.Landscape;
         MainViewModel.Current.UpdatePrinterSettings();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
