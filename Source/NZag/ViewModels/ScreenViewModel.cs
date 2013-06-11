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

namespace NZag.ViewModels
{
    [Export]
    public class ScreenViewModel : ViewModelBase<UserControl>, IScreen
    {
        private readonly GameService gameService;
        private readonly ZWindowManager windowManager;
        private Grid windowContainer;
        private ZWindow mainWindow;
        private ZWindow upperWindow;

        private int currentStatusHeight;
        private int machineStatusHeight;

        [ImportingConstructor]
        private ScreenViewModel(GameService gameService)
            : base("Views/ScreenView")
        {
            this.gameService = gameService;
            this.windowManager = new ZWindowManager();

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

        private async Task ResetStatusHeightAsync()
        {
            if (this.upperWindow != null)
            {
                int height = await this.upperWindow.GetHeightAsync();
                if (this.machineStatusHeight != height)
                {
                    await this.upperWindow.SetHeightAsync(machineStatusHeight);
                }
            }
        }

        public async Task<char> ReadCharAsync()
        {
            var ch = await this.windowManager.ActiveWindow.ReadCharAsync();

            await ResetStatusHeightAsync();
            this.currentStatusHeight = 0;

            return ch;
        }

        public async Task<string> ReadTextAsync(int maxChars)
        {
            string command;

            if (this.gameService.HasNextScriptCommand)
            {
                command = this.gameService.GetNextScriptCommand();
                var forceFixedWidthFont = this.gameService.Machine.ForceFixedWidthFont();
                await this.windowManager.ActiveWindow.PutTextAsync(command + "\r\n", forceFixedWidthFont);
            }
            else
            {
                command = await this.windowManager.ActiveWindow.ReadTextAsync(maxChars);
            }

            await ResetStatusHeightAsync();
            this.currentStatusHeight = 0;

            return command;
        }

        public Task WriteCharAsync(char value)
        {
            var forceFixedWidthFont = this.gameService.Machine.ForceFixedWidthFont();
            return this.windowManager.ActiveWindow.PutCharAsync(value, forceFixedWidthFont);
        }

        public Task WriteTextAsync(string value)
        {
            var forceFixedWidthFont = this.gameService.Machine.ForceFixedWidthFont();
            return this.windowManager.ActiveWindow.PutTextAsync(value, forceFixedWidthFont);
        }

        public async Task ClearAsync(int window)
        {
            if (window == 0)
            {
                await this.mainWindow.ClearAsync();
            }
            else if (window == 1 && this.upperWindow != null)
            {
                await this.upperWindow.ClearAsync();
                await ResetStatusHeightAsync();
                this.currentStatusHeight = 0;
            }
        }

        public async Task ClearAllAsync(bool unsplit)
        {
            await this.mainWindow.ClearAsync();

            if (this.upperWindow != null)
            {
                if (unsplit)
                {
                    await this.UnsplitAsync();
                }
                else
                {
                    await this.upperWindow.ClearAsync();
                }
            }
        }

        public async Task SplitAsync(int lines)
        {
            if (this.upperWindow == null)
            {
                return;
            }

            if (lines == 0 || lines > this.currentStatusHeight)
            {
                int height = await this.upperWindow.GetHeightAsync();
                if (lines != height)
                {
                    await this.upperWindow.SetHeightAsync(lines);
                    this.currentStatusHeight = lines;
                }
            }

            this.machineStatusHeight = lines;

            if (this.gameService.Machine.Memory.Version == 0)
            {
                await this.upperWindow.ClearAsync();
            }
        }

        public async Task UnsplitAsync()
        {
            if (this.upperWindow == null)
            {
                return;
            }

            await this.upperWindow.SetHeightAsync(0);
            await this.upperWindow.ClearAsync();
            await ResetStatusHeightAsync();
            this.currentStatusHeight = 0;
        }

        public async Task SetWindowAsync(int window)
        {
            if (window == 0)
            {
                await this.mainWindow.ActivateAsync();
            }
            else if (window == 1)
            {
                await this.upperWindow.ActivateAsync();
            }
        }

        public async Task ShowStatusAsync()
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
                int height = await this.upperWindow.GetHeightAsync();
                if (height != 1)
                {
                    await this.upperWindow.SetHeightAsync(1);
                    this.machineStatusHeight = 1;
                }
            }

            await this.upperWindow.ClearAsync();

            var charWidth = ScreenWidthInColumns;
            var locationObject = this.gameService.Machine.ReadGlobalVariable(0);
            var locationText = " " + this.gameService.Machine.ReadObjectShortName(locationObject);

            await this.upperWindow.SetReverseAsync(true);

            if (charWidth < 5)
            {
                await this.upperWindow.PutTextAsync(new string(' ', charWidth), forceFixedWidthFont: false);
                return;
            }

            if (locationText.Length > charWidth)
            {
                locationText = locationText.Substring(0, charWidth - 3) + "...";
                await this.upperWindow.PutTextAsync(locationText, forceFixedWidthFont: false);
                return;
            }

            await this.upperWindow.PutTextAsync(locationText, forceFixedWidthFont: false);

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
                await this.upperWindow.PutTextAsync(
                    new string(' ', charWidth - locationText.Length - rightText.Length),
                    forceFixedWidthFont: false);

                await this.upperWindow.PutTextAsync(rightText, forceFixedWidthFont: false);
            }
        }

        public Task<int> GetCursorColumnAsync()
        {
            return this.windowManager.ActiveWindow.GetCursorColumnAsync();
        }

        public Task<int> GetCursorLineAsync()
        {
            return this.windowManager.ActiveWindow.GetCursorLineAsync();
        }

        public Task SetCursorAsync(int line, int column)
        {
            return this.windowManager.ActiveWindow.SetCursorAsync(line, column);
        }

        public async Task SetTextStyleAsync(ZTextStyle style)
        {
            var window = this.windowManager.ActiveWindow;

            switch (style)
            {
                case ZTextStyle.Roman:
                    await window.SetBoldAsync(false);
                    await window.SetItalicAsync(false);
                    await window.SetFixedPitchAsync(false);
                    await window.SetReverseAsync(false);
                    break;
                case ZTextStyle.Bold:
                    await window.SetBoldAsync(true);
                    break;
                case ZTextStyle.Italic:
                    await window.SetItalicAsync(true);
                    break;
                case ZTextStyle.FixedPitch:
                    await window.SetFixedPitchAsync(true);
                    break;
                case ZTextStyle.Reverse:
                    await window.SetReverseAsync(true);
                    break;
            }
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
