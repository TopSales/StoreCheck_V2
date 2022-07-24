//using Seagull.BarTender.Print;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Threading;
using ZPF;

namespace StoreCheck
{
   /// <summary>
   /// Interaction logic for PrintPage.xaml
   /// </summary>
   public partial class PrintPage : Page
   {
      private const string appName = "Print Preview";

      public Engine engine = null;                 // The BarTender Print Engine.
      private LabelFormatDocument format = null;   // The format that will be exported.
      private string previewPath = "";             // The path to the folder where the previews will be exported.
      private int currentPage = 1;                 // The current page being viewed.
      private int totalPages;                      // Number of pages.
      private Messages messages;

      BackgroundWorker backgroundWorker = null;

      public PrintPage()
      {
         InitializeComponent();

         // Create and start a new BarTender Print Engine.
         try
         {
            engine = new Engine(true);
         }
         //catch (PrintEngineException ex)
         //{
         //   // If the engine is unable to start, a PrintEngineException will be thrown.
         //   MessageBox.Show(ex.Message, appName);
         //   //ToDo: this.Close(); // Close this app. We cannot run without connection to an engine.

         //   return;
         //}
         catch (Exception ex)
         {
            // If the engine is unable to start, a PrintEngineException will be thrown.
            MessageBox.Show(ex.Message, appName);
            //ToDo: this.Close(); // Close this app. We cannot run without connection to an engine.

            return;
         };

         backgroundWorker = new BackgroundWorker();
         backgroundWorker.DoWork += BackgroundWorker_DoWork;
         backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
      }

      private void Page_Loaded(object sender, RoutedEventArgs e)
      {
         MainViewModel.Instance.LoadExcel();

         /// Start BarTender Print Engine, Initialize printer list,
         /// enable/disable controls, setup temporary folder for images.

         // Hide/Disable preview controls.
         DisablePreview();

         // Create a temporary folder to hold the images.
         string tempPath = System.IO.Path.GetTempPath(); // Something like "C:\Documents and Settings\<username>\Local Settings\Temp\"
         string newFolder;

         // It's not likely we'll have a path that already exists, but we'll check for it anyway.
         do
         {
            newFolder = System.IO.Path.GetRandomFileName();
            previewPath = tempPath + newFolder; // newFolder is something crazy like "gulvwdmt.3r4"
         } while (Directory.Exists(previewPath));

         Directory.CreateDirectory(previewPath);
         Debug.WriteLine("Path=" + previewPath);
      }

      private void Page_Unloaded(object sender, RoutedEventArgs e)
      {
         /// Stop the BarTender Print Engine and delete our temporary images.

         // Quit the BarTender Print Engine but do not save changes to any open formats.
         if (engine != null)
            engine.Stop(SaveOptions.DoNotSaveChanges);

         // Remove the temporary path and all the images.
         if (previewPath.Length != 0)
         {
            try
            {
               Directory.Delete(previewPath, true);
            }
            catch { };
         }
      }

      private void btnPreview_Click(object sender, RoutedEventArgs e)
      {
         /// Runs when the Preview button is pressed. Disables controls and starts
         /// BarTender working to export print preview images.

         // Disable some controls until we are finished creating previews.
         //btnOpen.Enabled = false;
         //cboPrinters.Enabled = false;
         btnPreview.IsEnabled = false;
         btnPrint.IsEnabled = false;

         DisablePreview();

         // Get the printer from the dropdown and assign it to the format.
         // format.PrintSetup.PrinterName = cboPrinters.SelectedItem.ToString();

         // Set control states to show working. These will be reset when the work completes.
         //ToDo: picUpdating.Visibility = Visibility.Visible;

         // Have the background worker export the BarTender format.
         backgroundWorker.RunWorkerAsync(format);
      }

      /// <summary>
      /// Show the preview of the first label.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnFirst_Click(object sender, EventArgs e)
      {
         currentPage = 1;
         ShowPreview();
      }

      /// <summary>
      /// Show the preview of the previous label.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnPrev_Click(object sender, EventArgs e)
      {
         --currentPage;
         ShowPreview();
      }

      /// <summary>
      /// Show the preview of the next label.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnNext_Click(object sender, EventArgs e)
      {
         ++currentPage;
         ShowPreview();
      }

      /// <summary>
      /// Show the preview of the last label.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnLast_Click(object sender, EventArgs e)
      {
         currentPage = totalPages;
         ShowPreview();
      }

