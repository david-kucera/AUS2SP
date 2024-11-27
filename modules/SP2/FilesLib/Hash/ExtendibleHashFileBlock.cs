using FilesLib.Interfaces;

namespace FilesLib.Hash;

public class ExtendibleHashFileBlock<T> where T : class, IHashable<T>, new()
{
    #region Properties
    public int Address { get; set; }
    public int Depth { get; set; } = 1;
    #endregion // Properties
    
    #region Constructors
    public ExtendibleHashFileBlock(int address)
    {
        Address = address; 
    }

    public ExtendibleHashFileBlock(ExtendibleHashFileBlock<T> block)
    {
        Address = block.Address;
        Depth = block.Depth;
    }
    #endregion // Constructors
    
    #region Public methods
    public override string ToString()
    {
        return $"Address: {Address}, Depth: {Depth}";
    }
    #endregion // Public methods
}