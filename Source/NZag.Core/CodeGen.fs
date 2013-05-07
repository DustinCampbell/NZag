namespace NZag.Core

type IMachine =

    abstract member GetInitialLocalArray : Routine -> uint16[]
    abstract member ReleaseLocalArray : uint16[] -> unit

    abstract member GetOrCompileFunction : Address -> ZFuncInvoker

and ZFuncInvoker(machine: IMachine, routine: Routine, zfunc: ZFunc) =

    let invoke locals =
        try
            ()
        finally
            machine.ReleaseLocalArray(locals)

    member x.Invoke0() =
        let locals = machine.GetInitialLocalArray(routine)
        invoke locals

    member x.Invoke1(arg1) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        invoke locals

    member x.Invoke2(arg1, arg2) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        invoke locals

    member x.Invoke3(arg1, arg2, arg3) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        invoke locals

    member x.Invoke4(arg1, arg2, arg3, arg4) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        locals.[3] <- arg4
        invoke locals

    member x.Invoke5(arg1, arg2, arg3, arg4, arg5) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        locals.[3] <- arg4
        locals.[4] <- arg5
        invoke locals

    member x.Invoke6(arg1, arg2, arg3, arg4, arg5, arg6) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        locals.[3] <- arg4
        locals.[4] <- arg5
        locals.[5] <- arg6
        invoke locals

    member x.Invoke7(arg1, arg2, arg3, arg4, arg5, arg6, arg7) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        locals.[3] <- arg4
        locals.[4] <- arg5
        locals.[5] <- arg6
        locals.[6] <- arg7
        invoke locals

and ZFunc = uint16[] -> uint16

type CodeGenerator(memory: Memory) =

    do ()

