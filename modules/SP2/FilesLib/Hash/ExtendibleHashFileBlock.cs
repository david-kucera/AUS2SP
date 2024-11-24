using FilesLib.Heap;
using FilesLib.Interfaces;

namespace FilesLib.Hash;

public class ExtendibleHashFileBlock<T> where T : class, IHashable<T>, new()
{
    #region Class members
    private readonly HeapFile<T> _heapFile;
    #endregion // Class members
    
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
        _heapFile = block._heapFile;
		Depth = block.Depth;
	}
    #endregion // Constructors
    
    #region Public methods
    public override string ToString()
    {
        return $"{Address}, {Depth}";
    }
    #endregion // Public methods
}