      #region Print Preview Thread Methods
      /// <summary>
      /// Runs in a separate thread to allow BarTender to export the preview while allowing UI updates.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
      {
         LabelFormatDocument format = (LabelFormatDocument)e.Argument;

         // Delete any existing files.
         string[] oldFiles = Directory.GetFiles(previewPath, "*.*");
         for (int index = 0; index < oldFiles.Length; ++index)
         {
            try
            {
               File.Delete(oldFiles[index]);
            }
            catch { };
         };

         // Export the format's print previews.
         //Application.Current.Dispatcher.Invoke(new Action(() =>
         //{
         //}));
         int w = (int)borderImage.ActualWidth * 2;
         int h = (int)borderImage.ActualHeight * 2;

         format.ExportPrintPreviewToFile(previewPath, "PrintPreview%PageNumber%.jpg", ImageType.JPEG, Seagull.BarTender.Print.ColorDepth.ColorDepth24bit, new Resolution(w, h), System.Drawing.Color.White, OverwriteOptions.Overwrite, true, true, out messages);

         string[] files = Directory.GetFiles(previewPath, "*.*");
         totalPages = files.Length;
      }

      /// <summary>
      /// We are done exporting the preview, so let's show any messages
      /// and display the first label.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
      {
         // Report any messages.
         if (messages != null)
         {
            if (messages.Count > 5)
            {
               MessageBox.Show("There are more than 5 messages from the print preview. Only the first 5 will be displayed.", appName);
            };

            int count = 0;
            foreach (Seagull.BarTender.Print.Message message in messages)
            {
               MessageBox.Show(message.Text, appName);

               if (++count >= 5)
                  break;
            };
         };

         // btnOpen.Enabled = true;
         btnPreview.IsEnabled = true;
         btnPrint.IsEnabled = true;
         // cboPrinters.Enabled = true;

         // Only enable the preview if we actual got some pages.
         if (totalPages != 0)
         {
            lblNumPreviews.Visibility = Visibility.Visible;

            currentPage = 1;
            ShowPreview();
         }
      }
      #endregion

      #region Methods
      /// <summary>
      /// Disable/Hide preview controls.
      /// </summary>
      private void DisablePreview()
      {
         borderImage.Child = null;
         //picPreview.Source = null;
         //picPreview.Visibility = Visibility.Collapsed;

         btnFirst.IsEnabled = false;
         btnPrev.IsEnabled = false;
         lblNumPreviews.Visibility = Visibility.Collapsed;
         btnNext.IsEnabled = false;
         btnLast.IsEnabled = false;
      }

      /// <summary>
      /// Show the preview of the current page.
      /// </summary>
      private void ShowPreview()
      {
         // Our current preview number shouldn't be out of range,
         // but we'll practice good programming by checking it.
         if ((currentPage < 1) || (currentPage > totalPages))
            currentPage = 1;

         // Update the page label and the preview Image.
         lblNumPreviews.Text = string.Format("Page {0} of {1}", currentPage, totalPages);

         RefreshPicture();

         // Enable or Disable controls as needed.
         if (currentPage == 1)
         {
            btnPrev.IsEnabled = false;
            btnFirst.IsEnabled = false;
         }
         else
         {
            btnPrev.IsEnabled = true;
            btnFirst.IsEnabled = true;
         }

         if (currentPage == totalPages)
         {
            btnNext.IsEnabled = false;
            btnLast.IsEnabled = false;
         }
         else
         {
            btnNext.IsEnabled = true;
            btnLast.IsEnabled = true;
         }
      }

      private void RefreshPicture()
      {
         string filename = string.Format("{0}\\PrintPreview{1}.jpg", previewPath, currentPage);

         borderImage.Child = null;
         // picPreview = null;
         borderImage.Child = null;
         GC.Collect();

         BitmapImage logo = new BitmapImage();
         logo.BeginInit();
         logo.CacheOption = BitmapCacheOption.OnLoad;
         logo.UriSource = new Uri(filename);
         logo.EndInit();

         var picPreview = new Image();
         picPreview.StretchDirection = StretchDirection.DownOnly;
         picPreview.HorizontalAlignment = HorizontalAlignment.Center;
         picPreview.VerticalAlignment = VerticalAlignment.Center;
         picPreview.Source = logo;

         borderImage.Child = picPreview;
      }
      #endregion

