using System.Runtime.Serialization.Formatters.Binary;

namespace FilesLib.Helpers
{
        public static class Serializator
        {
            public static byte[] Serialize<T>(T obj)
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    #pragma warning disable SYSLIB0011 // Type or member is obsolete
                    BinaryFormatter binSerializer = new();
                    #pragma warning restore SYSLIB0011 // Type or member is obsolete
                    binSerializer.Serialize(memStream, obj!);
                    return memStream.ToArray();
                }
            }

            public static T Deserialize<T>(byte[] serializedObj)
            {
                T obj = default(T)!;

                using (MemoryStream memStream = new MemoryStream(serializedObj))
                {
                    #pragma warning disable SYSLIB0011 // Type or member is obsolete
                    BinaryFormatter binSerializer = new BinaryFormatter();
                    #pragma warning restore SYSLIB0011 // Type or member is obsolete
                    obj = (T)binSerializer.Deserialize(memStream);
                }
                return obj;
            }

            public static object Deserialize(byte[] serializedObj)
            {
                object retval = null!;

                using (MemoryStream memStream = new MemoryStream(serializedObj))
                {
                    #pragma warning disable SYSLIB0011 // Type or member is obsolete
                    BinaryFormatter binSerializer = new BinaryFormatter();
                    #pragma warning restore SYSLIB0011 // Type or member is obsolete
                    retval = binSerializer.Deserialize(memStream);
                }
                return retval;
            }
        }
}
