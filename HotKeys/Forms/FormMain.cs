// Copyright ©2017 Copper Mountain Technologies
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
// ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using CopperMountainTech;
using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HotKeys
{
    public partial class FormMain : Form
    {
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private enum ComConnectionStateEnum
        {
            INITIALIZED,
            NOT_CONNECTED,
            CONNECTED_VNA_NOT_READY,
            CONNECTED_VNA_READY
        }

        private ComConnectionStateEnum previousComConnectionState = ComConnectionStateEnum.INITIALIZED;
        private ComConnectionStateEnum comConnectionState = ComConnectionStateEnum.NOT_CONNECTED;

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private GlobalHotKey globalHotKey_F1;
        private GlobalHotKey globalHotKey_F2;
        private GlobalHotKey globalHotKey_F3;
        private GlobalHotKey globalHotKey_F4;

        private GlobalHotKey globalHotKey_F5;
        private GlobalHotKey globalHotKey_F6;
        private GlobalHotKey globalHotKey_F7;
        private GlobalHotKey globalHotKey_F8;

        private GlobalHotKey globalHotKey_F9;
        private GlobalHotKey globalHotKey_F10;
        private GlobalHotKey globalHotKey_F11;
        private GlobalHotKey globalHotKey_F12;

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public FormMain()
        {
            InitializeComponent();

            // --------------------------------------------------------------------------------------------------------

            // set form icon
            Icon = Properties.Resources.app_icon;

            // set form title
            Text = Program.programName;

            // disable resizing the window
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;

            // position the plug-in in the lower right corner of the screen
            Rectangle workingArea = Screen.GetWorkingArea(this);
            Location = new Point(workingArea.Right - Size.Width - 130,
                                 workingArea.Bottom - Size.Height - 50);

            // always display on top
            TopMost = true;

            // --------------------------------------------------------------------------------------------------------

            // disable ui
            panelMain.Enabled = false;

            // set version label text
            toolStripStatusLabelVersion.Text = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

            // --------------------------------------------------------------------------------------------------------

            // clear key pressed label
            labelKeysPressed.Text = "";

            // --------------------------------------------------------------------------------------------------------

            globalHotKey_F1 = new GlobalHotKey(GlobalHotKey.NOMOD, Keys.F1, this);
            globalHotKey_F2 = new GlobalHotKey(GlobalHotKey.NOMOD, Keys.F2, this);
            globalHotKey_F3 = new GlobalHotKey(GlobalHotKey.NOMOD, Keys.F3, this);
            globalHotKey_F4 = new GlobalHotKey(GlobalHotKey.NOMOD, Keys.F4, this);

            globalHotKey_F5 = new GlobalHotKey(GlobalHotKey.NOMOD, Keys.F5, this);
            globalHotKey_F6 = new GlobalHotKey(GlobalHotKey.NOMOD, Keys.F6, this);
            globalHotKey_F7 = new GlobalHotKey(GlobalHotKey.NOMOD, Keys.F7, this);
            globalHotKey_F8 = new GlobalHotKey(GlobalHotKey.NOMOD, Keys.F8, this);

            globalHotKey_F9 = new GlobalHotKey(GlobalHotKey.NOMOD, Keys.F9, this);
            globalHotKey_F10 = new GlobalHotKey(GlobalHotKey.NOMOD, Keys.F10, this);
            globalHotKey_F11 = new GlobalHotKey(GlobalHotKey.NOMOD, Keys.F11, this);
            globalHotKey_F12 = new GlobalHotKey(GlobalHotKey.NOMOD, Keys.F12, this);

            // --------------------------------------------------------------------------------------------------------

            // start the ready timer
            readyTimer.Interval = 250; // 250 ms interval
            readyTimer.Enabled = true;
            readyTimer.Start();

            // --------------------------------------------------------------------------------------------------------
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //
        // Timers
        //
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void readyTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // is vna ready?
                if (Program.vna.app.Ready)
                {
                    // yes... vna is ready
                    comConnectionState = ComConnectionStateEnum.CONNECTED_VNA_READY;
                }
                else
                {
                    // no... vna is not ready
                    comConnectionState = ComConnectionStateEnum.CONNECTED_VNA_NOT_READY;
                }
            }
            catch (COMException)
            {
                // com connection has been lost
                comConnectionState = ComConnectionStateEnum.NOT_CONNECTED;
                Application.Exit();
                return;
            }

            if (comConnectionState != previousComConnectionState)
            {
                previousComConnectionState = comConnectionState;

                switch (comConnectionState)
                {
                    default:
                    case ComConnectionStateEnum.NOT_CONNECTED:

                        // update vna info text box
                        toolStripStatusLabelVnaInfo.ForeColor = Color.White;
                        toolStripStatusLabelVnaInfo.BackColor = Color.Red;
                        toolStripStatusLabelSpacer.BackColor = toolStripStatusLabelVnaInfo.BackColor;
                        toolStripStatusLabelVnaInfo.Text = "VNA NOT CONNECTED";

                        // disable ui
                        panelMain.Enabled = false;

                        // unregister keys
                        unRegisterKeys();

                        break;

                    case ComConnectionStateEnum.CONNECTED_VNA_NOT_READY:

                        // update vna info text box
                        toolStripStatusLabelVnaInfo.ForeColor = Color.White;
                        toolStripStatusLabelVnaInfo.BackColor = Color.Red;
                        toolStripStatusLabelSpacer.BackColor = toolStripStatusLabelVnaInfo.BackColor;
                        toolStripStatusLabelVnaInfo.Text = "VNA NOT READY";

                        // disable ui
                        panelMain.Enabled = false;

                        // unregister keys
                        unRegisterKeys();

                        break;

                    case ComConnectionStateEnum.CONNECTED_VNA_READY:

                        // get vna info
                        Program.vna.PopulateInfo(Program.vna.app.NAME);

                        // update vna info text box
                        toolStripStatusLabelVnaInfo.ForeColor = SystemColors.ControlText;
                        toolStripStatusLabelVnaInfo.BackColor = SystemColors.Control;
                        toolStripStatusLabelSpacer.BackColor = toolStripStatusLabelVnaInfo.BackColor;
                        toolStripStatusLabelVnaInfo.Text = Program.vna.modelString + "   " + "SN:" + Program.vna.serialNumberString + "   " + Program.vna.versionString;

                        // enable ui
                        panelMain.Enabled = true;

                        // register keys
                        registerKeys();

                        break;
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            // clear key pressed label
            labelKeysPressed.Text = "";

            // stop the update timer
            updateTimer.Stop();
            updateTimer.Enabled = false;
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == GlobalHotKey.WM_HOTKEY_MSG_ID)
            {
                processHotkey(m.WParam.ToInt32());
            }

            base.WndProc(ref m);
        }

        // ============================================================================================================

        private void processHotkey(int id)
        {
            // parse key
            if (id == globalHotKey_F1.Id)
            {
                // F1 - Cycle Through Channels
                // When multiple channels are configured:
                //     ● cycles through each channel
                //     ● maximizes the channel
                //     ● Sets the trigger scope to the channel(S2VNA and S4VNA only)

                // ----------------------------------------------------------------------------------------------------

                // display the key that was pressed
                labelKeysPressed.Text = globalHotKey_F1.Description;

                // ----------------------------------------------------------------------------------------------------

                try
                {
                    // determine maximized state of the active channel window
                    bool isActiveChannelWindowMaximized = Program.vna.app.SCPI.DISPlay.MAXimize;

                    // determine number of channels
                    long splitIndex = Program.vna.app.SCPI.DISPlay.SPLit;
                    int numOfChannels = Program.vna.DetermineNumberOfChannels(splitIndex);

                    // determine the active channel
                    long activeChannel = Program.vna.app.SCPI.SERVice.CHANnel.ACTive;

                    // is active channel window maximized?
                    if (isActiveChannelWindowMaximized == false)
                    {
                        // no...

                        // set active channel to 1
                        activeChannel = 1;
                        object err = Program.vna.app.SCPI.DISPlay.WINDow(activeChannel).ACTivate;

                        if ((Program.vna.family == VnaFamilyEnum.S2) ||
                            (Program.vna.family == VnaFamilyEnum.S4))
                        {
                            // set trigger scope to active channel (s2vna and s4vna only)
                            Program.vna.app.SCPI.TRIGger.SEQuence.SCOPe = "ACTive";
                        }

                        // set active channel window maximize to true
                        Program.vna.app.SCPI.DISPlay.MAXimize = true;
                    }
                    else
                    {
                        // yes...

                        // is activeChannel < numOfChannels?
                        if (activeChannel < numOfChannels)
                        {
                            // yes... activeChannel < numOfChannels

                            // increment the active channel
                            ++activeChannel;
                            object err = Program.vna.app.SCPI.DISPlay.WINDow(activeChannel).ACTivate;
                        }
                        else
                        {
                            // no... activeChannel >= numOfChannels

                            // set active channel to 1
                            activeChannel = 1;
                            object err = Program.vna.app.SCPI.DISPlay.WINDow(activeChannel).ACTivate;

                            // set active channel window maximize to false
                            Program.vna.app.SCPI.DISPlay.MAXimize = false;

                            if ((Program.vna.family == VnaFamilyEnum.S2) ||
                                (Program.vna.family == VnaFamilyEnum.S4))
                            {
                                // set trigger scope to all channels (s2vna and s4vna only)
                                Program.vna.app.SCPI.TRIGger.SEQuence.SCOPe = "ALL";
                            }
                        }
                    }
                }
                catch (COMException comException)
                {
                    // display error message
                    showMessageBoxForComException(comException);
                    return;
                }

                // ----------------------------------------------------------------------------------------------------
            }
            else if (id == globalHotKey_F2.Id)
            {
                // F2 - Save a Touchstone File
                // Saves a touchstone file using the format as configured by the VNA software to the default
                // (“FixtureSim”) directory.The filename includes a timestamp of when the file was saved.

                // ----------------------------------------------------------------------------------------------------

                // display the key that was pressed
                labelKeysPressed.Text = globalHotKey_F2.Description;

                // ----------------------------------------------------------------------------------------------------

                try
                {
                    // save a touchstone file
                    Program.vna.app.SCPI.MMEMory.STORe.SNP.DATA = DateTime.Now.ToString("yyyyMMddTHHmmss");
                }
                catch (COMException comException)
                {
                    // display error message
                    showMessageBoxForComException(comException);
                    return;
                }

                // ----------------------------------------------------------------------------------------------------
            }
            else if (id == globalHotKey_F3.Id)
            {
                // F3 - Save a Screenshot File
                // Saves a screenshot file using the print settings as configured by the VNA software in PNG format
                // to the default(“Image”) directory.The filename includes a timestamp of when the file was saved.

                // ----------------------------------------------------------------------------------------------------

                // display the key that was pressed
                labelKeysPressed.Text = globalHotKey_F3.Description;

                // ----------------------------------------------------------------------------------------------------

                try
                {
                    // save a vna screen shot
                    Program.vna.app.SCPI.MMEMory.STORe.IMAGe = DateTime.Now.ToString("yyyyMMddTHHmmss") + ".png";
                }
                catch (COMException comException)
                {
                    // display error message
                    showMessageBoxForComException(comException);
                    return;
                }

                // ----------------------------------------------------------------------------------------------------
            }
            else if (id == globalHotKey_F4.Id)
            {
                // F4 - << put description for this key here >>

                // ----------------------------------------------------------------------------------------------------

                // display the key that was pressed
                labelKeysPressed.Text = globalHotKey_F4.Description;

                // ----------------------------------------------------------------------------------------------------

                // << put code for this key here >>

                // ----------------------------------------------------------------------------------------------------
            }
            else if (id == globalHotKey_F5.Id)
            {
                // F5 - << put description for this key here >>

                // ----------------------------------------------------------------------------------------------------

                // display the key that was pressed
                labelKeysPressed.Text = globalHotKey_F5.Description;

                // ----------------------------------------------------------------------------------------------------

                // << put code for this key here >>

                // ----------------------------------------------------------------------------------------------------
            }
            else if (id == globalHotKey_F6.Id)
            {
                // F6 - << put description for this key here >>

                // ----------------------------------------------------------------------------------------------------

                // display the key that was pressed
                labelKeysPressed.Text = globalHotKey_F6.Description;

                // ----------------------------------------------------------------------------------------------------

                // << put code for this key here >>

                // ----------------------------------------------------------------------------------------------------
            }
            else if (id == globalHotKey_F7.Id)
            {
                // F7 - << put description for this key here >>

                // ----------------------------------------------------------------------------------------------------

                // display the key that was pressed
                labelKeysPressed.Text = globalHotKey_F7.Description;

                // ----------------------------------------------------------------------------------------------------

                // << put code for this key here >>

                // ----------------------------------------------------------------------------------------------------
            }
            else if (id == globalHotKey_F8.Id)
            {
                // F8 - << put description for this key here >>

                // ----------------------------------------------------------------------------------------------------

                // display the key that was pressed
                labelKeysPressed.Text = globalHotKey_F8.Description;

                // ----------------------------------------------------------------------------------------------------

                // << put code for this key here >>

                // ----------------------------------------------------------------------------------------------------
            }
            else if (id == globalHotKey_F9.Id)
            {
                // F9 - << put description for this key here >>

                // ----------------------------------------------------------------------------------------------------

                // display the key that was pressed
                labelKeysPressed.Text = globalHotKey_F9.Description;

                // ----------------------------------------------------------------------------------------------------

                // << put code for this key here >>

                // ----------------------------------------------------------------------------------------------------
            }
            else if (id == globalHotKey_F10.Id)
            {
                // F10 - << put description for this key here >>

                // ----------------------------------------------------------------------------------------------------

                // display the key that was pressed
                labelKeysPressed.Text = globalHotKey_F10.Description;

                // ----------------------------------------------------------------------------------------------------

                // << put code for this key here >>

                // ----------------------------------------------------------------------------------------------------
            }
            else if (id == globalHotKey_F11.Id)
            {
                // F11 - << put description for this key here >>

                // ----------------------------------------------------------------------------------------------------

                // display the key that was pressed
                labelKeysPressed.Text = globalHotKey_F11.Description;

                // ----------------------------------------------------------------------------------------------------

                // << put code for this key here >>

                // ----------------------------------------------------------------------------------------------------
            }
            else if (id == globalHotKey_F12.Id)
            {
                // F12 - << put description for this key here >>

                // ----------------------------------------------------------------------------------------------------

                // display the key that was pressed
                labelKeysPressed.Text = globalHotKey_F12.Description;

                // ----------------------------------------------------------------------------------------------------

                // << put code for this key here >>

                // ----------------------------------------------------------------------------------------------------
            }

            // --------------------------------------------------------------------------------------------------------

            // start the update timer
            updateTimer.Interval = 750; // 750 ms interval
            updateTimer.Enabled = true;
            updateTimer.Start();

            // --------------------------------------------------------------------------------------------------------
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void registerKeys()
        {
            // IMPORTANT: unused keys should be commented-out so they are not registered

            globalHotKey_F1.Register();
            globalHotKey_F2.Register();
            globalHotKey_F3.Register();
            // globalHotKey_F4.Register();

            // globalHotKey_F5.Register();
            // globalHotKey_F6.Register();
            // globalHotKey_F7.Register();
            // globalHotKey_F8.Register();

            // globalHotKey_F9.Register();
            // globalHotKey_F10.Register();
            // globalHotKey_F11.Register();
            // globalHotKey_F12.Register();
        }

        // ------------------------------------------------------------------------------------------------------------

        private void unRegisterKeys()
        {
            // IMPORTANT: unused keys should be commented-out since they should not be registered

            globalHotKey_F1.Unregiser();
            globalHotKey_F2.Unregiser();
            globalHotKey_F3.Unregiser();
            // globalHotKey_F4.Unregiser();

            // globalHotKey_F5.Unregiser();
            // globalHotKey_F6.Unregiser();
            // globalHotKey_F7.Unregiser();
            // globalHotKey_F8.Unregiser();

            // globalHotKey_F9.Unregiser();
            // globalHotKey_F10.Unregiser();
            // globalHotKey_F11.Unregiser();
            // globalHotKey_F12.Unregiser();
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            unRegisterKeys();
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void showMessageBoxForComException(COMException e)
        {
            MessageBox.Show(Program.vna.GetUserMessageForComException(e),
                Program.programName,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    }
}