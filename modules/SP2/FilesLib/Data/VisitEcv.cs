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
        private const int ECV_MAX_SIZE = 10;
        #endregion // Constants
        
        #region Properties
        public string Ecv { get; set; } = string.Empty;
        public int Address { get; set; } = -1;
        #endregion // Properties

        #region Public functions
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
            throw new NotImplementedException(); // TODO
        }

        public BitArray GetHash()
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(Ecv);
            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(stringBytes);
            return new BitArray(hashBytes);
        }

        public int GetSize()
        {
            return ECV_MAX_SIZE + sizeof(int) + sizeof(int);
        }

        public byte[] ToByteArray()
        {
            throw new NotImplementedException(); // TODO
        }
        #endregion // Public functions
    }
}
