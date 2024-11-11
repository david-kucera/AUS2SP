namespace FilesLib
{
	public class Heap<T> where T : class, IData<T>, new()
	{
		#region Properties
		public int BlockSize { get; set; }
		public FileStream File { get; set; }
		public Type TType { get; set; }
		#endregion // Properties

		#region Constructors
		public Heap(string filePath, int blockSize)
		{
			BlockSize = blockSize;
			TType = typeof(T);
			try
			{
				File = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
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
