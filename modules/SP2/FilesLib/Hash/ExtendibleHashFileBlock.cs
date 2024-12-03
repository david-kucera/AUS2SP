using FilesLib.Interfaces;

namespace FilesLib.Hash;

/// <summary>
/// Class that represents hash file block.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ExtendibleHashFileBlock<T> : IRecord<T> where T : class, IHashable<T>, new()
{
	#region Properties
	/// <summary>
	/// Depth of block.
	/// </summary>
	public int Depth { get; set; } = 1;
    /// <summary>
    /// Address of block.
    /// </summary>
    public int Address { get; set; } = 0;
    #endregion // Properties

    #region Constructors
    public ExtendibleHashFileBlock()
    {

    }

	public ExtendibleHashFileBlock(int address)
	{
        Address = address;
    }

	public ExtendibleHashFileBlock(ExtendibleHashFileBlock<T> copy)
    {
        Depth = copy.Depth;
		Address = copy.Address;
    }
    #endregion // Constructors
    
    #region Public methods
    /// <summary>
    /// Returns string representation of class data.
    /// </summary>
    /// <returns>String</returns>
    public override string ToString()
    {
	    string ret = $"Address: {Address}, Depth: {Depth}";
		return ret;
    }

	/// <summary>
	/// Returns the size of the class in bytes.
	/// </summary>
	/// <returns>Integer</returns>
	public int GetSize()
	{
		return sizeof(int) + sizeof(int);
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

		BitConverter.GetBytes(Address).CopyTo(data, offset);

        return data;
	}

	/// <summary>
	/// Deserializes the class from byte array.
	/// </summary>
	/// <param name="data">Byte[]</param>
	/// <returns>Class instance</returns>
    public T FromByteArray(byte[] data)
    {
        int offset = 0;

        Depth = BitConverter.ToInt32(data, offset);
        offset += sizeof(int);

        Address = BitConverter.ToInt32(data, offset);

        return this as T;
    }
    #endregion // Public methods
}