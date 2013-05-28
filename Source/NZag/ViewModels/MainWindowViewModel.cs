using System.ComponentModel.Composition;
using System.Windows;
using SimpleMVVM;

namespace NZag.ViewModels
{
    [Export]
    public class MainWindowViewModel : ViewModelBase<Window>
    {
        [ImportingConstructor]
        private MainWindowViewModel()
            : base("Views/MainWindowView")
        {

        }

        public string Title
        {
            get { return "NZag"; }
        }
    }
}
