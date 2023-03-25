using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
using System.Runtime.InteropServices;

using dataManager;

namespace PokeHelper
{
    public partial class Settings : Form
    {
        // Functions to anable movin window without borders
        [DllImportAttribute("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")] public static extern bool ReleaseCapture();

        // Const to enable moving window without borders
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        // Variables
        public DataManager myData; // instance for my specific ini file

        // Interface elements
        private Label structureSizeLabel;
        private Label timerDelayLabel;
        private Label mouseMovementStepLabel;
        private Label mouseMovementTimeLabel;
        private Label mouseMovementLongLabel;

        private TextBox structureSizeTextBox;
        private TextBox timerDelayTextBox;
        private TextBox mouseMovementStepsTextBox;
        private TextBox mouseMovementTimeTextBox;
        private CheckBox mouseMovementLongCheckBox;

        private Button saveButton;
        private Button loadButton;

        public Settings()
        {
            InitializeComponent();
        }

        public Settings(ref DataManager iMyData)
        {
            InitializeComponent();

            // refer to data manager
            this.myData = iMyData;

            // generate new buttons
            this.CreateInterface();

            // fill fields with data
            this.WriteDataIntoFields();
        }

        public void CreateInterface()
        {
            int tempFirstRow = 5;
            int tempSecondRow = 130;

            int tempLenghtLabel = 120;
            int tempLenghtInput = 40;

            int sec1 = 0;
            int sec2 = 80;
            int sec3 = 180;
            this.Size = new Size(175, 200);

            Label tempLabel = new Label();
            this.SetParameters(ref tempLabel, new Size(tempLenghtLabel+30, 20), new Point(5, sec1 + 5), Color.Black, Color.White, "General options");
            tempLabel.Font = new Font(tempLabel.Font, FontStyle.Bold);
            this.Controls.Add(tempLabel);

            // Option: structure size 
            this.structureSizeLabel = new Label();
            this.SetParameters(ref this.structureSizeLabel, new Size(tempLenghtLabel, 20), new Point(tempFirstRow, sec1 + 30), Color.Black, Color.White, "Number of HotKeys");
            this.Controls.Add(this.structureSizeLabel);

            this.structureSizeTextBox = new TextBox();
            this.SetParameters(ref this.structureSizeTextBox, new Size(tempLenghtInput, 20), new Point(tempSecondRow, sec1 + 27), Color.Black, Color.White, "");
            this.structureSizeTextBox.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(this.structureSizeTextBox);

            // Option: structure size 
            this.timerDelayLabel = new Label();
            this.SetParameters(ref this.timerDelayLabel, new Size(tempLenghtLabel, 20), new Point(tempFirstRow, sec1 + 55), Color.Black, Color.White, "Waiting time");
            this.Controls.Add(this.timerDelayLabel);

            this.timerDelayTextBox = new TextBox();
            this.SetParameters(ref this.timerDelayTextBox, new Size(tempLenghtInput, 20), new Point(tempSecondRow, sec1 + 52), Color.Black, Color.White, "");
            this.timerDelayTextBox.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(this.timerDelayTextBox);

            tempLabel = new Label();
            this.SetParameters(ref tempLabel, new Size(tempLenghtLabel+30, 20), new Point(5, sec2 + 0), Color.Black, Color.White, "Mouse movement options");
            tempLabel.Font = new Font(tempLabel.Font, FontStyle.Bold);
            this.Controls.Add(tempLabel);

            // Option: mouse movement steps 
            this.mouseMovementStepLabel = new Label();
            this.SetParameters(ref this.mouseMovementStepLabel, new Size(tempLenghtLabel, 20), new Point(tempFirstRow, sec2 + 25), Color.Black, Color.White, "Number of steps");
            this.Controls.Add(this.mouseMovementStepLabel);

            this.mouseMovementStepsTextBox = new TextBox();
            this.SetParameters(ref this.mouseMovementStepsTextBox, new Size(tempLenghtInput, 20), new Point(tempSecondRow, sec2 + 25 - 3), Color.Black, Color.White, "");
            this.Controls.Add(this.mouseMovementStepsTextBox);
            
            // Option: mouse movement time
            this.mouseMovementTimeLabel = new Label();
            this.SetParameters(ref this.mouseMovementTimeLabel, new Size(tempLenghtLabel, 20), new Point(tempFirstRow, sec2 + 50), Color.Black, Color.White, "Movement time");
            this.Controls.Add(this.mouseMovementTimeLabel);

            this.mouseMovementTimeTextBox = new TextBox();
            this.SetParameters(ref this.mouseMovementTimeTextBox, new Size(tempLenghtInput, 20), new Point(tempSecondRow, sec2 + 50 - 3), Color.Black, Color.White, "");
            this.Controls.Add(this.mouseMovementTimeTextBox);

            // Option: mouse movement type 
            this.mouseMovementLongLabel = new Label();
            this.SetParameters(ref this.mouseMovementLongLabel, new Size(tempLenghtLabel, 20), new Point(tempFirstRow, sec2 + 75), Color.Black, Color.White, "Smooth movement");
            this.Controls.Add(this.mouseMovementLongLabel);

            this.mouseMovementLongCheckBox = new CheckBox();
            this.SetParameters(ref this.mouseMovementLongCheckBox, new Size(tempLenghtInput, 20), new Point(tempSecondRow + 13, sec2 + 75 - 1), Color.Black, Color.White, "");
            this.Controls.Add(this.mouseMovementLongCheckBox);

            // Positions for save and load buttons
            this.saveButton = new Button();
            this.SetParameters(ref this.saveButton, new Size(45, 20), new Point(5, sec3), Color.Black, Color.White, "SAVE");
            this.saveButton.Click += new EventHandler(this.SaveButton_Click);
            this.Controls.Add(this.saveButton);

            this.loadButton = new Button();
            this.SetParameters(ref this.loadButton, new Size(45, 20), new Point(55, sec3), Color.Black, Color.White, "LOAD");
            this.loadButton.Click += new EventHandler(this.LoadButton_Click);
            this.Controls.Add(this.loadButton);
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

        private void WriteDataIntoFields()
        {
            this.structureSizeTextBox.Text = this.myData.TakeOptionStructureSize("string");
            this.timerDelayTextBox.Text = this.myData.TakeOptionTimerDelay("string");
            this.mouseMovementStepsTextBox.Text = this.myData.TakeOptionouseMovementSteps("string");
            this.mouseMovementTimeTextBox.Text = this.myData.TakeOptionouseMovementTime("string");
            this.mouseMovementLongCheckBox.Checked = this.myData.TakeOptionouseMovementLong("bool");
        }

        private void LoadDataFromFields()
        {
            this.myData.PutOptionStructureSize(this.structureSizeTextBox.Text);
            this.myData.PutOptionTimerDelay(this.timerDelayTextBox.Text);
            this.myData.PutOptionMouseMovementSteps(this.mouseMovementStepsTextBox.Text);
            this.myData.PutOptionMouseMovementTime(this.mouseMovementTimeTextBox.Text);
            this.myData.PutOptionMouseMovementLong(this.mouseMovementLongCheckBox.Checked.ToString());
        }
        // ******************** EVENTS FOR GUI ELEMENTS ********************
        private void SaveButton_Click(object iSender, EventArgs iE) // Save actual page into ini file
        {
            // Write data from fields into hotkey structure
            this.LoadDataFromFields();

            // Write data from hotkey structure into ini file
            this.myData.PutSettingsIntoIni();

            // Rebuild hotkey structure to adjust size
            this.myData.RebuildHotkeyStructure();

            // Read data from ini
            this.myData.TakeDataFromIni();
        }
        private void LoadButton_Click(object iSender, EventArgs iE) // Load all data from ini file into fields and open page 1
        {
            // Write data from ini file into hotkey structure
            this.myData.TakeSettingsFromIni();

            // Write data from hotkey structure into fields
            this.WriteDataIntoFields();
        }
        private void SettingsForm_MouseDown(object iSender, MouseEventArgs iE) // allow moving window
        {
            //CheckBoxManagement();
            if (iE.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        // ******************** EVENTS FOR GUI ELEMENTS ********************
    }
}
