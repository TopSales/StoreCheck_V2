using ZPF.XF;

[assembly: Xamarin.Forms.Dependency(typeof(Scanner))]
namespace ZPF.XF
{
   /// <summary>
   /// </summary>
   public class Scanner : IScanner
   {
      static bool _IsOpen = false;

      public bool CloseScanner()
      {
         _IsOpen = false;
         return true;
      }

      public bool EnableAllEANs()
      {
         return true;
      }

      public bool EnableAllSymbologies()
      {
         return true;
      }

      public bool EnableEAN13Only()
      {
         return true;
      }

      public bool IsOpen()
      {
         return _IsOpen;
      }

      public bool OpenScanner()
      {
         _IsOpen = true;
         return true;
      }
   }
}
