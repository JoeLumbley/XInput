'XInput
'
'This is an example application that demonstrates the use of Xbox controllers,
'including the vibration effect (rumble). It was written in VB.NET in 2023 and
'is compatible with Windows 10 and 11. I'm making a video to explain the code on
'my YouTube channel. https://www.youtube.com/@codewithjoe6074
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

    <StructLayout(LayoutKind.Explicit)>
    Public Structure XINPUT_STATE
        <FieldOffset(0)>
        Public dwPacketNumber As UInteger 'Unsigned 32-bit (4-byte) integer range 0 through 4,294,967,295.
        <FieldOffset(4)>
        Public Gamepad As XINPUT_GAMEPAD
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure XINPUT_GAMEPAD
        Public wButtons As UShort 'Unsigned 16-bit (2-byte) integer range 0 through 65,535.
        Public bLeftTrigger As Byte 'Unsigned 8-bit (1-byte) integer range 0 through 255.
        Public bRightTrigger As Byte
        Public sThumbLX As Short 'Signed 16-bit (2-byte) integer range -32,768 through 32,767.
        Public sThumbLY As Short
        Public sThumbRX As Short
        Public sThumbRY As Short
    End Structure

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputSetState(playerIndex As Integer, ByRef vibration As XINPUT_VIBRATION) As Integer
    End Function

    Public Structure XINPUT_VIBRATION
        Public wLeftMotorSpeed As UShort
        Public wRightMotorSpeed As UShort
    End Structure

    <DllImport("xinput1_4.dll")>
    Private Shared Function XInputGetBatteryInformation(ByVal playerIndex As Integer, ByVal devType As Byte,
                                                        ByRef batteryInfo As XINPUT_BATTERY_INFORMATION) As Integer
    End Function

    Public Structure XINPUT_BATTERY_INFORMATION
        Public BatteryType As Byte
        Public BatteryLevel As Byte
    End Structure

    Public Enum BATTERY_TYPE As Byte
        DISCONNECTED = 0
        WIRED = 1
        ALKALINE = 2
        NIMH = 3 ' Nickel metal hydride battery.
        UNKNOWN = 4
    End Enum

    Public Enum BatteryLevel As Byte 'Unsigned 8-bit (1-byte) integer range 0 through 255.
        EMPTY = 0
        LOW = 1
        MEDIUM = 2
        FULL = 3
    End Enum

    Private batteryInfo As XINPUT_BATTERY_INFORMATION

    'The start of the thumbstick neutral zone.
    Private Const NeutralStart As Integer = -16256 'Signed 32-bit (4-byte) integer range -2,147,483,648 through 2,147,483,647.

    'The end of the thumbstick neutral zone.
    Private Const NeutralEnd As Integer = 16256

    'Set the trigger threshold to 64 or 1/4 pull.
    Private Const TriggerThreshold As Integer = 64 '63.75 = 255 / 4
    'The trigger position must be greater than the trigger threshold to register as pressed.

    Private ReadOnly Connected(0 To 3) As Boolean 'True or False

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

        UpdateBatteryInfo()

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
            Case 1 'Up
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Up"
                Timer2.Start()
            Case 2 'Down
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Down"
                Timer2.Start()
            Case 4 'Left
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Left"
                Timer2.Start()
            Case 5 'Up+Left
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Up+Left"
                Timer2.Start()
            Case 6 'Down+Left
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Down+Left"
                Timer2.Start()
            Case 8 'Right
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Right"
                Timer2.Start()
            Case 9 'Up+Right
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Up+Right"
                Timer2.Start()
            Case 10 'Down+Right
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Down+Right"
                Timer2.Start()
            Case 16 'Start
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Start"
                Timer2.Start()
            Case 32 'Back
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Back"
                Timer2.Start()
            Case 64 'Left Stick
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Left Stick"
                Timer2.Start()
            Case 128 'Right Stick
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Right Stick"
                Timer2.Start()
            Case 256 'Left bumper
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Left Bumper"
                Timer2.Start()
            Case 512 'Right bumper
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Right Bumper"
                Timer2.Start()
            Case 4096 'A
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: A"
                Timer2.Start()
            Case 8192 'B
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: B"
                Timer2.Start()
            Case 16384 'X
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: X"
                Timer2.Start()
            Case 32768 'Y
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Button: Y"
                Timer2.Start()
            Case 48 'Start+Back
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Start+Back"
                Timer2.Start()
            Case 192 'Left+Right Sticks
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Left+Right Stick"
                Timer2.Start()
            Case 768 'Left+Right Bumpers
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Left and Right Bumper"
                Timer2.Start()
            Case 12288 'A+B
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+B"
                Timer2.Start()
            Case 20480 'A+X
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+X"
                Timer2.Start()
            Case 36864 'A+Y
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+Y"
                Timer2.Start()
            Case 24576 'B+X
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: B+X"
                Timer2.Start()
            Case 40960 'B+Y
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: B+Y"
                Timer2.Start()
            Case 49152 'X+Y
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: X+Y"
                Timer2.Start()


            Case 28672 'A+B+X
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+B+X"
                Timer2.Start()
            Case 45056 'A+B+Y
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+B+Y"
                Timer2.Start()
            Case 53248 'A+X+Y
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+X+Y"
                Timer2.Start()
            Case 57344 'B+X+Y
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: B+X+Y"
                Timer2.Start()
            Case 61440 'A+B+X+Y
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: A+B+X+Y"
                Timer2.Start()

            Case 4097 'Up+A
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Up+A"
                Timer2.Start()
            Case 4098 'Down+A
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Down+A"
                Timer2.Start()
            Case 4100 'Left+A
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Left+A"
                Timer2.Start()
            Case 4104 'Right+A
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Right+A"
                Timer2.Start()
            Case 4105 'Up+Right+A
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Up+Right+A"
                Timer2.Start()
            Case 4101 'Up+Left+A
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Up+Left+A"
                Timer2.Start()

            Case 4106 'Down+Right+A
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Down+Right+A"
                Timer2.Start()
            Case 4102 'Down+Left+A
                LabelButtons.Text = "Controller: " & ControllerNumber.ToString & " Buttons: Down+Left+A"
                Timer2.Start()

            Case Else
                LabelButtons.Text = ControllerPosition.Gamepad.wButtons.ToString
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
        'The range of right trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.
        'The trigger position must be greater than the trigger threshold to register as pressed.

        'What position is the right trigger in?
        If ControllerPosition.Gamepad.bRightTrigger > TriggerThreshold Then
            'The right trigger is in the down position. Trigger Break. Bang!

            LabelRightTrigger.Text = "Controller: " & ControllerNumber.ToString & " Right Trigger"

            Timer2.Start()

        Else
            'The right trigger is in the neutral position. Pre-Travel.

        End If

    End Sub

    Private Sub UpdateLeftTriggerPosition()
        'The range of left trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.
        'The trigger position must be greater than the trigger threshold to register as pressed.

        'What position is the left trigger in?
        If ControllerPosition.Gamepad.bLeftTrigger > TriggerThreshold Then
            'The left trigger is in the down position. Trigger Break. Bang!

            LabelLeftTrigger.Text = "Controller: " & ControllerNumber.ToString & " Left Trigger"

            Timer2.Start()

        Else
            'The left trigger is in the neutral position. Pre-Travel.

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

        'Turn right motor off (set zero speed).
        vibration.wRightMotorSpeed = 0

        'Set left motor speed.
        vibration.wLeftMotorSpeed = Speed

        Vibrate(ControllerNumber)

    End Sub

    Private Sub VibrateRight(ByVal ControllerNumber As Integer, ByVal Speed As UShort)
        'The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        'The right motor is the high-frequency rumble motor.

        'Turn left motor off (set zero speed).
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

    Private Sub UpdateBatteryInfo()

        'Get battery level
        If XInputGetBatteryInformation(NumControllerToVib.Value, BATTERY_DEVTYPE_GAMEPAD, batteryInfo) = 0 Then
            'Success

            UpdateBatteryLevel()

            UpdateBatteryType()

        Else
            'Fail

            ClearBatteryLabels()

        End If

    End Sub

    Private Sub UpdateBatteryType()

        Select Case batteryInfo.BatteryType
            Case BATTERY_TYPE.DISCONNECTED
                LabelBatteryType.Text = "Controller is not connected"
            Case BATTERY_TYPE.WIRED
                LabelBatteryType.Text = "Controller is connected by a wired connection"
            Case BATTERY_TYPE.ALKALINE
                LabelBatteryType.Text = "Controller is connected wirelessly and is using alkaline batteries"
            Case BATTERY_TYPE.NIMH
                LabelBatteryType.Text = "Controller is connected wirelessly and is using rechargeable NiMH batteries"
            Case BATTERY_TYPE.UNKNOWN
                LabelBatteryType.Text = "Controller is connected wirelessly and is using unknown battery type."
        End Select

    End Sub

    Private Sub UpdateBatteryLevel()

        With batteryInfo
            'This value is only valid for wireless controllers with a known battery type.
            If .BatteryType = BATTERY_TYPE.ALKALINE Or .BatteryType = BATTERY_TYPE.NIMH Then
                'Valid.
                Select Case .BatteryLevel
                    Case BatteryLevel.EMPTY
                        LabelBatteryLevel.Text = "Battery Level: EMPTY"
                    Case BatteryLevel.LOW
                        LabelBatteryLevel.Text = "Battery Level: LOW"
                    Case BatteryLevel.MEDIUM
                        LabelBatteryLevel.Text = "Battery Level: MEDIUM"
                    Case BatteryLevel.FULL
                        LabelBatteryLevel.Text = "Battery Level: FULL"
                End Select
            Else
                'Invalid.
                LabelBatteryLevel.Text = ""
            End If
        End With

    End Sub

    Private Sub ClearBatteryLabels()

        LabelBatteryLevel.Text = ""

        LabelBatteryType.Text = ""

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
'Short Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/short-data-type
'
'Byte Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/byte-data-type
'
'UShort Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/ushort-data-type
'
'UInteger Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/uinteger-data-type
'
'Boolean Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/boolean-data-type
'
'Integer Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/integer-data-type
'