using FilesLib.Interfaces;

namespace FilesLib.Heap
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
        /// <summary>
        /// Maximálny počet záznamov v bloku.
        /// </summary>
        public int BlockFactor => BlockSize / new T().GetSize();
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

   //         if (_file.Length == 0) SaveInitData();
			//else LoadInitData();
        }
        #endregion // Constructor

        #region Public functions
        /// <summary>
        /// Vloží dáta do súboru.
        /// </summary>
        /// <param name="data">Trieda dát</param>
        /// <returns></returns>
        /// <exception cref="Exception">Ak nastane chyba pri pridávaní dát.</exception>
        public int Insert(T data)
        {
	        Block<T> blockToAdd;
	        int address;
	        
            if (_nextFreeBlockAddress != -1 || _nextEmptyBlockAddress != -1)
            {
	            if (_nextFreeBlockAddress != -1) address = _nextFreeBlockAddress;
	            else address = _nextEmptyBlockAddress;

	            blockToAdd = GetBlock(address);
	            blockToAdd.AddRecord(data);

	            if (blockToAdd.ValidCount < blockToAdd.BlockFactor) AddBlockToFreeList(blockToAdd, address);
	            else RemoveBlockFromLinkedLists(blockToAdd, address);
            }
            else
            {
	            address = (int)_file.Length;
	            blockToAdd = new Block<T>(BlockSize, new T());
                blockToAdd.AddRecord(data);

                if (blockToAdd.ValidCount < blockToAdd.BlockFactor) AddNewBlockToFreeList(blockToAdd, address);
            }
            
            WriteBlock(blockToAdd, address);
            RecordsCount++;
            return address;
        }

        /// <summary>
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

        public void Update(int address, List<T> newData)
        {
	        CheckAddress(address);
	        
	        var block = GetBlock(address);
	        var blockRecordsCount = block.ValidCount;
	        RecordsCount -= blockRecordsCount;
	        block.ClearRecords();
	        foreach (var data in newData)
	        {
		        block.AddRecord(data);
	        }
	        WriteBlock(block, address);
        }

        /// <summary>
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
				if (blockToDeleteFrom.ValidCount > 0) AddBlockToFreeList(blockToDeleteFrom, address);
				else AddBlockToEmptyList(blockToDeleteFrom, address);
			}
			else
			{
				if (blockToDeleteFrom.ValidCount == 0) MoveBlockFromFreeToEmptyList(blockToDeleteFrom, address);
			}
			
			WriteBlock(blockToDeleteFrom, address);
			RecordsCount--;
			
			if (_file.Length - BlockSize == address) CheckFileEnding();
			
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
        /// Vytvorí nový blok na koniec súboru.
        /// </summary>
        /// <returns>Adresa nového bloku.</returns>
        public int CreateNewBlock()
        {
	        var address = (int)_file.Length;
	        var newBlock = new Block<T>(BlockSize, new T());
	        WriteBlock(newBlock, address);
	        return address;
        }
        
        /// <summary>
        /// Zapíše dáta bloku do súboru na danú adresu.
        /// </summary>
        /// <param name="block">Blok</param>
        /// <param name="address">Adresa</param>
        public void WriteBlock(Block<T> block, int address)
        {
	        byte[] bytes = block.ToByteArray();
	        _file.Seek(address, SeekOrigin.Begin);
	        _file.Write(bytes, 0, BlockSize);
	        _file.Flush();
        }

        /// <summary>
        /// Vráti sekvečnú reprezentáciu celého súboru na disku.
        /// </summary>
        /// <returns>Reťazec tvorený dátami bloku.</returns>
        public string SequentialOutput()
        {
	        string ret = string.Empty;
	        var allBlocks = GetAllBlocks();
	        int por = 0;
	        foreach (var block in allBlocks)
	        {
		        ret += por + ". Block:" +  " at address: " + por * BlockSize + Environment.NewLine;
		        ret += block.ToString();
		        ret += Environment.NewLine;
		        por++;
	        }
	        if (allBlocks.Count == 0) ret += "There are no records in this block."; 
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

        /// <summary>
        /// Metóda pre výpis zreťazenia heap file.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
	        return "NextFreeBlockAddress: " + _nextFreeBlockAddress + ", NextEmptyBlockAddress: " + _nextEmptyBlockAddress;
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
        
        private void AddBlockToFreeList(Block<T> block, int address)
        {
	        if (_nextEmptyBlockAddress == address)
	        {
		        if (block.Next != -1)
		        {
			        var nextBlock = GetBlock(block.Next);
			        nextBlock.Previous = -1;
			        WriteBlock(nextBlock, block.Next);
		        }
		        _nextEmptyBlockAddress = block.Next;
		        
		        if (_nextFreeBlockAddress != -1)
		        {
			        var nextFreeBlock = GetBlock(_nextFreeBlockAddress);
			        nextFreeBlock.Previous = address;
			        WriteBlock(nextFreeBlock, _nextFreeBlockAddress);
		        }
		        block.Next = _nextFreeBlockAddress;
		        _nextFreeBlockAddress = address;
	        }
        }
        
        private void AddNewBlockToFreeList(Block<T> block, int address)
        {
	        if (_nextFreeBlockAddress != -1)
	        {
		        var nextFreeBlock = GetBlock(_nextFreeBlockAddress);
		        nextFreeBlock.Previous = (int)_file.Length;
		        block.Next = _nextFreeBlockAddress;
		        WriteBlock(nextFreeBlock, _nextFreeBlockAddress);
	        }
	        _nextFreeBlockAddress = address;
        }
        
        private void AddBlockToEmptyList(Block<T> block, int address)
        {
	        if (_nextEmptyBlockAddress != -1)
	        {
		        var nextEmptyBlock = GetBlock(_nextEmptyBlockAddress);
		        nextEmptyBlock.Previous = address;
		        block.Next = _nextEmptyBlockAddress;
		        WriteBlock(nextEmptyBlock, _nextEmptyBlockAddress);
	        }
	        _nextEmptyBlockAddress = address;
					
	        if (_nextFreeBlockAddress == address) _nextFreeBlockAddress = -1;
        }

        private void RemoveBlockFromLinkedLists(Block<T> block, int address)
        {
	        if (block.Next != -1)
	        {
		        var nextBlock = GetBlock(block.Next);
		        nextBlock.Previous = -1;
		        WriteBlock(nextBlock, block.Next);
	        }

	        if (_nextEmptyBlockAddress == address) _nextEmptyBlockAddress = block.Next;
	        if (_nextFreeBlockAddress == address) _nextFreeBlockAddress = block.Next;
	        
	        block.Next = -1;
        }
        
        private void MoveBlockFromFreeToEmptyList(Block<T> block, int address)
        {
	        if (block.Next != -1)
	        {
		        var nextBlock = GetBlock(block.Next);
		        nextBlock.Previous = block.Previous;
		        if (_nextFreeBlockAddress == address)
		        {
			        _nextFreeBlockAddress = block.Next;
			        nextBlock.Previous = -1;
		        }
		        WriteBlock(nextBlock, block.Next);
	        }

	        if (block.Previous != -1)
	        {
		        var prevBlock = GetBlock(block.Previous);
		        prevBlock.Next = block.Next;
		        WriteBlock(prevBlock, block.Previous);
	        }
					
	        block.Next = -1;
	        block.Previous = -1;
	        
	        AddBlockToEmptyList(block, address);
        }
        
        private void CheckFileEnding()
        {
	        while (_file.Length > 0)
	        {
		        int lastBlockAddress = (int)_file.Length - BlockSize;
		        var lastBlock = GetBlock(lastBlockAddress);

		        if (lastBlock.ValidCount > 0) break;
		        
		        if (_nextEmptyBlockAddress == lastBlockAddress) _nextEmptyBlockAddress = lastBlock.Next;
		        
		        if (lastBlock.Next != -1)
		        {
			        var nextBlock = GetBlock(lastBlock.Next);
			        nextBlock.Previous = lastBlock.Previous;
			        WriteBlock(nextBlock, lastBlock.Next);
		        }

		        if (lastBlock.Previous != -1)
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