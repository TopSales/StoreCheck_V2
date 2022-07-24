using GenCode128;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using static BackboneViewModel;

namespace ZPF
{
   class PrintLabelHelper
   {
      public static void PrintLabel(Label.PrintLabel label, PrintPreviewControl ppc = null)
      {
         PrintToPrinterHelper._Label = label;

         if (!string.IsNullOrEmpty(label.WaterMark))
         {
            PrintToPrinterHelper.Watermark = label.WaterMark;
         };

         try
         {
            System.Drawing.Printing.PrintDocument printDocumentConfig = new System.Drawing.Printing.PrintDocument();
            printDocumentConfig.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(PrintPage_Label);

            printDocumentConfig.DocumentName = label.Title;

            printDocumentConfig.PrinterSettings.PrinterName = MainViewModel.Current.PrinterSettings.Name;
            printDocumentConfig.DefaultPageSettings.Margins = MainViewModel.Current.PrinterSettings.GetMargins();
            printDocumentConfig.DefaultPageSettings.PaperSize = MainViewModel.Current.PrinterSettings.GetPaperSize();
            printDocumentConfig.DefaultPageSettings.Landscape = MainViewModel.Current.PrinterSettings.Landscape;

            if (ppc != null)
            {
               ppc.Document = printDocumentConfig;
            }
            else
            {
               printDocumentConfig.Print();
            };
         }
         catch (Exception ex)
         {
            BackboneViewModel.Current.MessageBox(MessageBoxType.Error, ex.Message);
         };
      }

      private static void PrintPage_Label(object sender, System.Drawing.Printing.PrintPageEventArgs e)
      {
         PrintPageLabel(sender, e);
         e.HasMorePages = false;
      }

      private static void PrintPageLabel(object sender, System.Drawing.Printing.PrintPageEventArgs e)
      {
         // - - -  - - -  - - -  - - - 

         #region Init

         int BarCodeWeight = 1;

         int PosY = 0;

         PrintToPrinterHelper.SmallFont = new Font("Arial", 8);
         PrintToPrinterHelper.MediumFont = new Font("Arial", 14, System.Drawing.FontStyle.Bold);
         PrintToPrinterHelper.BigFont = new Font("Arial", 16, System.Drawing.FontStyle.Bold);
         PrintToPrinterHelper.VeryBigFont = new Font("Arial", 24, System.Drawing.FontStyle.Bold);

         RectangleF rectBody;			  // Area for the list items
         PrintToPrinterHelper.rectFull = e.MarginBounds;

         #endregion

         // - - - Draw page top & bottom - - -  - - -  - - - 

         int PrintPageTop(Graphics g, RectangleF rectFull)
         {
            int _PosY = (int)rectFull.Top;
            int LineWidth = 1;

            if (g == null) return 0;

            int Height = 0;
            Height = Math.Max(Height, PrintToPrinterHelper.PrintText(g, rectFull, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 1 && x.Row == "H").FirstOrDefault()));
            Height = Math.Max(Height, PrintToPrinterHelper.PrintText(g, rectFull, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 2 && x.Row == "H").FirstOrDefault()));
            Height = Math.Max(Height, PrintToPrinterHelper.PrintText(g, rectFull, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 3 && x.Row == "H").FirstOrDefault()));

            _PosY += Height;

            if (PrintToPrinterHelper._Label.HeaderHasLine) g.DrawLine(new Pen(Color.Black, LineWidth), new Point((int)rectFull.Left, _PosY), new Point((int)rectFull.Right, _PosY));
            _PosY += LineWidth;

