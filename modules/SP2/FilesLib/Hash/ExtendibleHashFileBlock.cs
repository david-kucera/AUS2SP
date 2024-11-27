using FilesLib.Heap;
using FilesLib.Interfaces;

namespace FilesLib.Hash;

public class ExtendibleHashFileBlock<T> where T : class, IHashable<T>, new()
{
    private HeapFile<T> _heapFile;
    
    #region Properties
    public Block<T> Block => _heapFile.GetBlock(Address);
    public int Address { get; set; }
    public int Depth { get; set; } = 1;
    #endregion // Properties
    
    #region Constructors
    public ExtendibleHashFileBlock(int address, HeapFile<T> heapFile)
    {
        Address = address; 
        _heapFile = heapFile;
    }

    public ExtendibleHashFileBlock(ExtendibleHashFileBlock<T> block)
    {
        Address = block.Address;
        Depth = block.Depth;
        _heapFile = block._heapFile;
    }
    #endregion // Constructors
    
    #region Public methods
    public override string ToString()
    {
        return $"Address: {Address}, Depth: {Depth}";
    }
    #endregion // Public methods
}