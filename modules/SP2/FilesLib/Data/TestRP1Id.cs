using System.Collections;
using FilesLib.Interfaces;

namespace FilesLib.Data
{
    public class TestRP1Id : IHashable<TestRP1Id>
    {
        public int Id { get; set; }
        public int Address { get; set; }

        public TestRP1Id CreateClass()
        {
            return new TestRP1Id();
        }

        public bool Equals(TestRP1Id data)
        {
            return Id == data.Id;
        }

        public TestRP1Id FromByteArray(byte[] byteArray)
        {
            throw new NotImplementedException();
        }

        public BitArray GetHash()
        {
            return new BitArray(BitConverter.GetBytes(Id));
        }

        public int GetSize()
        {
            return sizeof(int) + sizeof(int);
        }

        public byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }
    }
}
