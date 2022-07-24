using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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
using System.Diagnostics;
using Configurator;

namespace StoreCheck.Pages
{
   /// <summary>
   /// Interaction logic for WinCE_Page.xaml
   /// </summary>
   public partial class WinCE_Page : Page
   {
      public WinCE_Page()
      {
         InitializeComponent();
      }

      private void Page_Loaded(object sender, RoutedEventArgs e)
      {
         //List<ZPF.WWW.Excel.Data> list = new List<ZPF.WWW.Excel.Data>();

         //list.Add(new ZPF.WWW.Excel.Data
         //{
         //   Title = "Articles",
         //   SheetName = "Articles",
         //   dataTable = (DB_SQL.QuickQueryView("SELECT * FROM Article") as DataTable)
         //});

         //list.Add(new ZPF.WWW.Excel.Data
         //{
         //   Title = "Emplacements",
         //   SheetName = "Emplacements",
         //   dataTable = (DB_SQL.QuickQueryView("SELECT * FROM Emplacement") as DataTable)
         //});

         //ZPF.WWW.Excel.ExportXLS(list, System.IO.Path.GetTempFileName() + ".xlsx", true);

         tbLog.Text = Environment.MachineName + Environment.NewLine + GetIP();
      }

      private string GetIP()
      {
         IPHostEntry host;
         string localIP = "?";
         host = Dns.GetHostEntry(Dns.GetHostName());

         foreach (IPAddress ip in host.AddressList)
         {
            if (ip.AddressFamily.ToString() == "InterNetwork")
            {
               localIP = ip.ToString();
            };
         };

         return localIP;
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         MainViewModel.Current.Save();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void TileEx_Click(object sender, RoutedEventArgs e)
      {
         Configuration.Clear();
         Configuration.Add($"PWD={MainViewModel.Current.MasterPwd}");
         Configuration.Add($"IP={GetIP()}");

         Configurations.Clear();
         Configurations.Add("StoreCheck config=" + Configuration.Text.Replace("\r\n", "\n"));

         PrintIt();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      TStrings Configurations = new TStrings();
      TStrings Configuration = new TStrings();
      int IndConfiguration = 0;
      byte CRC = 0;
      bool DebugInfo = false;

      string HelpText = "Planche pour configuration rapide ...";

      void PrintIt()
      {
         string DocumentName = "";

         if (Configurations.Count == 1)
         {
            DocumentName = Configurations.Names(0);
         }
         else
         {
            DocumentName = "Configuration Merge";
         };

         {
            // http://www.pdfsharp.com/PDFsharp/index.php?option=com_content&task=view&id=24&Itemid=35

            // Create a new PDF document
            PdfDocument pdfDocument = new PdfDocument();

            // Create an empty page
            PdfPage pdfPage = pdfDocument.AddPage();

            bool HasMorePages = true;

            while (HasMorePages)
            {
               DocumentName = Configurations.Names(IndConfiguration);

               UpdateConfig(Configurations.ValueFromIndex(IndConfiguration));

               PDFPage(pdfDocument, pdfPage, DocumentName);

               DocumentName = DocumentName.Replace('/', ' ');
               DocumentName = DocumentName.Replace('\\', ' ');

               IndConfiguration += 1;

               HasMorePages = IndConfiguration < Configurations.Count;

               if (!HasMorePages)
               {
                  IndConfiguration = 0;
               }
               else
               {
                  // if (checkBoxSeperateFiles.Checked)
                  {
                     // Save the document...
                     pdfDocument.Save(DocumentName + ".pdf");

                     // Create a new PDF document
                     pdfDocument = new PdfDocument();
                  };

                  // Create an empty page
                  pdfPage = pdfDocument.AddPage();
               };
            };

            //if (!checkBoxSeperateFiles.Checked)
            //{
            //   DocumentName = "Configuration Merge";
            //};

            // Save the document...
            string FileName = System.IO.Path.GetTempPath() + DocumentName + ".pdf";
            pdfDocument.Save(FileName);

            // ...and start a viewer.
            Process.Start(FileName);
         };
      }

      private void UpdateConfig(string Config, bool BigCB = false)
      {
         int FrameSize = 22;

         if (BigCB)
         {
            FrameSize = 19;
         };

         string st = Config.Replace("\r\n", "\n");
         //if (radioButtonExit.Checked) st = st + "\n" + "{Exit}";
         //if (radioButtonWarmBoot.Checked) st = st + "\n" + "{WarmBoot}";
         //if (radioButtonColdBoot.Checked) st = st + "\n" + "{ColdBoot}";

         Configuration.Clear();

         while (st.Length > FrameSize)
         {
            Configuration.Add(st.Substring(0, FrameSize));
            st = st.Substring(FrameSize);
         };

         Configuration.Add(st);

         //textBoxClipboard.Text = Configuration.Text;
         CRC = ConfigHelper.CCITT8(Configuration.Text);
      }

      private void PDFPage(PdfDocument pdfDocument, PdfPage pdfPage, string PageTitle)
      {
         int Margin = 30;
         int Indent = 30;
         int PosY = 0;
         int BarCodeWeight = 1;

         //if (checkBoxBig.Checked)
         //{
         //   BarCodeWeight = 2;
         //};

         // Create the fonts
         XFont VeryBigFont = new XFont("Verdana", 24, XFontStyle.Bold);
         XFont BigFont = new XFont("Verdana", 16);
         XFont SmallFont = new XFont("Verdana", 8);
         XFont MediumFont = new XFont("Verdana", 14, XFontStyle.Bold);

         //
         // - - -  - - -  - - -  - - - 

         XRect rectFull = new XRect();			  // The full available space
         XRect rectBody;			              // Area for the list items

         rectFull.Width = pdfPage.Width;
         rectFull.Height = pdfPage.Height;
         rectFull = XRect.Inflate(rectFull, -Margin * 2, -Margin * 2);

         //
         // - - - Draw page top & bottom - - -  - - -  - - - 

         // Get an XGraphics object for drawing
         XGraphics gfx = XGraphics.FromPdfPage(pdfPage);

         int PageTop = PDFPageTop(gfx, rectFull, VeryBigFont, PageTitle);
         int PageBottom = PDFPageBottom(gfx, rectFull, SmallFont, 1);

         rectBody = XRect.Inflate(rectFull, 0, -PageTop - PageBottom);

         PosY = PageTop;
         PosY += 20;

         //
         // - - -  - - - 

         for (int i = 0; i < Configuration.Count; i++)
         {
            gfx.DrawString((i + 1).ToString(), BigFont, XBrushes.Black, rectFull.Left, PosY + 20);

            XImage BarCode = MakeBarCode(String.Format("{0:x2}{1}{2}{3}", CRC, Convert.ToChar(i + 1), Convert.ToChar(Configuration.Count), Configuration[i]), BarCodeWeight);

            int Height = BarCode.Height;

            if (true)
            {
               //if (checkBoxBig.Checked)
               //{
               //   Height = 35;
               //}
               //else
               {
                  Height = 28;
               };
            };

            gfx.DrawImage(BarCode, rectFull.Left + Indent, PosY, BarCode.Width * 0.75, Height);

            PosY += Height;

            if (DebugInfo)
            {
               PosY += 2;
               gfx.DrawString(Configuration[i], SmallFont, XBrushes.Black, rectFull.Left + Indent, PosY);
               PosY += 16;
            };

            //if (checkBoxBig.Checked)
            //{
            //   PosY += 27;
            //}
            //else
            {
               PosY += 15;
            };
         };

         XRect rect = new XRect(rectFull.X, PosY + 100, rectFull.Width, 100);

         PDFHelp(gfx, rect, MediumFont, HelpText);
      }

      private int PDFPageTop(XGraphics gfx, XRect rectFull, XFont font, string Title)
      {
         int Result = (int)rectFull.Top;
         int LineWidth = 1;

         if (gfx == null) return Result;

         XSize size = gfx.MeasureString(Title, font);
         XRect rect = new XRect(rectFull.Left, rectFull.Top, rectFull.Width, size.Height);

         // Display title at top
         gfx.DrawString(Title, font, XBrushes.Black, rect, XStringFormat.Center);

         Result += (int)(size.Height);

         gfx.DrawLine(new XPen(System.Drawing.Color.Black, LineWidth), new XPoint(rect.Left, rect.Bottom), new XPoint(rect.Right, rect.Bottom));
         Result += LineWidth;

         return Result;
      }

      private int PDFHelp(XGraphics gfx, XRect rect, XFont font, string Help)
      {
         int Result = (int)rect.Top;

         if (gfx == null) return Result;

         XTextFormatter tf = new XTextFormatter(gfx);

         tf.Alignment = XParagraphAlignment.Center;
         tf.DrawString(Help, font, XBrushes.Black, rect, XStringFormat.TopLeft);

         XSize size = gfx.MeasureString(Help, font);
         Result += (int)(size.Height);

         return Result;
      }

      private int PDFPageBottom(XGraphics gfx, XRect rectFull, XFont font, int m_nPageNumber)
      {
         int PosY = (int)rectFull.Bottom;
         int Result = 0;
         int LineWidth = 1;

         if (gfx == null) return Result;

         XSize size = gfx.MeasureString("Azerty", font);
         XRect rect = new XRect(rectFull.Left, rectFull.Bottom - size.Height, rectFull.Width, size.Height);

         XTextFormatter tf = new XTextFormatter(gfx);

         // Display page number at bottom
         string st = "Page " + m_nPageNumber;

         tf.Alignment = XParagraphAlignment.Right;
         tf.DrawString(st, font, XBrushes.Black, rect, XStringFormat.TopLeft);

         tf.Alignment = XParagraphAlignment.Left;
         tf.DrawString(DateTime.Now.ToString(), font, XBrushes.Black, rect, XStringFormat.TopLeft);

         PosY -= LineWidth;
         gfx.DrawLine(new XPen(System.Drawing.Color.Black, LineWidth), new XPoint(rect.Left, rect.Top), new XPoint(rect.Right, rect.Top));

         return (int)(rectFull.Bottom - PosY);
      }

      System.Drawing.Image MakeBarCode(string Text, int Weight)
      {
         try
         {
            //ToDo: return Code128Rendering.MakeBarcodeImage(Text, Weight, false);
            return null;
         }
         catch 
         {
            //MessageBox.Show(this, ex.Message, this.Text);
            return null;
         }
      }
   }
}
