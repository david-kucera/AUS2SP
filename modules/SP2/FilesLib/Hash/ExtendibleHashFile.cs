using System.Collections;
using FilesLib.Heap;
using FilesLib.Interfaces;

namespace FilesLib.Hash;

public class ExtendibleHashFile<T> where T : class, IHashable<T>, new()
{
    #region Properties
    public HeapFile<T> HeapFile { get; set; }
    public List<ExtendibleHashFileBlock<T>> Addresses { get; set; }
    public int Depth { get; set; } = 1;
    public int RecordsCount { get; set; } = 0;
    #endregion // Properties
    
    #region Constructors
    public ExtendibleHashFile(string initFilePath, string filePath, int blockSize)
    {
        HeapFile = new HeapFile<T>(initFilePath, filePath, blockSize);
        HeapFile.Clear();

        Addresses = [];
        var initialAddr0 = HeapFile.CreateNewBlock();
        var initialBlock0 = new ExtendibleHashFileBlock<T>(initialAddr0);
        var initialAddr1 = HeapFile.CreateNewBlock();
        var initialBlock1 = new ExtendibleHashFileBlock<T>(initialAddr1);
        Addresses.Add(initialBlock0);
        Addresses.Add(initialBlock1);
        
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
        var hash = data.GetHash();
        while (notInserted)
        {
            int prefix = GetPrefix(hash);
            var block = HeapFile.GetBlock(Addresses[prefix].Address);
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
        
        // TODO 
        
        return ret;
    }
    #endregion // Public methods
    
    #region Private methods
    private int GetPrefix(BitArray hash)
    {
        var hashInt = BitArrayToInt(hash);
        return hashInt & ((1 << Depth) - 1);
    }

    private int GetPrefix(BitArray hash, int depth)
    {
        var hashInt = BitArrayToInt(hash);
        return hashInt & ((1 << depth) - 1);
    }
    
    private int BitArrayToInt(BitArray bitArray)
    {
        if (bitArray.Length > 32) throw new ArgumentException("BitArray must be less than 32 bits!");

        int ret = 0;
        for (int i = 0; i < bitArray.Length; i++)
        {
            if (bitArray[i]) ret |= (1 << i);
        }

        return ret;
    }

    private Block<T> GetBlock(T data)
    {
        var hash = data.GetHash();
        int prefix = GetPrefix(hash);
        var block = HeapFile.GetBlock(Addresses[prefix].Address);
        return block;
    }
    
    private void SplitBlock(int splittingIndex)
    {
        // Najdem si blok, ktory idem delit
        var splittingBlockIndex = splittingIndex % (int)Math.Pow(2, Addresses[splittingIndex].Depth);
        var splittingBlock = Addresses[splittingIndex];
        
        // Najdem k nemu novy blok, do ktoreho budem ukladat po prehashovani nove data
        var newBlockAddress = HeapFile.CreateNewBlock();
        var newBlock = Addresses[splittingBlockIndex + (int)Math.Pow(2, splittingBlock.Depth)];
        newBlock.Address = newBlockAddress;
        
        // Vynulujem valid count pre adresy
        newBlock.ValidCount = 0;
        splittingBlock.ValidCount = 0;

        // Rozdelim data po prehashovani do deleneho a noveho blocku
        var splittBlock = HeapFile.GetBlock(splittingBlock.Address);
        var newSplitBlock = new Block<T>(splittBlock);
        var newBlockSplit = new Block<T>(HeapFile.GetBlock(newBlockAddress));
        for (int i = 0; i < splittBlock.ValidCount; i++)
        {
            var record = splittBlock.Records[i];
            var hash = record.GetHash();
            int newPrefix = GetPrefix(hash, splittingBlock.Depth + 1);

            if (newPrefix == splittingBlockIndex)
            {
                newSplitBlock.AddRecord(record);
                splittingBlock.ValidCount++;
            }
            else
            {
                newBlockSplit.AddRecord(record);
                newBlock.ValidCount++;
            }
        }
        
        // Zvysim hlbku novemu a delenemu blocku
        newBlock.Depth = splittingBlock.Depth + 1;
        splittingBlock.Depth += 1;
        
        // Nakoniec data zapisem do suboru
        HeapFile.WriteBlock(newSplitBlock, splittingBlock.Address);
        HeapFile.WriteBlock(newBlockSplit, newBlock.Address);
    }

    private void IncreaseDepth()
    {
        Depth++;
        var size = Addresses.Count;
        for (int i = 0; i < size; i++)
        {
            Addresses.Add(new ExtendibleHashFileBlock<T>(Addresses[i].Address));
        }
    }
    #endregion // Private methods
}