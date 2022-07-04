using Microsoft.Maui.Handlers;
using SkiaSharp;
using ZPF.XF.Compos;

namespace StoreCheck;
#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

public partial class App : Application
{
   const int WindowHeight = 1400;
   const int WindowWidth = 900;

   public App()
   {
      InitializeComponent();

      Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
      {
#if WINDOWS
            var mauiWindow = handler.VirtualView;
            var nativeWindow = handler.PlatformView;
            nativeWindow.Activate();
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new SizeInt32(WindowWidth, WindowHeight));
#endif
      });


      MainPage = new AppShell();
      //MainPage = new NavigationPage(new MainPage());
      WindowHandler.Mapper.ModifyMapping(nameof(IWindow.Content), OnWorkaround);
   }

   // https://github.com/dotnet/maui/issues/8062
   private void OnWorkaround(IWindowHandler arg1, IWindow arg2, Action<IElementHandler, IElement> arg3)
   {
      WindowHandler.MapContent(arg1, arg2);
   }

   protected override async void OnStart()
   {
      base.OnStart();

      var fn = await CopyFile("IconFont.ttf");
      Tile.IconFont = SKTypeface.FromFile(fn);
   }

   public async Task<string> CopyFile(string name)
   {
      string basePath = "";

      basePath = FileSystem.AppDataDirectory;
      var finalPath = Path.Combine(basePath, name);

      if (File.Exists(finalPath))
      {
         return finalPath;
         //File.Delete(finalPath);
      };

      using var stream = await FileSystem.OpenAppPackageFileAsync(name);

      using (var tempFileStream = await FileSystem.OpenAppPackageFileAsync(name))
      {
         using (var fileStream = File.Open(finalPath, FileMode.CreateNew))
         {
            await tempFileStream.CopyToAsync(fileStream);
         };
      };

      return finalPath;
   }
}
