Imports System.Text

Namespace Mocks

    Class MockScreen
        Implements IScreen

        Private ReadOnly _builder As New StringBuilder
        Private ReadOnly _script As String()
        Private _scriptIndex As Integer

        Public Sub New(Optional script As XCData = Nothing)
            _script = If(script IsNot Nothing,
                         script.Value.Split({vbLf}, StringSplitOptions.RemoveEmptyEntries),
                         New String() {})
        End Sub

        Public Function ReadCharAsync() As Task(Of Char) Implements IInputStream.ReadCharAsync
            Return Task.FromResult(Chr(0))
        End Function

        Public Function ReadTextAsync(maxChars As Integer) As Task(Of String) Implements IInputStream.ReadTextAsync
            Dim command = _script(_scriptIndex)
            _scriptIndex += 1

            _builder.Append(command)
            _builder.Append(vbLf)

            Return Task.FromResult(command)
        End Function

        Public Function WriteCharAsync(ch As Char) As Task Implements IOutputStream.WriteCharAsync
            Return Task.Factory.StartNew(Sub()
                                             _builder.Append(ch)
                                         End Sub)
        End Function

        Public Function WriteTextAsync(s As String) As Task Implements IOutputStream.WriteTextAsync
            Return Task.Factory.StartNew(Sub()
                                             _builder.Append(s)
                                         End Sub)
        End Function

        Public Function ClearAsync(window As Integer) As Task Implements IScreen.ClearAsync
            Return Task.Factory.StartNew(Sub() Exit Sub)
        End Function

        Public Function ClearAllAsync(unsplit As Boolean) As Task Implements IScreen.ClearAllAsync
            Return Task.Factory.StartNew(Sub() Exit Sub)
        End Function

        Public Function SplitAsync(lines As Integer) As Task Implements IScreen.SplitAsync
            Return Task.Factory.StartNew(Sub() Exit Sub)
        End Function

        Public Function UnsplitAsync() As Task Implements IScreen.UnsplitAsync
            Return Task.Factory.StartNew(Sub() Exit Sub)
        End Function

        Public Function SetWindowAsync(window As Integer) As Task Implements IScreen.SetWindowAsync
            Return Task.Factory.StartNew(Sub() Exit Sub)
        End Function

        Public Function ShowStatusAsync() As Task Implements IScreen.ShowStatusAsync
            Return Task.Factory.StartNew(Sub() Exit Sub)
        End Function

        Public Function GetCursorColumnAsync() As Task(Of Integer) Implements IScreen.GetCursorColumnAsync
            Return Task.FromResult(0)
        End Function

        Public Function GetCursorLineAsync() As Task(Of Integer) Implements IScreen.GetCursorLineAsync
            Return Task.FromResult(0)
        End Function

        Public Function SetCursorAsync(line As Integer, column As Integer) As Task Implements IScreen.SetCursorAsync
            Return Task.Factory.StartNew(Sub() Exit Sub)
        End Function

        Public Function SetTextStyleAsync(style As ZTextStyle) As Task Implements IScreen.SetTextStyleAsync
            Return Task.Factory.StartNew(Sub() Exit Sub)
        End Function

        Public Function SetBackgroundColorAsync(color As ZColor) As Task Implements IScreen.SetBackgroundColorAsync
            Return Task.Factory.StartNew(Sub() Exit Sub)
        End Function

        Public Function SetForegroundColorAsync(color As ZColor) As Task Implements IScreen.SetForegroundColorAsync
            Return Task.Factory.StartNew(Sub() Exit Sub)
        End Function

        Public ReadOnly Property FontHeightInUnits As Byte Implements IScreen.FontHeightInUnits
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property FontWidthInUnits As Byte Implements IScreen.FontWidthInUnits
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property ScreenHeightInLines As Byte Implements IScreen.ScreenHeightInLines
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property ScreenHeightInUnits As UShort Implements IScreen.ScreenHeightInUnits
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property ScreenWidthInColumns As Byte Implements IScreen.ScreenWidthInColumns
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property ScreenWidthInUnits As UShort Implements IScreen.ScreenWidthInUnits
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property Output As String
            Get
                Return _builder.ToString()
            End Get
        End Property
    End Class
End Namespace