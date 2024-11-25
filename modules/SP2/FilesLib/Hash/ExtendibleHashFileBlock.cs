using FilesLib.Heap;
using FilesLib.Interfaces;

namespace FilesLib.Hash;

public class ExtendibleHashFileBlock<T> where T : class, IHashable<T>, new()
{
    #region Properties
    public int Address { get; set; }
    public int Depth { get; set; } = 1;
    public int ValidCount { get; set; } = 0;
    #endregion // Properties
    
    #region Constructors
    public ExtendibleHashFileBlock(int address)
    {
        Address = address;
    }
    #endregion // Constructors
    
    #region Public methods
    public override string ToString()
    {
        return $"Address: {Address}, Depth: {Depth}, ValidCount: {ValidCount}";
    }
    #endregion // Public methods
}