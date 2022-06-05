using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPF.XF
{
    public static class Display
    {
        public static double Height { get => _Height; set => _Height = value; }
        static double _Height = -1;

        public static double Width { get => _Width; set => _Width = value; }
        static double _Width = -1;

        public static double Scale { get; internal set; } = 1;
    }
}
