using System;
using System.Windows;
using System.Windows.Media;

namespace NZag.Extensions
{
    public static class DependencyObjectExtensions
    {
        private static T FindFirstVisualChildAux<T>(DependencyObject obj, Func<T, bool> predicate)
            where T : DependencyObject
        {
            var childCount = VisualTreeHelper.GetChildrenCount(obj);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                var typedChild = child as T;
                if (typedChild != null)
                {
                    if (predicate(typedChild))
                    {
                        return typedChild;
                    }
                }
                else
                {
                    var foundChild = FindFirstVisualChildAux<T>(child, predicate);
                    if (foundChild != null)
                    {
                        return foundChild;
                    }
                }
            }

            return null;
        }

        public static T FindFirstVisualChild<T>(this DependencyObject obj, Func<T, bool> predicate = null) where T : DependencyObject
        {
            predicate = predicate ?? (_ => true);

            return FindFirstVisualChildAux(obj, predicate);
        }
    }
}
