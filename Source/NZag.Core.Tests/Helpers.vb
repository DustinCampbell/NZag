Imports System.IO
Imports System.Reflection

Module Helpers

    Public Const Zork1 = "zork1.z3"

    Function GameMemory(name As String) As Memory
        Dim asm = Assembly.GetExecutingAssembly()
        Using stream = asm.GetManifestResourceStream(asm.GetName().Name + "." + Zork1)
            Return Memory.CreateFrom(stream)
        End Using
    End Function

    Function CreateMemory(version As Byte, bytes As Byte()) As Memory
        Dim size As UInteger = &H40 + bytes.Length

        Dim multiplier As Integer
        If version >= 1 AndAlso version <= 3 Then
            multiplier = 2
        ElseIf version >= 4 AndAlso version <= 5 Then
            multiplier = 4
        Else ' 6, 7, 8
            multiplier = 8
        End If

        Dim packedSize As UShort = size / multiplier
        If size Mod multiplier > 0 Then
            packedSize += 1
        End If

        Using stream = New MemoryStream(size)
            ' Write version
            stream.Seek(0, SeekOrigin.Begin)
            stream.WriteByte(version)

            ' Write size
            stream.Seek(&H1A, SeekOrigin.Begin)
            stream.WriteByte(packedSize >> 8)
            stream.WriteByte(packedSize And &HFF)

            ' Write data
            stream.Seek(&H40, SeekOrigin.Begin)
            stream.Write(bytes, 0, bytes.Length)

            Return Memory.CreateFrom(stream)
        End Using
    End Function

    Function CreateMemory(version As Byte, dataSize As Integer) As Memory
        Return CreateMemory(version, New Byte(dataSize) {})
    End Function

End Module
