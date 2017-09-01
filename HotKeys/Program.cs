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
using System.Windows.Forms;

namespace HotKeys
{
    internal static class Program
    {
        public static string programName = "HotKeys";
        public static Vna vna = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            // check for an existing instance
            using (SingleGlobalInstanceChecker checker = new SingleGlobalInstanceChecker())
            {
                if (checker.isSingleInstance == true)
                {
                    // args = new string[] { "S4" };  // for testing S4
                    // args = new string[] { "S2" };  // for testing S2
                    // args = new string[] { "TR" };  // for testing TR
                    // args = new string[] { "R" };   // for testing R

                    // check if the vna family argument was passed
                    if (args.Length > 0)
                    {
                        bool supported = false;

                        // yes... parse the vna family type
                        if (string.Compare(args[0], "S4", true) == 0)
                        {
                            vna = new VnaS4();
                            supported = true;
                        }
                        else if (string.Compare(args[0], "S2", true) == 0)
                        {
                            vna = new VnaS2();
                            supported = true;
                        }
                        else if (string.Compare(args[0], "TR", true) == 0)
                        {
                            vna = new VnaTr();
                            supported = true;
                        }
                        else if (string.Compare(args[0], "R", true) == 0)
                        {
                            vna = new VnaR();
                            supported = true;
                        }

                        // check if this plug-in is supported by the connected instrument
                        if (supported == false)
                        {
                            // no... display user message
                            MessageBox.Show("This Plug-in is not supported by the connected instrument.\n\nPlease contact Copper Mountain Technologies support at support@coppermountaintech.com or +1 317-222-5400.",
                                programName,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                        // check if com server is registered
                        else if (vna.type == null)
                        {
                            // no... display error message
                            MessageBox.Show("The VNA software COM server must be registered in order to use Plug-ins.\n\nPlease refer to the instrument's programming manual for details.",
                                programName,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                        else
                        {
                            // yes... continue
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new FormMain());
                        }
                    }
                    else
                    {
                        // no... display error message
                        MessageBox.Show("This Plug-in must be run from the main VNA software application.",
                            programName,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                }
                else
                {
                    MessageBox.Show("The Plug-in Is Already Running",
                        programName,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}