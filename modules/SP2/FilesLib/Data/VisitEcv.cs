using System.Collections;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
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
            byte[] stringBytes = Encoding.UTF8.GetBytes(Ecv);
            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(stringBytes);
            byte[] firstFourBytes = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				firstFourBytes[i] = hashBytes[i];
			}
			return new BitArray(firstFourBytes);
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
			if (Ecv.Length < MAX_ECV_LENGTH)
			{
				ecvToFile = Ecv.PadRight(MAX_ECV_LENGTH, ' ');
			}
			Encoding.ASCII.GetBytes(ecvToFile).CopyTo(bytes, offset);

			return bytes;
		}
        #endregion // Public functions
    }
}
