using System.Windows;
using System.Windows.Controls;

namespace NZag.Windows
{
    internal class ZPairWindow : ZWindow
    {
        private ZWindow child1;
        private ZWindow child2;

        public ZPairWindow(ZWindowManager manager, ZWindow child1, ZWindow child2, ZWindowPosition child2Position, GridLength child2Size)
            : base(manager)
        {
            this.child1 = child1;
            this.child2 = child2;

            switch (child2Position)
            {
                case ZWindowPosition.Left:
                    this.ColumnDefinitions.Add(new ColumnDefinition { Width = child2Size });
                    this.ColumnDefinitions.Add(new ColumnDefinition());
                    SetColumn(this.child1, 1);
                    SetColumn(this.child2, 0);
                    break;
                case ZWindowPosition.Right:
                    this.ColumnDefinitions.Add(new ColumnDefinition());
                    this.ColumnDefinitions.Add(new ColumnDefinition { Width = child2Size });
                    SetColumn(this.child1, 0);
                    SetColumn(this.child2, 1);
                    break;
                case ZWindowPosition.Above:
                    this.RowDefinitions.Add(new RowDefinition { Height = child2Size });
                    this.RowDefinitions.Add(new RowDefinition());
                    SetRow(this.child1, 1);
                    SetRow(this.child2, 0);
                    break;
                case ZWindowPosition.Below:
                    this.RowDefinitions.Add(new RowDefinition());
                    this.RowDefinitions.Add(new RowDefinition { Height = child2Size });
                    SetRow(this.child1, 0);
                    SetRow(this.child2, 1);
                    break;
            }

            child1.SetParentWindow(this);
            child2.SetParentWindow(this);

            this.Children.Add(child1);
            this.Children.Add(child2);
        }

        public void Replace(ZWindow child, ZWindow newChild)
        {
            if (this.child1.Equals(child))
            {
                this.child1 = newChild;
                this.child1.SetParentWindow(null);
                this.Children[0] = newChild;
                newChild.SetParentWindow(this);
            }
            else if (this.child2.Equals(child))
            {
                this.child2 = newChild;
                this.child2.SetParentWindow(null);
                this.Children[0] = newChild;
                newChild.SetParentWindow(this);
            }
        }

        public ZWindow Child1
        {
            get { return this.child1; }
        }

        public ZWindow Child2
        {
            get { return this.child2; }
        }
    }
}
