using System.Globalization;
using System.Windows;
using System.Windows.Media;
using NZag.Controls;
using NZag.Services;

namespace NZag.Windows
{
    internal class ZTextGridWindow : ZWindow
    {
        private readonly Size fontCharSize;
        private readonly ZTextGrid textGrid;

        private bool bold;
        private bool italic;
        private bool reverse;

        public ZTextGridWindow(ZWindowManager manager, FontAndColorService fontAndColorService)
            : base(manager, fontAndColorService)
        {
            var zero = new FormattedText(
                textToFormat: "0",
                culture: CultureInfo.InstalledUICulture,
                flowDirection: FlowDirection.LeftToRight,
                typeface: new Typeface(new FontFamily("Consolas"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                emSize: 16.0,
                foreground: Brushes.Black);

            this.fontCharSize = new Size(zero.Width, zero.Height);

            this.textGrid = new ZTextGrid();
            this.Children.Add(this.textGrid);
        }

        public override bool SetBold(bool value)
        {
            var oldValue = this.bold;
            this.bold = value;
            this.textGrid.SetBold(value);
            return oldValue;
        }

        public override bool SetItalic(bool value)
        {
            var oldValue = this.italic;
            this.italic = value;
            this.textGrid.SetItalic(value);
            return oldValue;
        }

        public override bool SetReverse(bool value)
        {
            var oldValue = this.reverse;
            this.reverse = value;
            this.textGrid.SetReverse(value);
            return oldValue;
        }

        public override void Clear()
        {
            this.textGrid.Clear();
        }

        public override void PutChar(char ch, bool forceFixedWidthFont)
        {
            Brush foregroundBrush, backgroundBrush;
            if (this.reverse)
            {
                foregroundBrush = BackgroundBrush;
                backgroundBrush = ForegroundBrush;
            }
            else
            {
                foregroundBrush = ForegroundBrush;
                backgroundBrush = BackgroundBrush;
            }

            this.textGrid.PutChar(ch, foregroundBrush, backgroundBrush);
        }

        public override void PutText(string text, bool forceFixedWidthFont)
        {
            Brush foregroundBrush, backgroundBrush;
            if (this.reverse)
            {
                foregroundBrush = BackgroundBrush;
                backgroundBrush = ForegroundBrush;
            }
            else
            {
                foregroundBrush = ForegroundBrush;
                backgroundBrush = BackgroundBrush;
            }

            foreach (var ch in text)
            {
                this.textGrid.PutChar(ch, foregroundBrush, backgroundBrush);
            }
        }

        public override int GetCursorColumn()
        {
            return this.textGrid.CursorColumn;
        }

        public override int GetCursorLine()
        {
            return this.textGrid.CursorLine;
        }

        public override void SetCursorAsync(int line, int column)
        {
            this.textGrid.SetCursor(line, column);
        }

        public override int GetHeight()
        {
            var rowIndex = GetRow(this);
            return (int)(this.ParentWindow.RowDefinitions[rowIndex].Height.Value / this.RowHeight);
        }

        public override void SetHeight(int lines)
        {
            var rowIndex = GetRow(this);
            this.ParentWindow.RowDefinitions[rowIndex].Height = new GridLength(lines * this.RowHeight, GridUnitType.Pixel);
            this.textGrid.SetHeight(lines);
        }

        public override int RowHeight
        {
            get { return (int)this.fontCharSize.Height; }
        }

        public override int ColumnWidth
        {
            get { return (int)this.fontCharSize.Width; }
        }
    }
}
