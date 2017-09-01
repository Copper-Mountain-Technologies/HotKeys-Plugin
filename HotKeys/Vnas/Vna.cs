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
using System.Runtime.InteropServices;

namespace CopperMountainTech
{
    // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    [Flags]
    public enum VnaFamilyEnum
    {
        UNKNOWN = 0,
        R = 1,
        TR = 2,
        S2 = 4,
        S4 = 8
    }

    public enum VnaModelEnum
    {
        UNKNOWN,
        R54, R140, RP60, RP5, RP180,
        TR1300_1, TR5048, TR7530,
        C1209, C2209, C1220, C2220, S5048, S5065, S5070, S5085, S5180, S7530, PLANAR_304_1, PLANAR_804_1, C4209, C4220,
        PLANAR_808_1, C1409, C2409, C1420, C4409
    }

    public enum VnaInfoStringIndexEnum : int
    {
        Manufacturer, Model, SerialNumber, Version
    }

    // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    public class Vna
    {
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public Type type { get; set; }

        public virtual dynamic app { get; set; }

        public VnaFamilyEnum family { get; set; }
        public VnaModelEnum model { get; set; }
        public string modelString { get; set; }
        public string manufacturerString { get; set; }
        public string serialNumberString { get; set; }
        public string versionString { get; set; }

        public int maxChannelSplitIndex { get; set; }
        public int maxNumberOfChannels { get; set; }
        public int maxNumberOfTraces { get; set; }
        public int maxNumberOfMarkers { get; set; }
        public int maxNumberOfPorts { get; set; }

        public double maxPower { get; set; }
        public double minPower { get; set; }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public Vna()
        {
            type = null;

            family = VnaFamilyEnum.UNKNOWN;
            model = VnaModelEnum.UNKNOWN;
            modelString = "";
            manufacturerString = "";
            serialNumberString = "";
            versionString = "";
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public void PopulateInfo(string s)
        {
            string[] infoArray = s.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // manufacturer
            manufacturerString = TrimStartAndEnd(infoArray[(int)VnaInfoStringIndexEnum.Manufacturer]);

            // model
            modelString = TrimStartAndEnd(infoArray[(int)VnaInfoStringIndexEnum.Model]);
            model = GetModelFromString(modelString);

            // serialNumber
            serialNumberString = TrimStartAndEnd(infoArray[(int)VnaInfoStringIndexEnum.SerialNumber]);

            // version
            versionString = TrimStartAndEnd(infoArray[(int)VnaInfoStringIndexEnum.Version]);
            versionString = FormatVersion(versionString);
        }

        private string TrimStartAndEnd(string s)
        {
            // trim spaces off start and end of string
            s = s.TrimStart();
            s = s.TrimEnd();
            return s;
        }

        private string FormatVersion(string s)
        {
            // trim-off hardware version
            int endIndex = s.IndexOf("/");
            if (endIndex > 0)
            {
                s = s.Substring(0, endIndex);
            }
            s = "v" + s;
            return s;
        }

        private VnaModelEnum GetModelFromString(string s)
        {
            switch (s)
            {
                default:
                    return VnaModelEnum.UNKNOWN;

                // R family types
                case "R54":
                    return VnaModelEnum.R54;

                case "R140":
                    return VnaModelEnum.R140;

                case "RP60":
                    return VnaModelEnum.RP60;

                case "RP5":
                    return VnaModelEnum.RP5;

                case "RP180":
                    return VnaModelEnum.RP180;

                // TR family types
                case "PLANAR TR1300/1":
                    return VnaModelEnum.TR1300_1;

                case "TR5048":
                    return VnaModelEnum.TR5048;

                case "TR7530":
                    return VnaModelEnum.TR7530;

                // S2 family types
                case "C1209":
                    return VnaModelEnum.C1209;

                case "C2209":
                    return VnaModelEnum.C2209;

                case "C1220":
                    return VnaModelEnum.C1220;

                case "C2220":
                    return VnaModelEnum.C2220;

                case "S5048":
                    return VnaModelEnum.S5048;

                case "S5065":
                    return VnaModelEnum.S5065;

                case "S5070":
                    return VnaModelEnum.S5070;

                case "S5085":
                    return VnaModelEnum.S5085;

                case "S5180":
                    return VnaModelEnum.S5180;

                case "S7530":
                    return VnaModelEnum.S7530;

                case "PLANAR-304/1":
                    return VnaModelEnum.PLANAR_304_1;

                case "PLANAR-804/1": // includes 814/1
                    return VnaModelEnum.PLANAR_804_1;

                case "C4209":
                    return VnaModelEnum.C4209;

                case "C4220":
                    return VnaModelEnum.C4220;

                // S4 family types
                case "PLANAR-808/1":
                    return VnaModelEnum.PLANAR_808_1;

                case "C1409":
                    return VnaModelEnum.C1409;

                case "C2409":
                    return VnaModelEnum.C2409;

                case "C1420":
                    return VnaModelEnum.C1420;

                case "C4409":
                    return VnaModelEnum.C4409;
            }
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public int DetermineNumberOfChannels(long splitIndex)
        {
            int numOfChannels = 1;

            // parse the vna family type
            switch (family)
            {
                case VnaFamilyEnum.S4:
                case VnaFamilyEnum.S2:
                    {
                        int[] splitArray = new int[] { 0, 1, 2, 2, 3, 3, 3, 4, 4, 6, 6, 8, 8, 9, 12, 12, 16 };
                        if (splitIndex <= splitArray.Length)
                        {
                            numOfChannels = splitArray[splitIndex];
                        }
                    }
                    break;

                case VnaFamilyEnum.TR:
                    {
                        int[] splitArray = new int[] { 0, 1, 2, 2, 3, 3, 4, 4, 6, 8, 9 };
                        if (splitIndex <= splitArray.Length)
                        {
                            numOfChannels = splitArray[splitIndex];
                        }
                    }
                    break;

                case VnaFamilyEnum.R:
                    {
                        int[] splitArray = new int[] { 0, 1, 2, 2, 3, 4, 4 };
                        if (splitIndex <= splitArray.Length)
                        {
                            numOfChannels = splitArray[splitIndex];
                        }
                    }
                    break;

                default:
                    break;
            }

            return numOfChannels;
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public string GetUserMessageForComException(COMException e)
        {
            string message = "Unknown error.";

            if (e != null)
            {
                uint errorCode = ((uint)e.ErrorCode & 0xFFFF) - 0x200;
                message = GetUserMessageForErrorCode(errorCode);
            }

            return message;
        }

        private string GetUserMessageForErrorCode(uint errorCode)
        {
            switch (errorCode)
            {
                case 114:
                    return "Header suffix out of range.";

                case 200:
                    return "Execution error.";

                case 211:
                    return "Trigger ignored.";

                case 213:
                    return "Init ignored.";

                case 220:
                    return "Parameter Error.";

                case 222:
                    return "Data out of range.";

                case 224:
                    return "Illegal parameter value.";

                case 201:
                    return "Invalid channel index.";

                case 202:
                    return "Invalid trace index.";

                case 203:
                    return "Invalid marker index.";

                case 204:
                    return "Marker is not active.";

                case 205:
                    return "Invalid save type specifier.";

                case 206:
                    return "Invalid sweep type specifier.";

                case 207:
                    return "Invalid trigger source specifier.";

                case 208:
                    return "Invalid measurement parameter specifier.";

                case 209:
                    return "Invalid format specifier.";

                case 210:
                    return "Invalid data math specifier.";

                case 214:
                    return "Invalid limit data.";

                case 215:
                    return "Invalid segment data.";

                case 216:
                    return "Invalid standard type specifier.";

                case 217:
                    return "Invalid conversion specifier.";

                case 218:
                    return "Invalid gating shape specifier.";

                case 219:
                    return "Invalid gating type specifier.";

                case 300:
                    return "Device-specific error.";

                case 302:
                    return "Status reporting system error.";

                default:
                    return "Unknown Error Code: " + errorCode.ToString();
            }
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    }
}