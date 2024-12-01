using FilesLib.Data;
using FilesLib.Interfaces;

namespace FilesLib.Hash;

/// <summary>
/// Class that represents hash file block.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ExtendibleHashFileBlock<T> where T : class, IHashable<T>, new()
{
	#region Properties
	/// <summary>
	/// Depth of block.
	/// </summary>
	public int Depth { get; set; } = 1;
    /// <summary>
    /// Values/records in this block.
    /// </summary>
    public List<T> Values { get; set; } = new();
	/// <summary>
	/// Block size.
	/// </summary>
	public int BlockFactor { get; set; } = 0;
	#endregion // Properties

	#region Constructors
	public ExtendibleHashFileBlock()
    {

    }

	public ExtendibleHashFileBlock(int blockFactor)
	{
		BlockFactor = blockFactor;
	}

	public ExtendibleHashFileBlock(ExtendibleHashFileBlock<T> copy)
    {
        Depth = copy.Depth;
        Values = copy.Values;
		BlockFactor = copy.BlockFactor;
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
		return sizeof(int) + sizeof(int) + sizeof(int) + BlockFactor * new VisitEcv().GetSize(); // Depth + BlockFactor + Count of values + BlockFactor*sizeof(VisitEcv)
	}

	/// <summary>
	/// Serializes the class to byte array.
	/// </summary>
	/// <returns>Byte[]</returns>
	public byte[] ToByteArray()
	{
        byte[] data = new byte[GetSize()];
		int offset = 0;

		BitConverter.GetBytes(Depth).CopyTo(data, offset);
		offset += sizeof(int);

		BitConverter.GetBytes(BlockFactor).CopyTo(data, offset);
		offset += sizeof(int);

		BitConverter.GetBytes(Values.Count).CopyTo(data, offset);
        offset += sizeof(int);

		var valueSize = new VisitEcv().GetSize();
		foreach (var value in Values)
		{
			value.ToByteArray().CopyTo(data, offset);
			offset += valueSize;
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

		BlockFactor = BitConverter.ToInt32(data, offset);
		offset += sizeof(int);

		int count = BitConverter.ToInt32(data, offset);
		offset += sizeof(int);

		Values.Clear();

		var valueSize = new VisitEcv().GetSize();
		for (int i = 0; i < count; i++)
		{
			T value = new();

            byte[] valueData = data[offset..(offset + valueSize)];
			value.FromByteArray(valueData);
			Values.Add(value);
			offset += valueSize;
		}

		return this;
	}
	#endregion // Public methods
}