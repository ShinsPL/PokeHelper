using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using IniFileManagement;

namespace dataManager
{
    public struct iniFileManagement // Structure with data for ini file
    {
        // ******************** DEFINITIONS FOR VARIABLES ********************
        public string fileName; // name of ini file
        public string secMain;  // section with general parameters
        public string structureSize; // numbers of hotkeys
        public string timerDelay;
        public string mouseMovementSteps; // number of steps for mouse movement between poinst
        public string mouseMovementTime; // time of mouse movement
        public string mouseMovementLong; // infromation in smooth mouse movement should be selected

        public string secHetkey; // sections for hotkeys
        public string posX; // position x
        public string posY; // position y
        public string button; // using hotkey
        public string desc; // description

        public string defaultStructureSize; // default value of positions
        public string defaultTimerDelay;
        public string defaultMouseMovementSteps; // default value of steps
        public string defaultMouseMovementTime; // default value of time
        public string defaultMouseMovementLong; // default value of load
        public string defaultPos; // default value for positions X and Y
        public string defaultButton;  // default value for hotkey button
        public string defaultDesc;  // default value for description

        // ******************** DEFINITIONS FOR VARIABLES ********************

        public iniFileManagement(bool mainIni = false)
        {
            if (mainIni)
            {
                this.fileName = "PokeHelper.ini";
                this.secMain = "GENERAL";
                this.structureSize = "quantity";
                this.timerDelay = "timer_delay";
                this.mouseMovementSteps = "movement_steps";
                this.mouseMovementTime = "movement_time";
                this.mouseMovementLong = "mavement_long";
                this.secHetkey = "HOTKEY";
                this.posX = "posX";
                this.posY = "poxY";
                this.button = "button";
                this.desc = "desc";
                this.defaultStructureSize = "6";
                this.defaultTimerDelay = "100";
                this.defaultMouseMovementSteps = "8";
                this.defaultMouseMovementTime = "100";
                this.defaultMouseMovementLong = "false";
                this.defaultPos = "0";
                this.defaultButton = " ";
                this.defaultDesc = "empty";
            }
            else
            {
                this.fileName = "PokeHelperAdditional.ini";
                this.secMain = "GENERAL";
                this.structureSize = "quantity";
                this.timerDelay = "timer_delay";
                this.mouseMovementSteps = "movement_steps";
                this.mouseMovementTime = "movement_time";
                this.mouseMovementLong = "mavement_long";
                this.secHetkey = "HOTKEY";
                this.posX = "posX";
                this.posY = "poxY";
                this.button = "button";
                this.desc = "desc";
                this.defaultStructureSize = "6";
                this.defaultTimerDelay = "100";
                this.defaultMouseMovementSteps = "8";
                this.defaultMouseMovementTime = "100";
                this.defaultMouseMovementLong = "false";
                this.defaultPos = "0";
                this.defaultButton = " ";
                this.defaultDesc = "empty";
            }
        }
        public string BundleSecHotkey(int number)
        {
            return this.secHetkey + " POSITION " + number.ToString();
        }
    }
    public struct OptionsStruct // Structure with options
    {

        public string structureSize;
        public string timerDelay;
        public string mouseMovementSteps;
        public string mouseMovementTime;
        public string mouseMovementLong;

        public OptionsStruct(int iStructureSize)
        {
            structureSize = iStructureSize.ToString();
            timerDelay = "100";
            mouseMovementSteps = "8";
            mouseMovementTime = "250";
            mouseMovementLong = "false";
        }
        public void ShowData(string iTitle)
        {
            MessageBox.Show(iTitle +
                    "\nStr.size = " + this.structureSize +
                    "\ntimerDelay = " + this.timerDelay +
                    "\nsteps = " + this.mouseMovementSteps +
                    "\ntime = " + this.mouseMovementTime +
                    "\nlong = " + this.mouseMovementLong);
        }
    }
    public struct hotKeyButtonStruct // Structure for hotkey data
    {
        public string posX;
        public string posY;
        public string button;
        public string desc;

