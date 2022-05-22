namespace StoreCheck;

public static class MauiProgram
{
   public static MauiApp CreateMauiApp()
   {
      var builder = MauiApp.CreateBuilder();
      builder
         .UseMauiApp<App>()
         .ConfigureFonts(fonts =>
         {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            fonts.AddFont("desyrel.ttf", "desyrel");
            fonts.AddFont("IconFont.ttf", "IconFont");
            fonts.AddFont("MediaPlayerFont.ttf", "MediaPlayerFont");
            fonts.AddFont("Montserrat.ttf", "Montserrat");
         });

      return builder.Build();
   }
}
