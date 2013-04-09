﻿Public Module TextTests

    <Fact>
    Sub InstructionText()
        Dim memory = GameMemory(Zork1)
        Dim reader = New ZTextReader(memory)
        Dim s = reader.ReadString(Address.NewRawAddress(&H4ED1))
        Assert.Equal("The grating is closed!", s)
    End Sub

    <Fact>
    Sub StaticText()
        Dim memory = GameMemory(Zork1)
        Dim reader = New ZTextReader(memory)
        Dim s = reader.ReadString(Address.NewRawAddress(&H1154A))
        Assert.Equal("There is a suspicious-looking individual, holding a large bag, leaning against one wall. He is armed with a deadly stiletto.", s)
    End Sub

End Module
