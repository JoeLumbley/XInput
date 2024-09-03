# XInput

🎮 Welcome to XInput, your go-to solution for integrating Xbox controller support into your applications! This feature-rich application showcases the seamless integration of Xbox controllers, complete with vibration effects and real-time controller state monitoring. 


![024](https://github.com/user-attachments/assets/bef33051-0d5d-4248-9afc-68a9d0fc3e5d)


With a clean and well-commented codebase, this project serves as an invaluable resource for developers looking to harness the power of XInput in their Windows applications. Whether you're a seasoned developer or just getting started, the XInput app provides a solid foundation for building immersive gaming experiences and beyond.






![063](https://github.com/user-attachments/assets/42017bca-10fc-4792-a0e2-005893763b00)




This application demonstrates the use of Xbox controllers and their vibration effect (rumble). It includes the implementation of the XInput API to retrieve the current state of the controllers, process button and thumbstick input, and control the vibration motors.

The application uses the XInput1_4.dll library to interact with Xbox controllers. 

``` VB
<DllImport("XInput1_4.dll")>
Private Shared Function XInputGetState(dwUserIndex As Integer, ByRef pState As XINPUT_STATE) As Integer
End Function

```

It defines various structures and constants to represent the controller state, including the XINPUT_STATE and XINPUT_GAMEPAD structures.

``` VB
<StructLayout(LayoutKind.Explicit)>
Public Structure XINPUT_STATE
  <FieldOffset(0)>
  Public dwPacketNumber As UInteger 
  <FieldOffset(4)>
  Public Gamepad As XINPUT_GAMEPAD
End Structure

<StructLayout(LayoutKind.Sequential)>
Public Structure XINPUT_GAMEPAD
  Public wButtons As UShort 
  Public bLeftTrigger As Byte 
  Public bRightTrigger As Byte
  Public sThumbLX As Short 
  Public sThumbLY As Short
  Public sThumbRX As Short
  Public sThumbRY As Short
End Structure

```





The application can detect the connection status of up to 4 controllers and update the UI accordingly. 


It can process various input from the controllers, such as button presses, thumbstick positions, and trigger levels, and update the UI to display the corresponding information.

The application includes functionality to control the vibration motors of the controllers, allowing the user to vibrate the left and right motors independently. 



It also includes logic to manage the vibration timers and turn off the motors when the specified vibration duration is reached. 





The article provides references to the relevant Microsoft documentation for the XInput API and data types used in the application.



The application detects and updates the connection status of the controllers using the following approach:

**Polling Mechanism:** The application uses a timer to periodically check the connection status of each controller. This is typically done every second.

``` VB
Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

  UpdateControllerData()

  UpdateVibrateTimer()

End Sub

```

**XInputGetState Function:** For each controller (up to 4), the application calls the XInputGetState function. This function retrieves the current state of the specified controller, including whether it is connected.


``` VB
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
    
```

**Connection Check:** The return value of XInputGetState is checked:

A return value of 0 indicates that the controller is connected.
Any non-zero return value indicates that the controller is not connected.


**Updating UI:** Based on the connection status retrieved, the application updates the corresponding UI elements (e.g., status labels) to reflect whether each controller is connected or not.

**Storing State:** The connection status for each controller is stored in a boolean array, which is updated during each polling cycle.

This method ensures that the application can dynamically reflect the current connection status of the Xbox controllers in real-time.











