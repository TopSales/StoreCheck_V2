using ZPF;

namespace ZPF
{
   interface IColorModel
   {
      Microsoft.Maui.Graphics.Color ToolbarColor { get; set; }

      Microsoft.Maui.Graphics.Color BackgroundColor { get; set; }
      Microsoft.Maui.Graphics.Color BackgroundColor50 { get; set; }

      Microsoft.Maui.Graphics.Color TextColor { get; set; }
      Microsoft.Maui.Graphics.Color TextColor80 { get; set; }

      Microsoft.Maui.Graphics.Color HighLightColor { get; set; }

      Microsoft.Maui.Graphics.Color ActionTextColor { get; set; }
      Microsoft.Maui.Graphics.Color ActionBackgroundColor { get; set; }

      Microsoft.Maui.Graphics.Color PostItColor { get; set; }
   }
}
