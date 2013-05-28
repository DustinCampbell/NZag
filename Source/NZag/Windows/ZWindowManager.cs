namespace NZag.Windows
{
    internal class ZWindowManager
    {
        private ZWindow rootWindow;
        private ZWindow activeWindow;

        public void ActivateWindow(ZWindow window)
        {
            this.activeWindow = window;
        }

        public ZWindow RootWindow
        {
            get { return this.rootWindow; }
        }

        public ZWindow ActiveWindow
        {
            get { return this.activeWindow; }
        }
    }
}
