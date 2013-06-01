using System;
using System.Windows.Threading;

namespace NZag.Extensions
{
    public static class DispatcherObjectExtensions
    {
        public static void PostAction(
            this DispatcherObject obj,
            Action action,
            DispatcherPriority priority = DispatcherPriority.Input)
        {
            obj.Dispatcher.BeginInvoke(priority, action);
        }
    }
}
