'XInput

'This is an example application that demonstrates the use of Xbox controllers,
'including the vibration effect (rumble).

'MIT License
'Copyright(c) 2023 Joseph W. Lumbley

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

Imports System.IO
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

    Private ConButtons(0 To 3) As UShort

    Private IsConLeftTriggerNeutral(0 To 3) As Boolean

    Private IsConRightTriggerNeutral(0 To 3) As Boolean

    Private IsConThumbLXNeutral(0 To 3) As Boolean

    Private IsConThumbLYNeutral(0 To 3) As Boolean

    Private IsConThumbRXNeutral(0 To 3) As Boolean

    Private IsConThumbRYNeutral(0 To 3) As Boolean

    Private ControllerPosition As XINPUT_STATE

    'Set the start of the thumbstick neutral zone to 1/2 over.
    Private Const NeutralStart As Short = -16384 '-16,384 = -32,768 / 2
    'The thumbstick position must be more than 1/2 over the neutral start to register as moved.
    'A short is a signed 16-bit (2-byte) integer range -32,768 through 32,767. This gives us 65,536 values.

    'Set the end of the thumbstick neutral zone to 1/2 over.
    Private Const NeutralEnd As Short = 16384 '16,383.5 = 32,767 / 2
    'The thumbstick position must be more than 1/2 over the neutral end to register as moved.
    'A short is a signed 16-bit (2-byte) integer range -32,768 through 32,767. This gives us 65,536 values.

    'Set the trigger threshold to 1/4 pull.
    Private Const TriggerThreshold As Byte = 64 '64 = 256 / 4
    'The trigger position must be greater than the trigger threshold to register as pulled.
    'A byte is a unsigned 8-bit (1-byte) integer range 0 through 255. This gives us 256 values.

    Private ReadOnly Connected(0 To 3) As Boolean 'True or False

    Private Const StartButton As Integer = 16
    Private Const BackButton As Integer = 32

    Private Const LeftStickButton As Integer = 64
    Private Const RightStickButton As Integer = 128

    Private Const LeftBumperButton As Integer = 256
    Private Const RightBumperButton As Integer = 512

    Private Const AButton As Integer = 4096
    Private Const BButton As Integer = 8192
    Private Const XButton As Integer = 16384
    Private Const YButton As Integer = 32768

    Private DPadUpPressed As Boolean = False
    Private DPadDownPressed As Boolean = False
    Private DPadLeftPressed As Boolean = False
    Private DPadRightPressed As Boolean = False

    Private StartButtonPressed As Boolean = False
    Private BackButtonPressed As Boolean = False

    Private LeftStickButtonPressed As Boolean = False
    Private RightStickButtonPressed As Boolean = False

    Private LeftBumperButtonPressed As Boolean = False
    Private RightBumperButtonPressed As Boolean = False

    Private AButtonPressed As Boolean = False
    Private BButtonPressed As Boolean = False
    Private XButtonPressed As Boolean = False
    Private YButtonPressed As Boolean = False

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputSetState(playerIndex As Integer, ByRef vibration As XINPUT_VIBRATION) As Integer
    End Function

    Public Structure XINPUT_VIBRATION
        Public wLeftMotorSpeed As UShort
        Public wRightMotorSpeed As UShort
    End Structure

    Private Vibration As XINPUT_VIBRATION

    Private LeftVibrateStart(0 To 3) As Date

    Private RightVibrateStart(0 To 3) As Date

    Private IsLeftVibrating(0 To 3) As Boolean

    Private IsRightVibrating(0 To 3) As Boolean

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

    Private Const BATTERY_DEVTYPE_GAMEPAD As Integer = 0

    Private Const DPadUp As Integer = 1
    Private Const DPadDown As Integer = 2
    Private Const DPadLeft As Integer = 4
    Private Const DPadRight As Integer = 8



    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Text = "XInput - Code with Joe"

        InitializeTimer1()

        ClearLabels()

        TrackBarSpeed.Value = 32767

        UpdateSpeedLabel()

        For Each Con In IsLeftVibrating

            IsLeftVibrating(Array.IndexOf(IsLeftVibrating, Con)) = False

        Next

        For Each Con In IsRightVibrating

            IsRightVibrating(Array.IndexOf(IsRightVibrating, Con)) = False

        Next

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

        UpdateVibrateTimer()

        'UpdateBatteryInfo()

    End Sub

    Private Sub UpdateControllerPosition()

        For ControllerNumber = 0 To 3 'Up to 4 controllers

            Try

                ' Check if the function call was successful
                If XInputGetState(ControllerNumber, ControllerPosition) = 0 Then
                    ' The function call was successful, so you can access the controller state now

                    UpdateButtonPosition(ControllerNumber)

                    DoButtonLogic(ControllerNumber)

                    UpdateLeftThumbstickPosition(ControllerNumber)

                    UpdateRightThumbstickPosition(ControllerNumber)

                    UpdateLeftTriggerPosition(ControllerNumber)

                    UpdateRightTriggerPosition(ControllerNumber)

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

    Private Sub UpdateButtonPosition(CID As Integer)
        'The range of buttons is 0 to 65,535. Unsigned 16-bit (2-byte) integer.

        If (ControllerPosition.Gamepad.wButtons And DPadUp) <> 0 Then
            DPadUpPressed = True
        Else
            DPadUpPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And DPadDown) <> 0 Then
            DPadDownPressed = True
        Else
            DPadDownPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And DPadLeft) <> 0 Then
            DPadLeftPressed = True
        Else
            DPadLeftPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And DPadRight) <> 0 Then
            DPadRightPressed = True
        Else
            DPadRightPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And StartButton) <> 0 Then
            StartButtonPressed = True
        Else
            StartButtonPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And BackButton) <> 0 Then
            BackButtonPressed = True
        Else
            BackButtonPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And LeftStickButton) <> 0 Then
            LeftStickButtonPressed = True
        Else
            LeftStickButtonPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And RightStickButton) <> 0 Then
            RightStickButtonPressed = True
        Else
            RightStickButtonPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And LeftBumperButton) <> 0 Then
            LeftBumperButtonPressed = True
        Else
            LeftBumperButtonPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And RightBumperButton) <> 0 Then
            RightBumperButtonPressed = True
        Else
            RightBumperButtonPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And AButton) <> 0 Then
            AButtonPressed = True
        Else
            AButtonPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And BButton) <> 0 Then
            BButtonPressed = True
        Else
            BButtonPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And XButton) <> 0 Then
            XButtonPressed = True
        Else
            XButtonPressed = False
        End If

        If (ControllerPosition.Gamepad.wButtons And YButton) <> 0 Then
            YButtonPressed = True
        Else
            YButtonPressed = False
        End If

        ConButtons(CID) = ControllerPosition.Gamepad.wButtons

        Dim ConSum As Integer

        For Each Con In ConButtons

            ConSum += Con

        Next

        'Are all controllers buttons up?
        If ConSum = 0 Then
            'Yes, all controller buttons are up.

            LabelButtons.Text = ""

        End If

    End Sub

    Private Sub DoButtonLogic(ControllerNumber As Integer)

        DoDPadLogic(ControllerNumber)

        DoLetterButtonLogic(ControllerNumber)

        DoStartBackLogic(ControllerNumber)

        DoBumperLogic(ControllerNumber)

        DoStickLogic(ControllerNumber)

    End Sub

    Private Sub DoLetterButtonLogic(ControllerNumber As Integer)

        If AButtonPressed = True Then
            If BButtonPressed = True Then
                If XButtonPressed = True Then
                    If YButtonPressed = True Then
                        LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: A+B+X+Y"
                    Else
                        LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: A+B+X"
                    End If
                Else
                    If YButtonPressed = True Then
                        LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: A+B+Y"
                    Else
                        LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: A+B"
                    End If
                End If
            Else
                If XButtonPressed = True Then
                    If YButtonPressed = True Then
                        LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: A+X+Y"
                    Else
                        LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: A+X"
                    End If
                Else
                    If YButtonPressed = True Then
                        LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: A+Y"
                    Else
                        LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: A"
                    End If
                End If
            End If
        End If

        If BButtonPressed = True AndAlso AButtonPressed = False Then
            If XButtonPressed = True Then
                If YButtonPressed = True Then
                    LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: B+X+Y"
                Else
                    LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: B+X"
                End If
            Else
                If YButtonPressed = True Then
                    LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: B+Y"
                Else
                    LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: B"
                End If
            End If
        End If

        If XButtonPressed = True AndAlso AButtonPressed = False AndAlso BButtonPressed = False Then
            If YButtonPressed = True Then
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: X+Y"
            Else
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: X"
            End If
        Else
            If YButtonPressed = True AndAlso AButtonPressed = False AndAlso BButtonPressed = False Then
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: Y"
            End If
        End If

    End Sub

    Private Sub DoStartBackLogic(ControllerNumber As Integer)

        If StartButtonPressed = True Then
            If BackButtonPressed = True Then
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: Start+Back"
            Else
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: Start"
            End If
        Else
            If BackButtonPressed = True Then
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: Back"
            End If
        End If

    End Sub

    Private Sub DoBumperLogic(ControllerNumber As Integer)

        If LeftBumperButtonPressed = True Then
            If RightBumperButtonPressed = True Then
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: Left Bumper+Right Bumper"
            Else
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: Left Bumper"
            End If
        Else
            If RightBumperButtonPressed = True Then
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: Right Bumper"
            End If
        End If

    End Sub

    Private Sub DoStickLogic(ControllerNumber As Integer)

        If LeftStickButtonPressed = True Then
            If RightStickButtonPressed = True Then
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: Left Stick+Right Stick"
            Else
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: Left Stick"
            End If
        Else
            If RightStickButtonPressed = True Then
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Buttons: Right Stick"
            End If
        End If

    End Sub

    Private Sub DoDPadLogic(ControllerNumber As Integer)

        If DPadUpPressed = True Then
            If DPadLeftPressed = True Then
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Button: Left+Up"
            ElseIf DPadRightPressed = True Then
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Button: Right+Up"
            Else
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Button: Up"
            End If
        End If

        If DPadDownPressed = True Then
            If DPadLeftPressed = True Then
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Button: Left+Down"
            ElseIf DPadRightPressed = True Then
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Button: Right+Down"
            Else
                LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Button: Down"
            End If
        End If

        If DPadLeftPressed = True AndAlso DPadDownPressed = False AndAlso DPadUpPressed = False Then
            LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Button: Left"
        End If

        If DPadRightPressed = True AndAlso DPadDownPressed = False AndAlso DPadUpPressed = False Then
            LabelButtons.Text = "Controller " & ControllerNumber.ToString & " Button: Right"
        End If

    End Sub

    Private Sub UpdateLeftThumbstickPosition(ControllerNumber As Integer)
        'The range on the X-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.
        'The range on the Y-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.

        'What position is the left thumbstick in on the X-axis?
        If ControllerPosition.Gamepad.sThumbLX <= NeutralStart Then
            'The left thumbstick is in the left position.

            LabelLeftThumbX.Text = "Controller " & ControllerNumber.ToString & " Left Thumbstick: Left"

            IsConThumbLXNeutral(ControllerNumber) = False

        ElseIf ControllerPosition.Gamepad.sThumbLX >= NeutralEnd Then
            'The left thumbstick is in the right position.

            LabelLeftThumbX.Text = "Controller " & ControllerNumber.ToString & " Left Thumbstick: Right"

            IsConThumbLXNeutral(ControllerNumber) = False

        Else
            'The left thumbstick is in the neutral position.

            IsConThumbLXNeutral(ControllerNumber) = True

        End If

        Dim ConSum As Boolean = True 'Assume all controllers left thumbsticks are neutral initially.

        'Search for a non-neutral thumbstick.
        For i As Integer = 0 To 3
            If Connected(i) Then
                If Not IsConThumbLXNeutral(i) Then
                    'A non-neutral thumbstick was found.

                    ConSum = False 'Report the non-neutral thumbstick.

                    Exit For 'No need to search further so stop the search.

                End If
            End If
        Next

        'Are all controllers left thumbsticks in the neutral position?
        If ConSum = True Then
            'Yes, all controllers left thumbsticks are in the neutral position.

            LabelLeftThumbX.Text = String.Empty

        End If

        'What position is the left thumbstick in on the Y-axis?
        If ControllerPosition.Gamepad.sThumbLY <= NeutralStart Then
            'The left thumbstick is in the down position.

            LabelLeftThumbY.Text = "Controller " & ControllerNumber.ToString & " Left Thumbstick: Down"

            IsConThumbLYNeutral(ControllerNumber) = False

        ElseIf ControllerPosition.Gamepad.sThumbLY >= NeutralEnd Then
            'The left thumbstick is in the up position.

            LabelLeftThumbY.Text = "Controller " & ControllerNumber.ToString & " Left Thumbstick: Up"

            IsConThumbLYNeutral(ControllerNumber) = False

        Else
            'The left thumbstick is in the neutral position.

            IsConThumbLYNeutral(ControllerNumber) = True

        End If

        ConSum = True 'Assume all controllers left thumbsticks are neutral initially.

        'Search for a non-neutral thumbstick.
        For i As Integer = 0 To 3
            If Connected(i) Then
                If Not IsConThumbLYNeutral(i) Then
                    'A non-neutral thumbstick was found.

                    ConSum = False 'Report the non-neutral thumbstick.

                    Exit For 'No need to search further so stop the search.

                End If
            End If
        Next

        'Are all controllers left thumbsticks in the neutral position?
        If ConSum = True Then
            'Yes, all controllers left thumbsticks are in the neutral position.

            LabelLeftThumbY.Text = String.Empty

        End If

    End Sub

    Private Sub UpdateRightThumbstickPosition(ControllerNumber As Integer)
        'The range on the X-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.
        'The range on the Y-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.

        'What position is the right thumbstick in on the X-axis?
        If ControllerPosition.Gamepad.sThumbRX <= NeutralStart Then
            'The right thumbstick is in the left position.

            LabelRightThumbX.Text = "Controller " & ControllerNumber.ToString & " Right Thumbstick: Left"

            IsConThumbRXNeutral(ControllerNumber) = False

        ElseIf ControllerPosition.Gamepad.sThumbRX >= NeutralEnd Then
            'The right thumbstick is in the right position.

            LabelRightThumbX.Text = "Controller " & ControllerNumber.ToString & " Right Thumbstick: Right"

            IsConThumbRXNeutral(ControllerNumber) = False

        Else
            'The right thumbstick is in the neutral position.

            IsConThumbRXNeutral(ControllerNumber) = True

        End If

        Dim ConSum As Boolean = True 'Assume all controllers right thumbsticks are neutral initially.

        'Search for a non-neutral thumbstick.
        For i As Integer = 0 To 3
            If Connected(i) Then
                If Not IsConThumbRXNeutral(i) Then
                    'A non-neutral thumbstick was found.

                    ConSum = False 'Report the non-neutral thumbstick.

                    Exit For 'No need to search further so stop the search.

                End If
            End If
        Next

        'Are all controllers right thumbsticks in the neutral position?
        If ConSum = True Then
            'Yes, all controllers right thumbsticks are in the neutral position.

            LabelRightThumbX.Text = String.Empty

        End If

        'What position is the right thumbstick in on the Y-axis?
        If ControllerPosition.Gamepad.sThumbRY <= NeutralStart Then
            'The right thumbstick is in the up position.

            LabelRightThumbY.Text = "Controller " & ControllerNumber.ToString & " Right Thumbstick: Down"

            IsConThumbRYNeutral(ControllerNumber) = False

        ElseIf ControllerPosition.Gamepad.sThumbRY >= NeutralEnd Then
            'The right thumbstick is in the down position.

            LabelRightThumbY.Text = "Controller " & ControllerNumber.ToString & " Right Thumbstick: Up"

            IsConThumbRYNeutral(ControllerNumber) = False

        Else
            'The right thumbstick is in the neutral position.

            IsConThumbRYNeutral(ControllerNumber) = True

        End If

        ConSum = True 'Assume all controllers right thumbsticks are neutral initially.

        'Search for a non-neutral thumbstick.
        For i As Integer = 0 To 3
            If Connected(i) Then
                If Not IsConThumbRYNeutral(i) Then
                    'A non-neutral thumbstick was found.

                    ConSum = False 'Report the non-neutral thumbstick.

                    Exit For 'No need to search further so stop the search.

                End If
            End If
        Next

        'Are all controllers right thumbsticks in the neutral position?
        If ConSum = True Then
            'Yes, all controllers right thumbsticks are in the neutral position.

            LabelRightThumbY.Text = String.Empty

        End If

    End Sub

    Private Sub UpdateRightTriggerPosition(ControllerNumber As Integer)
        'The range of right trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.
        'The trigger position must be greater than the trigger threshold to register as pressed.

        'What position is the right trigger in?
        If ControllerPosition.Gamepad.bRightTrigger > TriggerThreshold Then
            'The right trigger is in the down position. Trigger Break. Bang!

            LabelRightTrigger.Text = "Controller " & ControllerNumber.ToString & " Right Trigger"

            IsConRightTriggerNeutral(ControllerNumber) = False

        Else
            'The right trigger is in the neutral position. Pre-Travel.

            IsConRightTriggerNeutral(ControllerNumber) = True

        End If

        Dim ConSum As Boolean = True 'Assume all controllers right triggers are neutral initially.

        'Search for a non-neutral trigger.
        For i As Integer = 0 To 3
            If Connected(i) Then
                If Not IsConRightTriggerNeutral(i) Then
                    'A non-neutral trigger was found.

                    ConSum = False 'Report the non-neutral trigger.

                    Exit For  'No need to search further so stop the search.

                End If
            End If
        Next

        'Are all controllers right triggers in the neutral position?
        If ConSum = True Then
            'Yes, all controllers right triggers are in the neutral position.

            LabelRightTrigger.Text = String.Empty

        End If

    End Sub

    Private Sub UpdateLeftTriggerPosition(ControllerNumber As Integer)
        'The range of left trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.
        'The trigger position must be greater than the trigger threshold to register as pressed.

        'What position is the left trigger in?
        If ControllerPosition.Gamepad.bLeftTrigger > TriggerThreshold Then
            'The left trigger is in the down position. Trigger Break. Bang!

            LabelLeftTrigger.Text = "Controller " & ControllerNumber.ToString & " Left Trigger"

            IsConLeftTriggerNeutral(ControllerNumber) = False

        Else
            'The left trigger is in the neutral position. Pre-Travel.

            IsConLeftTriggerNeutral(ControllerNumber) = True

        End If

        ClearLeftTriggerLabel()

    End Sub

    Private Sub ClearLeftTriggerLabel()
        'Clears the left trigger label when all controllers left triggers are neutral.

        Dim ConSum As Boolean = True 'Assume all controllers left triggers are neutral initially.

        'Search for a non-neutral trigger.
        For i As Integer = 0 To 3
            If Connected(i) Then
                If Not IsConLeftTriggerNeutral(i) Then
                    'A non-neutral trigger was found.

                    ConSum = False 'Report the non-neutral trigger.

                    Exit For 'No need to search further so stop the search.

                End If
            End If
        Next

        'Are all controllers left triggers in the neutral position?
        If ConSum = True Then
            'Yes, all controllers left triggers are in the neutral position.

            LabelLeftTrigger.Text = String.Empty

        End If

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

    Private Sub VibrateLeft(CID As Integer, Speed As UShort)
        'The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        'The left motor is the low-frequency rumble motor.

        'Set left motor speed.
        Vibration.wLeftMotorSpeed = Speed

        Vibrate(CID)

        LeftVibrateStart(CID) = Now

        IsLeftVibrating(CID) = True

    End Sub

    Private Sub VibrateRight(CID As Integer, Speed As UShort)
        'The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        'The right motor is the high-frequency rumble motor.

        'Set right motor speed.
        Vibration.wRightMotorSpeed = Speed

        Vibrate(CID)

        RightVibrateStart(CID) = Now

        IsRightVibrating(CID) = True

    End Sub

    Private Sub Vibrate(CID As Integer)

        Try

            'Turn motor on.
            If XInputSetState(CID, Vibration) = 0 Then
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

    Private Sub UpdateVibrateTimer()

        UpdateLeftVibrateTimer()

        UpdateRightVibrateTimer()

    End Sub

    Private Sub UpdateLeftVibrateTimer()

        For Each IsConVibrating In IsLeftVibrating

            Dim Index As Integer = Array.IndexOf(IsLeftVibrating, IsConVibrating)

            If Index <> -1 AndAlso IsConVibrating = True Then

                Dim ElapsedTime As TimeSpan = Now - LeftVibrateStart(Index)

                If ElapsedTime.TotalSeconds >= 1 Then

                    IsLeftVibrating(Index) = False

                    ' Turn left motor off (set zero speed).
                    Vibration.wLeftMotorSpeed = 0

                    Vibrate(Index)

                End If

            End If

        Next

    End Sub

    Private Sub UpdateRightVibrateTimer()

        For Each IsConVibrating In IsRightVibrating

            Dim Index As Integer = Array.IndexOf(IsRightVibrating, IsConVibrating)

            If Index <> -1 AndAlso IsConVibrating = True Then

                Dim ElapsedTime As TimeSpan = Now - RightVibrateStart(Index)

                If ElapsedTime.TotalSeconds >= 1 Then

                    IsRightVibrating(Index) = False

                    ' Turn right motor off (set zero speed).
                    Vibration.wRightMotorSpeed = 0

                    Vibrate(Index)

                End If

            End If

        Next

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
                'Valid
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
                'Invalid
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


