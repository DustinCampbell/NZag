using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using NZag.Core;

namespace NZag.Services
{
    [Export]
    public class FontAndColorService
    {
        private Brush defaultForegroundBrush = Brushes.Black;
        private Brush defaultBackgroundBrush = Brushes.White;

        private Brush foregroundBrush = Brushes.Black;
        private Brush backgroundBrush = Brushes.White;

        private static Brush GetZColorBrush(ZColor color)
        {
            switch (color)
            {
                case ZColor.Black:
                    return Brushes.Black;
                case ZColor.Blue:
                    return Brushes.Blue;
                case ZColor.Cyan:
                    return Brushes.Cyan;
                case ZColor.Gray:
                    return Brushes.Gray;
                case ZColor.Green:
                    return Brushes.Green;
                case ZColor.Magenta:
                    return Brushes.Magenta;
                case ZColor.Red:
                    return Brushes.Red;
                case ZColor.White:
                    return Brushes.White;
                case ZColor.Yellow:
                    return Brushes.Yellow;

                default:
                    throw new ArgumentException("Unexpected color: " + color, "color");
            }
        }

        public void SetForegroundColor(ZColor foreground)
        {
            this.foregroundBrush = foreground == ZColor.Default
                ? this.defaultForegroundBrush
                : GetZColorBrush(foreground);
        }

        public void SetBackgroundColor(ZColor background)
        {
            this.backgroundBrush = background == ZColor.Default
                ? this.defaultBackgroundBrush
                : GetZColorBrush(background);
        }

        public Brush ForegroundBrush
        {
            get { return this.foregroundBrush; }
        }

        public Brush BackgroundBrush
        {
            get { return this.backgroundBrush; }
        }
    }
}
