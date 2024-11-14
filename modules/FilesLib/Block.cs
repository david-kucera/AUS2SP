using System.Runtime.InteropServices;
using FilesLib.Helpers;

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
			return Serializator.Serialize(this);
        }

		public T FromByteArray(byte[] byteArray)
		{
			return Serializator.Deserialize<T>(byteArray);
        }

		public int GetSize()
		{
			try
			{
				return Marshal.SizeOf(ClassType) * BlockSize + sizeof(int) * 3; // 3 lebo pamatame 3 integer hodnoty
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
