using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Controls;
using NZag.Core;
using SimpleMVVM;

namespace NZag.ViewModels
{
    [Export]
    public class ProfilerViewModel : ViewModelBase<UserControl>, IProfiler
    {
        [ImportingConstructor]
        private ProfilerViewModel()
            : base("Views/ProfilerView")
        {
        }

        public void RoutineCompiled(Routine routine, TimeSpan compileTime)
        {
            Debug.WriteLine("Routine compiled: {0:x} - {1}", routine.Address.IntValue, compileTime);
        }
    }
}
