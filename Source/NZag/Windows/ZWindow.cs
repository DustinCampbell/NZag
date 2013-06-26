using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using NZag.Services;
using NZag.Utilities;
using SimpleMVVM.Threading;

namespace NZag.Windows
{
    internal abstract class ZWindow : Grid
    {
        protected readonly ZWindowManager Manager;
        private readonly FontAndColorService fontAndColorService;
        private readonly ForegroundThreadAffinitizedObject foregroundThreadAffinitizedObject;

        private ZPairWindow parentWindow;

        protected ZWindow(ZWindowManager manager, FontAndColorService fontAndColorService)
        {
            this.Manager = manager;
            this.fontAndColorService = fontAndColorService;
            this.foregroundThreadAffinitizedObject = new ForegroundThreadAffinitizedObject();

            UseLayoutRounding = true;
            SnapsToDevicePixels = true;

            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            TextOptions.SetTextRenderingMode(this, TextRenderingMode.Auto);
        }

        protected void AssertIsForeground()
        {
            this.foregroundThreadAffinitizedObject.AssertIsForeground();
        }

        public ZPairWindow ParentWindow
        {
            get { return this.parentWindow; }
        }

        protected Brush ForegroundBrush
        {
            get { return this.fontAndColorService.ForegroundBrush; }
        }

        protected Brush BackgroundBrush
        {
            get { return this.fontAndColorService.BackgroundBrush; }
        }

        protected double FontSize
        {
            get { return this.fontAndColorService.FontSize; }
        }

        public void SetParentWindow(ZPairWindow newParentWindow)
        {
            this.parentWindow = newParentWindow;
        }

        public void Activate()
        {
            this.Manager.ActivateWindow(this);
        }

        public virtual bool SetBold(bool value)
        {
            return false;
        }

        public virtual bool SetItalic(bool value)
        {
            return false;
        }

        public virtual bool SetFixedPitch(bool value)
        {
            return false;
        }

        public virtual bool SetReverse(bool value)
        {
            return false;
        }

        public virtual void Clear()
        {
            throw new Exceptions.RuntimeException("Window does not support clear operation.");
        }

        protected virtual Task<char> ReadCharCoreAsync()
        {
            throw new Exceptions.RuntimeException("Window does not support user input.");
        }

        public Task<char> ReadCharAsync()
        {
            return this.foregroundThreadAffinitizedObject.InvokeBelowInputPriority(() =>
            {
                return ReadCharCoreAsync();
            }).Unwrap();
        }

        protected virtual Task<string> ReadTextCoreAsync(int maxChars)
        {
            throw new Exceptions.RuntimeException("Window does not support user input.");
        }

        public Task<string> ReadTextAsync(int maxChars)
        {
            return this.foregroundThreadAffinitizedObject.InvokeBelowInputPriority(() =>
            {
                return ReadTextCoreAsync(maxChars);
            }).Unwrap();
        }

        public virtual void PutChar(char ch, bool forceFixedWidthFont)
        {
            throw new Exceptions.RuntimeException("Window does not support text display.");
        }

        public virtual void PutText(string text, bool forceFixedWidthFont)
        {
            throw new Exceptions.RuntimeException("Window does not support text display.");
        }

        public virtual int GetHeight()
        {
            return 0;
        }

        public virtual void SetHeight(int lines)
        {
            // Do nothing in base implementation.
        }

        public virtual int GetCursorColumn()
        {
            return 0;
        }

        public virtual int GetCursorLine()
        {
            return 0;
        }

        public virtual void SetCursorAsync(int line, int column)
        {
            // Do nothing in base implementation.
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
