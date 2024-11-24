using FilesLib.Heap;
using FilesLib.Interfaces;

namespace FilesLib.Hash;

public class ExtendibleHashFile<T> where T : class, IHashable<T>, new()
{
    #region Properties
    public HeapFile<T> HeapFile { get; set; }
    public ExtendibleHashFileBlock<T>[] Addresses { get; set; }
    public int Depth { get; set; } = 1;
    public int RecordsCount { get; set; } = 0;
    #endregion // Properties
    
    #region Constructors
    public ExtendibleHashFile(string initFilePath, string filePath, int blockSize)
    {
        HeapFile = new HeapFile<T>(initFilePath, filePath, blockSize);
        HeapFile.Clear();

        Addresses = new ExtendibleHashFileBlock<T>[(int)Math.Pow(2, Depth)];
        var initialAddr0 = HeapFile.CreateNewBlock();
        var initialBlock0 = new ExtendibleHashFileBlock<T>(initialAddr0, HeapFile);
        Addresses[0] = initialBlock0;
        var initialAddr1 = HeapFile.CreateNewBlock();
        var initialBlock1 = new ExtendibleHashFileBlock<T>(initialAddr1, HeapFile);
        Addresses[1] = initialBlock1;
        
    }
    #endregion // Constructors
    
    #region Public methods
    /// <summary>
    /// Vloží dáta do súboru.
    /// </summary>
    /// <param name="data">Dáta</param>
    /// <returns>Adresa dát, kde sú uložené.</returns>
    public int Insert(T data)
    {
        bool notInserted = true;
        int ret = -1;

        while (notInserted)
        {
            int hash = data.GetHash();
            int prefix = GetPrefix(hash);
            var block = Addresses[prefix].Block;
            var address = Addresses[prefix].Address;
            var blockDepth = Addresses[prefix].Depth;
            
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
                HeapFile.WriteBlock(block, address);
                RecordsCount++;
                ret = address;
                notInserted = false;
            }
        }

        return ret;
    }

    /// <summary>
    /// Nájde zadané dáta v súbore.
    /// </summary>
    /// <param name="data">Dáta</param>
    /// <returns>Dáta</returns>
    public T Find(T data)
    {
        return GetBlock(data).GetRecord(data);
    }
    
    /// <summary>
    /// Vymaže dané dáta zo súboru.
    /// </summary>
    /// <param name="data">Dáta</param>
    /// <returns>True, ak sa operácia podarila, False inak</returns>
    public bool Delete(T data)
    {
        var block = GetBlock(data);
        bool ret = block.RemoveRecord(data);
        RecordsCount--;
        
        // TODO zlucenia 
        
        return ret;
    }
    #endregion // Public methods
    
    #region Private methods
    private int GetPrefix(int hash)
    {
        return hash & ((1 << Depth) - 1);
    }

    private int GetPrefix(int hash, int depth)
    {
        return hash & ((1 << depth) - 1);
    }

    private Block<T> GetBlock(T data)
    {
        int hash = data.GetHash();
        int prefix = GetPrefix(hash);
        return Addresses[prefix].Block;
    }
    
    private void SplitBlock(int splittingBlockIndex)
    {
        var splittingIndex = splittingBlockIndex % (int)Math.Pow(2, Addresses[splittingBlockIndex].Depth);
        var splittingBlock = Addresses[splittingBlockIndex];
        
        var newBlockAddress = HeapFile.CreateNewBlock();
        var newBlock = Addresses[splittingIndex + (int)Math.Pow(2, splittingBlock.Depth)];
        newBlock.Address = newBlockAddress;

        var splittingBlockItems = new List<T>();
        var newBlockItems = new List<T>();
        for (int i = 0; i < splittingBlock.Block.ValidCount; i++)
        {
            var record = splittingBlock.Block.Records[i];
            int hash = record.GetHash();
            int newPrefix = GetPrefix(hash, splittingBlock.Depth + 1);

            if (newPrefix == splittingIndex) // ostava v rovnakom bloku
            {
                splittingBlockItems.Add(record);
            }
            else // presuva sa do noveho bloku
            {
                newBlockItems.Add(record);
            }
        }
        
        var blockSplitting = new Block<T>(splittingBlock.Block);
        foreach (var record in splittingBlockItems)
        {
            blockSplitting.AddRecord(record);
        }
        var blockNew = new Block<T>(newBlock.Block);
        foreach (var record in newBlockItems)
        {
            blockNew.AddRecord(record);
        }
        
        newBlock.Depth = splittingBlock.Depth + 1;
        splittingBlock.Depth += 1;
        
        HeapFile.WriteBlock(blockSplitting, splittingBlock.Address);
        HeapFile.WriteBlock(blockNew, newBlock.Address);
    }

    private void IncreaseDepth()
    {
        Depth++;
        var newAddresses = new ExtendibleHashFileBlock<T>[1 << Depth];

        for (int i = 0; i < Addresses.Length; i++)
        {
            newAddresses[i] = new ExtendibleHashFileBlock<T>(Addresses[i]);
            newAddresses[i + (1 << (Depth - 1))] = new ExtendibleHashFileBlock<T>(Addresses[i]);
        }

        Addresses = newAddresses;
    }
    #endregion // Private methods
}