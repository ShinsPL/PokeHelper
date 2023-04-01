using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using System.Diagnostics;
using System.Runtime.InteropServices;

using dataManager;

namespace PokeHelper
{
    struct RowInForm
    {
        public TextBox button;
        public TextBox desc;
        public Label posX;
        public Label posY;
        public CheckBox check;
    }
    public partial class HotkeyManagement : Form
    {
        // ******************** DEFINITIONS FOR VARIABLES ********************

        // Functions to anable movin window without borders
        [DllImportAttribute("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")] public static extern bool ReleaseCapture();

        // Const to enable moving window without borders
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        //Variables
        private const int ROWS_BY_PAGE = 6;

        private DataManager myData;  // instance for my specific ini file
        private int maxPages; // define how many pages we have
        private int actualPage; // set actual page

        // Interface elements
        private RowInForm[] dataInForm;
        private Label actualPageLabel;
        private Button leftButton;
        private Button rightButton;
        private Button saveButton;
        private Button loadButton;

        // ******************** DEFINITIONS FOR VARIABLES ********************

        public HotkeyManagement()
        {
            InitializeComponent();

            // Create ini file
            this.myData = new DataManager();

            // create structure for buttons
            this.dataInForm = new RowInForm[ROWS_BY_PAGE];

            // generate new buttons
            this.CreateInterface();

            // initial value for page managing
            this.MaxPagesCheck();
            this.actualPage = 1;

            // Read data from hotkey structure and place into fields in form
            this.WriteDataIntoFields();

            // Check visibility for buttons left and right and update page number
            this.CheckVisibilityAndPageNumber();
        }
        public HotkeyManagement(ref DataManager iMyData)
        {
            InitializeComponent();

            // refer to data manager
            this.myData = iMyData;

            // create structure for buttons
            this.dataInForm = new RowInForm[ROWS_BY_PAGE];

            // generate new buttons
            this.CreateInterface();

            // initial value for page managing
            this.MaxPagesCheck();
            this.actualPage = 1;

            // Read data from hotkey structure and place into fields in form
            this.WriteDataIntoFields();

            // Check visibility for buttons left and right and update page number
            this.CheckVisibilityAndPageNumber();
        }
        private void CreateInterface()
        {
            this.Size = new Size(425, 40 + 25 * ROWS_BY_PAGE + 5);

            int[] Column = { 20, 60, 275, 320, 380 };

            this.leftButton = new Button();
            this.SetParameters(ref this.leftButton, new Size(20, 20), new Point(Column[3] - 155, 15 - 3), Color.Black, Color.White, "<");
            this.leftButton.Click += new EventHandler(this.LeftButton_Click);
            this.Controls.Add(this.leftButton);

            this.actualPageLabel = new Label();
            this.SetParameters(ref this.actualPageLabel, new Size(35, 20), new Point(Column[3] - 130, 15), Color.Black, Color.White, "X / X");
            this.Controls.Add(this.actualPageLabel);

            this.rightButton = new Button();
            this.SetParameters(ref this.rightButton, new Size(20, 20), new Point(Column[3] - 95, 15 - 3), Color.Black, Color.White, ">");
            this.rightButton.Click += new EventHandler(this.RightButton_Click);
            this.Controls.Add(this.rightButton);

            Label tempLabel = new Label();
            this.SetParameters(ref tempLabel, new Size(50, 20), new Point(Column[0] - 10, 15), Color.Black, Color.White, "HotKeys");
            this.Controls.Add(tempLabel);

            tempLabel = new Label();
            this.SetParameters(ref tempLabel, new Size(75, 20), new Point(Column[1], 15), Color.Black, Color.White, "Descriptions");
            this.Controls.Add(tempLabel);

            tempLabel = new Label();
            this.SetParameters(ref tempLabel, new Size(40, 20), new Point(Column[2] - 3, 15), Color.Black, Color.White, "Pos X");
            this.Controls.Add(tempLabel);

            tempLabel = new Label();
            this.SetParameters(ref tempLabel, new Size(40, 20), new Point(Column[3] - 3, 15), Color.Black, Color.White, "Pos Y");
            this.Controls.Add(tempLabel);

            tempLabel = new Label();
            this.SetParameters(ref tempLabel, new Size(40, 20), new Point(Column[4] - 10, 15), Color.Black, Color.White, "Teach");
            this.Controls.Add(tempLabel);

            for (int i = 0; i < 6; i++)
            {
                this.dataInForm[i].button = new TextBox();
                this.SetParameters(ref this.dataInForm[i].button, new Size(25, 20), new Point(Column[0], 40 - 3 + 25 * i), Color.Black, Color.White, "X", "button" + i.ToString(), "button" + i.ToString());
                this.Controls.Add(dataInForm[i].button);

                this.dataInForm[i].desc = new TextBox();
                this.SetParameters(ref this.dataInForm[i].desc, new Size(200, 20), new Point(Column[1], 40 - 3 + 25 * i), Color.Black, Color.White, "initialized", "desc" + i.ToString(), "desc" + i.ToString());
                this.Controls.Add(dataInForm[i].desc);

                this.dataInForm[i].posX = new Label();
                this.SetParameters(ref this.dataInForm[i].posX, new Size(40, 20), new Point(Column[2], 40 + 25 * i), Color.Black, Color.White, "XXX", "posX" + i.ToString(), "posX" + i.ToString());
                this.Controls.Add(dataInForm[i].posX);

                this.dataInForm[i].posY = new Label();
                this.SetParameters(ref this.dataInForm[i].posY, new Size(40, 20), new Point(Column[3], 40 + 25 * i), Color.Black, Color.White, "YYY", "posY" + i.ToString(), "posY" + i.ToString());
                this.Controls.Add(dataInForm[i].posY);

                this.dataInForm[i].check = new CheckBox();
                this.SetParameters(ref this.dataInForm[i].check, new Size(40, 20), new Point(Column[4], 40 + 25 * i), Color.Black, Color.White, "", "check" + i.ToString(), "check" + i.ToString());
                this.Controls.Add(dataInForm[i].check);
            }

            this.saveButton = new Button();
            this.SetParameters(ref this.saveButton, new Size(45, 20), new Point(Column[0], 40 + 25 * ROWS_BY_PAGE), Color.Black, Color.White, "SAVE");
            this.saveButton.Click += new EventHandler(this.SaveButton_Click);
            this.Controls.Add(this.saveButton);

            this.loadButton = new Button();
            this.SetParameters(ref this.loadButton, new Size(45, 20), new Point(Column[0] + 50, 40 + 25 * ROWS_BY_PAGE), Color.Black, Color.White, "LOAD");
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
        private void MaxPagesCheck() // Check how many pages we can have
        {
            // divide to check how many full pages we have
            int quotient = this.myData.TakeOptionStructureSize() / ROWS_BY_PAGE;

            // if structure size is higher than count of pages multiplied by row at 1 page, then we need 1 more page 
            if (quotient * ROWS_BY_PAGE < this.myData.TakeOptionStructureSize()) quotient++;

            // write value 
            this.maxPages = quotient;
        }
        private void CheckVisibilityAndPageNumber() // Check visibility for buttons left and right
        {
            // Check visibility for button left
            if (this.actualPage == 1) this.leftButton.Visible = false;
            else this.leftButton.Visible = true;

            // Check visibility for button right
            if (this.actualPage < this.maxPages) this.rightButton.Visible = true;
            else this.rightButton.Visible = false;

            // write string to show status of pages
            this.actualPageLabel.Text = this.actualPage.ToString() + " / " + this.maxPages.ToString();
        }
        
        // ******************** FORM FIELDS MANAGEMENT ********************
        public void ShowLine(int iPosition) // Show all gui elements in selected row
        {
            this.dataInForm[iPosition].button.Visible = true;
            this.dataInForm[iPosition].desc.Visible = true;
            this.dataInForm[iPosition].posX.Visible = true;
            this.dataInForm[iPosition].posY.Visible = true;
            this.dataInForm[iPosition].check.Visible = true;
        }
        public void HideLine(int iPosition) // Hide all gui elements in selected row
        {
            this.dataInForm[iPosition].button.Visible = false;
            this.dataInForm[iPosition].desc.Visible = false;
            this.dataInForm[iPosition].posX.Visible = false;
            this.dataInForm[iPosition].posY.Visible = false;
            this.dataInForm[iPosition].check.Visible = false;
        }
        public void WriteDataIntoFields() // Copy data from hotkey structure into form fields
        {
            int shift = (this.actualPage - 1) * ROWS_BY_PAGE; // Check which page is opened
            int max = this.myData.TakeOptionStructureSize() - 1; // check max. index for hotkey structure
            
            for (int i = 0; i < ROWS_BY_PAGE; i++)
            {
                if (i + shift <= max)
                {
                    this.ShowLine(i);
                    this.dataInForm[i].button.Text = this.myData.structTakeHotKey(i + shift).button;
                    this.dataInForm[i].desc.Text = this.myData.structTakeHotKey(i + shift).desc;
                    this.dataInForm[i].posX.Text = this.myData.structTakeHotKey(i + shift).posX;
                    this.dataInForm[i].posY.Text = this.myData.structTakeHotKey(i + shift).posY;
                }
                else
                {
                    this.HideLine(i);
                    this.dataInForm[i].button.Text = " ";
                    this.dataInForm[i].desc.Text = "Not defined";
                    this.dataInForm[i].posX.Text = "---";
                    this.dataInForm[i].posY.Text = "---";
                }
            }
        } 
        public void LoadDataFromFields() // Copy data from form fields into hotkey structure
        {
            int shift = (this.actualPage - 1) * ROWS_BY_PAGE;  // Check which page is opened
            int max = this.myData.TakeOptionStructureSize() - 1;  // check max. index for hotkey structure

            for (int i = 0; i < ROWS_BY_PAGE; i++)
            {
                if (i + shift <= max) this.myData.PutHotKey(i + shift, this.dataInForm[i].button.Text, 
                    this.dataInForm[i].posX.Text, 
                    this.dataInForm[i].posY.Text,
                    this.dataInForm[i].desc.Text);
            }
        }

        // ******************** FORM FIELDS MANAGEMENT ********************

        // ******************** EVENTS FOR GUI ELEMENTS ********************
        private void SaveButton_Click(object iSender, EventArgs iE) // Save actual page into ini file
        {
            // Write data from fields into hotkey structure
            this.LoadDataFromFields();

            // Write data from hotkey structure into ini file
            this.myData.PutDataIntoIni();
        }
        private void LoadButton_Click(object iSender, EventArgs iE) // Load all data from ini file into fields and open page 1
        {
            // Initial value for page managing
            this.MaxPagesCheck();
            this.actualPage = 1;

            // Write data from ini file into hotkey structure
            this.myData.TakeDataFromIni();

            // Write data from hotkey structure into fields
            this.WriteDataIntoFields();

            // Check visibility for buttons left and right and update page number
            this.CheckVisibilityAndPageNumber();
        }
        private void LeftButton_Click(object iSender, EventArgs iE) // Change page to 1 lower (min. 1)
        {
            // If page is higher than 1 decrease page
            if (this.actualPage > 1) this.actualPage--;

            // Copy data from ini file into fields
            this.WriteDataIntoFields();

            // Check visibility for buttons left and right and update page number
            this.CheckVisibilityAndPageNumber();

        }
        private void RightButton_Click(object iSender, EventArgs iE) // Change page to 1 higher (max. maxPages)
        {
            // If page is lower than maxPages increase page
            if (this.actualPage < this.maxPages) this.actualPage++;

            // Copy data from ini file into fields
            this.WriteDataIntoFields();

            // Check visibility for buttons left and right and update page number
            this.CheckVisibilityAndPageNumber();
        }
        private void CheckBoxManagement() // After pressing mouse left button write cursor position into fields and uncheck box
        {
            for (int i = 0; i < ROWS_BY_PAGE; i++)
            {
                if (this.dataInForm[i].check.Checked)
                {
                    this.dataInForm[i].posX.Text = Cursor.Position.X.ToString();
                    this.dataInForm[i].posY.Text = Cursor.Position.Y.ToString();
                    this.dataInForm[i].check.Checked = false;
                }
            }
        }
        private void HotkeyManagement_Deactivate(object iSender, EventArgs iE) // when window hotkey management lost focus trigger check box management
        {
            this.CheckBoxManagement();
        }
        private void HotkeyManagement_MouseDown(object iSender, MouseEventArgs iE) // allow moving window
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
