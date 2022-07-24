using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BaseDoc
{
   /// <summary>
   /// Interaction logic for BaseDocMain.xaml
   /// </summary>
   public partial class BaseDocMain : Page
   {
      public BaseDocMain(bool TestMode = false, string Ref = "")
      {
         InitializeComponent();

         //tbClientRef.Text = Ref;

         //// - - -  - - - 

         //tbClientRef.IsReadOnly = !IsTestMode;

         ////btnSearchEID.Visibility = (IsTestMode ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);

         //// - - -  - - - 

         //DataContext = BaseDocViewModel.Current;

         //// listBoxDocuments.ItemsSource = BaseDocViewModel.Current.Documents;
         //listBoxDocuments.SetBinding(System.Windows.Controls.ListBox.ItemsSourceProperty, "Documents");

         //listBoxTimeOut.ItemsSource = BaseDocViewModel.Current.TimeOutDocuments;
         //listBoxDeleted.ItemsSource = BaseDocViewModel.Current.DeletedDocuments;
         //listBoxAll.ItemsSource = BaseDocViewModel.Current.DocumentsAll;

         //cbDocumentTypes.ItemsSource = BaseDocViewModel.Current.DocumentTypes;
         //BaseDocViewModel.Current.ReadOnly = true;
         ////gridDocument.DataContext = BaseDocViewModel.Current.Document;

         //BackboneViewModel.Current.DecBusy();

         //// Code equivalent de l'Application.DoEvents()
         //System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new System.Threading.ThreadStart(delegate { }));
      }


      private void btnAdd_Click(object sender, RoutedEventArgs e)
      {
         //   var vm = BaseDocViewModel.Current;
         //   if (vm.ExternalRef == -1)
         //   {
         //      MessageBox.Show("Vous devez d’abord choisir ou créer un client (REF).", "Attention", MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
         //      return;
         //   };

         //   // Configure open file dialog box
         //   Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
         //   //dlg.IniFileName = "Document";                 // Default file name
         //   dlg.DefaultExt = ".*";                       // Default file extension
         //   dlg.Filter = "PDF (.PDF)|*.pdf|Text documents (.txt)|*.txt|All documents (*.*)|*.*";  // Filter files by extension 

         //   // Show open file dialog box
         //   Nullable<bool> result = dlg.ShowDialog();

         //   // Process open file dialog box results 
         //   if (result == true)
         //   {
         //      // Open document 
         //      string FileName = dlg.FileName;
         //      string Title = System.IO.Path.GetFileNameWithoutExtension(FileName).Replace("_", " ");

         //      BackboneViewModel.Current.IncBusy();

         //      try
         //      {
         //         if (vm.databaseFilePut(Title, FileName))
         //         {
         //            vm.LoadData();
         //            BackboneViewModel.Current.DecBusy();

         //            listBoxDocuments.SelectedIndex = 0;
         //            btnEdit_Click(null, null);
         //            // vm.ReadOnly = false;
         //         };
         //      }
         //      catch (Exception ex)
         //      {
         //         BackboneViewModel.Current.DecBusy();
         //         MessageBox.Show("Oups ..." + Environment.NewLine + ex.Message);

         //         return;
         //      };

         //   }
      }

      //public string DecodeFromUtf8(string utf8String)
      //{
      //   // copy the string as UTF-8 bytes.
      //   byte[] utf8Bytes = new byte[utf8String.Length];
      //   for (int i = 0; i < utf8String.Length; ++i)
      //   {
      //      //Debug.Assert( 0 <= utf8String[i] && utf8String[i] <= 255, "the char must be in byte's range");
      //      utf8Bytes[i] = (byte)utf8String[i];
      //   }

      //   return Encoding.UTF8.GetString(utf8Bytes, 0, utf8Bytes.Length);
      //}


      private void btnExport_Click(object sender, RoutedEventArgs e)
      {
         //   var vm = BaseDocViewModel.Current;

         //   if (vm.Document == null)
         //   {
         //      return;
         //   };

         //   {
         //      // Configure Save file dialog box
         //      Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
         //      dlg.FileName = vm.Document.FileName;         // Default file name
         //      dlg.DefaultExt = vm.Document.FileExt;        // Default file extension
         //      dlg.Filter = "PDF (.PDF)|*.pdf|Text documents (.txt)|*.txt|All documents (*.*)|*.*";  // Filter files by extension 

         //      // Show Save file dialog box
         //      Nullable<bool> result = dlg.ShowDialog();

         //      // Process Save file dialog box results 
         //      if (result == true)
         //      {
         //         // Save document 
         //         string FileName = dlg.FileName;

         //         BackboneViewModel.Current.IncBusy();

         //         try
         //         {
         //            vm.databaseFileRead(FileName);
         //         }
         //         catch (Exception ex)
         //         {
         //            BackboneViewModel.Current.DecBusy();

         //            MessageBox.Show("Oups ..." + Environment.NewLine + ex.Message);

         //            return;
         //         };
         //      }
         //   };
      }

      private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         //   if ((sender as ListBox).SelectedItem != null)
         //   {
         //      var vm = BaseDocViewModel.Current;
         //      vm.Document = (sender as ListBox).SelectedItem as Document;

         //      btnEditImage.Source = new BitmapImage(new Uri(@"Assets/Edit.png", UriKind.Relative));
         //      btnEditText.Text = "edit";

         //      if ((vm.Document.FileExt.ToUpper() == ".JPG")
         //         || (vm.Document.FileExt.ToUpper() == ".JPEG")
         //         || (vm.Document.FileExt.ToUpper() == ".GIF")
         //         || (vm.Document.FileExt.ToUpper() == ".PNG")
         //         || (vm.Document.FileExt.ToUpper() == ".BMP")
         //         )
         //      {
         //         System.Drawing.Image img = DB_SQL.QuickQueryImage("select Document from sXc_Document where PK=" + vm.Document.PK);

         //         BitmapImage bi = new BitmapImage();
         //         if (img != null)
         //         {
         //            bi.BeginInit();

         //            MemoryStream ms = new MemoryStream();
         //            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
         //            ms.Seek(0, SeekOrigin.Begin);
         //            bi.StreamSource = ms;
         //            bi.EndInit();
         //         };

         //         imgPreview.Source = bi;
         //      }
         //      else
         //      {
         //         imgPreview.Source = null;
         //      };
         //   };
      }

      private void btnEdit_Click(object sender, RoutedEventArgs e)
      {
         //   if (BaseDocViewModel.Current.ReadOnly)
         //   {
         //      btnEditImage.Source = new BitmapImage(new Uri(@"Assets/Save.png", UriKind.Relative));
         //      btnEditText.Text = "save";

         //      btnEdit.Visibility = System.Windows.Visibility.Hidden;
         //      btnEdit.Visibility = System.Windows.Visibility.Visible;

         //      tbTitle.Focus();
         //   }
         //   else
         //   {
         //      BaseDocViewModel.Current.UpdateDoc();
         //      btnEditImage.Source = new BitmapImage(new Uri(@"Assets/Edit.png", UriKind.Relative));
         //      btnEditText.Text = "edit";
         //   };

         //   BaseDocViewModel.Current.ReadOnly = !BaseDocViewModel.Current.ReadOnly;
      }

      private void btnDelete_Click(object sender, RoutedEventArgs e)
      {
         //   if (BaseDocViewModel.Current.Document == null)
         //   {
         //      return;
         //   };

         //   if (MessageBox.Show("Delete current document?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
         //   {
         //      BaseDocViewModel.Current.DeleteDoc();
         //   }
      }

      private void btnScanner_Click(object sender, RoutedEventArgs e)
      {
         //   var vm = BaseDocViewModel.Current;
         //   if (vm.ExternalRef == -1)
         //   {
         //      MessageBox.Show("Vous devez d’abord choisir ou créer un client (REF).", "Attention", MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
         //      return;
         //   };

         //   //{
         //   //   string FileName = "TwainScan.jpg";

         //   //   if (File.Exists(FileName))
         //   //   {
         //   //      try
         //   //      {
         //   //         System.IO.File.Delete(FileName);
         //   //      }
         //   //      catch
         //   //      {
         //   //      };
         //   //   };

         //   //   FileName = System.IO.Path.ChangeExtension(FileName, ".pdf");

         //   //   if (File.Exists(FileName))
         //   //   {
         //   //      try
         //   //      {
         //   //         System.IO.File.Delete(FileName);
         //   //      }
         //   //      catch
         //   //      {
         //   //      };
         //   //   };
         //   //};

         //   //ScanWindow sw = new ScanWindow(App.Current.MainWindow);
         //   //sw.DocTitle = tbClientRef.Text + " - " + tbClientFullName.Text;
         //   //if (sw.ShowDialog() == true)
         //   //{
         //   //   BackboneViewModel.Current.IncBusy();

         //   //   // Open document 

         //   //   string FileName = "TwainScan.jpg";

         //   //   if (File.Exists(FileName))
         //   //   {
         //   //      string Title = System.IO.Path.GetFileNameWithoutExtension(FileName).Replace("_", " ");

         //   //      if (vm.databaseFilePut(Title, FileName))
         //   //      {
         //   //         vm.LoadData();
         //   //         vm.ReadOnly = false;

         //   //         try
         //   //         {
         //   //            System.IO.File.Delete(FileName);
         //   //         }
         //   //         catch
         //   //         {
         //   //         };
         //   //      };
         //   //   };

         //   //   FileName = System.IO.Path.ChangeExtension(FileName, ".pdf");

         //   //   if (File.Exists(FileName))
         //   //   {
         //   //      string Title = System.IO.Path.GetFileNameWithoutExtension(FileName).Replace("_", " ");


         //   //      if (vm.databaseFilePut(Title, FileName))
         //   //      {
         //   //         vm.LoadData();
         //   //         vm.ReadOnly = false;

         //   //         try
         //   //         {
         //   //            System.IO.File.Delete(FileName);
         //   //         }
         //   //         catch
         //   //         {
         //   //         };
         //   //      };
         //   //   };

         //   //   vmMain.SetBusy(false);
         //   //};
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //private void tbClientRef_LostFocus(object sender, RoutedEventArgs e)
      //{
      //   //if (IsTestMode)
      //   //{
      //   //   BackboneViewModel.Current.IncBusy();

      //   //   tbClientRef.Text = tbClientRef.Text.Trim().ToUpper();
      //   //   UpdateName();

      //   //   BackboneViewModel.Current.DecBusy();
      //   //};
      //}

      ////private void UpdateName()
      ////{
      ////   var vm = BaseDocViewModel.Current;

      ////   if (tbClientRef.Text != "")
      ////   {
      ////      if (DB_SQL.QuickQuery(DataModul.EGICConnection, string.Format("select Id from egi_client where Code='{0}'", tbClientRef.Text.ToUpper())) != "")
      ////      {
      ////         var vmMain = (new ViewModelLocator()).Main;
      ////         vmMain.SearchClient(tbClientRef.Text);
      ////      };

      ////      try
      ////      {
      ////         int Ref = int.Parse(DB_SQL.QuickQuery(DataModul.sXcConnection, string.Format("select PK from sXc_Client where Code='{0}'", tbClientRef.Text.ToUpper())));
      ////         vm.ExternalRef = Ref;

      ////         tbClientFullName.Text = DB_SQL.QuickQuery(DataModul.EGICConnection, string.Format("select Nom from egi_client where Code='{0}'", tbClientRef.Text.ToUpper()));
      ////      }
      ////      catch
      ////      {
      ////         if (MessageBox.Show("Ref not found. Create customer ref?", "Not found", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.OK)
      ////         {
      ////            DB_SQL.QuickQuery(DataModul.EGICConnection, string.Format("insert into egi_client (Code) values ('{0}')", tbClientRef.Text));
      ////            UpdateName();
      ////         }
      ////         else
      ////         {
      ////            // tbClientFullName.Text = "Oups ...";
      ////            tbClientFullName.Text = "";
      ////            vm.ExternalRef = -1;
      ////         };
      ////      };
      ////   }
      ////   else
      ////   {
      ////      tbClientFullName.Text = "";
      ////      vm.ExternalRef = -1;
      ////   };
      ////}

      private void btnShow_Click(object sender, RoutedEventArgs e)
      {
         //   var vm = BaseDocViewModel.Current;

         //   if (vm.Document == null)
         //   {
         //      return;
         //   };

         //   {
         //      String FileName = "Dummy" + vm.Document.FileExt;

         //      BackboneViewModel.Current.IncBusy();

         //      try
         //      {
         //         vm.databaseFileRead(FileName);
         //      }
         //      catch (Exception ex)
         //      {
         //         BackboneViewModel.Current.DecBusy();

         //         MessageBox.Show("Oups ..." + Environment.NewLine + ex.Message);

         //         return;
         //      };

         //      BackboneViewModel.Current.DecBusy();

         //      try
         //      {
         //         System.Diagnostics.Process process = new System.Diagnostics.Process();
         //         process.StartInfo.FileName = FileName;
         //         process.Start();
         //         //process.WaitForExit();
         //      }
         //      catch (Exception ex)
         //      {
         //         MessageBox.Show("Oups ..." + Environment.NewLine + ex.Message);
         //      };
         //   };
      }

      //// - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void btnExportAll_Click(object sender, RoutedEventArgs e)
      {
         //   bool AllDB = false;

         //   if (Keyboard.IsKeyDown(Key.LeftCtrl))
         //   {
         //      AllDB = true;

         //      if (MessageBox.Show("Export entire BaseDoc?", "Export", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
         //      {
         //         return;
         //      };
         //   };


         //   var vm = BaseDocViewModel.Current;
         //   if (vm.ExternalRef == -1 && !AllDB)
         //   {
         //      MessageBox.Show("Vous devez d’abord choisir ou créer un client (REF).", "Attention", MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
         //      return;
         //   };

         //   // Configure Save file dialog box
         //   Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
         //   dlg.FileName = String.Format("{0}.{1}.zip", tbClientRef.Text, DateTime.Now.ToString("yyyyMMdd HHmm")); // Default file name
         //   dlg.DefaultExt = ".zip";            // Default file extension
         //   dlg.Filter = "ZIP (.zip)|*.zip";    // Filter files by extension 

         //   // Show Save file dialog box
         //   Nullable<bool> result = dlg.ShowDialog();

         //   // Process Save file dialog box results 
         //   if (result == true)
         //   {
         //      BackboneViewModel.Current.IncBusy();

         //      // Save document 
         //      string FileName = dlg.FileName;

         //      FileStream fsOut = File.Create(FileName);
         //      ZipOutputStream zipStream = new ZipOutputStream(fsOut);
         //      zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

         //      // zipStream.Password = "eurogold";  // optional. Null is the same as not setting. Required if using AES.

         //      // - - -  - - -

         //      foreach (var doc in vm.Documents)
         //      {
         //         vm.Document = doc;

         //         AddDoc(zipStream, doc, doc.FileName);
         //      };

         //      // - - -  - - -

         //      zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
         //      zipStream.Close();

         //      BackboneViewModel.Current.DecBusy();
         //   };
         //}

         //private void AddDoc(ZipOutputStream zipStream, Document doc, string FileName, Stream stream)
         //{
         //   var vm = BaseDocViewModel.Current;

         //   try
         //   {
         //      ZipEntry newEntry = new ZipEntry(FileName);
         //      newEntry.DateTime = doc.TimeStampCreation;

         //      zipStream.PutNextEntry(newEntry);

         //      StreamUtils.Copy(stream, zipStream, new byte[4096]);
         //      zipStream.CloseEntry();
         //   }
         //   catch (Exception)
         //   {
         //      // MessageBox.Show(ex.ToString);
         //   };
         //}

         //private void AddDoc(ZipOutputStream zipStream, Document doc, string FileName, bool Picture = false)
         //{
         //   var vm = BaseDocViewModel.Current;

         //   try
         //   {
         //      ZipEntry newEntry = new ZipEntry(FileName);
         //      newEntry.DateTime = doc.TimeStampCreation;

         //      zipStream.PutNextEntry(newEntry);

         //      StreamUtils.Copy(vm.databaseFileReadStream(Picture), zipStream, new byte[4096]);
         //      zipStream.CloseEntry();
         //   }
         //   catch (Exception)
         //   {
         //      // MessageBox.Show(ex.ToString);
         //   };
      }

      //// - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
      //private void tbClientRef_KeyDown(object sender, KeyEventArgs e)
      //{
      //   if (e.Key == Key.Return)
      //   {
      //      tbTitle.Focus();
      //   };
      //}

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void listBoxDocuments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         //   if (listBoxDocuments.SelectedIndex != -1)
         //   {
         //      btnShow_Click(null, null);
         //   }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
