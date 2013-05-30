using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using NZag.Utilities;

namespace NZag.Windows
{
    internal abstract class ZWindow : Grid
    {
        private readonly TaskScheduler scheduler;
        private readonly TaskFactory factory;

        protected readonly ZWindowManager Manager;
        private ZWindow parentWindow;

        protected ZWindow(ZWindowManager manager)
        {
            this.Manager = manager;

            this.scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            this.factory = new TaskFactory(scheduler);

            UseLayoutRounding = true;
            SnapsToDevicePixels = true;

            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            TextOptions.SetTextRenderingMode(this, TextRenderingMode.Auto);
        }

        protected Task RunOnUIThread(Action a)
        {
            return this.factory.StartNew(a);
        }

        protected Task RunOnUIThread(Func<Task> f)
        {
            return this.factory.StartNew(f).Unwrap();
        }

        protected Task<T> RunOnUIThread<T>(Func<T> f)
        {
            return this.factory.StartNew(f);
        }

        protected Task<T> RunOnUIThread<T>(Func<Task<T>> f)
        {
            return this.factory.StartNew(f).Unwrap();
        }

        public ZWindow ParentWindow
        {
            get { return this.parentWindow; }
        }

        public void SetParentWindow(ZWindow newParentWindow)
        {
            this.parentWindow = newParentWindow;
        }

        public virtual Task<bool> SetBoldAsync(bool value)
        {
            return Task.FromResult(false);
        }

        public virtual Task<bool> SetItalicAsync(bool value)
        {
            return Task.FromResult(false);
        }

        public virtual Task<bool> SetFixedPitchAsync(bool value)
        {
            return Task.FromResult(false);
        }

        public virtual Task<bool> SetReverseAsync(bool value)
        {
            return Task.FromResult(false);
        }

        public virtual Task ClearAsync()
        {
            throw new Exceptions.RuntimeException("Window does not support clear operation.");
        }

        public virtual Task<char> ReadCharAsync()
        {
            throw new Exceptions.RuntimeException("Window does not support user input.");
        }

        public virtual Task<string> ReadTextAsync(int maxChars)
        {
            throw new Exceptions.RuntimeException("Window does not support user input.");
        }

        public virtual Task PutCharAsync(char ch, bool forceFixedWidthFont)
        {
            throw new Exceptions.RuntimeException("Window does not support text display.");
        }

        public virtual Task PutTextAsync(string text, bool forceFixedWidthFont)
        {
            throw new Exceptions.RuntimeException("Window does not support text display.");
        }

        public virtual Task<int> GetHeightAsync()
        {
            return Task.FromResult(0);
        }

        public virtual Task SetHeightAsync(int lines)
        {
            return new Task(() => { });
        }

        public virtual Task<int> GetCursorColumnAsync()
        {
            return Task.FromResult(0);
        }

        public virtual Task<int> GetCursorLineAsync()
        {
            return Task.FromResult(0);
        }

        public virtual Task SetCursorAsync(int line, int column)
        {
            return new Task(() => { });
        }

        public virtual int RowHeight
        {
            get { return 0; }
        }

        public virtual int ColumnWidth
        {
            get { return 0; }
        }
    }
}
