
Imports NZag.Utilities

Public Module GameTests

    Class Screen
        Implements IScreen

        Private builder As New Text.StringBuilder

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

        Public ReadOnly Property Output As String
            Get
                Return builder.ToString()
            End Get
        End Property
    End Class

    <Fact()>
    Sub RunCZech()
        Dim expected =
<![CDATA[
CZECH: the Comprehensive Z-machine Emulation CHecker, version 0.8
Test numbers appear in [brackets].

print works or you wouldn't be seeing this.

Jumps [2]: jump.je..........jg.....
bad [17]!
Quitting tests because jumps don't work!
]]>

        Test(CZech, expected)
    End Sub

    Private Sub Test(gameName As String, expected As XCData)
        Dim memory = GameMemory(gameName)
        Dim machine = New Machine(memory)
        Dim screen = New Screen()
        machine.RegisterScreen(screen)
        Try
            machine.Run()
        Catch ex As Exceptions.ZMachineQuitException
        End Try

        Dim expectedText = expected.Value.Trim()

        Assert.Equal(expectedText, screen.Output)
    End Sub

End Module
