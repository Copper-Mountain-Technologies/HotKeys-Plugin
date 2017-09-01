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
    public class VnaTr : Vna
    {
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private TR1300Lib.TRVNA _app;
        public override dynamic app { get { return _app; } set { _app = value; } }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public VnaTr()
        {
            // set family property
            family = VnaFamilyEnum.TR;

            // get type
            type = Type.GetTypeFromProgID("TRVNA.Application", "localhost");
            if (type != null)
            {
                // create com interface object
                app = (TR1300Lib.TRVNA)Activator.CreateInstance(type);

                // get vna info
                if (app != null)
                {
                    PopulateInfo(app.NAME);
                }
            }

            maxChannelSplitIndex = 10;
            maxNumberOfChannels = 9;
            maxNumberOfTraces = 8;
            maxNumberOfMarkers = 15;
            maxNumberOfPorts = 2;

            maxPower = 5.00;
            minPower = -55.00;
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    }
}