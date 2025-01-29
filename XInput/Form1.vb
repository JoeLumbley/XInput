' XInput

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

Public Structure XboxControllers

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputGetState(dwUserIndex As Integer, ByRef pState As XINPUT_STATE) As Integer
    End Function

    <StructLayout(LayoutKind.Explicit)>
    Public Structure XINPUT_STATE
        <FieldOffset(0)>
        Public dwPacketNumber As UInteger ' Unsigned 32-bit (4-byte) integer range 0 through 4,294,967,295.
        <FieldOffset(4)>
        Public Gamepad As XINPUT_GAMEPAD
    End Structure

    Private State As XINPUT_STATE


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

    Private Enum Button
        DPadUp = 1
        DPadDown = 2
        DPadLeft = 4
        DPadRight = 8
        Start = 16
        Back = 32
        LeftStick = 64
        RightStick = 128
        LeftBumper = 256
        RightBumper = 512
        A = 4096
        B = 8192
        X = 16384
        Y = 32768
    End Enum

    ' Set the start of the thumbstick neutral zone to 1/2 over.
    Private Const NeutralStart As Short = -16384 '-16,384 = -32,768 / 2
    ' The thumbstick position must be more than 1/2 over the neutral start to
    ' register as moved.
    ' A short is a signed 16-bit (2-byte) integer range -32,768 through 32,767.
    ' This gives us 65,536 values.

    ' Set the end of the thumbstick neutral zone to 1/2 over.
    Private Const NeutralEnd As Short = 16384 '16,383.5 = 32,767 / 2
    ' The thumbstick position must be more than 1/2 over the neutral end to
    ' register as moved.

    ' Set the trigger threshold to 1/4 pull.
    Private Const TriggerThreshold As Byte = 64 '64 = 256 / 4
    ' The trigger position must be greater than the trigger threshold to
    ' register as pulled.
    ' A byte is a unsigned 8-bit (1-byte) integer range 0 through 255.
    ' This gives us 256 values.

    Public Connected() As Boolean

    Private ConnectionStart As Date

    Public Buttons() As UShort

    Public LeftTriggerNeutral() As Boolean
    Public RightTriggerNeutral() As Boolean

    Public LeftThumbstickXaxisNeutral() As Boolean
    Public LeftThumbstickYaxisNeutral() As Boolean

    Public RightThumbstickXaxisNeutral() As Boolean
    Public RightThumbstickYaxisNeutral() As Boolean

    Public DPadNeutral() As Boolean

    Public LeftBumperNeutral() As Boolean
    Public RightBumperNeutral() As Boolean

    Public LetterButtonsNeutral() As Boolean

    Public StartNeutral() As Boolean
    Public BackNeutral() As Boolean

    Public LeftStickNeutral() As Boolean
    Public RightStickNeutral() As Boolean

    Public DPadUp() As Boolean
    Public DPadDown() As Boolean
    Public DPadLeft() As Boolean
    Public DPadRight() As Boolean

    Public Start() As Boolean
    Public Back() As Boolean

    Public LeftStick() As Boolean
    Public RightStick() As Boolean

    Public LeftBumper() As Boolean
    Public RightBumper() As Boolean

    Public A() As Boolean
    Public B() As Boolean
    Public X() As Boolean
    Public Y() As Boolean

    Public RightThumbstickUp() As Boolean
    Public RightThumbstickDown() As Boolean
    Public RightThumbstickLeft() As Boolean
    Public RightThumbstickRight() As Boolean

    Public LeftThumbstickUp() As Boolean
    Public LeftThumbstickDown() As Boolean
    Public LeftThumbstickLeft() As Boolean
    Public LeftThumbstickRight() As Boolean

    Public LeftTrigger() As Boolean
    Public RightTrigger() As Boolean

    Public TimeToVibe As Integer

    Private LeftVibrateStart() As Date

    Private RightVibrateStart() As Date

    Private IsLeftVibrating() As Boolean

    Private IsRightVibrating() As Boolean

    Public Sub Init()

        ' Initialize the Connected array to indicate whether controllers are connected.
        Connected = New Boolean(0 To 3) {}

        ' Record the current date and time when initialization starts.
        ConnectionStart = DateTime.Now

        ' Initialize the Buttons array to store the state of controller buttons.
        Buttons = New UShort(0 To 3) {}

        ' Initialize arrays to check if thumbstick axes are in the neutral position.
        LeftThumbstickXaxisNeutral = New Boolean(0 To 3) {}
        LeftThumbstickYaxisNeutral = New Boolean(0 To 3) {}
        RightThumbstickXaxisNeutral = New Boolean(0 To 3) {}
        RightThumbstickYaxisNeutral = New Boolean(0 To 3) {}

        ' Initialize arrays to check if triggers are in the neutral position.
        LeftTriggerNeutral = New Boolean(0 To 3) {}
        RightTriggerNeutral = New Boolean(0 To 3) {}

        ' Initialize array to check if the D-Pad is in the neutral position.
        DPadNeutral = New Boolean(0 To 3) {}

        ' Initialize array to check if letter buttons are in the neutral position.
        LetterButtonsNeutral = New Boolean(0 To 3) {}

        ' Initialize arrays to check if start and back buttons are in the neutral position.
        StartNeutral = New Boolean(0 To 3) {}
        BackNeutral = New Boolean(0 To 3) {}

        ' Initialize arrays to check if bumpers are in the neutral position.
        LeftBumperNeutral = New Boolean(0 To 3) {}
        RightBumperNeutral = New Boolean(0 To 3) {}

        ' Initialize arrays to check if stick buttons are in the neutral position.
        LeftStickNeutral = New Boolean(0 To 3) {}
        RightStickNeutral = New Boolean(0 To 3) {}

        ' Set all thumbstick axes, triggers, D-Pad, letter buttons, start/back buttons, bumpers,
        ' and stick buttons to neutral for all controllers (indices 0 to 3).
        For i As Integer = 0 To 3

            LeftThumbstickXaxisNeutral(i) = True
            LeftThumbstickYaxisNeutral(i) = True
            RightThumbstickXaxisNeutral(i) = True
            RightThumbstickYaxisNeutral(i) = True

            LeftTriggerNeutral(i) = True
            RightTriggerNeutral(i) = True

            DPadNeutral(i) = True

            LetterButtonsNeutral(i) = True

            StartNeutral(i) = True
            BackNeutral(i) = True

            LeftBumperNeutral(i) = True
            RightBumperNeutral(i) = True

            LeftStickNeutral(i) = True
            RightStickNeutral(i) = True

        Next

        ' Initialize arrays for thumbstick directional states.
        RightThumbstickLeft = New Boolean(0 To 3) {}
        RightThumbstickRight = New Boolean(0 To 3) {}
        RightThumbstickDown = New Boolean(0 To 3) {}
        RightThumbstickUp = New Boolean(0 To 3) {}
        LeftThumbstickLeft = New Boolean(0 To 3) {}
        LeftThumbstickRight = New Boolean(0 To 3) {}
        LeftThumbstickDown = New Boolean(0 To 3) {}
        LeftThumbstickUp = New Boolean(0 To 3) {}

        ' Initialize arrays for trigger states.
        LeftTrigger = New Boolean(0 To 3) {}
        RightTrigger = New Boolean(0 To 3) {}

        ' Initialize arrays for letter button states (A, B, X, Y).
        A = New Boolean(0 To 3) {}
        B = New Boolean(0 To 3) {}
        X = New Boolean(0 To 3) {}
        Y = New Boolean(0 To 3) {}

        ' Initialize arrays for bumper button states.
        LeftBumper = New Boolean(0 To 3) {}
        RightBumper = New Boolean(0 To 3) {}

        ' Initialize arrays for D-Pad directional states.
        DPadUp = New Boolean(0 To 3) {}
        DPadDown = New Boolean(0 To 3) {}
        DPadLeft = New Boolean(0 To 3) {}
        DPadRight = New Boolean(0 To 3) {}

        ' Initialize arrays for start and back button states.
        Start = New Boolean(0 To 3) {}
        Back = New Boolean(0 To 3) {}

        ' Initialize arrays for stick button states.
        LeftStick = New Boolean(0 To 3) {}
        RightStick = New Boolean(0 To 3) {}

        TimeToVibe = 1000 'ms

        LeftVibrateStart = New Date(0 To 3) {}
        RightVibrateStart = New Date(0 To 3) {}

        For i As Integer = 0 To 3

            LeftVibrateStart(i) = Now

            RightVibrateStart(i) = Now

        Next

        IsLeftVibrating = New Boolean(0 To 3) {}
        IsRightVibrating = New Boolean(0 To 3) {}


        ' Call the TestInitialization method to verify the initial state of the controllers.
        TestInitialization()

    End Sub

    Public Sub TestInitialization()

        ' Check that ConnectionStart is not Nothing (initialization was successful)
        Debug.Assert(Not ConnectionStart = Nothing, $"Connection Start should not be Nothing.")

        ' Check that Buttons array is initialized
        Debug.Assert(Buttons IsNot Nothing, $"Buttons should not be Nothing.")

        Debug.Assert(Not TimeToVibe = Nothing, $"TimeToVibe should not be Nothing.")

        For i As Integer = 0 To 3

            ' Check that all controllers are initialized as not connected
            Debug.Assert(Not Connected(i), $"Controller {i} should not be connected after initialization.")

            ' Check that all axes of the Left Thumbsticks are initialized as neutral. 
            Debug.Assert(LeftThumbstickXaxisNeutral(i), $"Left Thumbstick X-axis for Controller {i} should be neutral.")
            Debug.Assert(LeftThumbstickYaxisNeutral(i), $"Left Thumbstick Y-axis for Controller {i} should be neutral.")

            ' Check that all axes of the Right Thumbsticks are initialized as neutral. 
            Debug.Assert(RightThumbstickXaxisNeutral(i), $"Right Thumbstick X-axis for Controller {i} should be neutral.")
            Debug.Assert(RightThumbstickYaxisNeutral(i), $"Right Thumbstick Y-axis for Controller {i} should be neutral.")

            ' Check that all Triggers are initialized as neutral. 
            Debug.Assert(LeftTriggerNeutral(i), $"Left Trigger for Controller {i} should be neutral.")
            Debug.Assert(RightTriggerNeutral(i), $"Right Trigger for Controller {i} should be neutral.")

            ' Check that all DPads are initialized as neutral. 
            Debug.Assert(DPadNeutral(i), $"DPad for Controller {i} should be neutral.")

            ' Check that all Letter Buttons are initialized as neutral. 
            Debug.Assert(LetterButtonsNeutral(i), $"Letter Buttons for Controller {i} should be neutral.")

            ' Check that Start and Back Buttons are initialized as neutral. 
            Debug.Assert(StartNeutral(i), $"Start button for Controller {i} should be neutral.")
            Debug.Assert(BackNeutral(i), $"Back button for Controller {i} should be neutral.")

            ' Check that Bumpers are initialized as neutral. 
            Debug.Assert(LeftBumperNeutral(i), $"Left Bumper for Controller {i} should be neutral.")
            Debug.Assert(RightBumperNeutral(i), $"Right Bumper for Controller {i} should be neutral.")

            ' Check that Sticks are initialized as neutral. 
            Debug.Assert(LeftStickNeutral(i), $"Left Stick for Controller {i} should be neutral.")
            Debug.Assert(RightStickNeutral(i), $"Right Stick for Controller {i} should be neutral.")

            ' Check that additional Right Thumbstick states are not active.
            Debug.Assert(Not RightThumbstickLeft(i), $"Right Thumbstick Left for Controller {i} should not be true.")
            Debug.Assert(Not RightThumbstickRight(i), $"Right Thumbstick Right for Controller {i} should not be true.")
            Debug.Assert(Not RightThumbstickDown(i), $"Right Thumbstick Down for Controller {i} should not be true.")
            Debug.Assert(Not RightThumbstickUp(i), $"Right Thumbstick Up for Controller {i} should not be true.")

            ' Check that additional Left Thumbstick states are not active.
            Debug.Assert(Not LeftThumbstickLeft(i), $"Left Thumbstick Left for Controller {i} should not be true.")
            Debug.Assert(Not LeftThumbstickRight(i), $"Left Thumbstick Right for Controller {i} should not be true.")
            Debug.Assert(Not LeftThumbstickDown(i), $"Left Thumbstick Down for Controller {i} should not be true.")
            Debug.Assert(Not LeftThumbstickUp(i), $"Left Thumbstick Up for Controller {i} should not be true.")

            ' Check that trigger states are not active.
            Debug.Assert(Not LeftTrigger(i), $"Left Trigger for Controller {i} should not be true.")
            Debug.Assert(Not RightTrigger(i), $"Right Trigger for Controller {i} should not be true.")

            ' Check that letter button states (A, B, X, Y) are not active.
            Debug.Assert(Not A(i), $"A for Controller {i} should not be true.")
            Debug.Assert(Not B(i), $"B for Controller {i} should not be true.")
            Debug.Assert(Not X(i), $"X for Controller {i} should not be true.")
            Debug.Assert(Not Y(i), $"Y for Controller {i} should not be true.")

            ' Check that bumper button states are not active.
            Debug.Assert(Not LeftBumper(i), $"Left Bumper for Controller {i} should not be true.")
            Debug.Assert(Not RightBumper(i), $"Right Bumper for Controller {i} should not be true.")

            ' Check that D-Pad directional states are not active.
            Debug.Assert(Not DPadUp(i), $"D-Pad Up for Controller {i} should not be true.")
            Debug.Assert(Not DPadDown(i), $"D-Pad Down for Controller {i} should not be true.")
            Debug.Assert(Not DPadLeft(i), $"D-Pad Left for Controller {i} should not be true.")
            Debug.Assert(Not DPadRight(i), $"D-Pad Right for Controller {i} should not be true.")

            ' Check that start and back button states are not active.
            Debug.Assert(Not Start(i), $"Start Button for Controller {i} should not be true.")
            Debug.Assert(Not Back(i), $"Back Button for Controller {i} should not be true.")

            ' Check that stick button states are not active.
            Debug.Assert(Not LeftStick(i), $"Left Stick for Controller {i} should not be true.")
            Debug.Assert(Not RightStick(i), $"Right Stick for Controller {i} should not be true.")


            Debug.Assert(Not LeftVibrateStart(i) = Nothing, $"Left Vibrate Start for Controller {i} should not be Nothing.")
            Debug.Assert(Not RightVibrateStart(i) = Nothing, $"Right Vibrate Start for Controller {i} should not be Nothing.")

            Debug.Assert(Not IsLeftVibrating(i), $"Is Left Vibrating for Controller {i} should not be true.")
            Debug.Assert(Not IsRightVibrating(i), $"Is Right Vibrating for Controller {i} should not be true.")


        Next

        ' For Lex

    End Sub

    Public Sub Update()

        Dim ElapsedTime As TimeSpan = Now - ConnectionStart

        ' Every second check for connected controllers.
        If ElapsedTime.TotalSeconds >= 1 Then

            For controllerNumber As Integer = 0 To 3 ' Up to 4 controllers

                Connected(controllerNumber) =
                    IsConnected(controllerNumber)

            Next

            ConnectionStart = DateTime.Now

        End If

        For controllerNumber As Integer = 0 To 3

            If Connected(controllerNumber) Then

                UpdateState(controllerNumber)

            End If

        Next

        UpdateVibrateTimer()

    End Sub

    Public Function IsConnected(controllerNumber As Integer) As Boolean

        Try

            Return XInputGetState(controllerNumber, State) = 0
            ' 0 means the controller is connected.
            ' Anything else (a non-zero value) means the controller is not
            ' connected.

        Catch ex As Exception
            ' Something went wrong (An exception occured).

            DisplayError(ex)

            Return False

        End Try

    End Function

    Public Sub UpdateState(controllerNumber As Integer)

        Try

            XInputGetState(controllerNumber, State)

            UpdateButtons(controllerNumber)

            UpdateLeftThumbstick(controllerNumber)

            UpdateRightThumbstick(controllerNumber)

            UpdateLeftTrigger(controllerNumber)

            UpdateRightTrigger(controllerNumber)

        Catch ex As Exception
            ' Something went wrong (An exception occurred).

            DisplayError(ex)

        End Try

    End Sub

    Private Sub UpdateButtons(CID As Integer)
        ' The range of buttons is 0 to 65,535. Unsigned 16-bit (2-byte) integer.

        UpdateDPadButtons(CID)

        UpdateLetterButtons(CID)

        UpdateBumperButtons(CID)

        UpdateStickButtons(CID)

        UpdateStartBackButtons(CID)

        ' Store the button states for the current controller ID
        Buttons(CID) = State.Gamepad.wButtons

    End Sub

    Private Sub UpdateDPadButtons(CID As Integer)

        DPadUp(CID) = (State.Gamepad.wButtons And Button.DPadUp) <> 0
        DPadDown(CID) = (State.Gamepad.wButtons And Button.DPadDown) <> 0
        DPadLeft(CID) = (State.Gamepad.wButtons And Button.DPadLeft) <> 0
        DPadRight(CID) = (State.Gamepad.wButtons And Button.DPadRight) <> 0

    End Sub

    Private Sub UpdateLetterButtons(CID As Integer)

        A(CID) = (State.Gamepad.wButtons And Button.A) <> 0
        B(CID) = (State.Gamepad.wButtons And Button.B) <> 0
        X(CID) = (State.Gamepad.wButtons And Button.X) <> 0
        Y(CID) = (State.Gamepad.wButtons And Button.Y) <> 0

    End Sub

    Private Sub UpdateBumperButtons(CID As Integer)

        LeftBumper(CID) = (State.Gamepad.wButtons And Button.LeftBumper) <> 0
        RightBumper(CID) = (State.Gamepad.wButtons And Button.RightBumper) <> 0

    End Sub

    Private Sub UpdateStickButtons(CID As Integer)

        LeftStick(CID) = (State.Gamepad.wButtons And Button.LeftStick) <> 0
        RightStick(CID) = (State.Gamepad.wButtons And Button.RightStick) <> 0

    End Sub

    Private Sub UpdateStartBackButtons(CID As Integer)

        Start(CID) = (State.Gamepad.wButtons And Button.Start) <> 0
        Back(CID) = (State.Gamepad.wButtons And Button.Back) <> 0

    End Sub

    Private Sub UpdateLeftThumbstick(ControllerNumber As Integer)
        ' The range on the X-axis is -32,768 through 32,767.
        ' Signed 16-bit (2-byte) integer.
        ' The range on the Y-axis is -32,768 through 32,767.
        ' Signed 16-bit (2-byte) integer.

        ' What position is the left thumbstick in on the X-axis?
        If State.Gamepad.sThumbLX <= NeutralStart Then
            ' The left thumbstick is in the left position.

            LeftThumbstickLeft(ControllerNumber) = True

            LeftThumbstickRight(ControllerNumber) = False

            LeftThumbstickXaxisNeutral(ControllerNumber) = False

        ElseIf State.Gamepad.sThumbLX >= NeutralEnd Then
            ' The left thumbstick is in the right position.

            LeftThumbstickRight(ControllerNumber) = True

            LeftThumbstickLeft(ControllerNumber) = False

            LeftThumbstickXaxisNeutral(ControllerNumber) = False

        Else
            ' The left thumbstick is in the neutral position.

            LeftThumbstickRight(ControllerNumber) = False

            LeftThumbstickLeft(ControllerNumber) = False

            LeftThumbstickXaxisNeutral(ControllerNumber) = True

        End If

        ' What position is the left thumbstick in on the Y-axis?
        If State.Gamepad.sThumbLY <= NeutralStart Then
            ' The left thumbstick is in the down position.

            LeftThumbstickDown(ControllerNumber) = True

            LeftThumbstickUp(ControllerNumber) = False

            LeftThumbstickYaxisNeutral(ControllerNumber) = False

        ElseIf State.Gamepad.sThumbLY >= NeutralEnd Then
            ' The left thumbstick is in the up position.

            LeftThumbstickUp(ControllerNumber) = True

            LeftThumbstickDown(ControllerNumber) = False

            LeftThumbstickYaxisNeutral(ControllerNumber) = False

        Else
            ' The left thumbstick is in the neutral position.

            LeftThumbstickUp(ControllerNumber) = False

            LeftThumbstickDown(ControllerNumber) = False

            LeftThumbstickYaxisNeutral(ControllerNumber) = True

        End If

    End Sub

    Private Sub UpdateRightThumbstick(ControllerNumber As Integer)
        ' The range on the X-axis is -32,768 through 32,767.
        ' Signed 16-bit (2-byte) integer.
        ' The range on the Y-axis is -32,768 through 32,767.
        ' Signed 16-bit (2-byte) integer.

        ' What position is the right thumbstick in on the X-axis?
        If State.Gamepad.sThumbRX <= NeutralStart Then
            ' The right thumbstick is in the left position.

            RightThumbstickLeft(ControllerNumber) = True

            RightThumbstickRight(ControllerNumber) = False

            RightThumbstickXaxisNeutral(ControllerNumber) = False

        ElseIf State.Gamepad.sThumbRX >= NeutralEnd Then
            ' The right thumbstick is in the right position.

            RightThumbstickRight(ControllerNumber) = True

            RightThumbstickLeft(ControllerNumber) = False

            RightThumbstickXaxisNeutral(ControllerNumber) = False

        Else
            ' The right thumbstick is in the neutral position.

            RightThumbstickXaxisNeutral(ControllerNumber) = True

            RightThumbstickLeft(ControllerNumber) = False

            RightThumbstickRight(ControllerNumber) = False

        End If

        ' What position is the right thumbstick in on the Y-axis?
        If State.Gamepad.sThumbRY <= NeutralStart Then
            ' The right thumbstick is in the down position.

            RightThumbstickDown(ControllerNumber) = True

            RightThumbstickUp(ControllerNumber) = False

            RightThumbstickYaxisNeutral(ControllerNumber) = False

        ElseIf State.Gamepad.sThumbRY >= NeutralEnd Then
            ' The right thumbstick is in the up position.

            RightThumbstickUp(ControllerNumber) = True

            RightThumbstickDown(ControllerNumber) = False

            RightThumbstickYaxisNeutral(ControllerNumber) = False

        Else
            ' The right thumbstick is in the neutral position.

            RightThumbstickUp(ControllerNumber) = False

            RightThumbstickDown(ControllerNumber) = False

            RightThumbstickYaxisNeutral(ControllerNumber) = True

        End If

    End Sub

    Private Sub UpdateRightTrigger(ControllerNumber As Integer)
        ' The range of right trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.
        ' The trigger position must be greater than the trigger threshold to
        ' register as pressed.

        ' What position is the right trigger in?
        If State.Gamepad.bRightTrigger > TriggerThreshold Then
            ' The right trigger is in the down position. Trigger Break. Bang!

            RightTrigger(ControllerNumber) = True

            RightTriggerNeutral(ControllerNumber) = False

        Else
            ' The right trigger is in the neutral position. Pre-Travel.

            RightTrigger(ControllerNumber) = False

            RightTriggerNeutral(ControllerNumber) = True

        End If

    End Sub

    Private Sub UpdateLeftTrigger(ControllerNumber As Integer)
        ' The range of left trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.
        ' The trigger position must be greater than the trigger threshold to
        ' register as pressed.

        ' What position is the left trigger in?
        If State.Gamepad.bLeftTrigger > TriggerThreshold Then
            ' The left trigger is in the fire position. Trigger Break. Bang!

            LeftTrigger(ControllerNumber) = True

            LeftTriggerNeutral(ControllerNumber) = False

        Else
            ' The left trigger is in the neutral position. Pre-Travel.

            LeftTrigger(ControllerNumber) = False

            LeftTriggerNeutral(ControllerNumber) = True

        End If

    End Sub

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputSetState(playerIndex As Integer, ByRef vibration As XINPUT_VIBRATION) As Integer
    End Function

    Public Structure XINPUT_VIBRATION
        Public wLeftMotorSpeed As UShort
        Public wRightMotorSpeed As UShort
    End Structure

    Private Vibration As XINPUT_VIBRATION

    Public Sub VibrateLeft(CID As Integer, Speed As UShort)
        ' The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        ' The left motor is the low-frequency rumble motor.

        ' Set left motor speed.
        Vibration.wLeftMotorSpeed = Speed

        LeftVibrateStart(CID) = Now

        IsLeftVibrating(CID) = True

    End Sub

    Public Sub VibrateRight(CID As Integer, Speed As UShort)
        ' The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        ' The right motor is the high-frequency rumble motor.

        ' Set right motor speed.
        Vibration.wRightMotorSpeed = Speed

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

                Debug.Print($"{ControllerID} did not vibrate.")

            End If

        Catch ex As Exception

            DisplayError(ex)

            Exit Sub

        End Try

    End Sub

    Public Sub UpdateVibrateTimer()

        UpdateLeftVibrateTimer()

        UpdateRightVibrateTimer()

    End Sub

    Private Sub UpdateLeftVibrateTimer()

        For Each IsConVibrating In IsLeftVibrating

            Dim Index As Integer =
                Array.IndexOf(IsLeftVibrating, IsConVibrating)

            If Index <> -1 AndAlso IsConVibrating = True Then

                Dim ElapsedTime As TimeSpan = Now - LeftVibrateStart(Index)

                If ElapsedTime.TotalMilliseconds >= TimeToVibe Then

                    IsLeftVibrating(Index) = False

                    ' Turn left motor off (set zero speed).
                    Vibration.wLeftMotorSpeed = 0

                End If

                SendVibrationMotorCommand(Index)

            End If

        Next

    End Sub

    Private Sub UpdateRightVibrateTimer()

        For Each IsConVibrating In IsRightVibrating

            Dim Index As Integer =
                Array.IndexOf(IsRightVibrating, IsConVibrating)

            If Index <> -1 AndAlso IsConVibrating = True Then

                Dim ElapsedTime As TimeSpan = Now - RightVibrateStart(Index)

                If ElapsedTime.TotalMilliseconds >= TimeToVibe Then

                    IsRightVibrating(Index) = False

                    ' Turn right motor off (set zero speed).
                    Vibration.wRightMotorSpeed = 0

                End If

                SendVibrationMotorCommand(Index)

            End If

        Next

    End Sub

    Private Sub DisplayError(ex As Exception)

        MsgBox(ex.ToString()) ' Display the exception message in a message box.

    End Sub