'Consuming Unmanaged DLL Functions

'Consuming unmanaged DLL functions refers to the process of using functions that are defined in a DLL (Dynamic Link Library)
'which is written in a language like C or C++.

'This involves using Platform Invocation Services (P/Invoke) to call functions in the unmanaged DLL from your managed VB.NET
'code.

'To consume unmanaged DLL functions use the DllImport attribute to declare the external functions from the DLL.

'https://learn.microsoft.com/en-us/dotnet/framework/interop/consuming-unmanaged-dll-functions


'Passing Structures

'Passing structures refers to the process of sending structured data as a parameter to a function or method.

'Structures, also known as structs, allow you to group related data together under a single name.

'When passing structures as parameters you are essentially sending a block of data that contains multiple fields or members.

'This can be useful for organizing related data and passing them around your program efficiently.

'https://learn.microsoft.com/en-us/dotnet/framework/interop/passing-structures


'XInputGetState Function

'The XInputGetState function is used to retrieve the current state of a Xbox controller.

'https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputgetstate


'XINPUT_STATE Structure

'The XINPUT_STATE structure is used to hold the current state of an Xbox controller.

'https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_state


'XINPUT_GAMEPAD Structure

'The XINPUT_GAMEPAD structure represents the state of the gamepad (Xbox controller) input, including information about
'button presses, trigger values, and thumbstick positions.

'https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_gamepad


'XInputSetState Function
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputsetstate


'XINPUT_VIBRATION Structure
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_vibration


'XInputGetBatteryInformation Function
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputgetbatteryinformation


'XINPUT_BATTERY_INFORMATION Structure
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_battery_information


'Getting Started with XInput in Windows Applications
'https://learn.microsoft.com/en-us/windows/win32/xinput/getting-started-with-xinput


'XInput Game Controller APIs
'https://learn.microsoft.com/en-us/windows/win32/api/_xinput/


'XInput Versions
'https://learn.microsoft.com/en-us/windows/win32/xinput/xinput-versions


'Comparison of XInput and DirectInput Features
'https://learn.microsoft.com/en-us/windows/win32/xinput/xinput-and-directinput


'Short Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/short-data-type


'Byte Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/byte-data-type


'UShort Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/ushort-data-type


'UInteger Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/uinteger-data-type


'Boolean Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/boolean-data-type


'Integer Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/integer-data-type


'Monica is our an AI assistant.
'https://monica.im/


'I also make coding videos on my YouTube channel.
'https://www.youtube.com/@codewithjoe6074
