using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NZag.Extensions
{
    public static class InputElementExtensions
    {
        public static Task<TextCompositionEventArgs> TextInputAsync(this IInputElement element)
        {
            var tcs = new TaskCompletionSource<TextCompositionEventArgs>();

            TextCompositionEventHandler handler = null;
            handler = (sender, args) =>
            {
                element.TextInput -= handler;
                tcs.TrySetResult(args);
            };
            element.TextInput += handler;

            return tcs.Task;
        }

        public static Task<KeyEventArgs> KeyUpAsync(this IInputElement element)
        {
            var tcs = new TaskCompletionSource<KeyEventArgs>();

            KeyEventHandler handler = null;
            handler = (sender, args) =>
            {
                element.KeyUp -= handler;
                tcs.TrySetResult(args);
            };
            element.KeyUp += handler;

            return tcs.Task;
        }
    }
}
