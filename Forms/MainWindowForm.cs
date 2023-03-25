using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
using System.Runtime.InteropServices;

using dataManager;

namespace PokeHelper
{
    public partial class MainWindow : Form
    {
        // ******************** DEFINITIONS FOR VARIABLES ********************

        // import library for mouse events
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        
        // Functions to enable moving window without borders
        [DllImportAttribute("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")] public static extern bool ReleaseCapture();
        
        // Const values for mouse events
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        // Const to enable moving window without borders
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        // Variables
        public bool Enable; // turn on/off hotkeys
        public bool ContinousWork; // Continous work active
        public Stopwatch Timer; // timer for checking time from last shoportcut send in continous mode
        public int MilisecondDelay => this.myData.TakeOptionTimerDelay(); // time for timer
        private HotkeyManagement managementForm1;
        private Settings managementForm2;

        public DataManager myData; // instance for my specific ini file
        private GlobalKeyboardHook globalKeyboardHook; // instance for event "press button" -
                                                       // for reading button with no focus on app
        // Interface elements
        private Label titleLabel;
        private Button hotkeyButton;
        private Button settingsButton;
        private Button minimalizeButton;
        private RadioButton enableControl;
        private RadioButton continousControl;

        // ******************** DEFINITIONS FOR VARIABLES ********************
        public MainWindow()
        {
            InitializeComponent();

            // Register hotkeys
            this.RegisterHotKey();

            // Set starting value for enable
            this.Enable = true;

            // Set starting value for continous work
            this.ContinousWork = false;

            // Set timer
            this.Timer = new Stopwatch();

            // Create ini file and initialize data
            this.myData = new DataManager();

            // Generate GUI
            this.CreateInterface();
        }
        public void CreateInterface() // Create GUI elememnts
        {
            this.titleLabel = new Label();
            this.SetParameters(ref this.titleLabel, new Size(120, 20), new Point(40, 5), Color.Black, Color.White, "PokeHelper v1.6");
            this.Controls.Add(this.titleLabel);

            this.minimalizeButton = new Button();
            this.SetParameters(ref this.minimalizeButton, new Size(15, 5), new Point(250, 5), Color.Black, Color.White, "");
            this.minimalizeButton.Click += new EventHandler(this.MinimalizeButton_Click);
            this.Controls.Add(this.minimalizeButton);

            this.settingsButton = new Button();
            this.SetParameters(ref this.settingsButton, new Size(100, 30), new Point(170, 25), Color.Black, Color.White, "Settings");
            this.settingsButton.Click += new EventHandler(this.SettingsButton_Click);
            this.Controls.Add(this.settingsButton);

            this.hotkeyButton = new Button();
            this.SetParameters(ref this.hotkeyButton, new Size(100, 30), new Point(65, 25), Color.Black, Color.White, "HotKeys");
            this.hotkeyButton.Click += new EventHandler(this.HotKeysButton_Click);
            this.Controls.Add(this.hotkeyButton);

            this.enableControl = new RadioButton();
            this.enableControl.Appearance = Appearance.Button;
            this.SetParameters(ref this.enableControl, new Size(25, 25), new Point(5, 5), Color.Green, Color.Red, "");
            this.enableControl.Click += new EventHandler(this.EnableControl_Click);
            this.Controls.Add(this.enableControl);

            this.continousControl = new RadioButton();
            this.continousControl.Appearance = Appearance.Button;
            this.SetParameters(ref this.continousControl, new Size(25, 25), new Point(5, 35), Color.Red, Color.Red, "");
            this.continousControl.Click += new EventHandler(this.ContinousControl_Click);
            this.Controls.Add(this.continousControl);

            this.Size = new Size(270, 60);
        }

        // Group of methods to set parameters for gui elements
        private void SetParameters(ref Label iLabel, Size iSize, Point iPoint, Color iBckColor, Color iForColor, string iText, string iTag = "", string iName = "")
        {
            iLabel.Size = iSize;
            iLabel.Location = iPoint;
            iLabel.BackColor = iBckColor;
            iLabel.Text = iText;
            iLabel.ForeColor = iForColor;
            iLabel.Tag = iTag;
            iLabel.Name = iName;
        }
        private void SetParameters(ref Button iButton, Size iSize, Point iPoint, Color iBckColor, Color iForColor, string iText, string iTag = "", string iName = "")
        {
            iButton.Size = iSize;
            iButton.Location = iPoint;
            iButton.BackColor = iBckColor;
            iButton.ForeColor = iForColor;
            iButton.Text = iText;
            iButton.Tag = iTag;
            iButton.Name = iName;
        }
        private void SetParameters(ref RadioButton iRadioButton, Size iSize, Point iPoint, Color iBckColor, Color iForColor, string iText, string iTag = "", string iName = "")
        {
            iRadioButton.Size = iSize;
            iRadioButton.Location = iPoint;
            iRadioButton.BackColor = iBckColor;
            iRadioButton.FlatAppearance.CheckedBackColor = iForColor;
            iRadioButton.Text = iText;
            iRadioButton.Tag = iTag;
            iRadioButton.Name = iName;
        }
        private void SetParameters(ref TextBox iTextBox, Size iSize, Point iPoint, Color iBckColor, Color iForColor, string iText, string iTag = "", string iName = "")
        {
            iTextBox.Size = iSize;
            iTextBox.Location = iPoint;
            iTextBox.BackColor = iBckColor;
            iTextBox.ForeColor = iForColor;
            iTextBox.Text = iText;
            iTextBox.Tag = iTag;
            iTextBox.Name = iName;
            iTextBox.TextAlign = HorizontalAlignment.Center;
        }
        private void SetParameters(ref CheckBox iCheckBox, Size iSize, Point iPoint, Color iBckColor, Color iForColor, string iText, string iTag = "", string iName = "")
        {
            iCheckBox.Size = iSize;
            iCheckBox.Location = iPoint;
            iCheckBox.BackColor = iBckColor;
            iCheckBox.ForeColor = iForColor;
            iCheckBox.Text = iText;
            iCheckBox.Tag = iTag;
            iCheckBox.Name = iName;
        }

