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
	            // Ak mam nejake volne blocky na pridanie dat
	            // Najskor vyberam ciastocne plne, ak ciastocne plne nie su, tak potom celkom prazdne
	            // Prve nacitanie zo suboru - block, do ktoreho sa budu zapisovat nove data
	            // 1. Ak sa po vlozeni do blocku este zmestia dalsie data
	            //		1.1 Ak bol v zretazeni prazdnych, tak je treba ho preradit do zretazenia ciastocne naplnenych/volnych
	            //			- Nastavim nasledovnika ako zaciatok zretazenia
	            //				- Ak existuje, tak si ho nacitam, nastavim mu ako predchodcu -1 a zapisem do suboru - O(1,1)
	            //			- Block zaradim ako zaciatok zretazenia ciastocne naplnenych/volnych blokov
	            //				- Ak zaciatok zretazenia uz bol, prepisem mu predchodcu na vkladany block a zapisem do suboru - O(1,1)
	            //		1.2 Inak v tomto pripade neriesim nic, nakolko ak uz je zaradeny v zretazeni ciastocnych blokov, tak
	            //		    tam aj ostane a nic sa nemeni
	            // 2. Ak sa don uz ziadne data nezmestia
	            //		- Ak ma nejakeho nasledovnika v zretazeni, je treba ho nacitat a nastavit mu ako predchodcu -1, teda
	            //		  ze je zaciatkom zretazenia on, potom zapisat do suboru - O(1,1)
	            //		- A ak bol block zaciatkom nejakeho zretazenia, je potrebne ho len z tohto zretazenia odstrihnut
	            if (_nextFreeBlockAddress != -1) address = _nextFreeBlockAddress;
	            else address = _nextEmptyBlockAddress;
	            
	            blockToAdd = GetBlock(address);
	            blockToAdd.AddRecord(data);

	            if (blockToAdd.ValidCount < blockToAdd.BlockFactor)
	            {
		            if (_nextEmptyBlockAddress == address)
		            {
			            if (blockToAdd.Next != -1)
			            {
				            var nextBlock = GetBlock(blockToAdd.Next);
				            nextBlock.Previous = -1;
				            WriteBlock(nextBlock, blockToAdd.Next);
			            }
			            _nextEmptyBlockAddress = blockToAdd.Next;
			            
			            if (_nextFreeBlockAddress != -1)
			            {
				            var nextFreeBlock = GetBlock(_nextFreeBlockAddress);
				            nextFreeBlock.Previous = address;
				            WriteBlock(nextFreeBlock, _nextFreeBlockAddress);
			            }
			            blockToAdd.Next = _nextFreeBlockAddress;
			            _nextFreeBlockAddress = address;
		            }
	            }
	            else 
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
		            if (_nextFreeBlockAddress == address)
		            {
			            _nextFreeBlockAddress = blockToAdd.Next;
			            blockToAdd.Next = -1;
		            }
	            }
            }
            else 
            {
	            // Vytvaram novy blok na konci suboru
	            // 1. Ak sa blok pridanim dat zaplnil, tak ho len zapiseme na koniec suboru - O(0,1)
	            // 2. Ak sa do noveho bloku nieco este zmesti, je treba ho zaradit na zaciatok zretazenia 
	            //	  aktualne zretazenych volnych blokov - teda je treba nacitat prvy v zretazeni, tomu nastavit
	            //	  ako predchodcu novo pridany blok, novemu ako nasledovnika stary, zapisat zmeny a nasledne
	            //	  nastavit novo vytvoreny blok ako zaciatok a zapisat do suboru - O(1,1)
	            address = (int)_file.Length;
	            blockToAdd = new Block<T>(BlockSize, new T());
                blockToAdd.AddRecord(data);
                
                if (blockToAdd.ValidCount < blockToAdd.BlockFactor) 
                {
	                if (_nextFreeBlockAddress != -1)
	                {
		                var nextBlock = GetBlock(_nextFreeBlockAddress);
		                nextBlock.Previous = address;
		                blockToAdd.Next = _nextFreeBlockAddress;
		                WriteBlock(nextBlock, _nextFreeBlockAddress);
	                }
	                _nextFreeBlockAddress = address;
                }
            }
            
            // Zapisanie dat blocku do suboru
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
	        // Jedna sa len o jeden pristup do suboru - citanie - O(1,0)
	        CheckAddress(address);
	        return GetBlock(address).GetRecord(data);
        }

        /// <summary>
        /// Daný prvok vymaže zo súboru.
        /// </summary>
        /// <param name="address">Adresa bloku, kde sa prvok nachádza.</param>
        /// <param name="data">Dáta prvku.</param>
        /// <returns>True - ak sa operácia podarila, inak False.</returns>
        public bool Delete(int address, T data)
        {
	        // Najprv sa nacita blok na danej adrese a vyhodi sa z neho dany zaznam - O(1,0);
	        // 1. Ak blok nie je v ziadnom zretazeni - pretoze bol plny
	        //    1.1 Ak blok obsahuje nejake validne data
	        //       - je potrebne ho zaradit do zretazenia ciastocne plnych/volnych blockov
	        //          1.1.1 Ak je adresa zaciatkom zretazenia, je to ok, neriesime nic
	        //          1.1.2 Ak je vsak adresa zaciatku zretazenia ina, musime si nacitat blok na zaciatku zretazenia,
	        //                nastavit mu ako predchodcu aktualny blok - zmeny zapisat - O(1,1)
	        //                a ako zaciatok zretazenia nastavit aktualny blok
	        //    1.2 Ak blok neobsahuje ziadne validne data - je prazdny
	        //       - je potrebne ho zaradit do zretazenia prazdnych blockov
	        //			- ak uz zaciatok zretazenia existuje, je potrebne si block nacitat, ako predchodcu mu dat
	        //            aktualny block, aktualnemu blocku nastavit nasledovnika na tento block a zmeny v nom zapisat
	        //            do suboru - teda O(1,1) ... nakoniec ako zaciatok zretazenia uviest nas block
	        // 2. Ak blok je sucastou nejakeho zretazenia
	        //    - sucastou prazdnych blockov byt nemohol, nakolko obsahoval nejaky zaznam, tak teda je potrebne len
	        //      osledovat, ci bol sucastou zretazenia ciastocne plnych blockov a stal sa z neho prazdny block
	        //    2.1 Ak sa z blocku stal vymazanim zaznamu prazdny block - treba vystrihnut z retazenia a zaradit
	        //		  do zretazenia prazdnych
	        //        - Ak mal nejakeho nasledovnika, je potrebne si ho nacitat, nastavit mu predchodcu
	        //          na predchodcu aktualneho blocku a zapisat do suboru - O(1,1)
	        //        - Ak mal nejakeho predchodcu, je potrebne si ho nacitat, nastavit mu nasledovnika
	        //          na nasledovnika aktualneho blocku a zapisat do suboru - O(1,1)
	        //        - Vynulujeme zretazenie blocku a zaradime do zretazenia prazdnych blockov
	        //        - Ak uz v zretazeni nejaky block na zaciatku je, nacitame si ho, ako predchodcu mu
	        //          nastavime aktualny block, aktualnemu nastavime ako nasledovnika dany block a zmenu zapiseme
	        //          do suboru - O(1,1) ... nakoniec ako zaciatok zretazenia dame aktualny block
	        //    2.2 Inak netreba menit nic
	        // Nakoniec sa blok zapise do suboru - O(0,1)
	        // a nasledne sa v subore este skontroluje, ci na konci nie su prazdne bloky - ak ano, vyhodia sa.
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
        /// Zapíše block na danú adresu.
        /// </summary>
        /// <param name="block">Block</param>
        /// <param name="address">Adresa</param>
        public void WriteBlock(Block<T> block, int address)
        {
	        byte[] bytes = block.ToByteArray();
	        _file.Seek(address, SeekOrigin.Begin);
	        _file.Write(bytes, 0, BlockSize);
	        _file.Flush();
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
        
        private void CheckFileEnding()
        {
	        // Postupne sa prechadzaju blocky od konca, kym sa nenarazi na block, ktory obsahuje nejake validne data
	        // Nacita sa posledny block - O(1,0)
	        // Ak neobsahuje ziadne validne data - ide sa mazat, ak obsahuje => koniec
	        // Ak obsahuje nejake data, bude teda v zretazeni ciastocne plnych/volnych
	        //    - Ak je zaciatkom zretazenia, treba nastavit zaciatok na jeho nasledovnika
	        // 1. Ak block ma nejakeho nasledovnika
	        //     - nacitam si dany block, ako predchodcu mu nastavim predchodcu mazaneho blocku - zapisem - O(1,1)
	        // 2. Ak block obsahuje predchodcu
	        //     - nacitam si dany block, ako nasledovnika mu nastavim nasledovnika mazaneho blocku - zapisem - O(1,1)
	        // Nakoniec subor skratim o dany block
	        while (true)
	        {
		        int lastBlockAddress = (int)_file.Length - BlockSize;
		        var lastBlock = GetBlock(lastBlockAddress);

		        if (lastBlock.ValidCount > 0) break;
		        
		        if (_nextEmptyBlockAddress == lastBlockAddress)
		        {
			        _nextEmptyBlockAddress = lastBlock.Next;
		        }
		        
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
