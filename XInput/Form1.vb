﻿' XInput

' This is an example application that demonstrates the use of Xbox controllers,
' including the vibration effect (rumble).

' MIT License
' Copyright(c) 2023 Joseph W. Lumbley

' Permission Is hereby granted, free Of charge, to any person obtaining a copy
' of this software And associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, And/Or sell
' copies of the Software, And to permit persons to whom the Software Is
' furnished to do so, subject to the following conditions:

' The above copyright notice And this permission notice shall be included In all
' copies Or substantial portions of the Software.

' THE SOFTWARE Is PROVIDED "AS IS", WITHOUT WARRANTY Of ANY KIND, EXPRESS Or
' IMPLIED, INCLUDING BUT Not LIMITED To THE WARRANTIES Of MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE And NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS Or COPYRIGHT HOLDERS BE LIABLE For ANY CLAIM, DAMAGES Or OTHER
' LIABILITY, WHETHER In AN ACTION Of CONTRACT, TORT Or OTHERWISE, ARISING FROM,
' OUT OF Or IN CONNECTION WITH THE SOFTWARE Or THE USE Or OTHER DEALINGS IN THE
' SOFTWARE.

Imports System.Runtime.InteropServices


Public Class Form1

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputGetState(dwUserIndex As Integer, ByRef pState As XINPUT_STATE) As Integer
    End Function

    ' XInput1_4.dll seems to be the current version
    ' XInput9_1_0.dll is maintained primarily for backward compatibility. 

    <StructLayout(LayoutKind.Explicit)>
    Public Structure XINPUT_STATE
        <FieldOffset(0)>
        Public dwPacketNumber As UInteger ' Unsigned 32-bit (4-byte) integer range 0 through 4,294,967,295.
        <FieldOffset(4)>
        Public Gamepad As XINPUT_GAMEPAD
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure XINPUT_GAMEPAD
        Public wButtons As UShort ' Unsigned 16-bit (2-byte) integer range 0 through 65,535.
        Public bLeftTrigger As Byte ' Unsigned 8-bit (1-byte) integer range 0 through 255.
        Public bRightTrigger As Byte
        Public sThumbLX As Short ' Signed 16-bit (2-byte) integer range -32,768 through 32,767.
        Public sThumbLY As Short
        Public sThumbRX As Short
        Public sThumbRY As Short

    End Structure

    Private ConButtons(0 To 3) As UShort

    Private IsConLeftTriggerNeutral(0 To 3) As Boolean
    ' A boolean is a logical value that is either true or false.

    Private IsConRightTriggerNeutral(0 To 3) As Boolean

    Private IsConThumbLXNeutral(0 To 3) As Boolean

    Private IsConThumbLYNeutral(0 To 3) As Boolean

    Private IsConThumbRXNeutral(0 To 3) As Boolean

    Private IsConThumbRYNeutral(0 To 3) As Boolean

    Private IsDPadNeutral(0 To 3) As Boolean

    Private IsLeftBumperNeutral(0 To 3) As Boolean

    Private IsRightBumperNeutral(0 To 3) As Boolean

    Private IsLetterButtonsNeutral(0 To 3) As Boolean

    Private IsStartButtonsNeutral(0 To 3) As Boolean

    Private IsBackButtonsNeutral(0 To 3) As Boolean

    Private IsLeftStickButtonsNeutral(0 To 3) As Boolean

    Private IsRightStickButtonsNeutral(0 To 3) As Boolean

    Private ControllerPosition As XINPUT_STATE

    ' Set the start of the thumbstick neutral zone to 1/2 over.
    Private Const NeutralStart As Short = -16384 '-16,384 = -32,768 / 2
    ' The thumbstick position must be more than 1/2 over the neutral start to register as moved.
    ' A short is a signed 16-bit (2-byte) integer range -32,768 through 32,767. This gives us 65,536 values.

    ' Set the end of the thumbstick neutral zone to 1/2 over.
    Private Const NeutralEnd As Short = 16384 '16,383.5 = 32,767 / 2
    ' The thumbstick position must be more than 1/2 over the neutral end to register as moved.

    ' Set the trigger threshold to 1/4 pull.
    Private Const TriggerThreshold As Byte = 64 '64 = 256 / 4
    ' The trigger position must be greater than the trigger threshold to register as pulled.
    ' A byte is a unsigned 8-bit (1-byte) integer range 0 through 255. This gives us 256 values.

    Private ReadOnly Connected(0 To 3) As Boolean

    Private ConnectionStart As Date = Now

    Private Const DPadUp As Integer = 1
    Private Const DPadDown As Integer = 2

    Private Const DPadLeft As Integer = 4
    Private Const DPadRight As Integer = 8

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

    Public Enum BatteryLevel As Byte ' Unsigned 8-bit (1-byte) integer range 0 through 255.
        EMPTY = 0
        LOW = 1
        MEDIUM = 2
        FULL = 3
    End Enum

    Private batteryInfo As XINPUT_BATTERY_INFORMATION

    Private Const BATTERY_DEVTYPE_GAMEPAD As Integer = 0

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        InitializeApp()

    End Sub

    Private Sub InitializeTimer1()

        ' The tick frequency in milliseconds.
        ' Also called the polling frequency.
        Timer1.Interval = 15 '1000/60 = 16.67 ms
        ' To get 60 FPS (Frames Per Second) in milliseconds.
        ' We divide 1000 (the number of milliseconds in a second) by 60 the FPS.

        Timer1.Start()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        UpdateControllerData()

        UpdateVibrateTimer()

    End Sub

    Private Sub ButtonVibrateLeft_Click(sender As Object, e As EventArgs) Handles ButtonVibrateLeft.Click

        VibrateLeft(NumControllerToVib.Value, TrackBarSpeed.Value)

    End Sub

    Private Sub ButtonVibrateRight_Click(sender As Object, e As EventArgs) Handles ButtonVibrateRight.Click

        VibrateRight(NumControllerToVib.Value, TrackBarSpeed.Value)

    End Sub

    Private Sub TrackBarSpeed_Scroll(sender As Object, e As EventArgs) Handles TrackBarSpeed.Scroll

        UpdateSpeedLabel()

    End Sub

    Private Sub UpdateControllerData()

        Dim ElapsedTime As TimeSpan = Now - ConnectionStart

        ' Every second check for connected controllers.
        If ElapsedTime.TotalSeconds >= 1 Then

            For controllerNumber As Integer = 0 To 3 ' Up to 4 controllers

                Connected(controllerNumber) = IsControllerConnected(controllerNumber)

                UpdateControllerStatusLabel(controllerNumber)

            Next

            ConnectionStart = DateTime.Now

        End If

        For controllerNumber As Integer = 0 To 3

            If Connected(controllerNumber) Then

                UpdateControllerState(controllerNumber)

            End If

        Next

    End Sub

    Private Sub UpdateControllerStatusLabel(controllerNumber As Integer)
        ' Update the status label based on connection state

        Dim status As String = If(Connected(controllerNumber), "Connected", "Not Connected")

        Dim labelText As String = $"Controller {controllerNumber} {status}"

        Select Case controllerNumber

            Case 0

                LabelController0Status.Text = labelText



            Case 1

                LabelController1Status.Text = labelText



            Case 2

                LabelController2Status.Text = labelText



            Case 3

                LabelController3Status.Text = labelText



        End Select

    End Sub

    Private Sub UpdateControllerState(controllerNumber As Integer)

        Try

            XInputGetState(controllerNumber, ControllerPosition)

            UpdateButtonPosition(controllerNumber)

            UpdateLeftThumbstickPosition(controllerNumber)

            UpdateRightThumbstickPosition(controllerNumber)

            UpdateLeftTriggerPosition(controllerNumber)

            UpdateRightTriggerPosition(controllerNumber)

        Catch ex As Exception
            ' Something went wrong (An exception occurred).

            DisplayError(ex)

        End Try

    End Sub

    Private Sub UpdateButtonPosition(CID As Integer)
        ' The range of buttons is 0 to 65,535. Unsigned 16-bit (2-byte) integer.

        DPadUpPressed = (ControllerPosition.Gamepad.wButtons And DPadUp) <> 0

        DPadDownPressed = (ControllerPosition.Gamepad.wButtons And DPadDown) <> 0

        DPadLeftPressed = (ControllerPosition.Gamepad.wButtons And DPadLeft) <> 0

        DPadRightPressed = (ControllerPosition.Gamepad.wButtons And DPadRight) <> 0

        StartButtonPressed = (ControllerPosition.Gamepad.wButtons And StartButton) <> 0

        BackButtonPressed = (ControllerPosition.Gamepad.wButtons And BackButton) <> 0

        LeftStickButtonPressed = (ControllerPosition.Gamepad.wButtons And LeftStickButton) <> 0

        RightStickButtonPressed = (ControllerPosition.Gamepad.wButtons And RightStickButton) <> 0

        LeftBumperButtonPressed = (ControllerPosition.Gamepad.wButtons And LeftBumperButton) <> 0

        RightBumperButtonPressed = (ControllerPosition.Gamepad.wButtons And RightBumperButton) <> 0

        AButtonPressed = (ControllerPosition.Gamepad.wButtons And AButton) <> 0

        BButtonPressed = (ControllerPosition.Gamepad.wButtons And BButton) <> 0

        XButtonPressed = (ControllerPosition.Gamepad.wButtons And XButton) <> 0

        YButtonPressed = (ControllerPosition.Gamepad.wButtons And YButton) <> 0

        ConButtons(CID) = ControllerPosition.Gamepad.wButtons

        ClearLetterButtonsLabel()

        DoButtonLogic(CID)

    End Sub

    Private Sub DoButtonLogic(ControllerNumber As Integer)

        DoDPadLogic(ControllerNumber)

        DoLetterButtonLogic(ControllerNumber)

        DoStartBackLogic(ControllerNumber)

        DoBumperLogic(ControllerNumber)

        DoStickLogic(ControllerNumber)

    End Sub

    Private Sub DoDPadLogic(controllerNumber As Integer)

        Dim direction As String = GetDPadDirection()

        ' Are all DPad buttons up?
        If Not String.IsNullOrEmpty(direction) Then
            ' No, all DPad buttons are not up.

            LabelDPad.Text = $"Controller {controllerNumber} DPad {direction}"

            IsDPadNeutral(controllerNumber) = False

        Else
            ' Yes, all DPad buttons are up.

            IsDPadNeutral(controllerNumber) = True

        End If

        ClearDPadLabel()

    End Sub

    Private Sub DoLetterButtonLogic(controllerNumber As Integer)

        Dim buttonText As String = GetButtonText(controllerNumber)

        ' Are any letter buttons pressed?
        If Not String.IsNullOrEmpty(buttonText) Then
            ' Yes, letter buttons are pressed.

            LabelButtons.Text = buttonText

            IsLetterButtonsNeutral(controllerNumber) = False

        Else
            ' No, letter buttons are Not pressed.

            IsLetterButtonsNeutral(controllerNumber) = True

        End If

        ClearLetterButtonsLabel()

    End Sub

    Private Sub DoStartBackLogic(ControllerNumber As Integer)

        If StartButtonPressed Then

            LabelStart.Text = $"Controller {ControllerNumber} Start"

            IsStartButtonsNeutral(ControllerNumber) = False

        Else

            IsStartButtonsNeutral(ControllerNumber) = True

        End If

        ClearStartLabel()

        If BackButtonPressed Then

            LabelBack.Text = $"Controller {ControllerNumber} Back"

            IsBackButtonsNeutral(ControllerNumber) = False

        Else

            IsBackButtonsNeutral(ControllerNumber) = True

        End If

        ClearBackLabel()

    End Sub

    Private Sub DoBumperLogic(ControllerNumber As Integer)

        If LeftBumperButtonPressed Then

            LabelLeftBumper.Text = $"Controller {ControllerNumber} Left Bumper"

            IsLeftBumperNeutral(ControllerNumber) = False

        Else

            IsLeftBumperNeutral(ControllerNumber) = True

        End If

        ClearLeftBumperLabel()

        If RightBumperButtonPressed Then

            LabelRightBumper.Text = $"Controller {ControllerNumber} Right Bumper"

            IsRightBumperNeutral(ControllerNumber) = False

        Else

            IsRightBumperNeutral(ControllerNumber) = True

        End If

        ClearRightBumperLabel()

    End Sub

    Private Sub DoStickLogic(ControllerNumber As Integer)

        If LeftStickButtonPressed Then

            LabelLeftThumbButton.Text = $"Controller {ControllerNumber} Left Thumbstick Button"

            IsLeftStickButtonsNeutral(ControllerNumber) = False

        Else

            IsLeftStickButtonsNeutral(ControllerNumber) = True

        End If

        ClearLeftThumbButtonLabel()

        If RightStickButtonPressed Then

            LabelRightThumbButton.Text = $"Controller {ControllerNumber} Right Thumbstick Button"

            IsRightStickButtonsNeutral(ControllerNumber) = False

        Else

            IsRightStickButtonsNeutral(ControllerNumber) = True

        End If

        ClearRightThumbButtonLabel()

    End Sub

    Private Sub UpdateLeftThumbstickPosition(ControllerNumber As Integer)
        ' The range on the X-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.
        ' The range on the Y-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.

        ' What position is the left thumbstick in on the X-axis?
        If ControllerPosition.Gamepad.sThumbLX <= NeutralStart Then
            ' The left thumbstick is in the left position.

            LabelLeftThumbX.Text = $"Controller {ControllerNumber} Left Thumbstick Left"

            IsConThumbLXNeutral(ControllerNumber) = False

        ElseIf ControllerPosition.Gamepad.sThumbLX >= NeutralEnd Then
            ' The left thumbstick is in the right position.

            LabelLeftThumbX.Text = $"Controller {ControllerNumber} Left Thumbstick Right"

            IsConThumbLXNeutral(ControllerNumber) = False

        Else
            ' The left thumbstick is in the neutral position.

            IsConThumbLXNeutral(ControllerNumber) = True

        End If

        ClearLeftThumbstickXLabel()

        ' What position is the left thumbstick in on the Y-axis?
        If ControllerPosition.Gamepad.sThumbLY <= NeutralStart Then
            ' The left thumbstick is in the down position.

            LabelLeftThumbY.Text = $"Controller {ControllerNumber} Left Thumbstick Down"

            IsConThumbLYNeutral(ControllerNumber) = False

        ElseIf ControllerPosition.Gamepad.sThumbLY >= NeutralEnd Then
            ' The left thumbstick is in the up position.

            LabelLeftThumbY.Text = $"Controller {ControllerNumber} Left Thumbstick Up"

            IsConThumbLYNeutral(ControllerNumber) = False

        Else
            ' The left thumbstick is in the neutral position.

            IsConThumbLYNeutral(ControllerNumber) = True

        End If

        ClearLeftThumbstickYLabel()

    End Sub

    Private Sub UpdateRightThumbstickPosition(ControllerNumber As Integer)
        ' The range on the X-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.
        ' The range on the Y-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.

        ' What position is the right thumbstick in on the X-axis?
        If ControllerPosition.Gamepad.sThumbRX <= NeutralStart Then
            ' The right thumbstick is in the left position.

            LabelRightThumbX.Text = $"Controller {ControllerNumber} Right Thumbstick Left"

            IsConThumbRXNeutral(ControllerNumber) = False

        ElseIf ControllerPosition.Gamepad.sThumbRX >= NeutralEnd Then
            ' The right thumbstick is in the right position.

            LabelRightThumbX.Text = $"Controller {ControllerNumber} Right Thumbstick Right"

            IsConThumbRXNeutral(ControllerNumber) = False

        Else
            ' The right thumbstick is in the neutral position.

            IsConThumbRXNeutral(ControllerNumber) = True

        End If

        ClearRightThumbstickXLabel()

        ' What position is the right thumbstick in on the Y-axis?
        If ControllerPosition.Gamepad.sThumbRY <= NeutralStart Then
            ' The right thumbstick is in the down position.

            LabelRightThumbY.Text = $"Controller {ControllerNumber} Right Thumbstick Down"

            IsConThumbRYNeutral(ControllerNumber) = False

        ElseIf ControllerPosition.Gamepad.sThumbRY >= NeutralEnd Then
            ' The right thumbstick is in the up position.

            LabelRightThumbY.Text = $"Controller {ControllerNumber} Right Thumbstick Up"

            IsConThumbRYNeutral(ControllerNumber) = False

        Else
            ' The right thumbstick is in the neutral position.

            IsConThumbRYNeutral(ControllerNumber) = True

        End If

        ClearRightThumbstickYLabel()

    End Sub

    Private Sub UpdateRightTriggerPosition(ControllerNumber As Integer)
        ' The range of right trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.
        ' The trigger position must be greater than the trigger threshold to register as pressed.

        ' What position is the right trigger in?
        If ControllerPosition.Gamepad.bRightTrigger > TriggerThreshold Then
            ' The right trigger is in the down position. Trigger Break. Bang!

            LabelRightTrigger.Text = $"Controller {ControllerNumber} Right Trigger"

            IsConRightTriggerNeutral(ControllerNumber) = False

        Else
            ' The right trigger is in the neutral position. Pre-Travel.

            IsConRightTriggerNeutral(ControllerNumber) = True

        End If

        ClearRightTriggerLabel()

    End Sub

    Private Sub UpdateLeftTriggerPosition(ControllerNumber As Integer)
        ' The range of left trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.
        ' The trigger position must be greater than the trigger threshold to register as pressed.

        ' What position is the left trigger in?
        If ControllerPosition.Gamepad.bLeftTrigger > TriggerThreshold Then
            ' The left trigger is in the fire position. Trigger Break. Bang!

            LabelLeftTrigger.Text = $"Controller {ControllerNumber} Left Trigger"

            IsConLeftTriggerNeutral(ControllerNumber) = False

        Else
            ' The left trigger is in the neutral position. Pre-Travel.

            IsConLeftTriggerNeutral(ControllerNumber) = True

        End If

        ClearLeftTriggerLabel()

    End Sub

    Private Sub ClearLetterButtonsLabel()
        ' Clears the letter buttons label when all controllers' letter buttons are up.

        Dim ConSum As Boolean = True ' Assume all controllers' letter buttons are neutral initially.

        ' Search for a non-neutral letter button.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsLetterButtonsNeutral(i) Then
                ' A non-neutral letter button was found.

                ConSum = False ' Report the non-neutral letter button.

                Exit For ' No need to search further, so stop the search.

            End If

        Next

        ' Are all controllers' letter buttons in the neutral position?
        If ConSum Then
            ' Yes, all controllers' letter buttons are in the neutral position.

            LabelButtons.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearLeftThumbstickYLabel()
        ' Clears the left thumbstick Y-axis label when all controllers left thumbsticks on the Y-axis are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers left thumbsticks on the Y-axis are neutral initially.

        ' Search for a non-neutral left thumbstick on the Y-axis.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsConThumbLYNeutral(i) Then
                ' A non-neutral thumbstick was found.

                ConSum = False ' Report the non-neutral thumbstick.

                Exit For ' No need to search further so stop the search.

            End If

        Next

        ' Are all controllers left thumbsticks on the Y-axis in the neutral position?
        If ConSum = True Then
            ' Yes, all controllers left thumbsticks on the Y-axis are in the neutral position.

            LabelLeftThumbY.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearLeftThumbstickXLabel()
        ' Clears the left thumbstick X-axis label when all controllers left thumbsticks on the X-axis are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers left thumbsticks on the X-axis are neutral initially.

        ' Search for a non-neutral left thumbstick on the X-axis.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsConThumbLXNeutral(i) Then
                ' A non-neutral thumbstick was found.

                ConSum = False ' Report the non-neutral thumbstick.

                Exit For ' No need to search further so stop the search.

            End If

        Next

        ' Are all controllers left thumbsticks on the X-axis in the neutral position?
        If ConSum = True Then
            ' Yes, all controllers left thumbsticks on the X-axis are in the neutral position.

            LabelLeftThumbX.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearRightThumbstickXLabel()
        ' Clears the right thumbstick X-axis label when all controllers right thumbsticks on the X-axis are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers right thumbsticks on the X-axis are neutral initially.

        ' Search for a non-neutral right thumbstick on the X-axis.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsConThumbRXNeutral(i) Then
                ' A non-neutral thumbstick was found.

                ConSum = False ' Report the non-neutral thumbstick.

                Exit For ' No need to search further so stop the search.

            End If

        Next

        ' Are all controllers right thumbsticks on the X-axis in the neutral position?
        If ConSum = True Then
            ' Yes, all controllers right thumbsticks on the X-axis are in the neutral position.

            LabelRightThumbX.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearRightThumbstickYLabel()
        ' Clears the right thumbstick Y-axis label when all controllers right thumbsticks on the Y-axis are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers right thumbsticks on the Y-axis are neutral initially.

        ' Search for a non-neutral right thumbstick on the Y-axis.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsConThumbRYNeutral(i) Then
                ' A non-neutral thumbstick was found.

                ConSum = False ' Report the non-neutral thumbstick.

                Exit For ' No need to search further so stop the search.

            End If

        Next

        ' Are all controllers right thumbsticks on the Y-axis in the neutral position?
        If ConSum = True Then
            ' Yes, all controllers right thumbsticks on the Y-axis are in the neutral position.

            LabelRightThumbY.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearRightTriggerLabel()
        ' Clears the right trigger label when all controllers right triggers are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers right triggers are neutral initially.

        ' Search for a non-neutral right trigger.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsConRightTriggerNeutral(i) Then
                ' A non-neutral right trigger was found.

                ConSum = False ' Report the non-neutral right trigger.

                Exit For  ' No need to search further, so stop the search.

            End If

        Next

        ' Are all controllers right triggers in the neutral position?
        If ConSum = True Then
            ' Yes, all controllers right triggers are in the neutral position.

            LabelRightTrigger.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearLeftTriggerLabel()
        ' Clears the left trigger label when all controllers left triggers are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers left triggers are neutral initially.

        ' Search for a non-neutral left trigger.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsConLeftTriggerNeutral(i) Then
                ' A non-neutral left trigger was found.

                ConSum = False ' Report the non-neutral left trigger.

                Exit For ' No need to search further so stop the search.

            End If

        Next

        ' Are all controllers left triggers in the neutral position?
        If ConSum = True Then
            ' Yes, all controllers left triggers are in the neutral position.

            LabelLeftTrigger.Text = String.Empty ' Clear label. 

        End If

    End Sub

    Private Sub ClearLabels()

        LabelButtons.Text = String.Empty

        LabelLeftThumbX.Text = String.Empty

        LabelLeftThumbY.Text = String.Empty

        LabelLeftTrigger.Text = String.Empty

        LabelRightThumbX.Text = String.Empty

        LabelRightThumbY.Text = String.Empty

        LabelRightTrigger.Text = String.Empty

    End Sub

    Private Sub ClearDPadLabel()
        ' Clears the DPad label when all controllers' DPad are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers' DPad are neutral initially.

        ' Search for a non-neutral DPad.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsDPadNeutral(i) Then
                ' A non-neutral DPad was found.

                ConSum = False ' Report the non-neutral DPad.

                Exit For ' No need to search further, so stop the search.

            End If

        Next

        ' Are all controllers' DPad in the neutral position?
        If ConSum Then
            ' Yes, all controllers' DPad are in the neutral position.

            LabelDPad.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearStartLabel()
        ' Clears the start label when all controllers' start buttons are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers' start buttons are neutral initially.

        ' Search for a non-neutral start button.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsStartButtonsNeutral(i) Then
                ' A non-neutral start button was found.

                ConSum = False ' Report the non-neutral start button.

                Exit For ' No need to search further, so stop the search.

            End If

        Next

        ' Are all controllers' start buttons in the neutral position?
        If ConSum Then
            ' Yes, all controllers' start buttons are in the neutral position.

            LabelStart.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearBackLabel()
        ' Clears the back label when all controllers' back buttons are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers' back buttons are neutral initially.

        ' Search for a non-neutral back button.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsBackButtonsNeutral(i) Then
                ' A non-neutral back button was found.

                ConSum = False ' Report the non-neutral back button.

                Exit For ' No need to search further, so stop the search.

            End If

        Next

        ' Are all controllers' back buttons in the neutral position?
        If ConSum Then
            ' Yes, all controllers' back buttons are in the neutral position.

            LabelBack.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearLeftBumperLabel()
        ' Clears the left bumper label when all controllers' left bumpers are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers' left bumpers are neutral initially.

        ' Search for a non-neutral left bumper.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsLeftBumperNeutral(i) Then
                ' A non-neutral left bumper was found.

                ConSum = False ' Report the non-neutral left bumper.

                Exit For ' No need to search further, so stop the search.

            End If

        Next

        ' Are all controllers' left bumpers in the neutral position?
        If ConSum Then
            ' Yes, all controllers' left bumpers are in the neutral position.

            LabelLeftBumper.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearRightBumperLabel()
        ' Clears the right bumper label when all controllers' right bumpers are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers' right bumpers are neutral initially.

        ' Search for a non-neutral right bumper.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsRightBumperNeutral(i) Then
                ' A non-neutral right bumper was found.

                ConSum = False ' Report the non-neutral right bumper.

                Exit For ' No need to search further, so stop the search.

            End If

        Next

        ' Are all controllers' right bumpers in the neutral position?
        If ConSum Then
            ' Yes, all controllers' right bumpers are in the neutral position.

            LabelRightBumper.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearLeftThumbButtonLabel()
        ' Clears the left thumb button label when all controllers' left thumb buttons are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers' left thumb buttons are neutral initially.

        ' Search for a non-neutral left thumb button.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsLeftStickButtonsNeutral(i) Then
                ' A non-neutral left thumb button was found.

                ConSum = False ' Report the non-neutral left thumb button.

                Exit For ' No need to search further, so stop the search.

            End If

        Next

        ' Are all controllers' left thumb buttons in the neutral position?
        If ConSum Then
            ' Yes, all controllers' left thumb buttons are in the neutral position.

            LabelLeftThumbButton.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearRightThumbButtonLabel()
        ' Clears the right thumb button label when all controllers' right thumb buttons are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers' right thumb buttons are neutral initially.

        ' Search for a non-neutral right thumb button.
        For i As Integer = 0 To 3

            If Connected(i) AndAlso Not IsRightStickButtonsNeutral(i) Then
                ' A non-neutral right thumb button was found.

                ConSum = False ' Report the non-neutral right thumb button.

                Exit For ' No need to search further, so stop the search.

            End If

        Next

        ' Are all controllers' right thumb buttons in the neutral position?
        If ConSum Then
            ' Yes, all controllers' right thumb buttons are in the neutral position.

            LabelRightThumbButton.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Function GetDPadDirection() As String

        If DPadUpPressed Then

            If DPadLeftPressed Then Return "Left+Up"

            If DPadRightPressed Then Return "Right+Up"

            Return "Up"

        End If

        If DPadDownPressed Then

            If DPadLeftPressed Then Return "Left+Down"

            If DPadRightPressed Then Return "Right+Down"

            Return "Down"

        End If

        If DPadLeftPressed Then Return "Left"

        If DPadRightPressed Then Return "Right"

        Return String.Empty ' Return an empty string if no buttons are pressed.

    End Function

    Private Function GetButtonText(controllerNumber As Integer) As String

        Dim buttons As New List(Of String)

        If AButtonPressed Then buttons.Add("A")

        If BButtonPressed Then buttons.Add("B")

        If XButtonPressed Then buttons.Add("X")

        If YButtonPressed Then buttons.Add("Y")

        If buttons.Count > 0 Then

            Return $"Controller {controllerNumber} Buttons: {String.Join("+", buttons)}"

        End If

        Return String.Empty ' Return an empty string if no buttons are pressed

    End Function

    Private Sub VibrateLeft(CID As Integer, Speed As UShort)
        ' The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        ' The left motor is the low-frequency rumble motor.

        ' Set left motor speed.
        Vibration.wLeftMotorSpeed = Speed

        SendVibrationMotorCommand(CID)

        LeftVibrateStart(CID) = Now

        IsLeftVibrating(CID) = True

    End Sub

    Private Sub VibrateRight(CID As Integer, Speed As UShort)
        ' The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        ' The right motor is the high-frequency rumble motor.

        ' Set right motor speed.
        Vibration.wRightMotorSpeed = Speed

        SendVibrationMotorCommand(CID)

        RightVibrateStart(CID) = Now

        IsRightVibrating(CID) = True

    End Sub

    Private Sub SendVibrationMotorCommand(ControllerID As Integer)
        ' Sends vibration motor speed command to the specified controller.

        Try

            ' Send motor speed command to the specified controller.
            If XInputSetState(ControllerID, Vibration) = 0 Then
                ' The motor speed was set. Success.

            Else
                ' The motor speed was not set. Fail.
                ' You can log or handle the failure here if needed.
                ' Example: Console.WriteLine(XInputSetState(ControllerID, Vibration).ToString())

            End If

        Catch ex As Exception

            DisplayError(ex)

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

                If ElapsedTime.TotalMilliseconds >= NumericUpDownTimeToVib.Value Then

                    IsLeftVibrating(Index) = False

                    ' Turn left motor off (set zero speed).
                    Vibration.wLeftMotorSpeed = 0

                    SendVibrationMotorCommand(Index)

                End If

            End If

        Next

    End Sub

    Private Sub UpdateRightVibrateTimer()

        For Each IsConVibrating In IsRightVibrating

            Dim Index As Integer = Array.IndexOf(IsRightVibrating, IsConVibrating)

            If Index <> -1 AndAlso IsConVibrating = True Then

                Dim ElapsedTime As TimeSpan = Now - RightVibrateStart(Index)

                If ElapsedTime.TotalMilliseconds >= NumericUpDownTimeToVib.Value Then

                    IsRightVibrating(Index) = False

                    ' Turn right motor off (set zero speed).
                    Vibration.wRightMotorSpeed = 0

                    SendVibrationMotorCommand(Index)

                End If

            End If

        Next

    End Sub

    Private Sub UpdateSpeedLabel()

        LabelSpeed.Text = $"Speed: {TrackBarSpeed.Value}"

    End Sub

    Private Function IsControllerConnected(controllerNumber As Integer) As Boolean

        Try

            Return XInputGetState(controllerNumber, ControllerPosition) = 0 ' 0 means the controller is connected.
            ' Anything else (a non-zero value) means the controller is not connected.

        Catch ex As Exception
            ' Something went wrong (An exception occured).

            DisplayError(ex)

            Return False

        End Try

    End Function

    Private Sub DisplayError(ex As Exception)

        MsgBox(ex.ToString()) ' Display the exception message in a message box.

    End Sub

    Private Sub InitializeApp()

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

        LabelBatteryLevel.Text = String.Empty

        LabelBatteryType.Text = String.Empty

        InitializeToolTips()

    End Sub

    Private Sub InitializeToolTips()

        Dim ToolTipTimeToVib As New ToolTip With {
            .AutoPopDelay = 8000,
            .InitialDelay = 1000,
            .ReshowDelay = 500
        }

        Dim TipText As String = $"Time to Vibrate {Environment.NewLine}Enter a value between 1 and 5000 milliseconds{Environment.NewLine}1 second = 1000 milliseconds"

        ToolTipTimeToVib.SetToolTip(NumericUpDownTimeToVib, TipText)


        Dim ToolTipConToVib As New ToolTip With {
            .AutoPopDelay = 8000,
            .InitialDelay = 1000,
            .ReshowDelay = 500
        }

        TipText = $"Controller to Vibrate {Environment.NewLine}Enter a value between 0 and 3 {Environment.NewLine}Supports up to 4 controllers"

        ToolTipConToVib.SetToolTip(NumControllerToVib, TipText)


        Dim ToolTipVibSpeed As New ToolTip() With {
            .AutoPopDelay = 10000,
            .InitialDelay = 1000,
            .ReshowDelay = 500
        }

        TipText = $"Vibration Speed {Environment.NewLine}Enter a value between 1 and 65,535 {Environment.NewLine}Higher speeds can create stronger feedback {Environment.NewLine}while lower speeds produce a more subtle effect"

        ToolTipVibSpeed.SetToolTip(TrackBarSpeed, TipText)


        Dim ToolTipRumbleGroup As New ToolTip() With {
            .AutoPopDelay = 10000,
            .InitialDelay = 1000,
            .ReshowDelay = 500
        }

        TipText = $"The vibration motors in controllers {Environment.NewLine}provide haptic feedback during gameplay {Environment.NewLine}enhancing the immersive experience"

        ToolTipRumbleGroup.SetToolTip(GroupBox1, TipText)

    End Sub

    Private Sub ClearButtonsLabel()
        ' Clears the buttons label when all controllers buttons are up.

        Dim ConSum As Integer = 0

        For Each Con In ConButtons

            ConSum += Con

        Next

        ' Are all controllers buttons up?
        If ConSum = 0 Then
            ' Yes, all controller buttons are up.

            LabelButtons.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub UpdateBatteryInfo()

        ' Get battery level
        If XInputGetBatteryInformation(NumControllerToVib.Value, BATTERY_DEVTYPE_GAMEPAD, batteryInfo) = 0 Then
            ' Success

            UpdateBatteryLevel()

            UpdateBatteryType()

        Else
            ' Fail

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

            ' This value is only valid for wireless controllers with a known battery type.
            If .BatteryType = BATTERY_TYPE.ALKALINE Or .BatteryType = BATTERY_TYPE.NIMH Then
                ' Valid
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
                ' Invalid
                LabelBatteryLevel.Text = ""
            End If

        End With

    End Sub

    Private Sub ClearBatteryLabels()

        LabelBatteryLevel.Text = ""

        LabelBatteryType.Text = ""

    End Sub

