using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Controls;
using NZag.Core;
using NZag.Extensions;
using NZag.Profiling;
using SimpleMVVM;
using SimpleMVVM.Collections;

namespace NZag.ViewModels
{
    [Export]
    public class ProfilerViewModel : ViewModelBase<UserControl>, IProfiler
    {
        private readonly SortedList<int, RoutineViewModel> routineList;

        private readonly BulkObservableCollection<RoutineViewModel> routines;
        private readonly ReadOnlyBulkObservableCollection<RoutineViewModel> readOnlyRoutines;

        private bool refreshingData;
        private object gate = new object();

        [ImportingConstructor]
        private ProfilerViewModel()
            : base("Views/ProfilerView")
        {
            this.routineList = new SortedList<int, RoutineViewModel>();
            this.routines = new BulkObservableCollection<RoutineViewModel>();
            this.readOnlyRoutines = routines.AsReadOnly();
        }

        public void RoutineCompiled(Routine routine, TimeSpan compileTime, int ilByteSize, bool optimized)
        {
            var address = routine.Address;

            RoutineViewModel routineData;
            if (!this.routineList.TryGetValue(address, out routineData))
            {
                Debug.Assert(!optimized);
                routineData = new RoutineViewModel(address, compileTime, ilByteSize, routine.Locals.Length, routine.Instructions.Length);
                lock (gate)
                {
                    this.routineList.Add(address, routineData);
                }
            }
            else
            {
                Debug.Assert(optimized);
                routineData.Recompiled(compileTime, ilByteSize);
            }
        }

        public void EnterRoutine(Routine routine)
        {
            var address = routine.Address;
            var routineData = this.routineList[address];
            routineData.IncrementInvocationCount();

            if (!refreshingData)
            {
                this.View.PostAction(RefreshData);
                refreshingData = true;
            }
        }

        public void ExitRoutine(Routine routine)
        {
        }

        private void RefreshData()
        {
            lock (gate)
            {
                var routinesCopy = this.routines;

                routinesCopy.BeginBulkOperation();
                try
                {
                    routinesCopy.Clear();
                    foreach (var pair in this.routineList)
                    {
                        routinesCopy.Add(pair.Value);
                    }
                }
                finally
                {
                    routinesCopy.EndBulkOperation();
                }

                refreshingData = false;
            }
        }

        public ReadOnlyBulkObservableCollection<RoutineViewModel> Routines
        {
            get { return this.readOnlyRoutines; }
        }
    }
}