      private void btnPrint_Click(object sender, RoutedEventArgs e)
      {
         /// Prints the currently loaded format using the selected printer.

         // We lock on the engine here so we don't lose our format object
         // if the user were to click on a format that wouldn't load.
         lock (engine)
         {
            bool success = true;

            // Assign number of identical copies if it is not datasourced.
            if (format.PrintSetup.SupportsIdenticalCopies)
            {
               int copies = 1;
               format.PrintSetup.IdenticalCopiesOfLabel = copies;
            };

            // Assign number of serialized copies if it is not datasourced.
            if (format.PrintSetup.SupportsSerializedLabels)
            {
               int copies = 1;
               format.PrintSetup.NumberOfSerializedLabels = copies;
            };

            // If there are no incorrect values in the copies boxes then print.
            if (success)
            {
               //Cursor.Current = Cursors.WaitCursor;

               // Get the printer from the dropdown and assign it to the format.
               if (!string.IsNullOrEmpty(MainViewModel.Instance.PrinterName))
               {
                  format.PrintSetup.PrinterName = MainViewModel.Instance.PrinterName;
               };

               Messages messages;
               int waitForCompletionTimeout = 10000; // 10 seconds
               Result result = format.Print(appName, waitForCompletionTimeout, out messages);
               string messageString = "\n\nMessages:";

               foreach (Seagull.BarTender.Print.Message message in messages)
               {
                  messageString += "\n\n" + message.Text;
               }

               if (result == Result.Failure)
               {
                  MessageBox.Show("Print Failed" + messageString, appName);
               }
               else
               {
                  MessageBox.Show("Label was successfully sent to printer." + messageString, appName);
               };
            }
         }
      }

      private void btnSearch_Click(object sender, RoutedEventArgs e)
      {
         //Demo Saint Gobain
         /*
                  Article art = MainViewModel.Instance.GetArticle(tbRef.Text);
                  tbInfo.Text = art.Description;

                  btnPreview.IsEnabled = art != null;
                  btnPrint.IsEnabled = art != null;

                  // Let's disable some controls until we are done.
                  btnPreview.IsEnabled = false;
                  btnPrint.IsEnabled = false;

                  {
                     //Cursor.Current = Cursors.WaitCursor;

                     // Close the previous format.
                     if (format != null)
                        format.Close(SaveOptions.DoNotSaveChanges);

                     // We need to delete the files associated with the last format.
                     // picPreview.Source = null;
                     borderImage.Child = null;
                     string[] files = Directory.GetFiles(previewPath);
                     foreach (string file in files)
                     {
                        try
                        {
                           File.Delete(file);
                        }
                        catch { };
                     }

                     // Put the UI back into a non-preview state.
                     DisablePreview();

                     // Open the format.
                     //string FileName = @"D:\OneDrive\ZPF\Devis\20161010 - MadSoft - Saint Gobain\StoreCheck\Test01.btw";
                     string FileName = art.BarTenderLabel;

                     try
                     {
                        format = engine.Documents.Open(System.AppDomain.CurrentDomain.BaseDirectory + FileName);
                     }
                     catch (System.Runtime.InteropServices.COMException comException)
                     {
                        MessageBox.Show(String.Format("Unable to open format: {0}\nReason: {1}", FileName, comException.Message), appName);
                        format = null;
                     };

                     try
                     {
                        //string st = format.SubStrings["Ref"].Value;
                        //format.SubStrings["Now"].Value = DateTime.Now.ToString();

                        //format.SubStrings["Ref"].Value = art.Ref;
                        format.SubStrings["ProductCode"].Value = art.Ref;

                        format.SubStrings["Description"].Value = art.Description;
                        format.SubStrings["QTE"].Value = art.QTE.ToString();
                        format.SubStrings["DateMfg"].Value = DateTime.Now.ToString("dd/MM/yyyy");
                        format.SubStrings["DateExp"].Value = DateTime.Now.AddYears(art.DLUO).ToString("dd/MM/yyyy");

                        format.SubStrings["CustomerPN"].Value = art.CustomerPN;
                        format.SubStrings["Revision"].Value = art.Revision;
                     }
                     catch (Exception ex)
                     {
                        Debug.WriteLine(ex.Message);
                     };

                     // Only allow preview button if we successfully loaded the format.
                     btnPreview.IsEnabled = (format != null);
                     btnPrint.IsEnabled = (format != null);

                     //Cursor.Current = Cursors.Default;
                  };

                  // Restore some controls.
                  btnPreview.IsEnabled = art != null;
                  btnPrint.IsEnabled = art != null;

                  if (format != null)
                  {
                     Application.Current.Dispatcher.Invoke(new Action(() =>
                     {
                        btnPreview_Click(null, null);
                     }));
                  };
               */
      }
   }
}
