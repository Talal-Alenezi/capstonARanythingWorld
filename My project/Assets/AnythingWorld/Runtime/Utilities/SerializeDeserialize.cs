using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AnythingWorld.Utilities
{
    public class SerializeDeserialize
    {

        // Serialize collection of any type to a byte stream

        public static byte[] Serialize<T>(T obj)
        {
            using (var memStream = new MemoryStream())
            {
                var binSerializer = new BinaryFormatter();
                binSerializer.Serialize(memStream, obj);
                return memStream.ToArray();
            }
        }

        // DSerialize collection of any type to a byte stream

        public static T Deserialize<T>(byte[] serializedObj)
        {
            var obj = default(T);
            using (var memStream = new MemoryStream(serializedObj))
            {
                var binSerializer = new BinaryFormatter();
                obj = (T)binSerializer.Deserialize(memStream);
            }
            return obj;
        }

    }
}
