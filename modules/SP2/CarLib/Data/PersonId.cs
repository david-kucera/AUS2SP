using System.Collections;
using FilesLib.Interfaces;

namespace CarLib.Data
{
    /// <summary>
    /// Class used for storing in ExtendibleHashFile.
    /// </summary>
    public class PersonId : IHashable<PersonId>
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

		public PersonId CreateClass()
        {
            return new PersonId();
        }

        public bool Equals(PersonId data)
        {
            return Id == data.Id;
        }

        public PersonId FromByteArray(byte[] byteArray)
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
