using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using NZag.Extensions;

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

        public ZTextBufferWindow(ZWindowManager manager)
            : base(manager)
        {
            this.normalFontFamily = new FontFamily("Cambria");
            this.fixedFontFamily = new FontFamily("Consolas");

            var zero = new FormattedText(
                textToFormat: "0",
                culture: CultureInfo.InstalledUICulture,
                flowDirection: FlowDirection.LeftToRight,
                typeface: new Typeface(normalFontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                emSize: 16.0,
                foreground: Brushes.Black);

            this.fontCharSize = new Size(zero.Width, zero.Height);

            this.document = new FlowDocument
            {
                FontFamily = normalFontFamily,
                FontSize = 16.0,
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

        private async Task ForceFixedWidthFontAsync(bool value, Action action)
        {
            var oldValue = await this.SetFixedPitchAsync(value);
            action();
            await this.SetFixedPitchAsync(oldValue);
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
                run.Background = Brushes.Black;
                run.Foreground = Brushes.White;
            }
            else
            {
                run.Background = Brushes.White;
                run.Foreground = Brushes.Black;
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

        private Task SetKeyboardFocusAsync(IInputElement element)
        {
            return RunOnUIThread(() =>
                Keyboard.Focus(element));
        }

        private Task ClearInlinesAsync()
        {
            return RunOnUIThread(() =>
                this.paragraph.Inlines.Clear());
        }

        public override async Task ClearAsync()
        {
            await ClearInlinesAsync();
        }

        public override async Task<char> ReadCharAsync()
        {
            await SetKeyboardFocusAsync(this.scrollViewer);
            var args = await this.scrollViewer.TextInputAsync();

            return args.Text[0];
        }

        public override Task<string> ReadTextAsync(int maxChars)
        {
            return RunOnUIThread(async () =>
            {
                var inputTextBox = new TextBox
                {
                    FontFamily = normalFontFamily,
                    FontSize = 16.0,
                    Padding = new Thickness(0.0),
                    Margin = new Thickness(0.0),
                    BorderBrush = Brushes.Transparent,
                    BorderThickness = new Thickness(0.0),
                    Background = Brushes.WhiteSmoke,
                    MaxLength = maxChars
                };

                var scrollContext = this.scrollViewer.FindFirstVisualChild<ScrollContentPresenter>();
                var lastCharacterRect = this.document.ContentEnd.GetCharacterRect(LogicalDirection.Forward);
                inputTextBox.MinWidth = scrollContext.ActualHeight - this.document.PagePadding.Right -
                                        lastCharacterRect.Right;

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
                await PutTextAsync(text + "\r\n", forceFixedWidthFont: false);

                return text;
            });
        }

        public override Task PutCharAsync(char ch, bool forceFixedWidthFont)
        {
            return RunOnUIThread(async () =>
                await ForceFixedWidthFontAsync(forceFixedWidthFont, () =>
                {
                    var run = CreateFormattedRun(ch.ToString(CultureInfo.InvariantCulture));
                    this.paragraph.Inlines.Add(run);
                    ScrollToEnd();
                }));
        }

        public override Task PutTextAsync(string text, bool forceFixedWidthFont)
        {
            return RunOnUIThread(async () =>
                await ForceFixedWidthFontAsync(forceFixedWidthFont, () =>
                {
                    var run = CreateFormattedRun(text);
                    this.paragraph.Inlines.Add(run);
                    ScrollToEnd();
                }));
        }

        public override Task<bool> SetBoldAsync(bool value)
        {
            return RunOnUIThread(() =>
            {
                var oldValue = this.bold;
                this.bold = value;
                return oldValue;
            });
        }

        public override Task<bool> SetItalicAsync(bool value)
        {
            return RunOnUIThread(() =>
            {
                var oldValue = this.italic;
                this.italic = value;
                return oldValue;
            });
        }

        public override Task<bool> SetFixedPitchAsync(bool value)
        {
            return RunOnUIThread(() =>
            {
                var oldValue = this.fixedPitch;
                this.fixedPitch = value;
                return oldValue;
            });
        }

        public override Task<bool> SetReverseAsync(bool value)
        {
            return RunOnUIThread(() =>
            {
                var oldValue = this.reverse;
                this.reverse = value;
                return oldValue;
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
