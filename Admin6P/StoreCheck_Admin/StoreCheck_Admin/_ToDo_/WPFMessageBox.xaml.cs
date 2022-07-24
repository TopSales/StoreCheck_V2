using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZPF.WPF
{
   internal enum AccentState
   {
      ACCENT_DISABLED = 0,
      ACCENT_ENABLE_GRADIENT = 1,
      ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
      ACCENT_ENABLE_BLURBEHIND = 3,
      ACCENT_INVALID_STATE = 4
   }

   [StructLayout(LayoutKind.Sequential)]
   internal struct AccentPolicy
   {
      public AccentState AccentState;
      public int AccentFlags;
      public int GradientColor;
      public int AnimationId;
   }

   [StructLayout(LayoutKind.Sequential)]
   internal struct WindowCompositionAttributeData
   {
      public WindowCompositionAttribute Attribute;
      public IntPtr Data;
      public int SizeOfData;
   }

   internal enum WindowCompositionAttribute
   {
      // ...
      WCA_ACCENT_POLICY = 19
      // ...
   }

   /// <summary>
   /// Interaction logic for MessageBox.xaml
   /// </summary>
   public partial class WPFMessageBox : Window
   {
      public static TStrings Dico = TStrings.FromJSon("[{\"ok\":\"ok\"}, {\"cancel\":\"cancel\"}, {\"Info\":\"Info\"}, {\"Warning\":\"Warning\"}, {\"Error\":\"Error\"}, {\"Confirmation\":\"Confirmation\"} ]");
      public static TStrings DicoFR = TStrings.FromJSon("[{\"ok\":\"ok\"}, {\"cancel\":\"annuler\"}, {\"Info\":\"Info\"}, {\"Warning\":\"Avertissement\"}, {\"Error\":\"Erreur\"}, {\"Confirmation\":\"Confirmation\"} ]");

      // https://github.com/riverar/sample-win10-aeroglass
      // https://web.archive.org/web/20170701081532/http://withinrafael.com/adding-the-aero-glass-blur-to-your-windows-10-apps/

      public WPFMessageBox()
      {
         InitializeComponent();
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         EnableBlur();
      }

      [DllImport("user32.dll")]
      internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

      internal void EnableBlur()
      {
         var windowHelper = new WindowInteropHelper(this);

         var accent = new AccentPolicy();
         accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

         var accentStructSize = Marshal.SizeOf(accent);

         var accentPtr = Marshal.AllocHGlobal(accentStructSize);
         Marshal.StructureToPtr(accent, accentPtr, false);

         var data = new WindowCompositionAttributeData();
         data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
         data.SizeOfData = accentStructSize;
         data.Data = accentPtr;

         SetWindowCompositionAttribute(windowHelper.Handle, ref data);

         Marshal.FreeHGlobal(accentPtr);
      }

      public static bool Show(Window Owner, BackboneViewModel.MessageBoxType type, string Text, string Caption = "")
      {
         var m = new ZPF.WPF.WPFMessageBox();

         if (Owner != null && Owner.IsVisible)
         {
            m.Owner = Owner;
         };

         m.Show(Text, Caption, type);

         return m.ShowDialog() == true;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void Show(string text, string caption, BackboneViewModel.MessageBoxType type)
      {
         btnOK.Content = Dico["ok"];
         btnCancel.Content = Dico["cancel"];

         tbCaption.Text = caption;
         tbText.Text = text;
         btnCancel.Visibility = Visibility.Collapsed;

         if (string.IsNullOrEmpty(tbCaption.Text))
         {
            tbCaption.FontStyle = FontStyles.Italic;
         }
         else
         {
            tbCaption.FontStyle = FontStyles.Normal;
         };

         switch (type)
         {
            case BackboneViewModel.MessageBoxType.Info:
               tbCaption.Text = (string.IsNullOrEmpty(tbCaption.Text) ? Dico["Info"] : tbCaption.Text);
               tbIcon.Text = "i";
               break;

            case BackboneViewModel.MessageBoxType.Warning:
               tbCaption.Text = (string.IsNullOrEmpty(tbCaption.Text) ? Dico["Warning"] : tbCaption.Text);
               tbIcon.Text = "!";
               break;

            case BackboneViewModel.MessageBoxType.Error:
               tbCaption.Text = (string.IsNullOrEmpty(tbCaption.Text) ? Dico["Error"] : tbCaption.Text);
               tbIcon.Text = "!";
               break;

            case BackboneViewModel.MessageBoxType.Confirmation:
               tbCaption.Text = (string.IsNullOrEmpty(tbCaption.Text) ? Dico["Confirmation"] : tbCaption.Text);
               tbIcon.Text = "?";
               btnCancel.Visibility = Visibility.Visible;
               break;
         };

      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void btnOK_Click(object sender, RoutedEventArgs e)
      {
         DialogResult = true;
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         DialogResult = false;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
