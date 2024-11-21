namespace FilesLib
{
	public class HeapFile<T> where T : class, IData<T>, new()
	{
		#region Class members
		private FileStream _file;
		private string _initFilePath;
		private Block<T> _currentBlock = null!;
        private int _currentBlockAddress = 0;
		private int _nextFreeBlockAddress = -1;
		private int _nextEmptyBlockAddress = -1;
        #endregion // Class members

        #region Properties
        public int BlockSize { get; set; }
		public int BlockCount => (int)(_file.Length / BlockSize);
		public int NextFreeBlockAddress => _nextFreeBlockAddress;
		public int NextEmptyBlockAddress => _nextEmptyBlockAddress;
		#endregion // Properties

		#region Constructors
		public HeapFile(string initFilePath, string filePath, int blockSize)
		{
			BlockSize = blockSize;

			if (!File.Exists(initFilePath))
			{
				File.Create(initFilePath).Close();
			}

			if (!File.Exists(filePath))
			{
				File.Create(filePath).Close();
			}
			
			_initFilePath = initFilePath;
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
				SaveInitData();
            }
			else
			{
				LoadInitData();
			}
        }
        #endregion // Constructors

        #region Public functions
        public int Insert(T data)
		{
            if (_nextFreeBlockAddress != -1)
            {
                SetCurrentBlock(_nextFreeBlockAddress);
                _currentBlock.AddRecord(data);

                if (_currentBlock.ValidCount == _currentBlock.BlockFactor)
                {
	                if (_currentBlock.Next != -1)
	                {
		                var nextBlock = GetBlock(_currentBlock.Next);
		                nextBlock.Previous = -1;
		                WriteBlock(nextBlock, _currentBlock.Next);
	                }
	                
	                _nextFreeBlockAddress = _currentBlock.Next;
	                _currentBlock.Next = -1;
	                //_currentBlock.Previous = -1;
                }
            }
            else if (_nextEmptyBlockAddress != -1)
            {
                SetCurrentBlock(_nextEmptyBlockAddress);
                _currentBlock.AddRecord(data);

                if (_currentBlock.ValidCount == _currentBlock.BlockFactor)
                {
	                if (_currentBlock.Next != -1)
	                {
		                var nextBlock = GetBlock(_currentBlock.Next);
		                nextBlock.Previous = -1;
		                WriteBlock(nextBlock, _currentBlock.Next);
	                }
	                
	                _nextEmptyBlockAddress = _currentBlock.Next;
	                _currentBlock.Next = -1;
	                //_currentBlock.Previous = -1;
                }
                else
                {
	                _nextEmptyBlockAddress = _currentBlock.Next;
	                if (_currentBlock.Next != -1)
	                {
		                var nextBlock = GetBlock(_currentBlock.Next);
		                nextBlock.Previous = -1;
		                WriteBlock(nextBlock, _currentBlock.Next);
	                }
	                
	                _currentBlock.Next = _nextFreeBlockAddress;
	                if (_nextFreeBlockAddress != -1)
	                {
		                var nextBlock = GetBlock(_nextFreeBlockAddress);
		                nextBlock.Previous = _currentBlockAddress;
		                WriteBlock(nextBlock, _nextFreeBlockAddress);
	                }
	                _nextFreeBlockAddress = _currentBlockAddress;
                }
            }
            else
            {
                SetCurrentBlock((int)_file.Length);
                _currentBlock.AddRecord(data);

                if (_currentBlock.ValidCount < _currentBlock.BlockFactor) // zaradujem do zretazenia ciastocne plnych blokov
                {
	                if (_nextFreeBlockAddress != -1)
	                {
		                var nextBlock = GetBlock(_nextFreeBlockAddress);
		                nextBlock.Previous = _currentBlockAddress;
		                WriteBlock(nextBlock, _nextFreeBlockAddress);
		                _currentBlock.Next = _nextFreeBlockAddress;
	                }
	                _nextFreeBlockAddress = _currentBlockAddress;
                }
            }
            
            WriteBlock(_currentBlock, _currentBlockAddress);
            return _currentBlockAddress;
        }

        public T Find(int address, T data)
        {
	        if (address % BlockSize != 0 || address < 0 || address > _file.Length) throw new ArgumentException("Invalid address");
	        
            SetCurrentBlock(address);
            return _currentBlock.GetRecord(data);
        }

        public bool Delete(int address, T data)
		{
			if (address % BlockSize != 0 || address < 0 || address > _file.Length) throw new ArgumentException("Invalid address");

			var blockToDeleteFrom = GetBlock(address);
			if (!blockToDeleteFrom.RemoveRecord(data)) return false;
			
			if (blockToDeleteFrom.Next == -1 && blockToDeleteFrom.Previous == -1)
			{
				if (blockToDeleteFrom.ValidCount > 0)
				{
					if (_nextFreeBlockAddress != -1)
					{
						var nextFreeBlock = GetBlock(_nextFreeBlockAddress);
						nextFreeBlock.Next = address;
						blockToDeleteFrom.Previous = _nextFreeBlockAddress;
						WriteBlock(nextFreeBlock, _nextFreeBlockAddress);
					}
					_nextFreeBlockAddress = address;
				}
				else
				{
					if (_nextEmptyBlockAddress != -1)
					{
						var nextEmptyBlock = GetBlock(_nextEmptyBlockAddress);
						nextEmptyBlock.Next = address;
						blockToDeleteFrom.Previous = _nextEmptyBlockAddress;
						WriteBlock(nextEmptyBlock, _nextEmptyBlockAddress);
					}
					_nextEmptyBlockAddress = address;
					if (_nextFreeBlockAddress != address)
					{
						_nextFreeBlockAddress = -1;
					}
				}	
			}
			else
			{
				if (blockToDeleteFrom.ValidCount == 0)
				{
					if (blockToDeleteFrom.Next != -1)
					{
						var nextBlock = GetBlock(blockToDeleteFrom.Next);
						nextBlock.Previous = blockToDeleteFrom.Previous;
						WriteBlock(nextBlock, blockToDeleteFrom.Next);
					}

					if (blockToDeleteFrom.Previous != -1)
					{
						var prevBlock = GetBlock(blockToDeleteFrom.Previous);
						prevBlock.Next = blockToDeleteFrom.Next;
						WriteBlock(prevBlock, blockToDeleteFrom.Previous);
					}
					
					blockToDeleteFrom.Next = -1;
					blockToDeleteFrom.Previous = -1;
					
					if (_nextEmptyBlockAddress != -1)
					{
						var nextEmptyBlock = GetBlock(_nextEmptyBlockAddress);
						nextEmptyBlock.Next = address;
						blockToDeleteFrom.Previous = _nextEmptyBlockAddress;
						WriteBlock(nextEmptyBlock, _nextEmptyBlockAddress);
					}
					_nextEmptyBlockAddress = address;
				}
			}
			
			WriteBlock(blockToDeleteFrom, address);
			CheckFileEnding();
			return true;
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

        public List<Block<T>> GetBlocks()
        {
	        var ret = new List<Block<T>>();
	        for (int i = 0; i < BlockCount; i++)
	        {
		        Block<T> block = new Block<T>(BlockSize, new T());
		        int offset = i * BlockSize;
		        _file.Seek(offset, SeekOrigin.Begin);
		        byte[] bytes = new byte[BlockSize];
		        _file.Read(bytes, 0, BlockSize);
		        block.FromByteArray(bytes);
		        
		        ret.Add(block);
	        }

	        return ret;
        }

        public void Close()
        {
	        SaveInitData();
	        
	        _currentBlockAddress = 0;
	        _nextFreeBlockAddress = -1;
	        _nextEmptyBlockAddress = -1;
	        _file.Flush();
	        _file.Close();
        }
        #endregion // Public functions

        #region Private functions
        private void SaveInitData()
        {
	        var initFile = new FileStream(_initFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
	        
	        byte[] buffer = new byte[BlockSize];
	        int offset = 0;
	        BitConverter.GetBytes(_nextFreeBlockAddress).CopyTo(buffer, offset);
	        offset += sizeof(int);
	        BitConverter.GetBytes(_nextEmptyBlockAddress).CopyTo(buffer, offset);

	        initFile.Seek(0, SeekOrigin.Begin);
	        initFile.Write(buffer, 0, BlockSize);
	        initFile.Flush();
	        initFile.Close();
        }

        private void LoadInitData()
        {
	        var initFile = new FileStream(_initFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
	        
	        byte[] buffer = new byte[BlockSize];
	        int offset = 0;
	        initFile.Seek(offset, SeekOrigin.Begin);
	        initFile.Read(buffer, offset, BlockSize);
            
	        _nextFreeBlockAddress = BitConverter.ToInt32(buffer, offset);
	        offset += sizeof(int);
	        _nextEmptyBlockAddress = BitConverter.ToInt32(buffer, offset);
        }
        
        private void SetCurrentBlock(int address)
        {
	        if (address % BlockSize != 0 || address < 0 || address > _file.Length) throw new ArgumentException("Invalid address");
	        
	        if (_currentBlock == null! || _currentBlockAddress != address)
	        {
		        _currentBlock = new Block<T>(BlockSize, new T());
		        byte[] bytes = new byte[BlockSize];
		        _file.Seek(address, SeekOrigin.Begin);
		        _file.Read(bytes, 0, BlockSize);
		        _currentBlock.FromByteArray(bytes);
		        _currentBlockAddress = address;
		        
		        if (address == _file.Length)
		        {
			        _currentBlock.Next = -1;
			        _currentBlock.Previous = -1;
		        }
	        }
        }
        
        private void WriteBlock(Block<T> currentBlock, int currentBlockAddress)
        {
	        byte[] bytes = currentBlock.ToByteArray();
	        _file.Seek(currentBlockAddress, SeekOrigin.Begin);
	        _file.Write(bytes, 0, BlockSize);
	        _file.Flush();
        }

        private Block<T> GetBlock(int address)
        {
	        _file.Seek(address, SeekOrigin.Begin);
	        byte[] bytes = new byte[BlockSize];
	        _file.Read(bytes, 0, BlockSize);
	        var block = new Block<T>(BlockSize, new T());
	        block.FromByteArray(bytes);
	        return block;
        }
        
        private void CheckFileEnding()
        {
	        while (_file.Length > 0)
	        {
		        int lastBlockAddress = (int)_file.Length - BlockSize;
		        var lastBlock = GetBlock(lastBlockAddress);

		        if (lastBlock.ValidCount > 0) break;
		        
		        if (lastBlock.Next != -1 /*&& lastBlock.Next < _file.Length && lastBlock.Previous < _file.Length*/)
		        {
			        var nextBlock = GetBlock(lastBlock.Next);
			        nextBlock.Previous = lastBlock.Previous;
			        WriteBlock(nextBlock, lastBlock.Next);
		        }

		        if (lastBlock.Previous != -1 /*&& lastBlock.Previous < _file.Length && lastBlock.Next < _file.Length*/)
		        {
			        var prevBlock = GetBlock(lastBlock.Previous);
			        prevBlock.Next = lastBlock.Next;
			        WriteBlock(prevBlock, lastBlock.Previous);
		        }

		        if (_nextFreeBlockAddress == lastBlockAddress) _nextFreeBlockAddress = lastBlock.Next;
		        if (_nextEmptyBlockAddress == lastBlockAddress) _nextEmptyBlockAddress = lastBlock.Next;
		        
		        _file.SetLength(_file.Length - BlockSize);
	        }
        }
        #endregion // Private functions
    }
}
