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
'Monica is our ChatGPT copilot.
'https://monica.im/

Imports System.Runtime.InteropServices

Public Class Form1

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputGetState(dwUserIndex As Integer, ByRef pState As XINPUT_STATE) As Integer
    End Function

    'XInput1_4.dll seems to be the current version
    'XInput9_1_0.dll is maintained primarily for backward compatibility. 

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
        Public sThumbLY As Short 'Signed 16-bit (2-byte) integer range -32,768 through 32,767.
        Public sThumbRX As Short 'Signed 16-bit (2-byte) integer range -32,768 through 32,767.
        Public sThumbRY As Short 'Signed 16-bit (2-byte) integer range -32,768 through 32,767.
    End Structure

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputSetState(playerIndex As Integer, ByRef vibration As XINPUT_VIBRATION) As Integer
    End Function

    Public Structure XINPUT_VIBRATION
        Public wLeftMotorSpeed As UShort 'Unsigned 16-bit (2-byte) integer range 0 through 65,535.
        Public wRightMotorSpeed As UShort 'Unsigned 16-bit (2-byte) integer range 0 through 65,535.
    End Structure

    <DllImport("xinput1_4.dll")>
    Private Shared Function XInputGetBatteryInformation(ByVal playerIndex As Integer, ByVal devType As Byte, ByRef batteryInfo As XINPUT_BATTERY_INFORMATION) As Integer
    End Function

    Public Structure XINPUT_BATTERY_INFORMATION
        Public BatteryType As Byte
        Public BatteryLevel As Byte
    End Structure

    Private batteryInfo As XINPUT_BATTERY_INFORMATION

    'The start of the thumbstick neutral zone.
    Private Const NeutralStart = -16256

    'The end of the thumbstick neutral zone.
    Private Const NeutralEnd = 16256

    'The end of the trigger neutral zone.
    Private Const TriggerNeutralEnd = 64

    Private ReadOnly Connected(0 To 3) As Boolean

    Private ControllerNumber As Integer = 0

    Private ControllerPosition As XINPUT_STATE

    Private vibration As XINPUT_VIBRATION

    Private Const BATTERY_DEVTYPE_GAMEPAD As Integer = 0

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Text = "XInput - Code with Joe"

        InitializeTimer1()

        InitializeTimer2()

        ClearLabels()

        TrackBarSpeed.Value = 32767

        UpdateSpeedLabel()

    End Sub

    Private Sub InitializeTimer1()

        Timer1.Interval = 15 'Polling frequency in milliseconds.

        Timer1.Start()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        UpdateControllerData()

    End Sub

    Private Sub UpdateControllerData()

        UpdateControllerPosition()

        UpdateBatteryLevel()

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
                'LabelButtons.Text = ""
            Case 1 'DPad up is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Up"
                Timer2.Start()
            Case 2 'DPad down is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Down"
                Timer2.Start()
            Case 4 'DPad left is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Left"
                Timer2.Start()
            Case 5 'DPad up and left is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Up Left"
                Timer2.Start()
            Case 6 'DPad down left is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Down Left"
                Timer2.Start()
            Case 8 'DPad right is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Right"
                Timer2.Start()
            Case 9 'DPad up right is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Up Right"
                Timer2.Start()
            Case 10 'DPad down right is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Dpad: Down Right"
                Timer2.Start()
            Case 16 'Start is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Start"
                Timer2.Start()
            Case 32 'Back is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Back"
                Timer2.Start()
            Case 64 'Left stick is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Left Stick"
                Timer2.Start()
            Case 128 'Right stick is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Right Stick"
                Timer2.Start()
            Case 256 'Left bumper is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Left Bumper"
                Timer2.Start()
            Case 512 'Right bumper is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Right Bumper"
                Timer2.Start()
            Case 4096 'A is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: A"
                Timer2.Start()
            Case 8192 'B is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: B"
                Timer2.Start()
            Case 16384 'X is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: X"
                Timer2.Start()
            Case 32768 'Y is down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Y"
                Timer2.Start()
            Case 48 'Back and start are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Back+Start"
                Timer2.Start()
            Case 192 'Left and right sticks are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Left and Right Stick"
                Timer2.Start()
            Case 768 'Left and right bumpers are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Left and Right Bumper"
                Timer2.Start()
            Case 12288 'A and b are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+B"
                Timer2.Start()
            Case 20480 'A and x are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+X"
                Timer2.Start()
            Case 36864 'A and y are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+Y"
                Timer2.Start()
            Case 24576 'B and x are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: B+X"
                Timer2.Start()
            Case 40960 'B and y are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: B+Y"
                Timer2.Start()
            Case 49152 'X and y are down.
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: X+Y"
                Timer2.Start()
        End Select

    End Sub

    Private Sub UpdateLeftThumbstickPosition()
        'The range on the X-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.
        'The range on the Y-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.

        'What position is the left thumbstick in on the X-axis?
        If ControllerPosition.Gamepad.sThumbLX <= NeutralStart Then
            'The left thumbstick is in the left position.

            LabelLeftThumbX.Text = "Controller: " & ControllerNumber.ToString & " Left Thumbstick: Left"

            Timer2.Start()


        ElseIf ControllerPosition.Gamepad.sThumbLX >= NeutralEnd Then
            'The left thumbstick is in the right position.

            LabelLeftThumbX.Text = "Controller: " & ControllerNumber.ToString & " Left Thumbstick: Right"

            Timer2.Start()

        Else
            'The left thumbstick is in the neutral position.

        End If

        'What position is the left thumbstick in on the Y-axis?
        If ControllerPosition.Gamepad.sThumbLY <= NeutralStart Then
            'The left thumbstick is in the up position.

            LabelLeftThumbY.Text = "Controller: " & ControllerNumber.ToString & " Left Thumbstick: Down"

            Timer2.Start()

        ElseIf ControllerPosition.Gamepad.sThumbLY >= NeutralEnd Then
            'The left thumbstick is in the down position.

            LabelLeftThumbY.Text = "Controller: " & ControllerNumber.ToString & " Left Thumbstick: Up"

            Timer2.Start()

        Else
            'The left thumbstick is in the neutral position.

        End If

    End Sub

    Private Sub UpdateRightThumbstickPosition()
        'The range on the X-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.
        'The range on the Y-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.

        'What position is the right thumbstick in on the X-axis?
        If ControllerPosition.Gamepad.sThumbRX <= NeutralStart Then
            'The right thumbstick is in the left position.

            LabelRightThumbX.Text = "Controller: " & ControllerNumber.ToString & " Right Thumbstick: Left"

            Timer2.Start()


        ElseIf ControllerPosition.Gamepad.sThumbRX >= NeutralEnd Then
            'The right thumbstick is in the right position.

            LabelRightThumbX.Text = "Controller: " & ControllerNumber.ToString & " Right Thumbstick: Right"

            Timer2.Start()

        Else
            'The right thumbstick is in the neutral position.

        End If

        'What position is the right thumbstick in on the Y-axis?
        If ControllerPosition.Gamepad.sThumbRY <= NeutralStart Then
            'The right thumbstick is in the up position.

            LabelRightThumbY.Text = "Controller: " & ControllerNumber.ToString & " Right Thumbstick: Down"

            Timer2.Start()

        ElseIf ControllerPosition.Gamepad.sThumbRY >= NeutralEnd Then
            'The right thumbstick is in the down position.

            LabelRightThumbY.Text = "Controller: " & ControllerNumber.ToString & " Right Thumbstick: Up"

            Timer2.Start()

        Else
            'The right thumbstick is in the neutral position.

        End If

    End Sub

    Private Sub UpdateRightTriggerPosition()
        'The range of trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.

        'What position is the right trigger in?
        If ControllerPosition.Gamepad.bRightTrigger > TriggerNeutralEnd Then
            'The right trigger is in the down position.

            LabelRightTrigger.Text = "Controller: " & ControllerNumber.ToString & " Right Trigger"

            Timer2.Start()

        Else
            'The right trigger is in the neutral position.

        End If

    End Sub

    Private Sub UpdateLeftTriggerPosition()
        'The range of trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.

        'What position is the left trigger in?
        If ControllerPosition.Gamepad.bLeftTrigger > TriggerNeutralEnd Then
            'The left trigger is in the down position.

            LabelLeftTrigger.Text = "Controller: " & ControllerNumber.ToString & " Left Trigger"

            Timer2.Start()

        Else
            'The left trigger is in the neutral position.

        End If

    End Sub

    Private Sub InitializeTimer2()

        Timer2.Interval = 400 'Label display time in milliseconds.

    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick

        ClearLabels()

        Timer2.Stop()

    End Sub

    Private Sub ClearLabels()

        LabelButtons.Text = ""

        LabelLeftThumbX.Text = ""

        LabelLeftThumbY.Text = ""

        LabelLeftTrigger.Text = ""

        LabelRightThumbX.Text = ""

        LabelRightThumbY.Text = ""

        LabelRightTrigger.Text = ""

    End Sub

    Private Sub ButtonVibrateLeft_Click(sender As Object, e As EventArgs) Handles ButtonVibrateLeft.Click

        VibrateLeft(NumControllerToVib.Value, TrackBarSpeed.Value)

    End Sub

    Private Sub ButtonVibrateRight_Click(sender As Object, e As EventArgs) Handles ButtonVibrateRight.Click

        VibrateRight(NumControllerToVib.Value, TrackBarSpeed.Value)

    End Sub

    Private Sub VibrateLeft(ByVal ControllerNumber As Integer, ByVal Speed As UShort)
        'The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        'The left motor is the low-frequency rumble motor.

        'Set right motor off (zero speed).
        vibration.wRightMotorSpeed = 0

        'Set left motor speed.
        vibration.wLeftMotorSpeed = Speed

        Vibrate(ControllerNumber)

    End Sub

    Private Sub VibrateRight(ByVal ControllerNumber As Integer, ByVal Speed As UShort)
        'The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        'The right motor is the high-frequency rumble motor.

        'Set left motor off (zero speed).
        vibration.wLeftMotorSpeed = 0

        'Set right motor speed.
        vibration.wRightMotorSpeed = Speed

        Vibrate(ControllerNumber)

    End Sub

    Private Sub Vibrate(ByVal ControllerNumber As Integer)

        Try

            'Turn motor on.
            If XInputSetState(ControllerNumber, vibration) = 0 Then
                'Success
                'Text = XInputSetState(ControllerNumber, vibration).ToString
            Else
                'Fail
                'Text = XInputSetState(ControllerNumber, vibration).ToString
            End If

        Catch ex As Exception

            MsgBox(ex.ToString)

            Exit Sub

        End Try

    End Sub

    Private Sub UpdateBatteryLevel()

        'Get battery level
        If XInputGetBatteryInformation(NumControllerToVib.Value, BATTERY_DEVTYPE_GAMEPAD, batteryInfo) = 0 Then
            'Success

            Select Case batteryInfo.BatteryLevel
                Case 0
                    LabelBatteryLevel.Text = "Battery Level: EMPTY"
                Case 1
                    LabelBatteryLevel.Text = "Battery Level: LOW"
                Case 2
                    LabelBatteryLevel.Text = "Battery Level: MEDIUM"
                Case 3
                    LabelBatteryLevel.Text = "Battery Level: FULL"
            End Select

        Else
            'Fail

            LabelBatteryLevel.Text = ""

        End If

    End Sub

    Private Sub TrackBarSpeed_Scroll(sender As Object, e As EventArgs) Handles TrackBarSpeed.Scroll

        UpdateSpeedLabel()

    End Sub

    Private Sub UpdateSpeedLabel()

        LabelSpeed.Text = "Vibration Speed: " & TrackBarSpeed.Value

    End Sub

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
'XInputSetState Function
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputsetstate
'
'XINPUT_VIBRATION Structure
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_vibration
'
'XInputGetBatteryInformation Function
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputgetbatteryinformation
'
'XINPUT_BATTERY_INFORMATION Structure
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_battery_information
'
'Getting Started with XInput in Windows Applications
'https://learn.microsoft.com/en-us/windows/win32/xinput/getting-started-with-xinput
'
'XInput Game Controller APIs
'https://learn.microsoft.com/en-us/windows/win32/api/_xinput/
'
'XInput Versions
'https://learn.microsoft.com/en-us/windows/win32/xinput/xinput-versions
'
'Comparison of XInput and DirectInput Features
'https://learn.microsoft.com/en-us/windows/win32/xinput/xinput-and-directinput
'
'Short Data Type (Visual Basic)
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/short-data-type
'
'Byte Data Type (Visual Basic)
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/byte-data-type
'
'UShort Data Type (Visual Basic)
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/ushort-data-type
'