using FilesLib.Heap;
using FilesLib.Interfaces;

namespace FilesLib.Hash;

public class HashFile<T> where T : class, IHashable<T>, new()
{
    #region Properties
    public HeapFile<T> HeapFile { get; set; }
    public int Depth { get; set; } = 0;
    public int RecordsCount { get; set; } = 0;
    #endregion // Properties
    
    #region Constructors
    public HashFile(string filePath, string initFilePath, int blockSize)
    {
        HeapFile = new HeapFile<T>(initFilePath, filePath, blockSize);
        HeapFile.Clear();
    }
    #endregion // Constructors
    
    #region Public methods
    public int Insert(T data)
    {
        throw new NotImplementedException();
    }

    public T Find(T data)
    {
        throw new NotImplementedException();
    }

    public bool Delete(T data)
    {
        // TODO Kontrola rozpracovania z
        throw new NotImplementedException();
    }
    #endregion // Public methods
    
    #region Private methods
    
    #endregion // Private methods
}