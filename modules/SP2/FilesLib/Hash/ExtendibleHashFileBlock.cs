using FilesLib.Interfaces;

namespace FilesLib.Hash;

/// <summary>
/// Class that represents hash file block.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ExtendibleHashFileBlock<T> where T : class, IHashable<T>, new()
{
	#region Constants 
	private const int BLOCK_SIZE = 4096;
	#endregion // Constants

	#region Properties
	/// <summary>
	/// Depth of block.
	/// </summary>
	public int Depth { get; set; } = 1;
    /// <summary>
    /// Values/records in this block.
    /// </summary>
    public List<T> Values { get; set; } = new();
    #endregion // Properties

    #region Constructors
    public ExtendibleHashFileBlock()
    {

    }

    public ExtendibleHashFileBlock(ExtendibleHashFileBlock<T> copy)
    {
        Depth = copy.Depth;
        Values = copy.Values;
    }
    #endregion // Constructors
    
    #region Public methods
    /// <summary>
    /// Returns string representation of class data.
    /// </summary>
    /// <returns>String</returns>
    public override string ToString()
    {
	    string ret = $"Depth: {Depth}, ValidCount: {Values.Count}";
		foreach (var value in Values)
		{
			ret += $"\n{value}";
		}
		return ret;
    }

    /// <summary>
    /// Returns the value from the list of values.
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public T GetValue(T val)
    {
        foreach (var value in Values)
        {
            if (value.Equals(val)) return value;
        }
        return null!;
    }

	/// <summary>
	/// Returns the size of the class in bytes.
	/// </summary>
	/// <returns>Integer</returns>
	public int GetSize()
	{
		return BLOCK_SIZE;
	}

	/// <summary>
	/// Serializes the class to byte array.
	/// </summary>
	/// <returns>Byte[]</returns>
	public byte[] ToByteArray()
	{
        byte[] data = new byte[BLOCK_SIZE];
		int offset = 0;

		BitConverter.GetBytes(Depth).CopyTo(data, offset);
		offset += sizeof(int);

		BitConverter.GetBytes(Values.Count).CopyTo(data, offset);
        offset += sizeof(int);

		foreach (var value in Values)
		{
			value.ToByteArray().CopyTo(data, offset);
			offset += value.GetSize();
		}

        return data;
	}

	/// <summary>
	/// Deserializes the class from byte array.
	/// </summary>
	/// <param name="data">Byte[]</param>
	/// <returns>Class instance</returns>
	public ExtendibleHashFileBlock<T> FromByteArray(byte[] data)
	{
        int offset = 0;

		Depth = BitConverter.ToInt32(data, offset);
		offset += sizeof(int);

		int count = BitConverter.ToInt32(data, offset);
		offset += sizeof(int);

		Values.Clear();

		for (int i = 0; i < count; i++)
		{
			T value = new();
            int valueSize = value.GetSize();

            byte[] valueData = data[offset..(offset + valueSize)];
			value.FromByteArray(valueData);
			Values.Add(value);
			offset += value.GetSize();
		}

		return this;
	}
	#endregion // Public methods
}