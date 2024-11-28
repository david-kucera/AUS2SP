using System.Collections;
using FilesLib.Interfaces;

namespace FilesLib.Data
{
    /// <summary>
    /// Class for testing purposes...
    /// </summary>
    public class TestRP1Id : IHashable<TestRP1Id>
    {
        #region Properties
        public int Id { get; set; } = -1;
        public int Address { get; set; } = -1;
        #endregion // Properties

        #region Public functions
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
        #endregion // Public functions
    }
}
