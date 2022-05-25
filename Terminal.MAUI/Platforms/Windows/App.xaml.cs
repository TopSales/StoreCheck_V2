using System.Reflection.PortableExecutable;
using System.Security.Principal;
using Microsoft.UI.Xaml;
using System;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StoreCheck.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
   /// <summary>
   /// Initializes the singleton application object.  This is the first line of authored code
   /// executed, and as such is the logical equivalent of main() or WinMain().
   /// </summary>
   public App()
   {
      this.InitializeComponent();

      MainViewModel.Current.DeviceID = GetComputerSid().ToString();
   }

   public static SecurityIdentifier GetComputerSid()
   {
      //var x = new System.DirectoryServices.DirectoryEntry($"WinNT://{Environment.MachineName},Computer").Children.Cast<System.DirectoryServices.DirectoryEntry>().First().InvokeGet("objectSID");

      return new SecurityIdentifier((byte[])new System.DirectoryServices.DirectoryEntry($"WinNT://{Environment.MachineName},Computer").Children.Cast<System.DirectoryServices.DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid;
   }

   protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

