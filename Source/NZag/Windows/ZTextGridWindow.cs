using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NZag.Controls;

namespace NZag.Windows
{
    internal class ZTextGridWindow : ZWindow
    {
        private readonly Size fontCharSize;
        private readonly ZTextGrid textGrid;

        private bool bold;
        private bool italic;
        private bool reverse;

        public ZTextGridWindow(ZWindowManager manager)
            : base(manager)
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

        public override Task<bool> SetBoldAsync(bool value)
        {
            return RunOnUIThread(() =>
            {
                var oldValue = this.bold;
                this.bold = value;
                this.textGrid.SetBold(value);
                return oldValue;
            });
        }

        public override Task<bool> SetItalicAsync(bool value)
        {
            return RunOnUIThread(() =>
            {
                var oldValue = this.italic;
                this.italic = value;
                this.textGrid.SetItalic(value);
                return oldValue;
            });
        }

        public override Task<bool> SetReverseAsync(bool value)
        {
            return RunOnUIThread(() =>
            {
                var oldValue = this.reverse;
                this.reverse = value;
                this.textGrid.SetReverse(value);
                return oldValue;
            });
        }

        public override Task ClearAsync()
        {
            return RunOnUIThread(() =>
                this.textGrid.Clear());
        }

        public override Task PutCharAsync(char ch, bool forceFixedWidthFont)
        {
            return RunOnUIThread(() =>
                this.textGrid.PutChar(ch));
        }

        public override Task PutTextAsync(string text, bool forceFixedWidthFont)
        {
            return RunOnUIThread(() =>
            {
                foreach (var ch in text)
                {
                    this.textGrid.PutChar(ch);
                }
            });
        }

        public override Task<int> GetCursorColumnAsync()
        {
            return RunOnUIThread(() =>
                this.textGrid.CursorX);
        }

        public override Task<int> GetCursorLineAsync()
        {
            return RunOnUIThread(() =>
                this.textGrid.CursorY);
        }

        public override Task SetCursorAsync(int line, int column)
        {
            return RunOnUIThread(() =>
                this.textGrid.SetCursor(line, column));
        }

        public override Task<int> GetHeightAsync()
        {
            return RunOnUIThread(() =>
            {
                var rowIndex = GetRow(this);
                return (int)(this.ParentWindow.RowDefinitions[rowIndex].Height.Value / this.RowHeight);
            });
        }

        public override Task SetHeightAsync(int lines)
        {
            return RunOnUIThread(() =>
            {
                var rowIndex = GetRow(this);
                this.ParentWindow.RowDefinitions[rowIndex].Height = new GridLength(lines * this.RowHeight, GridUnitType.Pixel);
                this.textGrid.SetHeight(lines);
            });
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
