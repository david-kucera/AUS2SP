using FilesLib.Heap;
using FilesLib.Interfaces;

namespace FilesLib.Hash;

public class HashFile<T> where T : class, IHashable<T>, new()
{
    #region Properties
    public HeapFile<T> HeapFile { get; set; }
    public HashFileBlock<T>[] Adresses { get; set; }
    public int Depth { get; set; } = 1;
    public int RecordsCount { get; set; } = 0;
    #endregion // Properties
    
    #region Constructors
    public HashFile(string filePath, string initFilePath, int blockSize)
    {
        HeapFile = new HeapFile<T>(initFilePath, filePath, blockSize);
        HeapFile.Clear();

        Adresses = new HashFileBlock<T>[(int)Math.Pow(2, Depth)];
        var initialAddr0 = HeapFile.CreateNewBlock();
        var initialBlock0 = new HashFileBlock<T>(initialAddr0, HeapFile);
        Adresses[0] = initialBlock0;
        var initialAddr1 = HeapFile.CreateNewBlock();
        var initialBlock1 = new HashFileBlock<T>(initialAddr1, HeapFile);
        Adresses[1] = initialBlock1;
        
    }
    #endregion // Constructors
    
    #region Public methods
    public int Insert(T data)
    {
        bool notInserted = true;
        int ret = -1;

        while (notInserted)
        {
            int hash = data.GetHash();
            int prefix = GetPrefix(hash);
            var block = Adresses[prefix].Block;
            var address = Adresses[prefix].Address;
            var blockDepth = Adresses[prefix].Depth;
            
            if (block.BlockFactor == block.ValidCount)
            {
                if (blockDepth == Depth)
                {
                    IncreaseDepth();
                }

                SplitBlock(prefix);
            }
            else
            {
                block.AddRecord(data);
                ret = address;
                notInserted = false;
            }
        }

        return ret;
    }

    public T Find(T data)
    {
        return GetBlock(data).GetRecord(data);
    }
    
    public bool Delete(T data)
    {
        // TODO Kontrola rozpracovania 2
        throw new NotImplementedException();
    }
    #endregion // Public methods
    
    #region Private methods
    private int GetPrefix(int hash)
    {
        return hash & ((1 << Depth) - 1);
    }

    private Block<T> GetBlock(T data)
    {
        int hash = data.GetHash();
        int prefix = GetPrefix(hash);
        return Adresses[prefix].Block;
    }
    
    private void SplitBlock(int prefix)
    {
        throw new NotImplementedException();
    }

    private void IncreaseDepth()
    {
        throw new NotImplementedException();
    }
    #endregion // Private methods
}