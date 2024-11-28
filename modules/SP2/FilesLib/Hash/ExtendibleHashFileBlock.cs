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
    public List<IHashable<T>> Values { get; set; } = new();
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
        return $"Depth: {Depth}, ValidCount: {Values.Count}";
    }

    /// <summary>
    /// Returns the value from the list of values.
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public IHashable<T> GetValue(T val)
    {
        foreach (var value in Values)
        {
            if (value.Equals(val)) return value;
        }
        return null!;
    }
    #endregion // Public methods
}