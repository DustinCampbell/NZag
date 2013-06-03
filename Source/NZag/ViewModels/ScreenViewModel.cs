using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        private string[] script;
        private int scriptIndex = -1;

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

        public void LoadScript(string fileName)
        {
            this.script = File.ReadAllLines(fileName);
            this.scriptIndex = 0;
        }

        public Task<char> ReadCharAsync()
        {
            return this.windowManager.ActiveWindow.ReadCharAsync();
        }

        public Task<string> ReadTextAsync(int maxChars)
        {
            if (this.scriptIndex >= 0 && this.scriptIndex < this.script.Length)
            {
                var command = this.script[this.scriptIndex];
                this.scriptIndex++;
                this.windowManager.ActiveWindow.PutTextAsync(command + "\r\n", forceFixedWidthFont: false);
                return Task.FromResult(command);
            }

            return this.windowManager.ActiveWindow.ReadTextAsync(maxChars);
        }

        public Task WriteCharAsync(char value)
        {
            return this.windowManager.ActiveWindow.PutCharAsync(value, false);
        }

        public Task WriteTextAsync(string value)
        {
            return this.windowManager.ActiveWindow.PutTextAsync(value, false);
        }
    }
}
