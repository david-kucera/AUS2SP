using FilesLib.Interfaces;

namespace FilesLib.Hash;

public class ExtendibleHashFileBlock<T> where T : class, IHashable<T>, new()
{
    #region Properties
    public int Depth { get; set; } = 1;
    public List<IHashable<T>> Values { get; set; } = new();
    #endregion // Properties

    #region Constructors
    public ExtendibleHashFileBlock()
    {

    }

    public ExtendibleHashFileBlock(ExtendibleHashFileBlock<T> block)
    {
        Depth = block.Depth;
        Values = block.Values;
    }
    #endregion // Constructors
    
    #region Public methods
    public override string ToString()
    {
        return $"Depth: {Depth}, ValidCount: {Values.Count}";
    }
    #endregion // Public methods
}