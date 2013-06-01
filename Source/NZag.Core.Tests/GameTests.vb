Imports NZag.Utilities

Public Module GameTests

    Class Screen
        Implements IScreen

        Private ReadOnly _builder As New Text.StringBuilder

        Public Function ReadCharAsync() As Task(Of Char) Implements IInputStream.ReadCharAsync
            Return Task.FromResult(Chr(0))
        End Function

        Public Function ReadTextAsync(maxChars As Integer) As Task(Of String) Implements IInputStream.ReadTextAsync
            Return Task.FromResult("quit")
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

        Public ReadOnly Property Output As String
            Get
                Return _builder.ToString()
            End Get
        End Property
    End Class

    <Fact()>
    Async Function RunCZech() As task
        Dim expected =
<![CDATA[
CZECH: the Comprehensive Z-machine Emulation CHecker, version 0.8
Test numbers appear in [brackets].

print works or you wouldn't be seeing this.

Jumps [2]: jump.je..........jg.......jl.......jz...offsets..
Variables [32]: push/pull..store.load.dec.......inc.......
    dec_chk...........inc_chk.........
Arithmetic ops [70]: add.......sub.......
    mul........div...........mod...........
Logical ops [114]: not....and.....or.....art_shift........log_shift........
Memory [144]: loadw.loadb..storeb..storew...
Subroutines [152]: call_1s.call_2s..call_vs2...call_vs.....ret.
    call_1n.call_2n..call_vn..call_vn2..
    rtrue.rfalse.ret_popped.
    Computed call...
    check_arg_count................
Objects [193]: get_parent....get_sibling.......get_child......jin.......
    test_attr......set_attr....clear_attr....set/clear/test_attr..
    get_next_prop......get_prop_len/get_prop_addr....
    get_prop..........put_prop ..........
    remove..insert.......
    Spec1.0 length-64 props...........
Indirect Opcodes [283]: load..................store.........................
    pull...............inc...............dec...............
    inc_chk...............dec_chk...............
Misc [401]: test...random.verify.piracy.
Header (No tests)
    standard 1.0 
    interpreter 6 A (IBM PC)
    Flags on: 
    Flags off: color, pictures, boldface, italic, fixed-space, sound, timer, transcripting on, fixed-pitch on, redraw pending, using pictures, using undo, using mouse, using colors, using sound, using menus, 
    Screen size: 0x0; in 0x0 units: 0x0
    Default color: current on current



Print opcodes [407]: Tests should look like... '[Test] opcode (stuff): stuff'
print_num (0, 1, -1, 32767,-32768, -1): 0, 1, -1, 32767, -32768, -1
[413] print_char (abcd): abcd
[417] new_line:

There should be an empty line above this line.
print_ret (should have newline after this).
print_addr (Hello.): Hello.

print_paddr (A long string that Inform will put in high memory):
A long string that Inform will put in high memory
Abbreviations (I love 'xyzzy' [two times]): I love 'xyzzy'  I love 'xyzzy'

[424] print_obj (Test Object #1Test Object #2): Test Object #1Test Object #2


Performed 425 tests.
Passed: 406, Failed: 0, Print tests: 19
Didn't crash: hooray!
Last test: quit!
]]>

        Await Test(CZech, expected)
    End Function

    <Fact()>
    Async Function RunZork1() As task
        Dim expected =
<![CDATA[
ZORK I: The Great Underground Empire
Copyright (c) 1981, 1982, 1983 Infocom, Inc. All rights reserved.
ZORK is a registered trademark of Infocom, Inc.
Revision 88 / Serial number 840726

West of House
You are standing in an open field west of a white house, with a boarded front door.
There is a small mailbox here.

>
]]>

        Await Test(Zork1, expected)
    End Function

    Private Async Function Test(gameName As String, expected As XCData) As task
        Dim memory = GameMemory(gameName)
        Dim machine = New Machine(memory, debugging:=False)
        Dim screen = New Screen()
        machine.RegisterScreen(screen)
        Try
            Await machine.RunAsync()
        Catch ex As Exceptions.ZMachineQuitException
        Catch
        End Try

        Dim expectedText = expected.Value.Trim()

        Assert.Equal(expectedText, screen.Output.Trim())
    End Function

End Module
