using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    [Serializable]

    public class Chunk 
    {
        private int myseq;
        private int mybytes;
        private byte[] mystreamingData;

        public int seq
        {
            get
            {
                return myseq;
            }
            set
            {
                myseq = value;
            }
        }

        public int bytes
        {
            get
            {
                return mybytes;
            }
            set
            {
                mybytes = value;
            }
        }

        public byte[] streamingData
        {
            get
            {
                return mystreamingData;
            }
            set
            {
                mystreamingData = value;
            }
        }

        public static Chunk Copy(Chunk ck)
        {
            return (Chunk)ck.MemberwiseClone();
        }

 

    } 

}
