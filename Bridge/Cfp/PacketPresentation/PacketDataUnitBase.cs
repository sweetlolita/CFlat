using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Bridge.Cfp.PacketPresentation
{
    public class PacketDataUnitBase
    {
        public string jsonPayload { get; set; }

        public byte[] rawBytes()
        {
            byte[] payloadBytes = System.Text.Encoding.Default.GetBytes(jsonPayload);

            byte[] raw = new byte[ 
                PacketDataUnitLineFlag.lineStartsPatternLength +
                payloadBytes.Length +
                PacketDataUnitLineFlag.lineEndsPatternLength ];
            System.Buffer.BlockCopy( 
                PacketDataUnitLineFlag.lineStartsPattern, 
                0, 
                raw, 
                0, 
                PacketDataUnitLineFlag.lineStartsPatternLength );
            
            System.Buffer.BlockCopy( 
                payloadBytes, 
                0, 
                raw, 
                PacketDataUnitLineFlag.lineStartsPatternLength, 
                payloadBytes.Length );
            System.Buffer.BlockCopy( 
                PacketDataUnitLineFlag.lineEndsPattern, 
                0, 
                raw, 
                PacketDataUnitLineFlag.lineStartsPatternLength + payloadBytes.Length, 
                PacketDataUnitLineFlag.lineEndsPatternLength );


            return raw;
        }
    }
}
