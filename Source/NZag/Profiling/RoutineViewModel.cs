using System;
using SimpleMVVM;

namespace NZag.Profiling
{
    public class RoutineViewModel : ViewModelBase
    {
        public int Address { get; private set; }
        public TimeSpan InitialCompileTime { get; private set; }
        public int ILByteSize { get; private set; }

        public bool IsOptimized { get; private set; }
        public TimeSpan OptimizedCompileTime { get; private set; }
        public int OptimizedILByteSize { get; private set; }

        public int InvocationCount { get; private set; }

        public int LocalCount { get; private set; }
        public int InstructionCount { get; private set; }

        public RoutineViewModel(int address, TimeSpan compileTime, int ilByteSize, int localCount, int instructionCount)
        {
            this.Address = address;
            this.InitialCompileTime = compileTime;
            this.LocalCount = localCount;
            this.InstructionCount = instructionCount;
            this.ILByteSize = ilByteSize;
        }

        public void IncrementInvocationCount()
        {
            this.InvocationCount += 1;
        }

        public void Recompiled(TimeSpan compileTime, int ilByteSize)
        {
            this.IsOptimized = true;
            this.OptimizedCompileTime = compileTime;
            this.OptimizedILByteSize = ilByteSize;
        }

        public double OptimizedILByteSizePercentage
        {
            get { return ((double)this.OptimizedILByteSize / (double)this.ILByteSize) * 100; }
        }

        public string OptimizedILByteSizeDisplay
        {
            get { return string.Format("{0:#,0} ({1:0.00}%)", this.OptimizedILByteSize, this.OptimizedILByteSizePercentage); }
        }
    }
}
