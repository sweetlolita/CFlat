using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Utility;
using CFlat.Bridge.Cfp.CommonEntity;

namespace CFlat.Bridge.Cfp.Presentation
{
    class PacketDataUnitParser
    {
        private Descriptor descriptor { get; set; }
        private IEnumerable<int> lineEnds { get; set; }
        private int lineEndsIndex { get; set; }
        private int lineEndsCount { get; set; }

        private static int lineEndsInEachMessage = 3;

        private static int lineEndsPatternLength = 2;
        public PacketDataUnitParser(Descriptor descriptor)
        {
            lineEndsIndex = 0;
            this.descriptor = descriptor;
            byte[] pattern = new byte[] { (byte)'\r', (byte)'\n' };
            lineEnds = BytesHelper.indexOf(descriptor.des, 0, descriptor.desLength, pattern);
            lineEndsCount = lineEnds.Count<int>();

        }

        public bool parseIfHasNext(out PacketDataUnitBase packetDataUnitBase)
        {
            packetDataUnitBase = null;
            if (lineEndsIndex > lineEndsCount)
            {
                throw new CfpException("corrupted message.");
            }
            if(lineEndsIndex == lineEndsCount)
            {
                return false;
            }
            
            int payloadOffset = lineEndsIndex == 0 ?
                0 :
                lineEnds.ElementAt(lineEndsIndex - 1) + lineEndsPatternLength;
            packetDataUnitBase = new PacketDataUnitBase();
            packetDataUnitBase.verb = System.Text.Encoding.Default.GetString(
                descriptor.des, payloadOffset, lineEnds.ElementAt(lineEndsIndex) - payloadOffset);
            payloadOffset = lineEnds.ElementAt(lineEndsIndex) + lineEndsPatternLength;
            packetDataUnitBase.jsonPayload = System.Text.Encoding.Default.GetString(
                descriptor.des, payloadOffset, lineEnds.ElementAt(lineEndsIndex + 1) - payloadOffset);
            lineEndsIndex += lineEndsInEachMessage;
            return true;
        }


    }
}
