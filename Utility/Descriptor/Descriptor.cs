﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Utility
{
    public class Descriptor
    {
        public byte[] des { get; protected set; }
	    public int desLength { get; set; }
	    public int desCapacity { get; protected set; }

        public void copy(int destIndex, byte[] src, int srcIndex, int copyLength)
        {
            if (destIndex > desLength)
	        {
                throw new CFlatExceptionBase(
                    string.Format("descriptor copy destIndex out of range. {0} is not in [0-{1}]",
                    destIndex, desLength));
	        }
            if (desCapacity - destIndex < copyLength)
            {
                throw new CFlatExceptionBase(
                    string.Format("descriptor copy out of capacity. {0} is required but {1} available",
                    copyLength, desCapacity - destIndex));
            }
            Array.Copy(src, srcIndex, des, destIndex, copyLength);
            if (destIndex + copyLength > desLength)
            {
                desLength = destIndex + copyLength;
            }
        }

        public void append(byte[] src, int srcIndex, int copyLength)
        {
            this.copy(this.desLength, des, srcIndex, copyLength);
        }

        public void clear()
        {
            this.desLength = 0;
        }

        protected Descriptor()
        {

        }

        public string toString(int index, int count)
        {
            return System.Text.Encoding.ASCII.GetString(des, index, count);
        }
	
    }

}