        public void ShowPos()
        {
            MessageBox.Show("Button: " + this.button + ", desc: " + this.desc + ", (X,Y): (" + this.posX + "," + this.posY + ")");
        }

    }
    public class DataManager // Class to manage specific ini file for this application
    {
        private OptionsStruct Options; // structure for option data
        private hotKeyButtonStruct[] hotKey; // structure for hotkey data

        private iniFileManagement myIni; // name of init file with positions for hotkeys
        public DataManager() // Constructor for specific ini file
        {
            // Create data for ini file
            this.myIni = new iniFileManagement(true);

            // Read hotkey options
            this.Options = new OptionsStruct();

            // Check if ini file exist and all settings data are correct
            this.CheckSettingsIniFile();

            // Read options for hotkey
            this.TakeSettingsFromIni();

            // Create hotkey structure
            this.hotKey = new hotKeyButtonStruct[this.TakeOptionStructureSize()];

            // Check if ini file exist and all hotkey data are correct
            this.CheckHotkeyIniFile();

            // Read data for hotkey
            this.TakeDataFromIni();
        }

        //******************** ACCESSING METHODS ********************

        // READ AND WRITE OPTION STRUCTURE SIZE
        public dynamic TakeOptionStructureSize(string iType = "int") // Read structure size from structure Option
        {
            switch (iType)
                {
                case "string":
                    return this.Options.structureSize;

                case "int":
                    int tempIntParam = 0;
                    try
                    {
                        tempIntParam = int.Parse(this.Options.structureSize);
                    }

                    catch (FormatException)
                    {
                        tempIntParam = int.Parse(this.myIni.defaultStructureSize);
                        this.PutOptionStructureSize(this.myIni.defaultStructureSize);
                        this.PutSettingsIntoIni(this.myIni.structureSize);
                    }
                    return tempIntParam;

                default:
                    return 0;
            }
        }
        /*public int intTakeOptionStructureSize()
        {
            int tempIntParam = 0;
            try
            {
                tempIntParam = int.Parse(this.Options.structureSize);
            }

            catch(FormatException)
            {
                tempIntParam = int.Parse(this.myIni.defaultStructureSize);
                this.PutOptionStructureSize(this.myIni.defaultStructureSize);
                this.PutSettingsIntoIni(this.myIni.structureSize);
            }

            return tempIntParam;
        }
        public string strTakeOptionStructureSize()
        {
            return this.Options.structureSize;
        } */
        public void PutOptionStructureSize(string iStructureSize) // Write structure size into structure Option
        {
            this.Options.structureSize = iStructureSize;
        }
        // READ AND WRITE OPTION TIMER DELAY
        public dynamic TakeOptionTimerDelay(string iType = "int") // Read structure size from structure Option
        {
            switch (iType)
            {
                case "string":
                    return this.Options.timerDelay;

                case "int":
                    int tempIntParam = 0;
                    try
                    {
                        tempIntParam = int.Parse(this.Options.timerDelay);
                    }

                    catch (FormatException)
                    {
                        tempIntParam = int.Parse(this.myIni.defaultTimerDelay);
                        this.PutOptionTimerDelay(this.myIni.defaultTimerDelay);
                        this.PutSettingsIntoIni(this.myIni.defaultTimerDelay);
                    }
                    return tempIntParam;

                default:
                    return 0;
            }
        }
        public void PutOptionTimerDelay(string iTimerDelay) // Write structure size into structure Option
        {
            this.Options.timerDelay = iTimerDelay;
        }
        // READ AND WRITE OPTION MOUSE MOVEMENT STEPS
        public dynamic TakeOptionouseMovementSteps(string iType = "int") // Read steps from structure Option
        {
            switch (iType)
            {
                case "string":
                    return this.Options.mouseMovementSteps;

                case "int":
                    int tempIntParam = 0;
                    try
                    {
                        tempIntParam = int.Parse(this.Options.mouseMovementSteps);
                    }

                    catch (FormatException)
                    {
                        tempIntParam = int.Parse(this.myIni.defaultMouseMovementSteps);
                        this.PutOptionStructureSize(this.myIni.defaultMouseMovementSteps);
                        this.PutSettingsIntoIni(this.myIni.mouseMovementSteps);
                    }
                    return tempIntParam;

                default:
                    return 0;
            }
        }
        /*public int intTakeOptionMouseMovementSteps()
        {
            int tempIntParam = 0;
            try
            {
                tempIntParam = int.Parse(this.Options.mouseMovementSteps);
            }

            catch (FormatException)
            {
                tempIntParam = int.Parse(this.myIni.defaultMouseMovementSteps);
                this.PutOptionMouseMovementSteps(this.myIni.defaultMouseMovementSteps);
                this.PutSettingsIntoIni(this.myIni.mouseMovementSteps);
            }
            return tempIntParam;
        }
        public string strTakeOptionMouseMovementSteps()
        {
            return this.Options.mouseMovementSteps;
        }*/
        public void PutOptionMouseMovementSteps(string iMouseMovementSteps) // Write steps into structure Option
        {
            this.Options.mouseMovementSteps = iMouseMovementSteps;
        }
        // READ AND WRITE OPTION MOUSE MOVEMENT TIME
        public dynamic TakeOptionouseMovementTime(string iType = "int") // Read time from structure Option
        {
            switch (iType)
            {
                case "string":
                    return this.Options.mouseMovementTime;

                case "int":
                    int tempIntParam = 0;
                    try
                    {
                        tempIntParam = int.Parse(this.Options.mouseMovementTime);
                    }

                    catch (FormatException)
                    {
                        tempIntParam = int.Parse(this.myIni.defaultMouseMovementTime);
                        this.PutOptionStructureSize(this.myIni.defaultMouseMovementTime);
                        this.PutSettingsIntoIni(this.myIni.mouseMovementTime);
                    }
                    return tempIntParam;

                default:
                    return 0;
            }
        }
        /*public int intTakeOptionMouseMovementTime()
        {
            int tempIntParam = 0;
            try
            {
                tempIntParam = int.Parse(this.Options.mouseMovementTime);
            }

            catch (FormatException)
            {
                tempIntParam = int.Parse(this.myIni.defaultMouseMovementTime);
                this.PutOptionMouseMovementTime(this.myIni.defaultMouseMovementTime);
                this.PutSettingsIntoIni(this.myIni.mouseMovementTime);
            }
            return tempIntParam;
        }
        public string strTakeOptionMouseMovementTime()
        {
            return this.Options.mouseMovementTime;
        }*/
        public void PutOptionMouseMovementTime(string iMouseMovementTime) // Write time into structure Option
        {
            this.Options.mouseMovementTime = iMouseMovementTime;
        }
        // READ AND WRITE OPTION MOUSE MOVEMENT LONG?
        public dynamic TakeOptionouseMovementLong(string iType = "bool") // Read long from structure Option
        {
            bool tempIntParam = false;
            switch (iType)
            {
                case "string":
                    return this.Options.mouseMovementLong;

                case "int":
                    try
                    {
                        tempIntParam = bool.Parse(this.Options.mouseMovementLong);
                    }

                    catch (FormatException)
                    {
                        tempIntParam = bool.Parse(this.myIni.defaultMouseMovementLong);
                        this.PutOptionStructureSize(this.myIni.defaultMouseMovementLong);
                        this.PutSettingsIntoIni(this.myIni.mouseMovementLong);
                    }
                    if (tempIntParam) return 1;
                    else return 0;

                case "bool":
                    try
                    {
                        tempIntParam = bool.Parse(this.Options.mouseMovementLong);
                    }

                    catch (FormatException)
                    {
                        tempIntParam = bool.Parse(this.myIni.defaultMouseMovementLong);
                        this.PutOptionStructureSize(this.myIni.defaultMouseMovementLong);
                        this.PutSettingsIntoIni(this.myIni.mouseMovementLong);
                    }
                    return tempIntParam;

                default:
                    return 0;
            }
        }
        /*public bool boolTakeOptionMouseMovementLong()
        {

            bool tempIntParam = false;
            try
            {
                tempIntParam = bool.Parse(this.Options.mouseMovementLong);
            }

            catch (FormatException)
            {
                tempIntParam = bool.Parse(this.myIni.defaultMouseMovementLong);
                this.PutOptionMouseMovementLong(this.myIni.defaultMouseMovementLong);
                this.PutSettingsIntoIni(this.myIni.mouseMovementLong);
            }
            return tempIntParam;
        }
        public string strTakeOptionMouseMovementLong()
        {
            return this.Options.mouseMovementLong;
        }*/
        public void PutOptionMouseMovementLong(string iMouseMovementLong) // Write long into structure Option
        {
            this.Options.mouseMovementLong = iMouseMovementLong;
        }
        // READ AND WRITE HOTKEY VALUES
        public hotKeyButtonStruct structTakeHotKey(int iPosition)
        {
            return this.hotKey[iPosition];
        }

