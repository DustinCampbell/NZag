using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using NZag.Extensions;
using NZag.Services;

namespace NZag.Windows
{
    internal class ZTextBufferWindow : ZWindow
    {
        private readonly FontFamily normalFontFamily;
        private readonly FontFamily fixedFontFamily;
        private readonly Size fontCharSize;

        private readonly FlowDocument document;
        private readonly Paragraph paragraph;
        private readonly FlowDocumentScrollViewer scrollViewer;

        private bool bold;
        private bool italic;
        private bool fixedPitch;
        private bool reverse;

        public ZTextBufferWindow(ZWindowManager manager, FontAndColorService fontAndColorService)
            : base(manager, fontAndColorService)
        {
            this.normalFontFamily = new FontFamily("Cambria");
            this.fixedFontFamily = new FontFamily("Consolas");

            var zero = new FormattedText(
                textToFormat: "0",
                culture: CultureInfo.InstalledUICulture,
                flowDirection: FlowDirection.LeftToRight,
                typeface: new Typeface(normalFontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                emSize: this.FontSize,
                foreground: Brushes.Black);

            this.fontCharSize = new Size(zero.Width, zero.Height);

            this.document = new FlowDocument
            {
                FontFamily = normalFontFamily,
                FontSize = this.FontSize,
                PagePadding = new Thickness(8.0)
            };

            this.paragraph = new Paragraph();
            this.document.Blocks.Add(this.paragraph);

            this.scrollViewer = new FlowDocumentScrollViewer
            {
                FocusVisualStyle = null,
                Document = this.document
            };

            this.Children.Add(this.scrollViewer);
        }

        private void ForceFixedWidthFontAsync(bool value, Action action)
        {
            var oldValue = this.SetFixedPitch(value);
            action();
            this.SetFixedPitch(oldValue);
        }

        private Run CreateFormattedRun(string text)
        {
            var run = new Run(text);

            if (this.bold)
            {
                run.FontWeight = FontWeights.Bold;
            }

            if (this.italic)
            {
                run.FontStyle = FontStyles.Italic;
            }

            run.FontFamily = this.fixedPitch
                ? this.fixedFontFamily
                : this.normalFontFamily;

            if (this.reverse)
            {
                run.Background = ForegroundBrush;
                run.Foreground = BackgroundBrush;
            }
            else
            {
                run.Background = BackgroundBrush;
                run.Foreground = ForegroundBrush;
            }

            return run;
        }

        private void ScrollToEnd()
        {
            var scroller = this.scrollViewer.FindFirstVisualChild<ScrollViewer>();
            if (scroller != null)
            {
                scroller.ScrollToEnd();
            }
        }

        private void ClearInlines()
        {
            this.paragraph.Inlines.Clear();
        }

        public override void Clear()
        {
            ClearInlines();
        }

        protected override async Task<char> ReadCharCoreAsync()
        {
            AssertIsForeground();

            Keyboard.Focus(this.scrollViewer);
            var args = await this.scrollViewer.TextInputAsync();

            return args.Text[0];
        }

        protected override async Task<string> ReadTextCoreAsync(int maxChars)
        {
            AssertIsForeground();

            var inputTextBox = new TextBox
            {
                FontFamily = normalFontFamily,
                FontSize = this.FontSize,
                Padding = new Thickness(0.0),
                Margin = new Thickness(0.0),
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0.0),
                Background = Brushes.WhiteSmoke,
                MaxLength = maxChars
            };

            var scrollContext = this.scrollViewer.FindFirstVisualChild<ScrollContentPresenter>();
            var lastCharacterRect = this.document.ContentEnd.GetCharacterRect(LogicalDirection.Forward);
            var minWidth = scrollContext.ActualHeight - this.document.PagePadding.Right - lastCharacterRect.Right;
            inputTextBox.MinWidth = Math.Max(minWidth, 0);

            var container = new InlineUIContainer(inputTextBox, this.document.ContentEnd)
            {
                BaselineAlignment = BaselineAlignment.TextBottom
            };

            this.paragraph.Inlines.Add(container);

            if (!inputTextBox.Focus())
            {
                inputTextBox.PostAction(() => inputTextBox.Focus());
            }

            string text = null;
            while (text == null)
            {
                var args = await inputTextBox.KeyUpAsync();
                if (args.Key == Key.Return)
                {
                    text = inputTextBox.Text;
                }
            }

            this.paragraph.Inlines.Remove(container);
            PutText(text + "\r\n", forceFixedWidthFont: false);

            return text;
        }

        public override void PutChar(char ch, bool forceFixedWidthFont)
        {
            ForceFixedWidthFontAsync(forceFixedWidthFont, () =>
            {
                var run = CreateFormattedRun(ch.ToString(CultureInfo.InvariantCulture));
                this.paragraph.Inlines.Add(run);
                ScrollToEnd();
            });
        }

        public override void PutText(string text, bool forceFixedWidthFont)
        {
            ForceFixedWidthFontAsync(forceFixedWidthFont, () =>
            {
                var run = CreateFormattedRun(text);
                this.paragraph.Inlines.Add(run);
                ScrollToEnd();
            });
        }

        public override bool SetBold(bool value)
        {
            var oldValue = this.bold;
            this.bold = value;
            return oldValue;
        }

        public override bool SetItalic(bool value)
        {
            var oldValue = this.italic;
            this.italic = value;
            return oldValue;
        }

        public override bool SetFixedPitch(bool value)
        {
            var oldValue = this.fixedPitch;
            this.fixedPitch = value;
            return oldValue;
        }

        public override bool SetReverse(bool value)
        {
            var oldValue = this.reverse;
            this.reverse = value;
            return oldValue;
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
