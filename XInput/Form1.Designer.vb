<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Timer1 = New Timer(components)
        LabelButtons = New Label()
        LabelLeftTrigger = New Label()
        LabelRightTrigger = New Label()
        LabelLeftThumbX = New Label()
        LabelLeftThumbY = New Label()
        LabelRightThumbY = New Label()
        LabelRightThumbX = New Label()
        ButtonVibrateLeft = New Button()
        ButtonVibrateRight = New Button()
        NumControllerToVib = New NumericUpDown()
        LabelBatteryLevel = New Label()
        TrackBarSpeed = New TrackBar()
        LabelSpeed = New Label()
        Label1 = New Label()
        LabelBatteryType = New Label()
        GroupBox1 = New GroupBox()
        GroupBox2 = New GroupBox()
        GroupBox3 = New GroupBox()
        LabelController3Status = New Label()
        LabelController2Status = New Label()
        LabelController1Status = New Label()
        LabelController0Status = New Label()
        LabelDPad = New Label()
        CType(NumControllerToVib, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrackBarSpeed, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox1.SuspendLayout()
        GroupBox2.SuspendLayout()
        GroupBox3.SuspendLayout()
        SuspendLayout()
        ' 
        ' Timer1
        ' 
        ' 
        ' LabelButtons
        ' 
        LabelButtons.AutoSize = True
        LabelButtons.Location = New Point(620, 86)
        LabelButtons.Name = "LabelButtons"
        LabelButtons.Size = New Size(114, 25)
        LabelButtons.TabIndex = 0
        LabelButtons.Text = "LabelButtons"
        ' 
        ' LabelLeftTrigger
        ' 
        LabelLeftTrigger.AutoSize = True
        LabelLeftTrigger.Location = New Point(16, 38)
        LabelLeftTrigger.Name = "LabelLeftTrigger"
        LabelLeftTrigger.Size = New Size(136, 25)
        LabelLeftTrigger.TabIndex = 1
        LabelLeftTrigger.Text = "LabelLeftTrigger"
        ' 
        ' LabelRightTrigger
        ' 
        LabelRightTrigger.AutoSize = True
        LabelRightTrigger.Location = New Point(620, 38)
        LabelRightTrigger.Name = "LabelRightTrigger"
        LabelRightTrigger.Size = New Size(149, 25)
        LabelRightTrigger.TabIndex = 2
        LabelRightTrigger.Text = "LabelRightTrigger"
        ' 
        ' LabelLeftThumbX
        ' 
        LabelLeftThumbX.AutoSize = True
        LabelLeftThumbX.Location = New Point(16, 86)
        LabelLeftThumbX.Name = "LabelLeftThumbX"
        LabelLeftThumbX.Size = New Size(149, 25)
        LabelLeftThumbX.TabIndex = 3
        LabelLeftThumbX.Text = "LabelLeftThumbX"
        ' 
        ' LabelLeftThumbY
        ' 
        LabelLeftThumbY.AutoSize = True
        LabelLeftThumbY.Location = New Point(16, 111)
        LabelLeftThumbY.Name = "LabelLeftThumbY"
        LabelLeftThumbY.Size = New Size(148, 25)
        LabelLeftThumbY.TabIndex = 4
        LabelLeftThumbY.Text = "LabelLeftThumbY"
        ' 
        ' LabelRightThumbY
        ' 
        LabelRightThumbY.AutoSize = True
        LabelRightThumbY.Location = New Point(620, 136)
        LabelRightThumbY.Name = "LabelRightThumbY"
        LabelRightThumbY.Size = New Size(161, 25)
        LabelRightThumbY.TabIndex = 5
        LabelRightThumbY.Text = "LabelRightThumbY"
        ' 
        ' LabelRightThumbX
        ' 
        LabelRightThumbX.AutoSize = True
        LabelRightThumbX.Location = New Point(620, 111)
        LabelRightThumbX.Name = "LabelRightThumbX"
        LabelRightThumbX.Size = New Size(162, 25)
        LabelRightThumbX.TabIndex = 6
        LabelRightThumbX.Text = "LabelRightThumbX"
        ' 
        ' ButtonVibrateLeft
        ' 
        ButtonVibrateLeft.Location = New Point(16, 80)
        ButtonVibrateLeft.Name = "ButtonVibrateLeft"
        ButtonVibrateLeft.Size = New Size(138, 34)
        ButtonVibrateLeft.TabIndex = 7
        ButtonVibrateLeft.Text = "Vibrate Left"
        ButtonVibrateLeft.UseVisualStyleBackColor = True
        ' 
        ' ButtonVibrateRight
        ' 
        ButtonVibrateRight.Location = New Point(440, 80)
        ButtonVibrateRight.Name = "ButtonVibrateRight"
        ButtonVibrateRight.Size = New Size(138, 34)
        ButtonVibrateRight.TabIndex = 8
        ButtonVibrateRight.Text = "Vibrate Right"
        ButtonVibrateRight.UseVisualStyleBackColor = True
        ' 
        ' NumControllerToVib
        ' 
        NumControllerToVib.Location = New Point(119, 35)
        NumControllerToVib.Maximum = New Decimal(New Integer() {3, 0, 0, 0})
        NumControllerToVib.Name = "NumControllerToVib"
        NumControllerToVib.Size = New Size(138, 31)
        NumControllerToVib.TabIndex = 9
        ' 
        ' LabelBatteryLevel
        ' 
        LabelBatteryLevel.AutoSize = True
        LabelBatteryLevel.Location = New Point(223, 540)
        LabelBatteryLevel.Name = "LabelBatteryLevel"
        LabelBatteryLevel.Size = New Size(147, 25)
        LabelBatteryLevel.TabIndex = 10
        LabelBatteryLevel.Text = "LabelBatteryLevel"
        ' 
        ' TrackBarSpeed
        ' 
        TrackBarSpeed.LargeChange = 16384
        TrackBarSpeed.Location = New Point(173, 83)
        TrackBarSpeed.Maximum = 65535
        TrackBarSpeed.Name = "TrackBarSpeed"
        TrackBarSpeed.Size = New Size(250, 69)
        TrackBarSpeed.SmallChange = 8192
        TrackBarSpeed.TabIndex = 11
        TrackBarSpeed.TickFrequency = 16384
        ' 
        ' LabelSpeed
        ' 
        LabelSpeed.AutoSize = True
        LabelSpeed.Location = New Point(238, 121)
        LabelSpeed.Name = "LabelSpeed"
        LabelSpeed.Size = New Size(103, 25)
        LabelSpeed.TabIndex = 12
        LabelSpeed.Text = "LabelSpeed"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(16, 37)
        Label1.Name = "Label1"
        Label1.Size = New Size(90, 25)
        Label1.TabIndex = 13
        Label1.Text = "Controller"
        ' 
        ' LabelBatteryType
        ' 
        LabelBatteryType.AutoSize = True
        LabelBatteryType.Location = New Point(42, 529)
        LabelBatteryType.Name = "LabelBatteryType"
        LabelBatteryType.Size = New Size(145, 25)
        LabelBatteryType.TabIndex = 14
        LabelBatteryType.Text = "LabelBatteryType"
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(Label1)
        GroupBox1.Controls.Add(NumControllerToVib)
        GroupBox1.Controls.Add(LabelSpeed)
        GroupBox1.Controls.Add(ButtonVibrateRight)
        GroupBox1.Controls.Add(ButtonVibrateLeft)
        GroupBox1.Controls.Add(TrackBarSpeed)
        GroupBox1.Location = New Point(17, 218)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(607, 163)
        GroupBox1.TabIndex = 15
        GroupBox1.TabStop = False
        GroupBox1.Text = "Rumble"
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(LabelDPad)
        GroupBox2.Controls.Add(LabelLeftTrigger)
        GroupBox2.Controls.Add(LabelRightTrigger)
        GroupBox2.Controls.Add(LabelLeftThumbX)
        GroupBox2.Controls.Add(LabelLeftThumbY)
        GroupBox2.Controls.Add(LabelRightThumbY)
        GroupBox2.Controls.Add(LabelRightThumbX)
        GroupBox2.Controls.Add(LabelButtons)
        GroupBox2.Location = New Point(17, 8)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(943, 203)
        GroupBox2.TabIndex = 16
        GroupBox2.TabStop = False
        GroupBox2.Text = "Monitor - Press any button on your controller"
        ' 
        ' GroupBox3
        ' 
        GroupBox3.Controls.Add(LabelController3Status)
        GroupBox3.Controls.Add(LabelController2Status)
        GroupBox3.Controls.Add(LabelController1Status)
        GroupBox3.Controls.Add(LabelController0Status)
        GroupBox3.Location = New Point(648, 219)
        GroupBox3.Name = "GroupBox3"
        GroupBox3.Size = New Size(312, 162)
        GroupBox3.TabIndex = 17
        GroupBox3.TabStop = False
        GroupBox3.Text = "Status"
        ' 
        ' LabelController3Status
        ' 
        LabelController3Status.AutoSize = True
        LabelController3Status.Location = New Point(23, 113)
        LabelController3Status.Name = "LabelController3Status"
        LabelController3Status.Size = New Size(189, 25)
        LabelController3Status.TabIndex = 3
        LabelController3Status.Text = "LabelController3Status"
        ' 
        ' LabelController2Status
        ' 
        LabelController2Status.AutoSize = True
        LabelController2Status.Location = New Point(23, 88)
        LabelController2Status.Name = "LabelController2Status"
        LabelController2Status.Size = New Size(189, 25)
        LabelController2Status.TabIndex = 2
        LabelController2Status.Text = "LabelController2Status"
        ' 
        ' LabelController1Status
        ' 
        LabelController1Status.AutoSize = True
        LabelController1Status.Location = New Point(23, 61)
        LabelController1Status.Name = "LabelController1Status"
        LabelController1Status.Size = New Size(189, 25)
        LabelController1Status.TabIndex = 1
        LabelController1Status.Text = "LabelController1Status"
        ' 
        ' LabelController0Status
        ' 
        LabelController0Status.AutoSize = True
        LabelController0Status.Location = New Point(23, 36)
        LabelController0Status.Name = "LabelController0Status"
        LabelController0Status.Size = New Size(189, 25)
        LabelController0Status.TabIndex = 0
        LabelController0Status.Text = "LabelController0Status"
        ' 
        ' LabelDPad
        ' 
        LabelDPad.AutoSize = True
        LabelDPad.Location = New Point(453, 94)
        LabelDPad.Name = "LabelDPad"
        LabelDPad.Size = New Size(95, 25)
        LabelDPad.TabIndex = 7
        LabelDPad.Text = "LabelDPad"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(10F, 25F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(980, 398)
        Controls.Add(GroupBox3)
        Controls.Add(GroupBox2)
        Controls.Add(GroupBox1)
        Controls.Add(LabelBatteryType)
        Controls.Add(LabelBatteryLevel)
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Form1"
        CType(NumControllerToVib, ComponentModel.ISupportInitialize).EndInit()
        CType(TrackBarSpeed, ComponentModel.ISupportInitialize).EndInit()
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        GroupBox2.ResumeLayout(False)
        GroupBox2.PerformLayout()
        GroupBox3.ResumeLayout(False)
        GroupBox3.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Timer1 As Timer
    Friend WithEvents LabelButtons As Label
    Friend WithEvents LabelLeftTrigger As Label
    Friend WithEvents LabelRightTrigger As Label
    Friend WithEvents LabelLeftThumbX As Label
    Friend WithEvents LabelLeftThumbY As Label
    Friend WithEvents LabelRightThumbY As Label
    Friend WithEvents LabelRightThumbX As Label
    Friend WithEvents ButtonVibrateLeft As Button
    Friend WithEvents ButtonVibrateRight As Button
    Friend WithEvents NumControllerToVib As NumericUpDown
    Friend WithEvents LabelBatteryLevel As Label
    Friend WithEvents TrackBarSpeed As TrackBar
    Friend WithEvents LabelSpeed As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents LabelBatteryType As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents LabelController0Status As Label
    Friend WithEvents LabelController3Status As Label
    Friend WithEvents LabelController2Status As Label
    Friend WithEvents LabelController1Status As Label
    Friend WithEvents LabelDPad As Label
End Class
