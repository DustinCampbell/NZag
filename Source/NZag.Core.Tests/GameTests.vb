Public Module GameTests

    <Fact(Skip:="Work in progress")>
    Sub RunCZech()
        Dim memory = GameMemory(CZech)
        Dim machine = New Machine(memory)
        machine.Run()
    End Sub

End Module
