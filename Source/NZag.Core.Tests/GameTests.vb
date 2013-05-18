Imports System.Text

Public Module GameTests

    Class Screen
        Implements IScreen

        Private builder As New StringBuilder

        Public Function WriteCharAsync(ch As Char) As Task Implements IOutputStream.WriteCharAsync
            Return Task.Factory.StartNew(Sub()
                                             builder.Append(ch)
                                         End Sub)
        End Function

        Public Function WriteTextAsync(s As String) As Task Implements IOutputStream.WriteTextAsync
            Return Task.Factory.StartNew(Sub()
                                             builder.Append(s)
                                         End Sub)
        End Function
    End Class

    <Fact(Skip:="Work in progress")>
    Sub RunCZech()
        Dim memory = GameMemory(CZech)
        Dim machine = New Machine(memory)
        Dim screen = New Screen()
        machine.RegisterScreen(screen)

        Try
            machine.Run()
        Catch ex As Exception
            Throw
        End Try
    End Sub

End Module
