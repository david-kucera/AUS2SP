namespace FilesLib
{
	public class HeapFile<T> where T : class, IData<T>, new()
	{
		#region Class members
		private FileStream _file;
		private Block<T> _currentBlock = null!;
        private int _currentBlockAddress = 0;
		private int _nextFreeBlockAddress = -1;
		private int _nextEmptyBlockAddress = -1;
        #endregion // Class members

        #region Properties
        public int BlockSize { get; set; }
		public int BlockCount => (int)(_file.Length / BlockSize);
        public Type TType { get; set; }
		#endregion // Properties

		#region Constructors
		public HeapFile(string filePath, int blockSize)
		{
			BlockSize = blockSize;
			TType = typeof(T);
			try
			{
				_file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			}
			catch (Exception ex)
			{
				throw new Exception("Error while opening file: " + ex.Message);
            }

            if (_file.Length == 0)
            {
                _nextEmptyBlockAddress = 0;
				SaveInitData();
            }
			else
			{
				LoadInitData();
			}
        }
        #endregion // Constructors

        #region Public functions
        public int Insert(T data) // vraciame adresu, kde sa zaznam nachadza
		{
			Block<T> block = new Block<T>(BlockSize, TType);
			return 0;
			// TODO insert operation
		}

        public T Find(int address, T data)
        {
            SetCurrentBlock(address);
            return _currentBlock.GetRecord(data);
        }

        public bool Delete(int adress)
		{
			// TODO delete operation
			Block<T> block = new Block<T>(BlockSize, TType);
			return false;
		}

        private void SaveInitData()
        {
            byte[] buffer = new byte[BlockSize];
            int offset = 0;

            BitConverter.GetBytes(_nextFreeBlockAddress).CopyTo(buffer, offset);
            offset += sizeof(int);
            BitConverter.GetBytes(_nextEmptyBlockAddress).CopyTo(buffer, offset);
            offset += sizeof(int);

			_file.Seek(0, SeekOrigin.Begin);
            _file.Write(buffer, 0, buffer.Length);
            _file.Flush();
        }

        private void LoadInitData()
        {
            byte[] buffer = new byte[BlockSize];
            _file.Seek(0, SeekOrigin.Begin);
            _file.Read(buffer, 0, BlockSize);

            int offset = 0;
            _nextFreeBlockAddress = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);
            _nextEmptyBlockAddress = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);
        }

        public void Clear()
		{
			_currentBlock.ResetBlock();
            _currentBlockAddress = 0;
            _nextFreeBlockAddress = -1;
            _nextEmptyBlockAddress = -1;

			SaveInitData();
			_file.SetLength(BlockSize);
            _file.Flush();
        }
        #endregion // Public functions

        #region Private functions
        private void SetCurrentBlock(int address)
        {
            if (_currentBlockAddress == address)
            {
                return;
            }

            byte[] bytes = new byte[BlockSize];
            _file.Seek(address, SeekOrigin.Begin);
            _file.Read(bytes, 0, BlockSize);

            _currentBlock.FromByteArray(bytes);
            _currentBlockAddress = address;
        }
        #endregion // Private functions
    }
}
