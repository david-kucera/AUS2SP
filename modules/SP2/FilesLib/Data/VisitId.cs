using System.Collections;
using FilesLib.Interfaces;

namespace FilesLib.Data
{
    public class VisitId : IHashable<VisitId>
    {
        public int Id { get; set; }
        public int Address { get; set; }

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