            return _PosY;
         }

         int PrintPageBottom(Graphics g, RectangleF rectFull)
         {
            int _PosY = (int)rectFull.Bottom;
            int LineWidth = 1;

            if (g == null) return 0;

            int Height = 0;
            Height = Math.Max(Height, PrintToPrinterHelper.GetHeight(g, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 1 && x.Row == "F").FirstOrDefault()));
            Height = Math.Max(Height, PrintToPrinterHelper.GetHeight(g, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 2 && x.Row == "F").FirstOrDefault()));
            Height = Math.Max(Height, PrintToPrinterHelper.GetHeight(g, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 3 && x.Row == "F").FirstOrDefault()));

            var rect = rectFull;
            rect.Location = new PointF(rect.Left, rect.Bottom - Height);
            rect.Height = Height;

            PrintToPrinterHelper.PrintText(g, rect, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 1 && x.Row == "F").FirstOrDefault());
            PrintToPrinterHelper.PrintText(g, rect, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 2 && x.Row == "F").FirstOrDefault());
            PrintToPrinterHelper.PrintText(g, rect, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 3 && x.Row == "F").FirstOrDefault());

            _PosY -= Height;

            _PosY -= LineWidth;
            if (PrintToPrinterHelper._Label.FooterHasLine) g.DrawLine(new Pen(Color.Black, LineWidth), new Point((int)rectFull.Left, _PosY), new Point((int)rectFull.Right, _PosY));

            return (int)(rectFull.Bottom - _PosY);
         }

         int PrintPageBody(Graphics g, RectangleF rectFull)
         {
            int _PosY = (int)rectFull.Top;

            if (g == null) return 0;

            int Height_B1 = 0;
            Height_B1 = Math.Max(Height_B1, PrintToPrinterHelper.GetHeight(g, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 1 && x.Row == "B1").FirstOrDefault()));
            Height_B1 = Math.Max(Height_B1, PrintToPrinterHelper.GetHeight(g, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 2 && x.Row == "B1").FirstOrDefault()));
            Height_B1 = Math.Max(Height_B1, PrintToPrinterHelper.GetHeight(g, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 3 && x.Row == "B1").FirstOrDefault()));

            int Height_B2 = 0;
            Height_B2 = Math.Max(Height_B2, PrintToPrinterHelper.GetHeight(g, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 1 && x.Row == "B2").FirstOrDefault()));
            Height_B2 = Math.Max(Height_B2, PrintToPrinterHelper.GetHeight(g, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 2 && x.Row == "B2").FirstOrDefault()));
            Height_B2 = Math.Max(Height_B2, PrintToPrinterHelper.GetHeight(g, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 3 && x.Row == "B2").FirstOrDefault()));

            var rect = rectFull;
            rect.Height = Height_B1;

            PrintToPrinterHelper.PrintText(g, rect, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 1 && x.Row == "B1").FirstOrDefault());
            PrintToPrinterHelper.PrintText(g, rect, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 2 && x.Row == "B1").FirstOrDefault());
            PrintToPrinterHelper.PrintText(g, rect, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 3 && x.Row == "B1").FirstOrDefault());
            _PosY += Height_B1;

            rect = rectFull;
            rect.Location = new PointF(rectFull.Left, rectFull.Top + Height_B1);

            PrintToPrinterHelper.PrintText(g, rect, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 1 && x.Row == "B2").FirstOrDefault());
            PrintToPrinterHelper.PrintText(g, rect, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 2 && x.Row == "B2").FirstOrDefault());
            PrintToPrinterHelper.PrintText(g, rect, PrintToPrinterHelper._Label.Cells.Where(x => x.Col == 3 && x.Row == "B2").FirstOrDefault());
            _PosY += Height_B2;

            return _PosY;
         }

         int PageTop = PrintPageTop(e.Graphics, PrintToPrinterHelper.rectFull);
         int PageBottom = PrintPageBottom(e.Graphics, PrintToPrinterHelper.rectFull);

         if (PrintToPrinterHelper.Watermark != "") PrintToPrinterHelper.PrintWatermark(e.Graphics, PrintToPrinterHelper.rectFull, PrintToPrinterHelper.Watermark);

         // - - -  - - - 

         rectBody = RectangleF.Inflate(PrintToPrinterHelper.rectFull, 0, -PageTop - PageBottom);
         rectBody.Location = new PointF(rectBody.Left, PageTop);
         PosY = PageTop;

         PageTop = PrintPageBody(e.Graphics, rectBody);

         //if (string.IsNullOrEmpty(_fields["Description"]))
         //{
         //   PosY += 20;
         //}
         //else
         //{
         //   PosY += 5;
         //};

         // - - -  - - - 

         //string sBarCode = "barcode";

         {
            //System.Drawing.Image BarCode = MakeBarCode(sBarCode, BarCodeWeight);

            //int Height = BarCode.Height;

            //Height = 35;

            //e.Graphics.DrawImage(BarCode, PrintToPrinterHelper.rectFull.Left + (PrintToPrinterHelper.rectFull.Width - BarCode.Width) / 2, PosY, BarCode.Width, Height);
            //PosY += Height;

            //{
            //   PosY += 1;
            //   PosY = (int)PrintToPrinterHelper.PrintText(e.Graphics, PosY, PrintToPrinterHelper.SmallFont, sBarCode, System.Windows.HorizontalAlignment.Center);
            //};

            //{
            //   PosY += 5;
            //   PosY = (int)PrintToPrinterHelper.PrintText(e.Graphics, PosY, MediumFont, _fields["Description"], System.Windows.HorizontalAlignment.Center);
            //};
         };
      }


      public static void Print(TStrings fields, string WaterMark = "", System.Windows.Forms.PrintPreviewControl ppc = null)
      {
         PrintToPrinterHelper.Watermark = WaterMark;

         _fields = fields;

         try
         {
            System.Drawing.Printing.PrintDocument printDocumentConfig = new System.Drawing.Printing.PrintDocument();
            printDocumentConfig.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocumentConfig_PrintPage);

            printDocumentConfig.DocumentName = _fields["Title"];

            printDocumentConfig.PrinterSettings.PrinterName = MainViewModel.Current.PrinterSettings.Name;
            printDocumentConfig.DefaultPageSettings.Margins = MainViewModel.Current.PrinterSettings.GetMargins();
            printDocumentConfig.DefaultPageSettings.PaperSize = MainViewModel.Current.PrinterSettings.GetPaperSize();
            printDocumentConfig.DefaultPageSettings.Landscape = MainViewModel.Current.PrinterSettings.Landscape;

            if (ppc != null)
            {
               ppc.Document = printDocumentConfig;
            }
            else
            {
               printDocumentConfig.Print();
            };
         }
         catch (Exception ex)
         {
            BackboneViewModel.Current.MessageBox(MessageBoxType.Error, ex.Message);
         };
      }


      static TStrings _fields = null;

      // Title=Stock>Rack 1 (demo)
      // Barcode = E01
      // Description=A01 - Article 1 (demo)

      public static void PrintOld(TStrings fields, string WaterMark = "", System.Windows.Forms.PrintPreviewControl ppc = null)
      {
         PrintToPrinterHelper.Watermark = WaterMark;

         _fields = fields;

         try
         {
            System.Drawing.Printing.PrintDocument printDocumentConfig = new System.Drawing.Printing.PrintDocument();
            printDocumentConfig.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocumentConfig_PrintPage);

            printDocumentConfig.DocumentName = _fields["Title"];

            printDocumentConfig.PrinterSettings.PrinterName = MainViewModel.Current.PrinterSettings.Name;
            printDocumentConfig.DefaultPageSettings.Margins = MainViewModel.Current.PrinterSettings.GetMargins();
            printDocumentConfig.DefaultPageSettings.PaperSize = MainViewModel.Current.PrinterSettings.GetPaperSize();
            printDocumentConfig.DefaultPageSettings.Landscape = MainViewModel.Current.PrinterSettings.Landscape;

            if (ppc != null)
            {
               ppc.Document = printDocumentConfig;
            }
            else
            {
               printDocumentConfig.Print();
            };
         }
         catch (Exception ex)
         {
            BackboneViewModel.Current.MessageBox(MessageBoxType.Error, ex.Message);
         };
      }

      public static FixedDocumentSequence Document { get; set; }


      private static void printDocumentConfig_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
      {
         PrintPage(sender, e);
         e.HasMorePages = false;
      }

      private static void PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
      {
         PrintToPrinterHelper.PageBottom = "{1}\t\tStoreCheck";

         // - - -  - - -  - - -  - - - 

         int PosY = 0;

         Font SmallFont = new Font("Arial", 8);
         Font MediumFont = new Font("Arial", 14, System.Drawing.FontStyle.Bold);
         Font BigFont = new Font("Arial", 16, System.Drawing.FontStyle.Bold);
         Font VeryBigFont = new Font("Arial", 24, System.Drawing.FontStyle.Bold);

         //
         // - - -  - - -  - - -  - - - 

         RectangleF rectBody;			  // Area for the list items

         PrintToPrinterHelper.rectFull = e.MarginBounds;

         // - - - Draw page top & bottom - - -  - - -  - - - 

         int PageTop = (int)PrintToPrinterHelper.PrintPageTop(e.Graphics, PrintToPrinterHelper.rectFull, BigFont, (sender as System.Drawing.Printing.PrintDocument).DocumentName, true);
         int PageBottom = (int)PrintToPrinterHelper.PrintPageBottom(e.Graphics, PrintToPrinterHelper.rectFull, SmallFont, 1, true);
         if (PrintToPrinterHelper.Watermark != "") PrintToPrinterHelper.PrintWatermark(e.Graphics, PrintToPrinterHelper.rectFull, PrintToPrinterHelper.Watermark);

         // - - -  - - - 

         rectBody = RectangleF.Inflate(PrintToPrinterHelper.rectFull, 0, -PageTop - PageBottom);

         PosY = PageTop;

         if (string.IsNullOrEmpty(_fields["Description"]))
         {
            PosY += 20;
         }
         else
         {
            PosY += 5;
         };

         // - - -  - - - 

         string sBarCode = _fields["Barcode"];

         {
            int BarCodeWeight = 1;
            System.Drawing.Image BarCode = PrintToPrinterHelper.MakeBarCode(sBarCode, BarCodeWeight);

            int Height = BarCode.Height;

            Height = 35;

            e.Graphics.DrawImage(BarCode, PrintToPrinterHelper.rectFull.Left + (PrintToPrinterHelper.rectFull.Width - BarCode.Width) / 2, PosY, BarCode.Width, Height);
            PosY += Height;

            {
               PosY += 1;
               PosY = (int)PrintToPrinterHelper.PrintText(e.Graphics, PosY, SmallFont, sBarCode, System.Windows.HorizontalAlignment.Center);
            };

            {
               PosY += 5;
               PosY = (int)PrintToPrinterHelper.PrintText(e.Graphics, PosY, MediumFont, _fields["Description"], System.Windows.HorizontalAlignment.Center);
            };
         };
      }

   }
}
