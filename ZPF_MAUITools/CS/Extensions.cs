using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Extensions
{
    public static SkiaSharp.SKColor ToSKColor(this Microsoft.Maui.Graphics.Color color)
    {
        return new SkiaSharp.SKColor((byte)(color.Alpha * 255), (byte)(color.Red * 255), (byte)(color.Green * 255), (byte)(color.Blue * 255));
    }
}
