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
        Timer2 = New Timer(components)
        ButtonVibrateLeft = New Button()
        ButtonVibrateRight = New Button()
        NumControllerToVib = New NumericUpDown()
        LabelBatteryLevel = New Label()
        TrackBarSpeed = New TrackBar()
        LabelSpeed = New Label()
        Label1 = New Label()
        LabelBatteryType = New Label()
        CType(NumControllerToVib, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrackBarSpeed, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Timer1
        ' 
        ' 
        ' LabelButtons
        ' 
        LabelButtons.AutoSize = True
        LabelButtons.Location = New Point(365, 49)
        LabelButtons.Name = "LabelButtons"
        LabelButtons.Size = New Size(114, 25)
        LabelButtons.TabIndex = 0
        LabelButtons.Text = "LabelButtons"
        ' 
        ' LabelLeftTrigger
        ' 
        LabelLeftTrigger.AutoSize = True
        LabelLeftTrigger.Location = New Point(12, 9)
        LabelLeftTrigger.Name = "LabelLeftTrigger"
        LabelLeftTrigger.Size = New Size(136, 25)
        LabelLeftTrigger.TabIndex = 1
        LabelLeftTrigger.Text = "LabelLeftTrigger"
        ' 
        ' LabelRightTrigger
        ' 
        LabelRightTrigger.AutoSize = True
        LabelRightTrigger.Location = New Point(365, 9)
        LabelRightTrigger.Name = "LabelRightTrigger"
        LabelRightTrigger.Size = New Size(149, 25)
        LabelRightTrigger.TabIndex = 2
        LabelRightTrigger.Text = "LabelRightTrigger"
        ' 
        ' LabelLeftThumbX
        ' 
        LabelLeftThumbX.AutoSize = True
        LabelLeftThumbX.Location = New Point(14, 68)
        LabelLeftThumbX.Name = "LabelLeftThumbX"
        LabelLeftThumbX.Size = New Size(149, 25)
        LabelLeftThumbX.TabIndex = 3
        LabelLeftThumbX.Text = "LabelLeftThumbX"
        ' 
        ' LabelLeftThumbY
        ' 
        LabelLeftThumbY.AutoSize = True
        LabelLeftThumbY.Location = New Point(15, 93)
        LabelLeftThumbY.Name = "LabelLeftThumbY"
        LabelLeftThumbY.Size = New Size(148, 25)
        LabelLeftThumbY.TabIndex = 4
        LabelLeftThumbY.Text = "LabelLeftThumbY"
        ' 
        ' LabelRightThumbY
        ' 
        LabelRightThumbY.AutoSize = True
        LabelRightThumbY.Location = New Point(366, 120)
        LabelRightThumbY.Name = "LabelRightThumbY"
        LabelRightThumbY.Size = New Size(161, 25)
        LabelRightThumbY.TabIndex = 5
        LabelRightThumbY.Text = "LabelRightThumbY"
        ' 
        ' LabelRightThumbX
        ' 
        LabelRightThumbX.AutoSize = True
        LabelRightThumbX.Location = New Point(365, 95)
        LabelRightThumbX.Name = "LabelRightThumbX"
        LabelRightThumbX.Size = New Size(162, 25)
        LabelRightThumbX.TabIndex = 6
        LabelRightThumbX.Text = "LabelRightThumbX"
        ' 
        ' Timer2
        ' 
        ' 
        ' ButtonVibrateLeft
        ' 
        ButtonVibrateLeft.Location = New Point(14, 194)
        ButtonVibrateLeft.Name = "ButtonVibrateLeft"
        ButtonVibrateLeft.Size = New Size(138, 34)
        ButtonVibrateLeft.TabIndex = 7
        ButtonVibrateLeft.Text = "Vibrate Left"
        ButtonVibrateLeft.UseVisualStyleBackColor = True
        ' 
        ' ButtonVibrateRight
        ' 
        ButtonVibrateRight.Location = New Point(389, 194)
        ButtonVibrateRight.Name = "ButtonVibrateRight"
        ButtonVibrateRight.Size = New Size(138, 34)
        ButtonVibrateRight.TabIndex = 8
        ButtonVibrateRight.Text = "Vibrate Right"
        ButtonVibrateRight.UseVisualStyleBackColor = True
        ' 
        ' NumControllerToVib
        ' 
        NumControllerToVib.Location = New Point(197, 147)
        NumControllerToVib.Maximum = New [Decimal](New Integer() {3, 0, 0, 0})
        NumControllerToVib.Name = "NumControllerToVib"
        NumControllerToVib.Size = New Size(138, 31)
        NumControllerToVib.TabIndex = 9
        ' 
        ' LabelBatteryLevel
        ' 
        LabelBatteryLevel.AutoSize = True
        LabelBatteryLevel.Location = New Point(341, 149)
        LabelBatteryLevel.Name = "LabelBatteryLevel"
        LabelBatteryLevel.Size = New Size(147, 25)
        LabelBatteryLevel.TabIndex = 10
        LabelBatteryLevel.Text = "LabelBatteryLevel"
        ' 
        ' TrackBarSpeed
        ' 
        TrackBarSpeed.Location = New Point(172, 194)
        TrackBarSpeed.Maximum = 65535
        TrackBarSpeed.Name = "TrackBarSpeed"
        TrackBarSpeed.Size = New Size(180, 69)
        TrackBarSpeed.TabIndex = 11
        TrackBarSpeed.TickFrequency = 5555
        ' 
        ' LabelSpeed
        ' 
        LabelSpeed.AutoSize = True
        LabelSpeed.Location = New Point(182, 238)
        LabelSpeed.Name = "LabelSpeed"
        LabelSpeed.Size = New Size(103, 25)
        LabelSpeed.TabIndex = 12
        LabelSpeed.Text = "LabelSpeed"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(14, 149)
        Label1.Name = "Label1"
        Label1.Size = New Size(177, 25)
        Label1.TabIndex = 13
        Label1.Text = "Controller to Vibrate:"
        ' 
        ' LabelBatteryType
        ' 
        LabelBatteryType.AutoSize = True
        LabelBatteryType.Location = New Point(14, 281)
        LabelBatteryType.Name = "LabelBatteryType"
        LabelBatteryType.Size = New Size(145, 25)
        LabelBatteryType.TabIndex = 14
        LabelBatteryType.Text = "LabelBatteryType"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(10F, 25F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 538)
        Controls.Add(LabelBatteryType)
        Controls.Add(Label1)
        Controls.Add(LabelSpeed)
        Controls.Add(TrackBarSpeed)
        Controls.Add(LabelBatteryLevel)
        Controls.Add(NumControllerToVib)
        Controls.Add(ButtonVibrateRight)
        Controls.Add(ButtonVibrateLeft)
        Controls.Add(LabelRightThumbX)
        Controls.Add(LabelRightThumbY)
        Controls.Add(LabelLeftThumbY)
        Controls.Add(LabelLeftThumbX)
        Controls.Add(LabelRightTrigger)
        Controls.Add(LabelLeftTrigger)
        Controls.Add(LabelButtons)
        Name = "Form1"
        Text = "Form1"
        CType(NumControllerToVib, ComponentModel.ISupportInitialize).EndInit()
        CType(TrackBarSpeed, ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents Timer2 As Timer
    Friend WithEvents ButtonVibrateLeft As Button
    Friend WithEvents ButtonVibrateRight As Button
    Friend WithEvents NumControllerToVib As NumericUpDown
    Friend WithEvents LabelBatteryLevel As Label
    Friend WithEvents TrackBarSpeed As TrackBar
    Friend WithEvents LabelSpeed As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents LabelBatteryType As Label
End Class
