using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

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

        public ZWindow OpenWindow(
            ZWindowKind kind,
            ZWindow splitWindow = null,
            ZWindowPosition position = ZWindowPosition.Left,
            ZWindowSizeKind sizeKind = ZWindowSizeKind.Fixed,
            int size = 0)
        {
            if (kind == ZWindowKind.Pair)
            {
                throw new InvalidOperationException("ZPairWindows can't be creatted directly");
            }

            if (rootWindow == null && splitWindow != null)
            {
                throw new InvalidOperationException("Cannot open a split window if the root window has not yet been created.");
            }

            if (rootWindow != null && splitWindow == null)
            {
                throw new InvalidOperationException("Cannot open a new root window if the root window has already bee created.");
            }

            var newWindow = CreateNewWindow(kind);

            if (rootWindow == null)
            {
                rootWindow = newWindow;
            }
            else
            {
                GridLength splitSize;
                switch (sizeKind)
                {
                    case ZWindowSizeKind.Fixed:
                        var pixels = IsVertical(position)
                            ? size * newWindow.RowHeight
                            : size * newWindow.ColumnWidth;
                        splitSize = new GridLength(pixels, GridUnitType.Pixel);
                        break;
                    case ZWindowSizeKind.Proportional:
                        splitSize = new GridLength(size / 100.0, GridUnitType.Star);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid size kind: " + sizeKind.ToString());
                }

                Debug.Assert(splitWindow != null, "splitWindow != null");

                var parentGrid = (Grid)splitWindow.Parent;
                parentGrid.Children.Remove(splitWindow);

                var oldParentWindow = splitWindow.ParentWindow as ZPairWindow;
                var newParentWindow = new ZPairWindow(this, splitWindow, newWindow, position, splitSize);

                if (oldParentWindow != null)
                {
                    oldParentWindow.Replace(splitWindow, newParentWindow);
                }
                else
                {
                    rootWindow = newParentWindow;
                }

                parentGrid.Children.Add(newParentWindow);
            }

            return newWindow;
        }

        private bool IsVertical(ZWindowPosition position)
        {
            switch (position)
            {
                case ZWindowPosition.Above:
                case ZWindowPosition.Below:
                    return true;
                case ZWindowPosition.Left:
                case ZWindowPosition.Right:
                    return false;
                default:
                    throw new InvalidOperationException("Invalid window position: " + position.ToString());
            }
        }

        private ZWindow CreateNewWindow(ZWindowKind kind)
        {
            switch (kind)
            {
                case ZWindowKind.Blank:
                    return new ZBlankWindow(this);
                case ZWindowKind.TextBuffer:
                    return new ZTextBufferWindow(this);
                case ZWindowKind.TextGrid:
                    return new ZTextGridWindow(this);
                default:
                    throw new InvalidOperationException("Invalid window kind: " + kind.ToString());
            }
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
