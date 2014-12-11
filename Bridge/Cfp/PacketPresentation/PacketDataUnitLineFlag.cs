using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Bridge.Cfp.PacketPresentation
{
    class PacketDataUnitLineFlag
    {
        public int start { get; set; }
        public int end { get; set; }

        public const int lineStartsPatternLength = 1;
        public const int lineEndsPatternLength = 1;

        public static readonly byte[] lineStartsPattern = { (byte)0x00 };
        public static readonly byte[] lineEndsPattern = { (byte)0xff };
    }
}
