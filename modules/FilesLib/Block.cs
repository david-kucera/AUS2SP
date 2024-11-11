using System.Runtime.InteropServices;

namespace FilesLib
{
	public class Block<T> : IRecord<T> where T : class, IData<T>, new()
	{
		#region Properties
		public int BlockSize { get; set; }
		public List<T> Records { get; set; }
		public int ValidCount { get; set; }
		public int NextBlock { get; set; }
		public int PrevBlock { get; set; }
		public Type ClassType { get; set; }
		#endregion // Properties

		#region Constructors
		public Block(int blockSize, Type classType)
		{
			BlockSize = blockSize;
			ClassType = classType;
			Records = new List<T>(BlockSize);
			for (int i = 0; i < BlockSize; i++)
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
			ValidCount = 0;
			NextBlock = -1;
			PrevBlock = -1;
		}
		#endregion // Constructors

		#region Public functions
		public bool AddRecord(T record)
		{
			if (ValidCount < BlockSize)
			{
				Records[ValidCount] = record;
				ValidCount++;
				return true;
			}
			return false;
		}

		public byte[] ToByteArray()
		{
			throw new NotImplementedException();
		}

		public T FromByteArray(byte[] byteArray)
		{
			throw new NotImplementedException();
		}

		public int GetSize()
		{
			try
			{
				int integerSize = Marshal.SizeOf(typeof(int));
				return Marshal.SizeOf(ClassType) * BlockSize + integerSize * 3; // 3 lebo pamatame 3 integer hodnoty
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return -1;
			}
		}
		#endregion // Public functions
	}
}
