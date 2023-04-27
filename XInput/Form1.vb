'XInput
'
'This is an example application that demonstrates how to use Xbox controllers
'in VB.NET. It was written in 2023 and works on Windows 10 and 11.
'I’m currently working on a video that explains the code in more detail on
'my YouTube channel at https://www.youtube.com/@codewithjoe6074.
'
'MIT License
'Copyright(c) 2023 Joseph W. Lumbley
'
'Permission Is hereby granted, free Of charge, to any person obtaining a copy
'of this software And associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, And/Or sell
'copies of the Software, And to permit persons to whom the Software Is
'furnished to do so, subject to the following conditions:

'The above copyright notice And this permission notice shall be included In all
'copies Or substantial portions of the Software.

'THE SOFTWARE Is PROVIDED "AS IS", WITHOUT WARRANTY Of ANY KIND, EXPRESS Or
'IMPLIED, INCLUDING BUT Not LIMITED To THE WARRANTIES Of MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE And NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS Or COPYRIGHT HOLDERS BE LIABLE For ANY CLAIM, DAMAGES Or OTHER
'LIABILITY, WHETHER In AN ACTION Of CONTRACT, TORT Or OTHERWISE, ARISING FROM,
'OUT OF Or IN CONNECTION WITH THE SOFTWARE Or THE USE Or OTHER DEALINGS IN THE
'SOFTWARE.
'
'Monica was our ChatGPT copilot on this app.
'https://monica.im/

Imports System.Runtime.InteropServices
Imports XInput.XInput

Public Class Form1

    'The start of the thumbstick neutral zone.
    Private Const NeutralStart = -16256

    'The end of the thumbstick neutral zone.
    Private Const NeutralEnd = 16256

    Private ReadOnly Connected(0 To 3) As Boolean

    Dim ControllerNumber As Integer = 0

    Dim ControllerPosition As XINPUT_STATE

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Text = "XInput - Code with Joe"

        Timer1.Interval = 15 'Polling frequency in milliseconds.

        Timer1.Start()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        UpdateControllerPosition()

    End Sub

    Private Sub UpdateControllerPosition()

        For ControllerNumber = 0 To 3 'Up to 4 controllers

            Try

                ' Check if the function call was successful
                If XInputGetState(ControllerNumber, ControllerPosition) = 0 Then
                    ' The function call was successful, so you can access the controller state now

                    UpdateButtonPosition()

                    UpdateLeftThumbstickPosition()

                    UpdateRightThumbstickPosition()

                    UpdateLeftTriggerPosition()

                    UpdateRightTriggerPosition()

                    Connected(ControllerNumber) = True

                Else
                    ' The function call failed, so you cannot access the controller state

                    'Text = "Failed to get controller state. Error code: " & XInputGetState(ControllerNumber, ControllerPosition).ToString

                    Connected(ControllerNumber) = False

                End If

            Catch ex As Exception

                MsgBox(ex.ToString)

                Exit Sub

            End Try

        Next

    End Sub

    Private Sub UpdateButtonPosition()
        'The range of buttons is 0 to 65,535. Unsigned 16-bit (2-byte) integer.

        'What buttons are down?
        Select Case ControllerPosition.Gamepad.wButtons
            Case 0 'All the buttons are up.
                LabelButtons.Text = ""
            Case 1 'DPad up is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Up"
            Case 2 'DPad down is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Down"
            Case 4 'DPad left is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Left"
            Case 5 'DPad up and left is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Up Left"
            Case 6 'DPad down left is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Down Left"
            Case 8 'DPad right is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Right"
            Case 9 'DPad up right is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Up Right"
            Case 10 'DPad down right is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Down Right"
            Case 16 'Start is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Start"
            Case 32 'Back is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Back"
            Case 64 'Left stick is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Left Stick"
            Case 128 'Right stick is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Right Stick"
            Case 256 'Left bumper is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Left Bumper"
            Case 512 'Right bumper is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Right Bumper"
            Case 4096 'A is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: A"
            Case 8192 'B is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: B"
            Case 16384 'X is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: X"
            Case 32768 'Y is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Y"
            Case 48 'Back and start are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Back+Start"
            Case 192 'Left and right sticks are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Left and Right Stick"
            Case 768 'Left and right bumpers are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Left and Right Bumper"
            Case 12288 'A and b are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+B"
            Case 20480 'A and x are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+X"
            Case 36864 'A and y are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+Y"
            Case 24576 'B and x are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: B+X"
            Case 40960 'B and y are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: B+Y"
            Case 49152 'X and y are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: X+Y"
        End Select

    End Sub

    Private Sub UpdateLeftThumbstickPosition()
        'The range on the X-axis is -32768 to 32512.
        'The range on the Y-axis is -32768 to 32512.

        'What position is the left thumbstick in on the X-axis?
        If ControllerPosition.Gamepad.sThumbLX <= NeutralStart Then
            'The left thumbstick is in the left position.

            LabelLeftThumbX.Text = "Controller: " & ControllerNumber.ToString & " Left Thumbstick: Left"
            'Timer2.Start()


        ElseIf ControllerPosition.Gamepad.sThumbLX >= NeutralEnd Then
            'The left thumbstick is in the right position.

            LabelLeftThumbX.Text = "Controller: " & ControllerNumber.ToString & " Left Thumbstick: Right"
            'Timer2.Start()

        Else
            'The left thumbstick is in the neutral position.

            LabelLeftThumbX.Text = ""

        End If

        'What position is the left thumbstick in on the Y-axis?
        If ControllerPosition.Gamepad.sThumbLY <= NeutralStart Then
            'The left thumbstick is in the up position.

            LabelLeftThumbY.Text = "Controller: " & ControllerNumber.ToString & " Left Thumbstick: Down"
            'Timer2.Start()

        ElseIf ControllerPosition.Gamepad.sThumbLY >= NeutralEnd Then
            'The left thumbstick is in the down position.

            LabelLeftThumbY.Text = "Controller: " & ControllerNumber.ToString & " Left Thumbstick: Up"
            'Timer2.Start()

        Else
            'The left thumbstick is in the neutral position.

            LabelLeftThumbY.Text = ""

        End If

    End Sub

    Private Sub UpdateRightThumbstickPosition()
        'The range on the X-axis is -32768 to 32512.
        'The range on the Y-axis is -32768 to 32512.

        'What position is the right thumbstick in on the X-axis?
        If ControllerPosition.Gamepad.sThumbRX <= NeutralStart Then
            'The right thumbstick is in the left position.

            LabelRightThumbX.Text = "Controller: " & ControllerNumber.ToString & " Right Thumbstick: Left"
            'Timer2.Start()


        ElseIf ControllerPosition.Gamepad.sThumbRX >= NeutralEnd Then
            'The right thumbstick is in the right position.

            LabelRightThumbX.Text = "Controller: " & ControllerNumber.ToString & " Right Thumbstick: Right"
            'Timer2.Start()

        Else
            'The right thumbstick is in the neutral position.

            LabelRightThumbX.Text = ""

        End If

        'What position is the right thumbstick in on the Y-axis?
        If ControllerPosition.Gamepad.sThumbRY <= NeutralStart Then
            'The right thumbstick is in the up position.

            LabelRightThumbY.Text = "Controller: " & ControllerNumber.ToString & " Right Thumbstick: Down"
            'Timer2.Start()

        ElseIf ControllerPosition.Gamepad.sThumbRY >= NeutralEnd Then
            'The right thumbstick is in the down position.

            LabelRightThumbY.Text = "Controller: " & ControllerNumber.ToString & " Right Thumbstick: Up"
            'Timer2.Start()

        Else
            'The right thumbstick is in the neutral position.

            LabelRightThumbY.Text = ""

        End If

    End Sub

    Private Sub UpdateRightTriggerPosition()
        'The range of trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.

        If ControllerPosition.Gamepad.bRightTrigger > 0 Then

            LabelRightTrigger.Text = "Controller: " & ControllerNumber.ToString & " Right Trigger: Down"

        Else

            LabelRightTrigger.Text = ""

        End If

    End Sub

    Private Sub UpdateLeftTriggerPosition()
        'The range of trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.

        If ControllerPosition.Gamepad.bLeftTrigger > 0 Then

            LabelLeftTrigger.Text = "Controller: " & ControllerNumber.ToString & " Left Trigger: Down"

        Else

            LabelLeftTrigger.Text = ""

        End If

    End Sub

