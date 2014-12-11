using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Utility;
using CFlat.Bridge.Cfp.CommonEntity;

namespace CFlat.Bridge.Cfp.PacketPresentation
{

    class PacketDataUnitParser
    {
        private Descriptor descriptor { get; set; }
        private IEnumerable<PacketDataUnitLineFlag> lineFlags { get; set; }
        private int lineFlagIndex { get; set; }
        private int lineFlagCount { get; set; }
        public PacketDataUnitParser(Descriptor descriptor)
        {
            
            this.descriptor = descriptor;
            IEnumerable<int> lineStarts = BytesHelper.indexOf(descriptor.des, 0, descriptor.desLength, PacketDataUnitLineFlag.lineStartsPattern);
            IEnumerable<int> lineEnds = BytesHelper.indexOf(descriptor.des, 0, descriptor.desLength, PacketDataUnitLineFlag.lineEndsPattern);
            int lineStartsCount = lineStarts.Count<int>();
            int lineEndsCount = lineEnds.Count<int>();
            if(lineStartsCount != lineEndsCount)
            {
                throw new CfpException("PacketDataUnitParser:lineStartsCount mismatch lineEndsCount");
            }

            lineFlags = lineStarts.Zip(lineEnds, (s, e) => new PacketDataUnitLineFlag { start = s, end = e });


            foreach (PacketDataUnitLineFlag lineFlag in lineFlags)
            {
                if(lineFlag.start >= lineFlag.end)
                {
                    throw new CfpException("PacketDataUnitParser:lineStarts isnot smaller than lineEnds");
                }
            }

            lineFlagIndex = 0;
            lineFlagCount = lineEndsCount;
        }

        public bool parseIfHasNext(out PacketDataUnitBase packetDataUnitBase)
        {
            packetDataUnitBase = null;
            if (lineFlagIndex == lineFlagCount)
            {
                return false;
            }
            
            packetDataUnitBase = new PacketDataUnitBase();

            int startIndex = lineFlags.ElementAt(lineFlagIndex).start + 1;
            int endIndex = lineFlags.ElementAt(lineFlagIndex).end;
            int count = endIndex - startIndex;

            packetDataUnitBase.jsonPayload = System.Text.Encoding.Default.GetString(
                descriptor.des,
                startIndex,
                count
            );

            lineFlagIndex++;

            return true;
        }


    }
}
