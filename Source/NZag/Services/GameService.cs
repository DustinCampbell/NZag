using System;
using System.ComponentModel.Composition;
using System.IO;
using NZag.Core;

namespace NZag.Services
{
    [Export]
    public class GameService
    {
        private Machine machine;

        private void OnGameOpening()
        {
            var handler = GameOpening;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnGameOpened()
        {
            var handler = GameOpened;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnGameClosing()
        {
            var handler = GameClosing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnGameClosed()
        {
            var handler = GameClosed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public void OpenGame(string fileName)
        {
            OnGameOpening();

            using (var file = File.OpenRead(fileName))
            {
                var memory = Memory.CreateFrom(file);
                this.machine = new Machine(memory, debugging: false);
            }

            OnGameOpened();
        }

        public void StartGame(IScreen screen)
        {
            this.machine.RegisterScreen(screen);
            this.machine.RunAsync();
        }

        public Machine Machine
        {
            get { return this.machine; }
        }

        public event EventHandler GameOpening;
        public event EventHandler GameOpened;
        public event EventHandler GameClosing;
        public event EventHandler GameClosed;
    }
}
