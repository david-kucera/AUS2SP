namespace FilesLib
{
	public class HeapFile<T> where T : class, IData<T>, new()
	{
		#region Class members
		private FileStream _file;
		private Block<T> _activeBlock = null!;
        private int _activeBlockAddr = -1;
		private int _nextFreeBlock = -1;
		private int _nextEmptyBlock = -1;
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
                _nextFreeBlock = -1;
                _nextEmptyBlock = 0;
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

		public bool Delete(int adress)
		{
			// TODO delete operation
			Block<T> block = new Block<T>(BlockSize, TType);
			return false;
		}

		public T Find(int adress)
		{
			// TODO find operation
			Block<T> block = new Block<T>(BlockSize, TType);
			return null!;
		}
		#endregion
	}
}
