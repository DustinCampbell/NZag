using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace NZag.Controls
{
    public class ZTextGrid : FrameworkElement
    {
        private readonly VisualCollection visuals;
        private readonly SortedList<Tuple<int, int>, VisualPair> visualPairs;

        private readonly double fontSize;
        private readonly Size fontCharSize;

        private int cursorLine;
        private int cursorColumn;

        private Typeface typeface;
        private bool bold;
        private bool italic;
        private bool reverse;

        public ZTextGrid(double fontSize)
        {
            this.visuals = new VisualCollection(this);
            this.visualPairs = new SortedList<Tuple<int, int>, VisualPair>();

            this.fontSize = fontSize;

            var zero = new FormattedText(
                textToFormat: "0",
                culture: CultureInfo.InstalledUICulture,
                flowDirection: FlowDirection.LeftToRight,
                typeface: GetTypeface(),
                emSize: fontSize,
                foreground: Brushes.Black);

            this.fontCharSize = new Size(zero.Width, zero.Height);
        }

        private Typeface GetTypeface()
        {
            if (typeface == null)
            {
                var style = this.italic ? FontStyles.Italic : FontStyles.Normal;
                var weight = bold ? FontWeights.Bold : FontWeights.Normal;
                typeface = new Typeface(new FontFamily("Consolas"), style, weight, stretch: FontStretches.Normal);
            }

            return typeface;
        }

        public void Clear()
        {
            this.visuals.Clear();
            this.visualPairs.Clear();
            this.cursorColumn = 0;
            this.cursorLine = 0;
        }

        public void PutChar(char ch, Brush foregroundBrush, Brush backgroundBrush)
        {
            if (ch == '\n')
            {
                this.cursorLine += 1;
                this.cursorColumn = 0;
            }
            else
            {
                // First, see if we've already inserted something at this position.
                // If so, delete the old visuals.
                var cursorPos = Tuple.Create(this.cursorColumn, this.cursorLine);
                if (this.visualPairs.ContainsKey(cursorPos))
                {
                    var visualPair = this.visualPairs[cursorPos];
                    this.visuals.Remove(visualPair.Background);
                    this.visuals.Remove(visualPair.Character);
                    this.visualPairs.Remove(cursorPos);
                }

                var backgroundVisual = new DrawingVisual();
                var backgroundContext = backgroundVisual.RenderOpen();

                var x = fontCharSize.Width * cursorColumn;
                var y = fontCharSize.Height * cursorLine;

                var backgroundRect = new Rect(
                    Math.Floor(x),
                    Math.Floor(y),
                    Math.Ceiling(fontCharSize.Width + 0.5),
                    Math.Ceiling(fontCharSize.Height));

                backgroundContext.DrawRectangle(backgroundBrush, null, backgroundRect);
                backgroundContext.Close();

                this.visuals.Insert(0, backgroundVisual);

                var textVisual = new DrawingVisual();
                var textContext = textVisual.RenderOpen();

                textContext.DrawText(
                    new FormattedText(
                        ch.ToString(CultureInfo.InvariantCulture),
                        CultureInfo.InstalledUICulture,
                        FlowDirection.LeftToRight,
                        GetTypeface(),
                        this.fontSize,
                        foregroundBrush,
                        new NumberSubstitution(NumberCultureSource.User, CultureInfo.InstalledUICulture, NumberSubstitutionMethod.AsCulture),
                        TextFormattingMode.Display),
                    new Point(x, y));

                textContext.Close();

                this.visuals.Add(textVisual);

                var newVisualPair = new VisualPair(backgroundVisual, textVisual);
                this.visualPairs.Add(cursorPos, newVisualPair);

                this.cursorColumn += 1;
            }
        }

        public void SetBold(bool value)
        {
            this.bold = value;
            this.typeface = null;
        }

        public void SetItalic(bool value)
        {
            this.italic = value;
            this.typeface = null;
        }

        public void SetReverse(bool value)
        {
            this.reverse = value;
        }

        public int CursorColumn
        {
            get { return this.cursorColumn; }
        }

        public int CursorLine
        {
            get { return this.cursorLine; }
        }

        public void SetCursor(int line, int column)
        {
            this.cursorLine = line;
            this.cursorColumn = column;
        }

        public void SetHeight(int lines)
        {
            for (int i = this.visualPairs.Count - 1; i >= 0; i--)
            {
                var cursorPos = this.visualPairs.Keys[i];
                var y = cursorPos.Item2;
                if (y > lines - 1)
                {
                    var visualPair = visualPairs[cursorPos];
                    this.visuals.Remove(visualPair.Background);
                    this.visuals.Remove(visualPair.Character);
                    this.visualPairs.Remove(cursorPos);
                }
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }

        protected override int VisualChildrenCount
        {
            get { return this.visuals.Count; }
        }
    }
}
