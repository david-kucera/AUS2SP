namespace FilesLib
{
	public class HeapFile<T> where T : class, IData<T>, new()
	{
		#region Class members
		private readonly FileStream _file;
		private readonly string _initFile;
		private int _nextFreeBlockAddress = -1;
		private int _nextEmptyBlockAddress = -1;
        #endregion // Class members

        #region Properties
        /// <summary>
        /// Veľkosť bloku dát.
        /// </summary>
        public int BlockSize { get; set; }
        /// <summary>
        /// Počet blokov v súbore.
        /// </summary>
		public int BlockCount => (int)(_file.Length / BlockSize);
        /// <summary>
        /// Celkový počet záznamov v súbore.
        /// </summary>
		public int RecordsCount = 0;
		#endregion // Properties

		#region Constructor
		/// <summary>
		/// Konštruktor triedy HeapFile.
		/// </summary>
		/// <param name="initFilePath">Cesta k inicializačnému súboru.</param>
		/// <param name="filePath">Cesta k dátovému súboru.</param>
		/// <param name="blockSize">Veľkosť bloku.</param>
		/// <exception cref="Exception">Ak nastane chyba pri otváraí súboru.</exception>
		public HeapFile(string initFilePath, string filePath, int blockSize)
		{
			if (!File.Exists(initFilePath)) File.Create(initFilePath).Close();
			if (!File.Exists(filePath)) File.Create(filePath).Close();
			
			BlockSize = blockSize;
			try
			{
				_file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
				_initFile = initFilePath;
			}
			catch (Exception ex)
			{
				throw new Exception("Error while opening file: " + ex.Message);
            }

            if (_file.Length == 0) SaveInitData();
			else LoadInitData();
        }
        #endregion // Constructor

        #region Public functions
        /// <summary>
        /// O(1,1) - jeden prístup do súboru, jeden zápis
        /// Vloží dáta do súboru.
        /// </summary>
        /// <param name="data">Trieda dát</param>
        /// <returns></returns>
        /// <exception cref="Exception">Ak nastane chyba pri pridávaní dát.</exception>
        public int Insert(T data)
		{
			// Ak mam nejake volne blocky na pridanie dat
            if (_nextFreeBlockAddress != -1 || _nextEmptyBlockAddress != -1)
            {
	            // najskor vyberam ciastocne plne, ak ciastocne plne nie su, tak potom celkom prazdne
	            int address;
	            if (_nextFreeBlockAddress != -1) address = _nextFreeBlockAddress;
	            else address = _nextEmptyBlockAddress;

	            var blockToAdd = GetBlock(address);
	            blockToAdd.AddRecord(data);

	            if (blockToAdd.ValidCount < blockToAdd.BlockFactor) // ak sa do neho este zmesti
	            {
		            if (_nextEmptyBlockAddress == address)
		            {
			            _nextEmptyBlockAddress = blockToAdd.Next;
			            if (blockToAdd.Next != -1)
			            {
				            var nextBlock = GetBlock(blockToAdd.Next);
				            nextBlock.Previous = -1;
				            WriteBlock(nextBlock, blockToAdd.Next);
			            }
			            blockToAdd.Next = _nextFreeBlockAddress;
			            if (_nextFreeBlockAddress != -1)
			            {
				            var nextFreeBlock = GetBlock(_nextFreeBlockAddress);
				            nextFreeBlock.Previous = address;
				            WriteBlock(nextFreeBlock, _nextFreeBlockAddress);
			            }
			            _nextFreeBlockAddress = address;
		            }
	            }
	            else if (blockToAdd.ValidCount == blockToAdd.BlockFactor) // ak je blok plny
	            {
		            if (blockToAdd.Next != -1)
		            {
			            var nextBlock = GetBlock(blockToAdd.Next);
			            nextBlock.Previous = -1;
			            WriteBlock(nextBlock, blockToAdd.Next);
		            }

		            if (_nextEmptyBlockAddress == address)
		            {
			            _nextEmptyBlockAddress = blockToAdd.Next;
			            blockToAdd.Next = -1;
		            }
		            else if (_nextFreeBlockAddress == address)
		            {
			            _nextFreeBlockAddress = blockToAdd.Next;
			            blockToAdd.Next = -1;
		            }
	            }
	            else
	            {
		            throw new Exception("Error while inserting data");
	            }
	            
	            WriteBlock(blockToAdd, address);
	            RecordsCount++;
	            return address;
            }
            else // vytvaram novy blok na koniec suboru
            {
	            var address = (int)_file.Length;
	            var newBlock = new Block<T>(BlockSize, new T());
                newBlock.AddRecord(data);

                if (newBlock.ValidCount < newBlock.BlockFactor) 
                {
	                if (_nextFreeBlockAddress != -1) // zaradujem do zretazenia ciastocne plnych blokov
	                {
		                var nextBlock = GetBlock(_nextFreeBlockAddress);
		                nextBlock.Previous = address;
		                newBlock.Next = _nextFreeBlockAddress;
		                WriteBlock(nextBlock, _nextFreeBlockAddress);
	                }
	                _nextFreeBlockAddress = address;
                }
                WriteBlock(newBlock, address);
                RecordsCount++;
                return address;
            }
        }

        /// <summary>
        /// O(1,0) - jeden prístup
        /// Nájde daný prvok na danej adrese.
        /// </summary>
        /// <param name="address">Adresa bloku v ktorom sa prvok nachádza.</param>
        /// <param name="data">Prvok, ktorý chceme nájsť.</param>
        /// <returns>Trieda repzerentujúca prvok.</returns>
        public T Find(int address, T data)
        {
	        CheckAddress(address);
	        return GetBlock(address).GetRecord(data);
        }

        /// <summary>
        /// O(1,1) - jeden prístup, jeden zápis
        /// Daný prvok vymaže zo súboru.
        /// </summary>
        /// <param name="address">Adresa bloku, kde sa prvok nachádza.</param>
        /// <param name="data">Dáta prvku.</param>
        /// <returns>True - ak sa operácia podarila, inak False.</returns>
        public bool Delete(int address, T data)
        {
	        CheckAddress(address);

			var blockToDeleteFrom = GetBlock(address);
			if (!blockToDeleteFrom.RemoveRecord(data)) return false;
			
			if (blockToDeleteFrom is { Next: -1, Previous: -1 })
			{
				if (blockToDeleteFrom.ValidCount > 0)
				{
					if (_nextFreeBlockAddress != address)
					{
						if (_nextFreeBlockAddress != -1)
						{
							var nextFreeBlock = GetBlock(_nextFreeBlockAddress);
							nextFreeBlock.Previous = address;
							blockToDeleteFrom.Next = _nextFreeBlockAddress;
							WriteBlock(nextFreeBlock, _nextFreeBlockAddress);
						}
						_nextFreeBlockAddress = address;
					}
				}
				else
				{
					if (_nextEmptyBlockAddress != -1)
					{
						var nextEmptyBlock = GetBlock(_nextEmptyBlockAddress);
						nextEmptyBlock.Previous = address;
						blockToDeleteFrom.Next = _nextEmptyBlockAddress;
						WriteBlock(nextEmptyBlock, _nextEmptyBlockAddress);
					}
					_nextEmptyBlockAddress = address;
					
					if (_nextFreeBlockAddress == address)
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
						if (_nextFreeBlockAddress == address)
						{
							_nextFreeBlockAddress = blockToDeleteFrom.Next;
							nextBlock.Previous = -1;
						}
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
						nextEmptyBlock.Previous = address;
						blockToDeleteFrom.Next = _nextEmptyBlockAddress;
						WriteBlock(nextEmptyBlock, _nextEmptyBlockAddress);
					}
					_nextEmptyBlockAddress = address;
				}
			}
			
			WriteBlock(blockToDeleteFrom, address);
			RecordsCount--;
			CheckFileEnding();
			return true;
		}
        
        /// <summary>
        /// Vráti blok na zadanej adrese.
        /// </summary>
        /// <param name="address">Adresa vrámci súboru.</param>
        /// <returns>Blok</returns>
		public Block<T> GetBlock(int address)
		{
			CheckAddress(address);
			_file.Seek(address, SeekOrigin.Begin);
			byte[] bytes = new byte[BlockSize];
			_file.Read(bytes, 0, BlockSize);
			var block = new Block<T>(BlockSize, new T());
			block.FromByteArray(bytes);
			return block;
		}
		
        /// <summary>
        /// Vráti všetky bloky.
        /// </summary>
        /// <returns>List blokov</returns>
        public List<Block<T>> GetAllBlocks()
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

        /// <summary>
        /// Vráti všetky údaje zapísané v blokoch.
        /// </summary>
        /// <returns>List údajov</returns>
        public List<T> GetAllRecords()
        {
	        var ret = new List<T>();
	        for (int i = 0; i < BlockCount; i++)
	        {
		        Block<T> block = new Block<T>(BlockSize, new T());
		        int offset = i * BlockSize;
		        _file.Seek(offset, SeekOrigin.Begin);
		        byte[] bytes = new byte[BlockSize];
		        _file.Read(bytes, 0, BlockSize);
		        block.FromByteArray(bytes);

		        for (int j = 0; j < block.ValidCount; j++)
		        {
			        ret.Add(block.Records[j]);
		        }
	        }
	        return ret;
        }

        /// <summary>
        /// Vyčistí celý súbor.
        /// </summary>
        public void Clear()
        {
	        _nextFreeBlockAddress = -1;
	        _nextEmptyBlockAddress = -1;
	        SaveInitData();
	        _file.SetLength(0);
	        _file.Flush();
        }
        
        /// <summary>
        /// Metóda nutná pre použitie pri ukočení práce so súborom.
        /// </summary>
        public void Close()
        {
	        SaveInitData();
	        _nextFreeBlockAddress = -1;
	        _nextEmptyBlockAddress = -1;
	        _file.Flush();
	        _file.Close();
        }
        #endregion // Public functions

        #region Private functions
        private void SaveInitData()
        {
	        byte[] buffer = new byte[BlockSize];
	        int offset = 0;
	        
	        BitConverter.GetBytes(_nextFreeBlockAddress).CopyTo(buffer, offset);
	        offset += sizeof(int);
	        BitConverter.GetBytes(_nextEmptyBlockAddress).CopyTo(buffer, offset);
	        
	        var initFile = new FileStream(_initFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
	        initFile.Seek(0, SeekOrigin.Begin);
	        initFile.Write(buffer, 0, BlockSize);
	        initFile.Flush();
	        initFile.Close();
        }

        private void LoadInitData()
        {
	        byte[] buffer = new byte[BlockSize];
	        int offset = 0;
	        
	        var initFile = new FileStream(_initFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
	        initFile.Seek(offset, SeekOrigin.Begin);
	        initFile.Read(buffer, offset, BlockSize);
            
	        _nextFreeBlockAddress = BitConverter.ToInt32(buffer, offset);
	        offset += sizeof(int);
	        _nextEmptyBlockAddress = BitConverter.ToInt32(buffer, offset);
        }
        
        private void WriteBlock(Block<T> currentBlock, int currentBlockAddress)
        {
	        byte[] bytes = currentBlock.ToByteArray();
	        _file.Seek(currentBlockAddress, SeekOrigin.Begin);
	        _file.Write(bytes, 0, BlockSize);
	        _file.Flush();
        }
        
        private void CheckFileEnding()
        {
	        while (_file.Length > 0)
	        {
		        int lastBlockAddress = (int)_file.Length - BlockSize;
		        var lastBlock = GetBlock(lastBlockAddress);

		        if (lastBlock.ValidCount > 0) break;
		        
		        if (lastBlock.Next != -1 && lastBlock.Next < _file.Length && lastBlock.Previous < _file.Length)
		        {
			        var nextBlock = GetBlock(lastBlock.Next);
			        nextBlock.Previous = lastBlock.Previous;
			        WriteBlock(nextBlock, lastBlock.Next);
			        if (_nextEmptyBlockAddress == lastBlockAddress)
			        {
				        _nextEmptyBlockAddress = lastBlock.Next;
				        nextBlock.Previous = -1;
			        }
		        }
		        else
		        {
			        if (_nextEmptyBlockAddress == lastBlockAddress)
			        {
				        _nextEmptyBlockAddress = -1;
			        }
		        }

		        if (lastBlock.Previous != -1 && lastBlock.Previous < _file.Length && lastBlock.Next < _file.Length)
		        {
			        var prevBlock = GetBlock(lastBlock.Previous);
			        prevBlock.Next = lastBlock.Next;
			        WriteBlock(prevBlock, lastBlock.Previous);
		        }
		        
		        _file.SetLength(_file.Length - BlockSize);
	        }
        }
        
        private void CheckAddress(int address)
        {
	        if (address % BlockSize != 0 || address < 0 || address > _file.Length) throw new ArgumentException("Invalid address");
        }
        #endregion // Private functions
    }
}
