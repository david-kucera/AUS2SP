using System.Collections;
using FilesLib.Interfaces;

namespace FilesLib.Data
{
    /// <summary>
    /// Class used for storing in ExtendibleHashFile.
    /// </summary>
    public class VisitId : IHashable<VisitId>
    {
        #region Properties
        public int Id { get; set; } = -1;
        public int Address { get; set; } = -1;
        #endregion // Properties

        #region Public functions
        public VisitId CreateClass()
        {
            return new VisitId();
        }

        public bool Equals(VisitId data)
        {
            return Id == data.Id;
        }

        public VisitId FromByteArray(byte[] byteArray)
        {
            throw new NotImplementedException(); // TODO
        }

        public BitArray GetHash()
        {
            return new BitArray(BitConverter.GetBytes(Id));
        }

        public int GetSize()
        {
            return sizeof(int) * 2;
        }

        public byte[] ToByteArray()
        {
            throw new NotImplementedException(); // TODO
        }
        #endregion // Public functions
        
    }
}
