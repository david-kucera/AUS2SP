using FilesLib.Interfaces;

namespace FilesLib.Heap
{
	public class Block<T> : IRecord<T> where T : class, IData<T>, new()
	{
        #region Class members
        private readonly int _blockSize;
        private readonly int _size;
		private readonly int _dataSize;
        private int _prevBlock = -1;
        private int _nextBlock = -1;
        #endregion // Class members

        #region Properties
        /// <summary>
        /// Pocet udajov, ktore sa mozu uchovat v bloku
        /// </summary>
        public int BlockFactor => _size / _dataSize;
		/// <summary>
		/// Zoznam udajov v danom bloku
		/// </summary>
        public List<T> Records { get; set; }
		/// <summary>
		/// Pocet realnych udajov v bloku
		/// </summary>
		public int ValidCount { get; set; } = 0;
		/// <summary>
		/// Typ dat ulozenych v bloku
		/// </summary>
		public Type ClassType { get; set; }
		/// <summary>
		/// Dalsi blok v zretazeni
		/// </summary>
		public int Next { get { return _nextBlock; } set => _nextBlock = value; }
		/// <summary>
		/// Predosly blok v zretazeni
		/// </summary>
		public int Previous { get { return _prevBlock; } set => _prevBlock = value; }
		#endregion // Properties

		#region Constructors
		public Block(int blockSize, T cls)
		{
			_blockSize = blockSize;
            _size = blockSize;
            _size -= sizeof(int) * 3;
			_dataSize = cls.CreateClass().GetSize();
			
            ClassType = cls.GetType();
			Records = new List<T>(BlockFactor);
			for (int i = 0; i < BlockFactor; i++)
			{
				try
				{
					T? dummy = Activator.CreateInstance(ClassType) as T;	
					Records.Add(dummy!);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}

		public Block(Block<T> block)
		{
			_blockSize = block._blockSize;
			_size = block._size;
			_dataSize = block._dataSize;
			ClassType = block.ClassType;

			ValidCount = 0;
			_prevBlock = block.Previous;
			_nextBlock = block.Next;
			
			Records = new List<T>(BlockFactor);
			for (int i = 0; i < BlockFactor; i++)
			{
				try
				{
					T? dummy = Activator.CreateInstance(ClassType) as T;	
					Records.Add(dummy!);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}
		#endregion // Constructors

		#region Public functions
		public void AddRecord(T record)
		{
			if (ValidCount < BlockFactor)
			{
				Records[ValidCount] = record;
				ValidCount++;
			}
			else throw new Exception("Block is full");
        }

		public void RemoveRecord(int index)
		{
			if (index < 0) throw new Exception("Invalid index pointer.");
			if (index >= ValidCount) throw new Exception("Index out of range.");

			for (int i = index; i < ValidCount - 1; i++)
			{
				Records[i] = Records[i + 1];
			}
            ValidCount--;
        }

		public bool RemoveRecord(T record)
		{
            for (int i = 0; i < ValidCount; i++)
            {
                if (Records[i].Equals(record))
                {
                    RemoveRecord(i);
                    return true;
                }
            }
            return false;
        }

		public T GetRecord(int index)
		{
			return Records[index];
		}

		public T GetRecord(T record)
		{
			for (int i = 0; i < ValidCount; i++)
			{
				if (record.Equals(Records[i])) return Records[i];
			}
            return null!;
        }

		public void ClearRecords()
		{
			Records = new List<T>(BlockFactor);
			ValidCount = 0;
		}

        public byte[] ToByteArray()
		{
			byte[] bytes = new byte[_blockSize];
			int offset = 0;

			BitConverter.GetBytes(ValidCount).CopyTo(bytes, offset);
            offset += sizeof(int);
			BitConverter.GetBytes(_prevBlock).CopyTo(bytes, offset);
            offset += sizeof(int);
            BitConverter.GetBytes(_nextBlock).CopyTo(bytes, offset);
            offset += sizeof(int);

            for (int i = 0; i < ValidCount; i++)
            {
                byte[] recordBytes = Records[i].ToByteArray();
                recordBytes.CopyTo(bytes, offset);
                offset += recordBytes.Length;
            }

            return bytes;
        }

		public T FromByteArray(byte[] byteArray)
		{
            int offset = 0;
            ValidCount = BitConverter.ToInt32(byteArray, offset);
            offset += sizeof(int);
            _prevBlock = BitConverter.ToInt32(byteArray, offset);
            offset += sizeof(int);
            _nextBlock = BitConverter.ToInt32(byteArray, offset);
            offset += sizeof(int);

            for (int i = 0; i < BlockFactor; i++)
            {
                byte[] recordBytes = new byte[_dataSize];
                Array.Copy(byteArray, offset, recordBytes, 0, _dataSize);
                Records[i].FromByteArray(recordBytes);
                offset += recordBytes.Length;
            }

			return this as T;
        }

		public int GetSize()
		{
			try
			{
				return _dataSize * BlockFactor + sizeof(int) * 3; // 3 lebo pamatame 3 integer hodnoty
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return -1;
			}
		}

		public void Clear()
		{
			ValidCount = 0;
			_nextBlock = -1;
			_prevBlock = -1;
            for (int i = 0; i < BlockFactor; i++)
            {
                Records[i] = null!;
            }
        }

		public override string ToString()
		{
			return "Next,Previous: " + Next + "," + Previous + " ... "+ ValidCount + " record(s):\n" + string.Join("\n", Records);
			
		}
		#endregion // Public functions
	}
}
