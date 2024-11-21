namespace FilesLib;

public class HashFile<T> where T : class, IData<T>, new()
{
    #region Properties
    public HeapFile<T> HeapFile { get; set; }
    public int Depth { get; set; } = 0;
    #endregion // Properties
    
    #region Constructors
    public HashFile(string filePath, string initFilePath, int blockSize)
    {
        HeapFile = new HeapFile<T>(initFilePath, filePath, blockSize);
        HeapFile.Clear();
    }
    #endregion // Constructors
    
    #region Public methods
    public void Insert(T data)
    {
        throw new NotImplementedException();
    }

    public T Find(T data)
    {
        throw new NotImplementedException();
    }

    public void Delete(T data)
    {
        throw new NotImplementedException();
    }
    #endregion // Public methods
    
    #region Private methods
    
    #endregion // Private methods
}