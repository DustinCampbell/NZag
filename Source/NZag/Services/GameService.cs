using System;
using System.ComponentModel.Composition;
using System.IO;
using NZag.Core;

namespace NZag.Services
{
    [Export]
    public class GameService
    {
        private string gameFileName;
        private Machine machine;

        private string scriptFileName;
        private string[] script;
        private int scriptIndex = -1;

        private void OnGameOpened()
        {
            var handler = GameOpened;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnScriptLoaded()
        {
            var handler = ScriptLoaded;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public void LoadScript(string fileName)
        {
            this.script = File.ReadAllLines(fileName);
            this.scriptIndex = 0;
            this.scriptFileName = fileName;

            OnScriptLoaded();
        }

        public bool HasNextScriptCommand
        {
            get { return this.scriptIndex >= 0 && this.scriptIndex < this.script.Length; }
        }

        public string GetNextScriptCommand()
        {
            if (!HasNextScriptCommand)
            {
                return string.Empty;
            }

            var command = this.script[this.scriptIndex];
            this.scriptIndex++;
            return command;
        }

        public void OpenGame(string fileName)
        {
            using (var file = File.OpenRead(fileName))
            {
                var memory = Memory.CreateFrom(file);
                this.machine = new Machine(memory, debugging: true);
            }

            this.gameFileName = fileName;

            OnGameOpened();
        }

        public void StartGame(IScreen screen, IProfiler profiler = null)
        {
            if (profiler != null)
            {
                this.machine.RegisterProfiler(profiler);
            }

            this.machine.RegisterScreen(screen);
            this.machine.Randomize(42);
            this.machine.RunAsync();
        }

        public bool IsGameOpen
        {
            get { return this.machine != null; }
        }

        public Machine Machine
        {
            get { return this.machine; }
        }

        public string GameFileName
        {
            get { return this.gameFileName; }
        }

        public bool IsScriptOpen
        {
            get { return this.script != null; }
        }

        public string ScriptFileName
        {
            get { return this.scriptFileName; }
        }

        public event EventHandler GameOpened;
        public event EventHandler ScriptLoaded;
    }
}
