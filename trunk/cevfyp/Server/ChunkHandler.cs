using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ClassLibrary;

namespace Server
{
    class ChunkHandler
    {
        BinaryFormatter bf;
        Chunk tempChunk;
      
        public ChunkHandler()
        {
            bf = new BinaryFormatter();
            tempChunk = new Chunk();
        }

        public Chunk streamingToChunk(int tempByte,byte[] tempData,int tempSeq)
        {
            tempChunk.bytes = tempByte;
            tempChunk.streamingData = tempData;
            tempChunk.seq = tempSeq;
            return tempChunk;
        }

        public byte[] chunkToByte(Chunk tempchunk,int tempSize)
        {
            byte[] tempMessage = new byte[tempSize];
            MemoryStream Memstream = new MemoryStream(tempSize);
            bf.Serialize(Memstream, tempchunk);
            tempMessage = Memstream.ToArray();
            Memstream.Close();
            return tempMessage;

        }

    }
}
