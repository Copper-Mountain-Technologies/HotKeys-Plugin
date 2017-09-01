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

using System;

namespace CopperMountainTech
{
    public class VnaS4 : Vna
    {
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private S4VNALib.S4VNA _app;
        public override dynamic app { get { return _app; } set { _app = value; } }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public VnaS4()
        {
            // set family property
            family = VnaFamilyEnum.S4;

            // get type
            type = Type.GetTypeFromProgID("S4VNA.Application", "localhost");
            if (type != null)
            {
                // create com interface object
                app = (S4VNALib.S4VNA)Activator.CreateInstance(type);

                // get vna info
                if (app != null)
                {
                    PopulateInfo(app.NAME);
                }
            }

            maxChannelSplitIndex = 16;
            maxNumberOfChannels = 16;
            maxNumberOfTraces = 16;
            maxNumberOfMarkers = 15;
            maxNumberOfPorts = 2;

            maxPower = 10.00;
            minPower = -60.00;
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    }
}