using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NZag.Core;
using NZag.Services;
using NZag.Windows;
using SimpleMVVM;
using SimpleMVVM.Threading;

namespace NZag.ViewModels
{
    [Export]
    public class ScreenViewModel : ViewModelBase<UserControl>, IScreen
    {
        private readonly ForegroundThreadAffinitizedObject foregroundThreadAffinitedObject;

        private readonly GameService gameService;
        private readonly FontAndColorService fontAndColorService;
        private readonly ZWindowManager windowManager;

        private Grid windowContainer;
        private ZWindow mainWindow;
        private ZWindow upperWindow;

        private int currentStatusHeight;
        private int machineStatusHeight;

        [ImportingConstructor]
        private ScreenViewModel(GameService gameService, FontAndColorService fontAndColorService)
            : base("Views/ScreenView")
        {
            this.foregroundThreadAffinitedObject = new ForegroundThreadAffinitizedObject();

            this.gameService = gameService;
            this.fontAndColorService = fontAndColorService;
            this.windowManager = new ZWindowManager(fontAndColorService);

            this.gameService.GameOpened += OnGameOpened;
        }

        protected override void OnViewCreated(UserControl view)
        {
            this.windowContainer = view.FindName<Grid>("WindowContainer");
        }

        private void OnGameOpened(object sender, EventArgs e)
        {
            this.mainWindow = this.windowManager.OpenWindow(ZWindowKind.TextBuffer);
            this.windowContainer.Children.Add(this.mainWindow);
            this.upperWindow = this.windowManager.OpenWindow(ZWindowKind.TextGrid, this.mainWindow, ZWindowPosition.Above);

            this.windowManager.ActivateWindow(this.mainWindow);
        }

        private void ResetStatusHeight()
        {
            this.foregroundThreadAffinitedObject.AssertIsForeground();

            if (this.upperWindow != null)
            {
                int height = this.upperWindow.GetHeight();
                if (this.machineStatusHeight != height)
                {
                    this.upperWindow.SetHeight(machineStatusHeight);
                }
            }
        }

        public Task<char> ReadCharAsync()
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(async () =>
            {
                var ch = await this.windowManager.ActiveWindow.ReadCharAsync();

                ResetStatusHeight();
                this.currentStatusHeight = 0;

                return ch;
            }).Unwrap();
        }

