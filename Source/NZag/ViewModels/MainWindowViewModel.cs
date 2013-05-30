using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using NZag.Core;
using NZag.Services;
using SimpleMVVM;

namespace NZag.ViewModels
{
    [Export]
    public class MainWindowViewModel : ViewModelBase<Window>
    {
        private readonly GameService gameService;
        private readonly ScreenViewModel screenViewModel;

        private Machine machine;

        [ImportingConstructor]
        private MainWindowViewModel(GameService gameService, ScreenViewModel screenViewModel)
            : base("Views/MainWindowView")
        {
            this.gameService = gameService;
            this.screenViewModel = screenViewModel;

            this.gameService.GameOpened += OnGameOpened;
        }

        public string Title
        {
            get { return "NZag"; }
        }

        protected override void OnViewCreated(Window view)
        {
            var screenContent = view.FindName<Grid>("ScreenContent");
            screenContent.Children.Add(screenViewModel.CreateView());

            this.OpenCommand = RegisterCommand("Open", "Open", OpenExecuted, () => true, new KeyGesture(Key.O, ModifierKeys.Control));
        }

        private void OnGameOpened(object sender, EventArgs e)
        {
            this.gameService.StartGame(this.screenViewModel);
        }

        private void OpenExecuted()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Open Z-Machine File"
            };

            if (dialog.ShowDialog() == true)
            {
                this.gameService.OpenGame(dialog.FileName);
            }
        }

        public ICommand OpenCommand { get; private set; }
    }
}
