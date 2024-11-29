using System.Collections;
using System.Text;
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
        public override string ToString()
		{
			return $"Address: {Address}, Id: {Id}";
		}

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
			int offset = 0;

			// Address
			Address = BitConverter.ToInt32(byteArray, offset);
			offset += sizeof(int);

			// Id
			Id = BitConverter.ToInt32(byteArray, offset);

			return this;
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
			byte[] bytes = new byte[GetSize()];
			int offset = 0;

			// Address
			BitConverter.GetBytes(Address).CopyTo(bytes, offset);
			offset += sizeof(int);

			// Id
			BitConverter.GetBytes(Id).CopyTo(bytes, offset);

			return bytes;
		}
        #endregion // Public functions
    }
}