        public Task<string> ReadTextAsync(int maxChars)
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(async () =>
            {
                string command;

                if (this.gameService.HasNextScriptCommand)
                {
                    command = this.gameService.GetNextScriptCommand();
                    var forceFixedWidthFont = this.gameService.Machine.ForceFixedWidthFont();
                    this.windowManager.ActiveWindow.PutText(command + "\r\n", forceFixedWidthFont);
                }
                else
                {
                    command = await this.windowManager.ActiveWindow.ReadTextAsync(maxChars);
                }

                ResetStatusHeight();
                this.currentStatusHeight = 0;

                return command;
            }).Unwrap();
        }

        public Task WriteCharAsync(char value)
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
            {
                var forceFixedWidthFont = this.gameService.Machine.ForceFixedWidthFont();
                this.windowManager.ActiveWindow.PutChar(value, forceFixedWidthFont);
            });
        }

        public Task WriteTextAsync(string value)
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
            {
                var forceFixedWidthFont = this.gameService.Machine.ForceFixedWidthFont();
                this.windowManager.ActiveWindow.PutText(value, forceFixedWidthFont);
            });
        }

        public Task ClearAsync(int window)
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
            {
                if (window == 0)
                {
                    this.mainWindow.Clear();
                }
                else if (window == 1 && this.upperWindow != null)
                {
                    this.upperWindow.Clear();

                    ResetStatusHeight();

                    this.currentStatusHeight = 0;
                }
            });
        }

        public Task ClearAllAsync(bool unsplit)
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(async () =>
            {
                this.mainWindow.Clear();

                if (this.upperWindow != null)
                {
                    if (unsplit)
                    {
                        await this.UnsplitAsync();
                    }
                    else
                    {
                        this.upperWindow.Clear();
                    }
                }
            }).Unwrap();
        }

        public Task SplitAsync(int lines)
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
            {
                if (this.upperWindow == null)
                {
                    return;
                }

                if (lines == 0 || lines > this.currentStatusHeight)
                {
                    int height = this.upperWindow.GetHeight();
                    if (lines != height)
                    {
                        this.upperWindow.SetHeight(lines);
                        this.currentStatusHeight = lines;
                    }
                }

                this.machineStatusHeight = lines;

                if (this.gameService.Machine.Memory.Version == 0)
                {
                    this.upperWindow.Clear();
                }
            });
        }

        public Task UnsplitAsync()
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
            {
                if (this.upperWindow == null)
                {
                    return;
                }

                this.upperWindow.SetHeight(0);
                this.upperWindow.Clear();
                ResetStatusHeight();
                this.currentStatusHeight = 0;
            });
        }

        public Task SetWindowAsync(int window)
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
            {
                if (window == 0)
                {
                    this.mainWindow.Activate();
                }
                else if (window == 1)
                {
                    this.upperWindow.Activate();
                }
            });
        }

        public Task ShowStatusAsync()
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
            {
                if (this.gameService.Machine.Memory.Version > 3)
                {
                    return;
                }

                if (this.upperWindow == null)
                {
                    this.upperWindow = this.windowManager.OpenWindow(
                        ZWindowKind.TextGrid,
                        this.mainWindow,
                        ZWindowPosition.Above,
                        ZWindowSizeKind.Fixed,
                        size: 1);
                }
                else
                {
                    int height = this.upperWindow.GetHeight();
                    if (height != 1)
                    {
                        this.upperWindow.SetHeight(1);
                        this.machineStatusHeight = 1;
                    }
                }

                this.upperWindow.Clear();

                var charWidth = ScreenWidthInColumns;
                var locationObject = this.gameService.Machine.ReadGlobalVariable(0);
                var locationText = " " + this.gameService.Machine.ReadObjectShortName(locationObject);

                this.upperWindow.SetReverse(true);

                if (charWidth < 5)
                {
                    this.upperWindow.PutText(new string(' ', charWidth), forceFixedWidthFont: false);
                    return;
                }

                if (locationText.Length > charWidth)
                {
                    locationText = locationText.Substring(0, charWidth - 3) + "...";
                    this.upperWindow.PutText(locationText, forceFixedWidthFont: false);
                    return;
                }

                this.upperWindow.PutText(locationText, forceFixedWidthFont: false);

                string rightText;
                if (this.gameService.Machine.IsScoreGame())
                {
                    int score = (short)this.gameService.Machine.ReadGlobalVariable(1);
                    int moves = this.gameService.Machine.ReadGlobalVariable(2);
                    rightText = string.Format("Score: {0,-8} Moves: {1,-6} ", score, moves);
                }
                else
                {
                    int hours = this.gameService.Machine.ReadGlobalVariable(1);
                    int minutes = this.gameService.Machine.ReadGlobalVariable(2);
                    var pm = (hours / 12) > 0;
                    if (pm)
                    {
                        hours = hours % 12;
                    }

                    rightText = string.Format("{0}:{1:n2} {2}", hours, minutes, (pm ? "pm" : "am"));
                }

                if (rightText.Length < charWidth - locationText.Length - 1)
                {
                    this.upperWindow.PutText(
                        new string(' ', charWidth - locationText.Length - rightText.Length),
                        forceFixedWidthFont: false);

                    this.upperWindow.PutText(rightText, forceFixedWidthFont: false);
                }
            });
        }

        public Task<int> GetCursorColumnAsync()
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
                this.windowManager.ActiveWindow.GetCursorColumn());
        }

        public Task<int> GetCursorLineAsync()
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
                this.windowManager.ActiveWindow.GetCursorLine());
        }

        public Task SetCursorAsync(int line, int column)
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
            {
                this.windowManager.ActiveWindow.SetCursorAsync(line, column);
            });
        }

        public Task SetTextStyleAsync(ZTextStyle style)
        {
            return this.foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
            {
                var window = this.windowManager.ActiveWindow;

                switch (style)
                {
                    case ZTextStyle.Roman:
                        window.SetBold(false);
                        window.SetItalic(false);
                        window.SetFixedPitch(false);
                        window.SetReverse(false);
                        break;
                    case ZTextStyle.Bold:
                        window.SetBold(true);
                        break;
                    case ZTextStyle.Italic:
                        window.SetItalic(true);
                        break;
                    case ZTextStyle.FixedPitch:
                        window.SetFixedPitch(true);
                        break;
                    case ZTextStyle.Reverse:
                        window.SetReverse(true);
                        break;
                }
            });
        }

        public Task SetForegroundColorAsync(ZColor color)
        {
            return foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
                this.fontAndColorService.SetForegroundColor(color));
        }

        public Task SetBackgroundColorAsync(ZColor color)
        {
            return foregroundThreadAffinitedObject.InvokeBelowInputPriority(() =>
                this.fontAndColorService.SetBackgroundColor(color));
        }

        private FormattedText GetFixedFontMeasureText()
        {
            return new FormattedText(
                textToFormat: "0",
                culture: CultureInfo.InstalledUICulture,
                flowDirection: FlowDirection.LeftToRight,
                typeface: new Typeface(new FontFamily("Consolas"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                emSize: 16.0,
                foreground: Brushes.Black);
        }

        public byte FontHeightInUnits
        {
            get { return (byte)GetFixedFontMeasureText().Height; }
        }

        public byte FontWidthInUnits
        {
            get { return (byte)GetFixedFontMeasureText().Width; }
        }

        public byte ScreenHeightInLines
        {
            get { return (byte)(this.windowContainer.ActualHeight / GetFixedFontMeasureText().Height); }
        }

        public ushort ScreenHeightInUnits
        {
            get { return (ushort)this.windowContainer.ActualHeight; }
        }

        public byte ScreenWidthInColumns
        {
            get { return (byte)(this.windowContainer.ActualWidth / GetFixedFontMeasureText().Width); }
        }

        public ushort ScreenWidthInUnits
        {
            get { return (ushort)this.windowContainer.ActualWidth; }
        }
    }
}
