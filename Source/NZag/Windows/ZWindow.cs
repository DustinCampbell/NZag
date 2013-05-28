using System.Windows.Controls;
using System.Windows.Media;

namespace NZag.Windows
{
    internal abstract class ZWindow : Grid
    {
        private readonly ZWindowManager manager;
        private ZWindow parentWindow;

        protected ZWindow(ZWindowManager manager)
        {
            this.manager = manager;

            UseLayoutRounding = true;
            SnapsToDevicePixels = true;

            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            TextOptions.SetTextRenderingMode(this, TextRenderingMode.Auto);
        }

        public ZWindow ParentWindow
        {
            get { return this.parentWindow; }
        }
    }
}
