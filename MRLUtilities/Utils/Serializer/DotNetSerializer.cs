using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MRL.Utils.Serializer
{
    public class DotNetSerializer : ISerializer
    {
        private BinaryFormatter binaryFormatter;

        public DotNetSerializer()
        {
            binaryFormatter = new BinaryFormatter();
        }

        #region ISerializer Members

        public byte[] Serialize(object data)
        {
            MemoryStream mStream = new MemoryStream();
            binaryFormatter.Serialize(mStream, data);
            return mStream.ToArray();
        }

        public object DeSerialize(byte[] data)
        {
            MemoryStream mStream = new MemoryStream(data);
            object retValue = binaryFormatter.Deserialize(mStream);
            return retValue;
        }
        #endregion
    }
}