        // ******************** HOTKEY MANAGEMENT ********************
        private void RegisterHotKey() // Register all keys at keyboard like events
        {
            // Hooks into all keys
            this.globalKeyboardHook = new GlobalKeyboardHook();

            // Register function to use during event
            this.globalKeyboardHook.KeyboardPressed += OnKeyPressed;
        }
        private void OnKeyPressed(object iSender, GlobalKeyboardHookEventArgs iE) // manage event of pressing keys with no focus
        {
            // Read key from event data
            Keys loggedKey = iE.KeyboardData.Key;

            // Check correct button status
            if (iE.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp)
            {
                // Check special keys
                switch (loggedKey)
                {
                    case Keys.Escape: // Disable or close system if esc pressed
                        this.DisableOrCloseSystem();
                        break;

                    case Keys.F12: // Enable system if F12 pressed
                        this.EnableSystem();
                        break;

                    default: // normally do nothing
                        // do nothing
                        break;
                }
                // Check hotkeys if functionality is enabled
                if( this.Enable ) MainHotkeyFunctionality(loggedKey);
            }


            if (iE.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                // Check hotkeys if functionality
                if (this.Enable && this.ContinousWork && this.Timer.ElapsedMilliseconds > this.MilisecondDelay)
                {
                    this.Timer.Reset();
                    MainHotkeyFunctionality(loggedKey);
                }
            }
            this.Timer.Start();
        }
        private void MainHotkeyFunctionality(Keys iLoggedKey) // check structure for hotkeys and use correct one
        {
            // Copy loggedkey and translate it to string (numbers are rewrite from DX and NumPadX to X)
            string loggedKey = AdjustNumber(iLoggedKey.ToString());

            // Check all hotkeys in structure
            for (int i = 0; i < this.myData.TakeOptionStructureSize(); i++)
            {
                // Check if button is pressed and do not care if it is upper or lower sign
                if (this.myData.structTakeHotKey(i).button.ToUpper() == loggedKey.ToUpper())
                {
                    // Move cursor "softly" (not big jump)
                    MoveCursor(new Point(int.Parse(this.myData.structTakeHotKey(i).posX), int.Parse(myData.structTakeHotKey(i).posY)));
                }
            }
        }
        private string AdjustNumber(string iKey) // Translate DX and NumPadX into X
        {
            switch (iKey)
            {
                case "D1":
                    iKey = "1";
                    break;

                case "D2":
                    iKey = "2";
                    break;

                case "D3":
                    iKey = "3";
                    break;

                case "D4":
                    iKey = "4";
                    break;

                case "D5":
                    iKey = "5";
                    break;

                case "D6":
                    iKey = "6";
                    break;

                case "D7":
                    iKey = "7";
                    break;

                case "D8":
                    iKey = "8";
                    break;

                case "D9":
                    iKey = "9";
                    break;

                case "D0":
                    iKey = "0";
                    break;

                case "NumPad1":
                    iKey = "1";
                    break;

                case "NumPad2":
                    iKey = "2";
                    break;

                case "NumPad3":
                    iKey = "3";
                    break;

                case "NumPad4":
                    iKey = "4";
                    break;

                case "NumPad5":
                    iKey = "5";
                    break;

                case "NumPad6":
                    iKey = "6";
                    break;

                case "NumPad7":
                    iKey = "7";
                    break;

                case "NumPad8":
                    iKey = "8";
                    break;

                case "NumPad9":
                    iKey = "9";
                    break;

                case "NumPad0":
                    iKey = "0";
                    break;

                default:
                    break;
            }
            return iKey;
        }
        private void MoveCursor(Point iTarget) // move cursor to pos. and press left button
        {
            // Define moving vector
            Point Vector = new Point(iTarget.X - Cursor.Position.X, iTarget.Y - Cursor.Position.Y);
            
            // Define time of evaluating movement
            TimeSpan deltaTime;
            if ( this.myData.TakeOptionouseMovementSteps() > 0) 
                deltaTime = new TimeSpan(0, 0, 0, 0, this.myData.TakeOptionouseMovementTime() / this.myData.TakeOptionouseMovementSteps());
            else deltaTime = new TimeSpan(0, 0, 0, 0, 0);

            // Realise movement in steps
            for (int i = 0; i < this.myData.TakeOptionouseMovementSteps(); i++)
            {
                // Waiting time between 2 points on the screen
                if(this.myData.TakeOptionouseMovementLong()) Thread.Sleep(deltaTime);

                // Move cursor to the next point on screen
                mouse_event(MOUSEEVENTF_MOVE, (uint)(Vector.X/ this.myData.TakeOptionouseMovementSteps()), (uint)(Vector.Y / this.myData.TakeOptionouseMovementSteps()), 0, 0);
            }
            
            // Move cursor to defined position
            mouse_event(MOUSEEVENTF_MOVE, (uint)(iTarget.X - Cursor.Position.X), (uint)(iTarget.Y - Cursor.Position.Y), 0, 0);
            
            // Press mouse left button
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)iTarget.X, (uint)iTarget.Y, 0, 0);
            