        public void PutHotKey(int iPosition, string iButton, string iPosX, string iPosY, string iDesc)
        {
            this.hotKey[iPosition].button = iButton;
            this.hotKey[iPosition].posX = iPosX;
            this.hotKey[iPosition].posY = iPosY;
            this.hotKey[iPosition].desc = iDesc;
        }
        public void PutHotKey(int iPosition, string iParam, string iValue)
        {
            if (iParam == this.myIni.button) this.hotKey[iPosition].button = iValue;
            if (iParam == this.myIni.desc) this.hotKey[iPosition].desc = iValue;
            if (iParam == this.myIni.posX) this.hotKey[iPosition].posX = iValue;
            if (iParam == this.myIni.posY) this.hotKey[iPosition].posY = iValue;
        }

        //******************** ACCESSING METHODS ********************

        private void CheckSettingsIniFile() // Check if into ini file all settings data are availaible
        {
            var MyIniFile = new IniFile(this.myIni.fileName);
            if (!MyIniFile.KeyExists(this.myIni.structureSize, this.myIni.secMain)) // check key in section existance
                MyIniFile.Write(this.myIni.structureSize, this.myIni.defaultStructureSize, this.myIni.secMain); //if not write default value

            if (!MyIniFile.KeyExists(this.myIni.timerDelay, this.myIni.secMain)) // check key in section existance
                MyIniFile.Write(this.myIni.timerDelay, this.myIni.defaultTimerDelay, this.myIni.secMain); //if not write default value

            if (!MyIniFile.KeyExists(this.myIni.mouseMovementSteps, this.myIni.secMain)) // check key in section existance
                MyIniFile.Write(this.myIni.mouseMovementSteps, this.myIni.defaultMouseMovementSteps, this.myIni.secMain); //if not write default value
            
            if (!MyIniFile.KeyExists(this.myIni.mouseMovementTime, this.myIni.secMain)) // check key in section existance
                MyIniFile.Write(this.myIni.mouseMovementTime, this.myIni.defaultMouseMovementTime, this.myIni.secMain); //if not write default value
            
            if (!MyIniFile.KeyExists(this.myIni.mouseMovementLong, this.myIni.secMain)) // check key in section existance
                MyIniFile.Write(this.myIni.mouseMovementLong, this.myIni.defaultMouseMovementLong, this.myIni.secMain); //if not write default value
        }
        private void CheckHotkeyIniFile() // Check if into ini file all hotkey data are availaible
        {
            var MyIniFile = new IniFile(this.myIni.fileName);

            for (int i = 0; i < this.TakeOptionStructureSize(); i++)
            {
                string sectionName = this.myIni.BundleSecHotkey(i);

                if (!MyIniFile.KeyExists(this.myIni.posX, sectionName)) MyIniFile.Write(this.myIni.posX, this.myIni.defaultPos, sectionName);

                if (!MyIniFile.KeyExists(this.myIni.posY, sectionName)) MyIniFile.Write(this.myIni.posY, this.myIni.defaultPos, sectionName);

                if (!MyIniFile.KeyExists(this.myIni.button, sectionName)) MyIniFile.Write(this.myIni.button, this.myIni.defaultButton, sectionName);

                if (!MyIniFile.KeyExists(this.myIni.desc, sectionName)) MyIniFile.Write(this.myIni.desc, this.myIni.defaultDesc, sectionName);
            }
        }
        public OptionsStruct TakeSettingsFromIni() // Read setting from ini file
        {
            // Open or create file
            var MyIniFile = new IniFile(this.myIni.fileName);
            this.PutOptionStructureSize(MyIniFile.Read(this.myIni.structureSize, this.myIni.secMain));
            this.PutOptionTimerDelay(MyIniFile.Read(this.myIni.timerDelay, this.myIni.secMain));
            this.PutOptionMouseMovementSteps(MyIniFile.Read(this.myIni.mouseMovementSteps, this.myIni.secMain));
            this.PutOptionMouseMovementTime(MyIniFile.Read(this.myIni.mouseMovementTime, this.myIni.secMain));
            this.PutOptionMouseMovementLong(MyIniFile.Read(this.myIni.mouseMovementLong, this.myIni.secMain));

            return this.Options;
        }
        public hotKeyButtonStruct[] TakeDataFromIni() // Read data from ini file
        {
            var MyIniFile = new IniFile(this.myIni.fileName);

            for (int i = 0; i < this.TakeOptionStructureSize(); i++)
            {
                string sectionName = this.myIni.BundleSecHotkey(i);
                this.PutHotKey(i, MyIniFile.Read(this.myIni.button, sectionName), // read hotkey
                    MyIniFile.Read(this.myIni.posX, sectionName), // read posX
                    MyIniFile.Read(this.myIni.posY, sectionName), // read posY
                    MyIniFile.Read(this.myIni.desc, sectionName)); // read description
            }
            return this.hotKey;
        }
        public void PutDataIntoIni(int iSinglePos = -1, string iParam = "All") // Write data into file - can select section and param
        {
            // Open or create ini file            
            var MyIniFile = new IniFile(this.myIni.fileName);

            for (int i = 0; i < this.TakeOptionStructureSize(); i++)
            {
                string sectionName = this.myIni.BundleSecHotkey(i); // Bundle section name
                if ((iParam == this.myIni.posX || iParam == "All") && (iSinglePos == i || iSinglePos < 0)) 
                    MyIniFile.Write(this.myIni.posX, this.structTakeHotKey(i).posX, sectionName); // write posX into ini file in bundled section
                if ((iParam == this.myIni.posY || iParam == "All") && (iSinglePos == i || iSinglePos < 0)) 
                    MyIniFile.Write(this.myIni.posY, this.structTakeHotKey(i).posY, sectionName); // write posY into ini file in bundled section
                if ((iParam == this.myIni.button || iParam == "All") && (iSinglePos == i || iSinglePos < 0)) 
                    MyIniFile.Write(this.myIni.button, this.structTakeHotKey(i).button, sectionName);  // write button into ini file in bundled section
                if ((iParam == this.myIni.desc || iParam == "All") && (iSinglePos == i || iSinglePos < 0)) 
                    MyIniFile.Write(this.myIni.desc, this.structTakeHotKey(i).desc, sectionName);  // write desc into ini file in bundled section
            }

        }
        public void PutSettingsIntoIni(string iSetting = "All") // Wrute setting into ni file - can selec param
        {
            // Open or create ini file            
            var MyIniFile = new IniFile(this.myIni.fileName);
            if (iSetting == "All" || iSetting == this.myIni.structureSize) 
                MyIniFile.Write(this.myIni.structureSize, this.TakeOptionStructureSize("string"), this.myIni.secMain); // write structure size into ini file
            if (iSetting == "All" || iSetting == this.myIni.timerDelay)
                MyIniFile.Write(this.myIni.timerDelay, this.TakeOptionTimerDelay("string"), this.myIni.secMain); // write timer delay into ini file
            if (iSetting == "All" || iSetting == this.myIni.mouseMovementSteps) 
                MyIniFile.Write(this.myIni.mouseMovementSteps, this.TakeOptionouseMovementSteps("string"), this.myIni.secMain); // write steps for mouse movement into ini file
            if (iSetting == "All" || iSetting == this.myIni.mouseMovementTime) 
                MyIniFile.Write(this.myIni.mouseMovementTime, this.TakeOptionouseMovementTime("string"), this.myIni.secMain); // write time for mouse movement into ini file
            if (iSetting == "All" || iSetting == this.myIni.mouseMovementLong) 
                MyIniFile.Write(this.myIni.mouseMovementLong, this.TakeOptionouseMovementLong("string"), this.myIni.secMain); // write if long mouse movement available into ini file
        }
        public void RebuildHotkeyStructure()
        {
            this.hotKey = new hotKeyButtonStruct[this.TakeOptionStructureSize()];
        }
    }
}