End Class

Public Class XInput
    ' Define the function signature for the XInputGetState function
    <DllImport("XInput9_1_0.dll")>
    Public Shared Function XInputGetState(
        dwUserIndex As Integer,
        ByRef pState As XINPUT_STATE
    ) As Integer
    End Function

    ' Define the XINPUT_STATE structure
    <StructLayout(LayoutKind.Explicit)>
    Public Structure XINPUT_STATE
        <FieldOffset(0)>
        Public dwPacketNumber As UInteger
        <FieldOffset(4)>
        Public Gamepad As XINPUT_GAMEPAD
    End Structure

    ' Define the XINPUT_GAMEPAD structure
    <StructLayout(LayoutKind.Sequential)>
    Public Structure XINPUT_GAMEPAD
        Public wButtons As UShort 'Unsigned 16-bit (2-byte) integer range 0 through 65,535.
        Public bLeftTrigger As Byte 'Unsigned 8-bit (1-byte) integer range 0 through 255.
        Public bRightTrigger As Byte 'Unsigned 8-bit (1-byte) integer range 0 through 255.
        Public sThumbLX As Short 'Signed 16-bit (2-byte) integer range -32,768 through 32,767.
        Public sThumbLY As Short
        Public sThumbRX As Short
        Public sThumbRY As Short
    End Structure
End Class

'Learn more:
'
'Consuming Unmanaged DLL Functions
'https://learn.microsoft.com/en-us/dotnet/framework/interop/consuming-unmanaged-dll-functions
'
'Passing Structures
'https://learn.microsoft.com/en-us/dotnet/framework/interop/passing-structures
'
'XInputGetState Function
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputgetstate
'
'XINPUT_STATE Structure
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_state
'
'XINPUT_GAMEPAD Structure
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_gamepad
'
'XInput Game Controller APIs
'https://learn.microsoft.com/en-us/windows/win32/api/_xinput/
'