End Class


' Consuming Unmanaged DLL Functions

' Consuming unmanaged DLL functions refers to the process of using functions that are defined in a
' DLL (Dynamic Link Library) which is written in a language like C or C++. This involves using
' Platform Invocation Services (P/Invoke) to call functions in the unmanaged DLL from your managed
' VB code. To consume unmanaged DLL functions use the DllImport attribute to declare the external
' functions from the DLL.

' https://learn.microsoft.com/en-us/dotnet/framework/interop/consuming-unmanaged-dll-functions


' Passing Structures

' Passing structures refers to the process of sending structured data as a parameter to a function
' or method. Structures, also known as structs, allow you to group related data together under a
' single name. When passing structures as parameters you are essentially sending a block of data
' that contains multiple fields or members. This can be useful for organizing related data and
' passing them around your program efficiently.

' https://learn.microsoft.com/en-us/dotnet/framework/interop/passing-structures


' XInputGetState Function

' The XInputGetState function is used to retrieve the current state of a Xbox controller.

' https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputgetstate


' XINPUT_STATE Structure

' The XINPUT_STATE structure is used to hold the current state of an Xbox controller.

' https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_state


' XINPUT_GAMEPAD Structure

' The XINPUT_GAMEPAD structure represents the state of the gamepad (Xbox controller) input,
' including information about button presses, trigger values, and thumbstick positions.

' https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_gamepad


' XInputSetState Function
' https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputsetstate


' XINPUT_VIBRATION Structure
' https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_vibration


' XInputGetBatteryInformation Function
' https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputgetbatteryinformation


' XINPUT_BATTERY_INFORMATION Structure
' https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_battery_information


' Getting Started with XInput in Windows Applications
' https://learn.microsoft.com/en-us/windows/win32/xinput/getting-started-with-xinput


' XInput Game Controller APIs
' https://learn.microsoft.com/en-us/windows/win32/api/_xinput/


' XInput Versions
' https://learn.microsoft.com/en-us/windows/win32/xinput/xinput-versions


' Comparison of XInput and DirectInput Features
' https://learn.microsoft.com/en-us/windows/win32/xinput/xinput-and-directinput


' Data Type Summary (Visual Basic)
' https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/


' Monica is our an AI assistant.
' https://monica.im/


' I also make coding videos on my YouTube channel.
' https://www.youtube.com/@codewithjoe6074
