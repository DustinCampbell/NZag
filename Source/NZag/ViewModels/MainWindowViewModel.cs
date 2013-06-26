using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using NZag.Services;
using SimpleMVVM;

namespace NZag.ViewModels
{
    [Export]
    public class MainWindowViewModel : ViewModelBase<Window>
    {
        private readonly GameService gameService;
        private readonly ScreenViewModel screenViewModel;
        private readonly ProfilerViewModel profilerViewModel;
        private bool profilingEnabled;

        [ImportingConstructor]
        private MainWindowViewModel(
            GameService gameService,
            ScreenViewModel screenViewModel,
            ProfilerViewModel profilerViewModel)
            : base("Views/MainWindowView")
        {
            this.gameService = gameService;
            this.screenViewModel = screenViewModel;
            this.profilerViewModel = profilerViewModel;

            this.gameService.GameOpened += OnGameOpened;
            this.gameService.ScriptLoaded += OnScriptLoaded;
        }

        public string Title
        {
            get
            {
                return this.gameService.IsGameOpen
                    ? "NZag - " + Path.GetFileName(this.gameService.GameFileName)
                    : "NZag";
            }
        }

        public string GameName
        {
            get
            {
                return this.gameService.IsGameOpen
                    ? Path.GetFileName(this.gameService.GameFileName)
                    : "None";
            }
        }

        public string ScriptName
        {
            get
            {
                return this.gameService.IsScriptOpen
                    ? Path.GetFileName(this.gameService.ScriptFileName)
                    : "None";
            }
        }

        protected override void OnViewCreated(Window view)
        {
            var screenContent = view.FindName<Grid>("ScreenContent");
            screenContent.Children.Add(screenViewModel.CreateView());

            var profilerContent = view.FindName<Grid>("ProfilerContent");
            profilerContent.Children.Add(profilerViewModel.CreateView());

            this.OpenGameCommand = RegisterCommand("Open", "Open", OpenGameExecuted, CanOpenGameExecute, new KeyGesture(Key.O, ModifierKeys.Control));
            this.LoadScriptCommand = RegisterCommand("Load Script...", "LoadScript", LoadScriptExecuted, CanLoadScriptExecute);
            this.ProfileCommand = RegisterCommand<bool>("Profile", "Profile", ProfileExecuted, CanProfileExecute);
            this.PlayGameCommand = RegisterCommand("Play", "Play", PlayGameExecuted, CanPlayGameExecute, new KeyGesture(Key.F5));
            this.ResetGameCommand = RegisterCommand("Reset", "Reset", ResetGameExecuted, CanResetGameExecute);
        }

        private void OnGameOpened(object sender, EventArgs e)
        {
            this.PropertyChanged("Title");
            this.PropertyChanged("GameName");
        }

        private void OnScriptLoaded(object sender, EventArgs e)
        {
            this.PropertyChanged("ScriptName");
        }

        private void StartGame()
        {
            if (this.profilingEnabled)
            {
                this.gameService.StartGame(this.screenViewModel, this.profilerViewModel);
            }
            else
            {
                this.gameService.StartGame(this.screenViewModel);
            }
        }

        private bool CanOpenGameExecute()
        {
            return true;
        }

        private void OpenGameExecuted()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Open Z-Machine File",
                Filter = "Story Files (*.z*)|*.z*"
            };

            if (dialog.ShowDialog() == true)
            {
                if (this.gameService.IsGameOpen)
                {
                    this.gameService.CloseGame();
                }

                this.gameService.OpenGame(dialog.FileName);
            }
        }

        private bool CanLoadScriptExecute()
        {
            return true;
        }

        private void LoadScriptExecuted()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Load Script File",
                Filter = "Script Files (*.script)|*.script"
            };

            if (dialog.ShowDialog() == true)
            {
                this.gameService.LoadScript(dialog.FileName);
            }
        }

        private bool CanProfileExecute(bool enabled)
        {
            return true;
        }

        private void ProfileExecuted(bool enabled)
        {
            this.profilingEnabled = enabled;
        }

        private bool CanPlayGameExecute()
        {
            return this.gameService.IsGameOpen;
        }

        private void PlayGameExecuted()
        {
            StartGame();
        }

        private bool CanResetGameExecute()
        {
            return this.gameService.IsGameOpen;
        }

        private void ResetGameExecuted()
        {
            var gameFileName = this.gameService.GameFileName;
            var scriptFileName = this.gameService.ScriptFileName;

            this.gameService.CloseGame();

            this.gameService.OpenGame(gameFileName);

            if (!string.IsNullOrWhiteSpace(scriptFileName))
            {
                this.gameService.LoadScript(scriptFileName);
            }

            StartGame();
        }

        public ICommand OpenGameCommand { get; private set; }
        public ICommand LoadScriptCommand { get; private set; }
        public ICommand ProfileCommand { get; private set; }
        public ICommand PlayGameCommand { get; private set; }
        public ICommand ResetGameCommand { get; private set; }
    }
}
