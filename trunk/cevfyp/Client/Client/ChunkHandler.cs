using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ClassLibrary;

namespace Client
{
    class ChunkHandler
    {
        object tempChunk;

        //Chunk tempChunk;

        public ChunkHandler()
        {

        }

        public object byteToChunk(BinaryFormatter bf,byte[] tempByte)
        {
            try
            {
                tempChunk = bf.Deserialize(new MemoryStream(tempByte));
                return tempChunk;
            }
            catch
            {
                return null;
            }
        }
        public byte[] chunkToByte(Chunk tempchunk, int tempSize)
        {
            BinaryFormatter bf = new BinaryFormatter();

            byte[] tempMessage = new byte[tempSize];
            MemoryStream Memstream = new MemoryStream(tempSize);
            bf.Serialize(Memstream, tempchunk);
            tempMessage = Memstream.ToArray();
            Memstream.Close();
            return tempMessage;

        }

    }
}
