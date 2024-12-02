using System.Collections;
using System.Security.Cryptography;
using System.Text;
using FilesLib.Interfaces;

namespace FilesLib.Data
{
    /// <summary>
    /// Class used for storing in ExtendibleHashFile.
    /// </summary>
    public class VisitEcv : IHashable<VisitEcv>
    {
        #region Constants
        private const int MAX_ECV_LENGTH = 10;
        #endregion // Constants
        
        #region Properties
        public string Ecv { get; set; } = string.Empty;
        public int Address { get; set; } = -1;
        #endregion // Properties

        #region Public functions
        public override string ToString()
        {
	        return $"Address: {Address}, Ecv: {Ecv}";
        }

        public VisitEcv CreateClass()
        {
            return new VisitEcv();
        }

        public bool Equals(VisitEcv data)
        {
            return Ecv == data.Ecv;
        }

        public VisitEcv FromByteArray(byte[] byteArray)
        {
			int offset = 0;

			// Address
			Address = BitConverter.ToInt32(byteArray, offset);
			offset += sizeof(int);

			// ECV length
			int ecvLength = BitConverter.ToInt32(byteArray, offset);
			offset += sizeof(int);

			// ECV
			Ecv = Encoding.ASCII.GetString(byteArray, offset, ecvLength);

			return this;
		}

        public BitArray GetHash()
        {
            byte[] stringBytes = Encoding.ASCII.GetBytes(Ecv);
            byte[] lastFourBytes = new byte[4];
            int bytesToCopy = stringBytes.Length >= 4 ? 4 : stringBytes.Length;
            for (int i = 0; i < bytesToCopy; i++)
			{
				lastFourBytes[i] = stringBytes[stringBytes.Length - bytesToCopy + i];
			}
			return new BitArray(lastFourBytes);
        }

        public int GetSize()
        {
            return MAX_ECV_LENGTH + sizeof(int) + sizeof(int);
        }

        public byte[] ToByteArray()
        {
			byte[] bytes = new byte[GetSize()];
			int offset = 0;

			// Address
			BitConverter.GetBytes(Address).CopyTo(bytes, offset);
			offset += sizeof(int);

			// ECV length
			var ecvLength = Ecv.Length;
			BitConverter.GetBytes(ecvLength).CopyTo(bytes, offset);
			offset += sizeof(int);

			// ECV
			var ecvToFile = string.Empty;
			if (Ecv.Length <= MAX_ECV_LENGTH)
			{
				ecvToFile = Ecv.PadRight(MAX_ECV_LENGTH, ' ');
			}
			Encoding.ASCII.GetBytes(ecvToFile).CopyTo(bytes, offset);

			return bytes;
		}
        #endregion // Public functions
    }
}