End Structure

Public Class Form1

    Private Controllers As XboxControllers

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        InitializeApp()

        Controllers.Init()

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

        Controllers.Update()

        DoButtonLogic()

        For controllerNumber As Integer = 0 To 3

            UpdateControllerStatusLabel(controllerNumber)

            If Controllers.Connected(controllerNumber) Then

                ' Right Thumbstick X Axis
                If Controllers.RightThumbstickLeft(controllerNumber) Then

                    LabelRightThumbX.Text =
                        $"Controller {controllerNumber} Right Thumbstick Left"

                End If

                If Controllers.RightThumbstickRight(controllerNumber) Then

                    LabelRightThumbX.Text =
                        $"Controller {controllerNumber} Right Thumbstick Right"

                End If

                ClearRightThumbstickXLabel()

                ' Right Thumbstick Y Axis
                If Controllers.RightThumbstickUp(controllerNumber) Then

                    LabelRightThumbY.Text =
                        $"Controller {controllerNumber} Right Thumbstick Up"

                End If

                If Controllers.RightThumbstickDown(controllerNumber) Then

                    LabelRightThumbY.Text =
                        $"Controller {controllerNumber} Right Thumbstick Down"

                End If

                ClearRightThumbstickYLabel()

                ' Left Thumbstick X Axis
                If Controllers.LeftThumbstickLeft(controllerNumber) Then

                    LabelLeftThumbX.Text =
                        $"Controller {controllerNumber} Left Thumbstick Left"

                End If

                If Controllers.LeftThumbstickRight(controllerNumber) Then

                    LabelLeftThumbX.Text =
                        $"Controller {controllerNumber} Left Thumbstick Right"

                End If

                ClearLeftThumbstickXLabel()

                ' Left Thumbstick Y Axis
                If Controllers.LeftThumbstickUp(controllerNumber) Then

                    LabelLeftThumbY.Text =
                        $"Controller {controllerNumber} Left Thumbstick Up"

                End If

                If Controllers.LeftThumbstickDown(controllerNumber) Then

                    LabelLeftThumbY.Text =
                        $"Controller {controllerNumber} Left Thumbstick Down"

                End If

                ClearLeftThumbstickYLabel()

                ' Left Trigger
                If Controllers.LeftTrigger(controllerNumber) Then

                    LabelLeftTrigger.Text =
                        $"Controller {controllerNumber} Left Trigger"

                End If

                ClearLeftTriggerLabel()

                ' Right Trigger
                If Controllers.RightTrigger(controllerNumber) Then

                    LabelRightTrigger.Text =
                        $"Controller {controllerNumber} Right Trigger"

                End If

                ClearRightTriggerLabel()

            End If

        Next

    End Sub

    Private Sub ButtonVibrateLeft_Click(sender As Object, e As EventArgs) Handles ButtonVibrateLeft.Click

        Controllers.VibrateLeft(NumControllerToVib.Value, TrackBarSpeed.Value)

    End Sub

    Private Sub ButtonVibrateRight_Click(sender As Object, e As EventArgs) Handles ButtonVibrateRight.Click

        Controllers.VibrateRight(NumControllerToVib.Value, TrackBarSpeed.Value)

    End Sub

    Private Sub TrackBarSpeed_Scroll(sender As Object, e As EventArgs) Handles TrackBarSpeed.Scroll

        UpdateSpeedLabel()

    End Sub

    Private Sub UpdateControllerStatusLabel(controllerNumber As Integer)
        ' Update the status label based on connection state

        Dim status As String =
            If(Controllers.Connected(controllerNumber),
               "Connected", "Not Connected")

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

    Private Sub DoButtonLogic()

        For controllerNumber As Integer = 0 To 3

            If Controllers.Connected(controllerNumber) Then

                DoDPadLogic(controllerNumber)

                DoLetterButtonLogic(controllerNumber)

                DoStartBackLogic(controllerNumber)

                DoBumperLogic(controllerNumber)

                DoStickLogic(controllerNumber)

            End If

        Next

    End Sub

    Private Sub DoDPadLogic(controllerNumber As Integer)

        Dim direction As String = GetDPadDirection(controllerNumber)

        ' Are all DPad buttons up?
        If Not String.IsNullOrEmpty(direction) Then
            ' No, all DPad buttons are not up.

            LabelDPad.Text = $"Controller {controllerNumber} DPad {direction}"

            Controllers.DPadNeutral(controllerNumber) = False

        Else
            ' Yes, all DPad buttons are up.

            Controllers.DPadNeutral(controllerNumber) = True

        End If

        ClearDPadLabel()

    End Sub

    Private Sub DoLetterButtonLogic(controllerNumber As Integer)

        Dim buttonText As String = GetButtonText(controllerNumber)

        ' Are any letter buttons pressed?
        If Not String.IsNullOrEmpty(buttonText) Then
            ' Yes, letter buttons are pressed.

            LabelButtons.Text = buttonText

            Controllers.LetterButtonsNeutral(controllerNumber) = False

        Else
            ' No, letter buttons are Not pressed.

            Controllers.LetterButtonsNeutral(controllerNumber) = True

        End If

        ClearLetterButtonsLabel()

    End Sub

    Private Sub DoStartBackLogic(ControllerNumber As Integer)

        If Controllers.Start(ControllerNumber) Then

            LabelStart.Text = $"Controller {ControllerNumber} Start"

            Controllers.StartNeutral(ControllerNumber) = False

        Else

            Controllers.StartNeutral(ControllerNumber) = True

        End If

        ClearStartLabel()

        If Controllers.Back(ControllerNumber) Then

            LabelBack.Text = $"Controller {ControllerNumber} Back"

            Controllers.BackNeutral(ControllerNumber) = False

        Else

            Controllers.BackNeutral(ControllerNumber) = True

        End If

        ClearBackLabel()

    End Sub

    Private Sub DoBumperLogic(ControllerNumber As Integer)

        If Controllers.LeftBumper(ControllerNumber) Then

            LabelLeftBumper.Text =
                $"Controller {ControllerNumber} Left Bumper"

            Controllers.LeftBumperNeutral(ControllerNumber) = False

        Else

            Controllers.LeftBumperNeutral(ControllerNumber) = True

        End If

        ClearLeftBumperLabel()

        If Controllers.RightBumper(ControllerNumber) Then

            LabelRightBumper.Text =
                $"Controller {ControllerNumber} Right Bumper"

            Controllers.RightBumperNeutral(ControllerNumber) = False

        Else

            Controllers.RightBumperNeutral(ControllerNumber) = True

        End If

        ClearRightBumperLabel()

    End Sub

    Private Sub DoStickLogic(ControllerNumber As Integer)

        If Controllers.LeftStick(ControllerNumber) Then

            LabelLeftThumbButton.Text =
                $"Controller {ControllerNumber} Left Thumbstick Button"

            Controllers.LeftStickNeutral(ControllerNumber) = False

        Else

            Controllers.LeftStickNeutral(ControllerNumber) = True

        End If

        ClearLeftThumbButtonLabel()

        If Controllers.RightStick(ControllerNumber) Then

            LabelRightThumbButton.Text =
                $"Controller {ControllerNumber} Right Thumbstick Button"

            Controllers.RightStickNeutral(ControllerNumber) = False

        Else

            Controllers.RightStickNeutral(ControllerNumber) = True

        End If

        ClearRightThumbButtonLabel()

    End Sub

    Private Sub ClearLetterButtonsLabel()
        ' Clears the letter buttons label when all controllers' letter buttons
        ' are up.

        Dim ConSum As Boolean = True ' Assume all controllers' letter buttons
        ' are neutral initially.

        ' Search for a non-neutral letter button.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.LetterButtonsNeutral(i) Then
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
        ' Clears the left thumbstick Y-axis label when all controllers left
        ' thumbsticks on the Y-axis are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers left thumbsticks
        ' on the Y-axis are neutral initially.

        ' Search for a non-neutral left thumbstick on the Y-axis.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.LeftThumbstickYaxisNeutral(i) Then
                ' A non-neutral thumbstick was found.

                ConSum = False ' Report the non-neutral thumbstick.

                Exit For ' No need to search further so stop the search.

            End If

        Next

        ' Are all controllers left thumbsticks on the Y-axis in the neutral
        ' position?
        If ConSum = True Then
            ' Yes, all controllers left thumbsticks on the Y-axis are in the
            ' neutral position.

            LabelLeftThumbY.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearLeftThumbstickXLabel()
        ' Clears the left thumbstick X-axis label when all controllers left
        ' thumbsticks on the X-axis are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers left thumbsticks
        ' on the X-axis are neutral initially.

        ' Search for a non-neutral left thumbstick on the X-axis.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.LeftThumbstickXaxisNeutral(i) Then
                ' A non-neutral thumbstick was found.

                ConSum = False ' Report the non-neutral thumbstick.

                Exit For ' No need to search further so stop the search.

            End If

        Next

        ' Are all controllers left thumbsticks on the X-axis in the neutral
        ' position?
        If ConSum = True Then
            ' Yes, all controllers left thumbsticks on the X-axis are in the
            ' neutral position.

            LabelLeftThumbX.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearRightThumbstickXLabel()
        ' Clears the right thumbstick X-axis label when all controllers right
        ' thumbsticks on the X-axis are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers right thumbsticks
        ' on the X-axis are neutral initially.

        ' Search for a non-neutral right thumbstick on the X-axis.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.RightThumbstickXaxisNeutral(i) Then
                ' A non-neutral thumbstick was found.

                ConSum = False ' Report the non-neutral thumbstick.

                Exit For ' No need to search further so stop the search.

            End If

        Next

        ' Are all controllers right thumbsticks on the X-axis in the neutral
        ' position?
        If ConSum = True Then
            ' Yes, all controllers right thumbsticks on the X-axis are in the
            ' neutral position.

            LabelRightThumbX.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearRightThumbstickYLabel()
        ' Clears the right thumbstick Y-axis label when all controllers right
        ' thumbsticks on the Y-axis are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers right thumbsticks
        ' on the Y-axis are neutral initially.

        ' Search for a non-neutral right thumbstick on the Y-axis.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.RightThumbstickYaxisNeutral(i) Then
                ' A non-neutral thumbstick was found.

                ConSum = False ' Report the non-neutral thumbstick.

                Exit For ' No need to search further so stop the search.

            End If

        Next

        ' Are all controllers right thumbsticks on the Y-axis in the neutral
        ' position?
        If ConSum = True Then
            ' Yes, all controllers right thumbsticks on the Y-axis are in the
            ' neutral position.

            LabelRightThumbY.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearRightTriggerLabel()
        ' Clears the right trigger label when all controllers right triggers
        ' are neutral.

        ' Assume all controllers right triggers are neutral initially.
        Dim ConSum As Boolean = True

        ' Search for a non-neutral right trigger.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
                Not Controllers.RightTriggerNeutral(i) Then
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
        ' Clears the left trigger label when all controllers left triggers are
        ' neutral.

        ' Assume all controllers left triggers are neutral initially.
        Dim ConSum As Boolean = True

        ' Search for a non-neutral left trigger.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.LeftTriggerNeutral(i) Then
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

        ' Assume all controllers' DPad are neutral initially.
        Dim ConSum As Boolean = True

        ' Search for a non-neutral DPad.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.DPadNeutral(i) Then
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

        ' Assume all controllers' start buttons are neutral initially.
        Dim ConSum As Boolean = True

        ' Search for a non-neutral start button.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.StartNeutral(i) Then
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

        ' Assume all controllers' back buttons are neutral initially.
        Dim ConSum As Boolean = True

        ' Search for a non-neutral back button.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.BackNeutral(i) Then
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
        ' Clears the left bumper label when all controllers' left bumpers are
        ' neutral.

        ' Assume all controllers' left bumpers are neutral initially.
        Dim ConSum As Boolean = True

        ' Search for a non-neutral left bumper.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.LeftBumperNeutral(i) Then
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
        ' Clears the right bumper label when all controllers' right bumpers are
        ' neutral.

        ' Assume all controllers' right bumpers are neutral initially.
        Dim ConSum As Boolean = True

        ' Search for a non-neutral right bumper.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.RightBumperNeutral(i) Then
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
        ' Clears the left thumb button label when all controllers' left thumb
        ' buttons are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers' left thumb
        ' buttons are neutral initially.

        ' Search for a non-neutral left thumb button.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.LeftStickNeutral(i) Then
                ' A non-neutral left thumb button was found.

                ConSum = False ' Report the non-neutral left thumb button.

                Exit For ' No need to search further, so stop the search.

            End If

        Next

        ' Are all controllers' left thumb buttons in the neutral position?
        If ConSum Then
            ' Yes, all controllers' left thumb buttons are in the neutral
            ' position.

            LabelLeftThumbButton.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub ClearRightThumbButtonLabel()
        ' Clears the right thumb button label when all controllers' right thumb
        ' buttons are neutral.

        Dim ConSum As Boolean = True ' Assume all controllers' right thumb
        ' buttons are neutral initially.

        ' Search for a non-neutral right thumb button.
        For i As Integer = 0 To 3

            If Controllers.Connected(i) AndAlso
               Not Controllers.RightStickNeutral(i) Then
                ' A non-neutral right thumb button was found.

                ConSum = False ' Report the non-neutral right thumb button.

                Exit For ' No need to search further, so stop the search.

            End If

        Next

        ' Are all controllers' right thumb buttons in the neutral position?
        If ConSum Then
            ' Yes, all controllers' right thumb buttons are in the neutral
            ' position.

            LabelRightThumbButton.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Function GetDPadDirection(controllerNumber As Integer) As String

        If Controllers.DPadUp(controllerNumber) Then

            If Controllers.DPadLeft(controllerNumber) Then Return "Left+Up"

            If Controllers.DPadRight(controllerNumber) Then Return "Right+Up"

            Return "Up"

        End If

        If Controllers.DPadDown(controllerNumber) Then

            If Controllers.DPadLeft(controllerNumber) Then Return "Left+Down"

            If Controllers.DPadRight(controllerNumber) Then Return "Right+Down"

            Return "Down"

        End If

        If Controllers.DPadLeft(controllerNumber) Then Return "Left"

        If Controllers.DPadRight(controllerNumber) Then Return "Right"

        Return String.Empty ' Return an empty string if no buttons are pressed.

    End Function

    Private Function GetButtonText(controllerNumber As Integer) As String

        Dim buttons As New List(Of String)

        If Controllers.A(controllerNumber) Then buttons.Add("A")

        If Controllers.B(controllerNumber) Then buttons.Add("B")

        If Controllers.X(controllerNumber) Then buttons.Add("X")

        If Controllers.Y(controllerNumber) Then buttons.Add("Y")

        If buttons.Count > 0 Then

            Return $"Controller {controllerNumber} Buttons: {String.Join("+", buttons)}"

        End If

        Return String.Empty ' Return an empty string if no buttons are pressed

    End Function

    Private Sub UpdateSpeedLabel()

        LabelSpeed.Text = $"Speed: {TrackBarSpeed.Value}"

    End Sub

    Private Sub DisplayError(ex As Exception)

        MsgBox(ex.ToString()) ' Display the exception message in a message box.

    End Sub

    Private Sub InitializeApp()

        Text = "XInput - Code with Joe"

        InitializeTimer1()

        ClearLabels()

        TrackBarSpeed.Value = 32767

        UpdateSpeedLabel()

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

        For Each Con In Controllers.Buttons

            ConSum += Con

        Next

        ' Are all controllers buttons up?
        If ConSum = 0 Then
            ' Yes, all controller buttons are up.

            LabelButtons.Text = String.Empty ' Clear label.

        End If

    End Sub

    Private Sub NumericUpDownTimeToVib_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDownTimeToVib.ValueChanged

        Controllers.TimeToVibe = NumericUpDownTimeToVib.Value

    End Sub


End Class


' Consuming Unmanaged DLL Functions

' Consuming unmanaged DLL functions refers to the process of using functions
' that are defined in a DLL (Dynamic Link Library) which is written in a
' language like C or C++. This involves using Platform Invocation Services
' (P/Invoke) to call functions in the unmanaged DLL from your managed VB code.
' To consume unmanaged DLL functions use the DllImport attribute to declare the
' external functions from the DLL.

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
