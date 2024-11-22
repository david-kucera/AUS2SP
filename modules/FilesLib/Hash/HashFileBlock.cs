using FilesLib.Heap;
using FilesLib.Interfaces;

namespace FilesLib.Hash;

public class HashFileBlock<T> where T : class, IHashable<T>, new()
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
    public HashFileBlock(int address, HeapFile<T> heapFile)
    {
        Address = address;
        _heapFile = heapFile;
    }
    #endregion // Constructors
    
    #region Public methods
    public override string ToString()
    {
        return $"{Address}, {Depth}";
    }
    #endregion // Public methods
}