            // Release mouse left button
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)iTarget.X, (uint)iTarget.Y, 0, 0);
        }
        // ******************** HOTKEY MANAGEMENT ********************
        
        // ******************** CONTROL ENABILITY OF HOTKEYS ********************
        private void ChangeEnability() // Switch between anable and disable state
        {
            if (this.Enable)
            {
                this.Enable = false;
                this.enableControl.BackColor = Color.Red;
            }
            else
            {
                this.Enable = true;
                this.enableControl.BackColor = Color.Green;
            }
        } 
        private void EnableSystem() // Enable functionality o hotkeys
        {
            this.Enable = true;
            this.enableControl.BackColor = Color.Green;
        } 
        private void DisableOrCloseSystem() // Disable dunctionality of hotkeys or close system
        {
            const string CLOSE_FORM1 = "closeForm1";
            const string CLOSE_FORM2 = "closeForm2";

            string option = "";
            if (managementForm1 != null && !managementForm1.IsDisposed) option = CLOSE_FORM1;
            if (managementForm2 != null && !managementForm2.IsDisposed) option = CLOSE_FORM2;

            switch (option)
            {

                case CLOSE_FORM1:
                    managementForm1.Close();
                    break;

                case CLOSE_FORM2:
                    managementForm2.Close();
                    break;

                default:
                    if (this.Enable)
                    {
                        this.Enable = false;
                        this.enableControl.BackColor = Color.Red;
                    }
                    // other way close application
                    else this.Close();
                    break;
            }
        } 
        private void CloseSystem() // Close aplication
        {
            this.Close();
        } 
        private void ManageHotkeyWindow() // Create or close window for hotkeys management
        {
            {
                if (managementForm1 == null || managementForm1.IsDisposed)
                {
                    if (managementForm2 != null && !managementForm2.IsDisposed)
                    {
                        managementForm2.Close();
                    }
                    // starting 2nd form
                    managementForm1 = new HotkeyManagement(ref this.myData);
                    managementForm1.Show();
                }
                else
                {
                    managementForm1.Close();
                }
            }
        }
        private void ManageSettingsWindow() // Create or close window for hotkeys management
        {
            {
                if (managementForm2 == null || managementForm2.IsDisposed)
                {
                    if (managementForm1 != null && !managementForm1.IsDisposed)
                    {
                        managementForm1.Close();
                    }
                    // starting 2nd form
                    managementForm2 = new Settings(ref this.myData);
                    managementForm2.Show();
                }
                else
                {
                    managementForm2.Close();
                }
            }
        }

        // ******************** CONTROL ENABILITY OF HOTKEYS ********************

        // ******************** EVENTS FOR GUI ELEMENTS ********************
        private void MainWindow_Click(object iSender, EventArgs iE) // not used
        {
            ;
        }
        private void MainWindow_DoubleClick(object iSender, EventArgs iE) // close system
        {
            this.CloseSystem();
        }
        private void MainWindow_MouseDown(object iSender, MouseEventArgs iE) // allow moving window
        {
            if (iE.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void MinimalizeButton_Click(object iSender, EventArgs iE) // minimalize main window
        {
            // Minimalize main window
            MainWindow.ActiveForm.WindowState = FormWindowState.Minimized;
        }
        private void EnableControl_Click(object iSender, EventArgs iE) // change enability
        {
            this.ChangeEnability();
        }
        private void ContinousControl_Click(object iSender, EventArgs iE) // change enability
        {
            if(this.ContinousWork)
            {
                this.ContinousWork = false;
                this.continousControl.BackColor = Color.Red;
            } else
            {
                this.ContinousWork = true;
                this.continousControl.BackColor = Color.Green;
            }
        }
        private void HotKeysButton_Click(object iSender, EventArgs iE) // manage hotkey window
        {
            this.ManageHotkeyWindow();
        }
        private void SettingsButton_Click(object sender, EventArgs iE) // temporary usade of SettingButton
        {
            this.ManageSettingsWindow();
        }

        // ******************** EVENTS FOR GUI ELEMENTS ********************
    }
}
