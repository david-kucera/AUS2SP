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
    public ExtendibleHashFile(string initFilePath, HeapFile<T> heapFile)
    {
        HeapFile = heapFile;
        HeapFile.Clear();

        Addresses = [];
        var initialAddr0 = HeapFile.CreateNewBlock();
        var initialBlock0 = new ExtendibleHashFileBlock<T>(initialAddr0, HeapFile);
        var initialAddr1 = HeapFile.CreateNewBlock();
        var initialBlock1 = new ExtendibleHashFileBlock<T>(initialAddr1, HeapFile);
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
    public void Insert(T data)
    {
        bool notInserted = true;
        var hash = data.GetHash();
        while (notInserted)
        {
            int prefix = GetPrefix(hash);
            var block = HeapFile.GetBlock(Addresses[prefix].Address);
            
            if (block.BlockFactor == block.ValidCount)
            {
                if (Addresses[prefix].Depth == Depth)
                {
                    IncreaseDepth();
                    SplitBlock(GetPrefix(hash));
                }
                else
                {
                    SplitBlock(prefix);
                }
            }
            else
            {
                block.AddRecord(data);
                HeapFile.WriteBlock(block, Addresses[prefix].Address);
                RecordsCount++;
                notInserted = false;
            }
        }
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

    /// <summary>
    /// Vypíše sekvenčne všetky dáta o adresách.
    /// </summary>
    /// <returns>Reťazec informácií o adresách.</returns>
    public string SequentialOutput()
    {
        string ret = string.Empty;
        ret += "Addresses:";
        ret += Environment.NewLine;
        int index = 0;
        foreach (var address in Addresses)
        {
            ret += index + ". address: ";
            ret += address.ToString();
            ret += Environment.NewLine;
            index++;
        }
        return ret;
    }

    /// <summary>
    /// Metóda potrebná pri skončení práce.
    /// </summary>
    public void Close()
    {
        HeapFile.Close();
        // TODO save metadata do suboru
    }
    #endregion // Public methods
    
    #region Private methods
    private int GetPrefix(BitArray hash)
    {
        var reversedHash = new BitArray(Depth);
        for (int i = 0; i < Depth; i++) reversedHash[reversedHash.Length - i - 1] = hash[i];
        return BitArrayToInt(reversedHash);
    }

    private int GetPrefix(BitArray hash, int depth)
    {
        var reversedHash = new BitArray(depth);
        for (int i = 0; i < depth; i++) reversedHash[reversedHash.Length - i - 1] = hash[i];
        return BitArrayToInt(reversedHash);
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
        var groupLength = (int)Math.Pow(2, Depth - Addresses[splittingIndex].Depth);
        var addressIndex = (splittingIndex/groupLength) * groupLength;
        var splittingBlock = Addresses[addressIndex];
        var localDepth = splittingBlock.Depth;
        splittingBlock.Depth++;
        
        var newAddressBlock = new ExtendibleHashFileBlock<T>(HeapFile.CreateNewBlock(), HeapFile)
        {
            Depth = splittingBlock.Depth
        };
        var halfLength = ((int)Math.Pow(2, Depth - localDepth))/2;
        var startIndex = addressIndex + halfLength;
        var endIndex = startIndex + halfLength;
    
        for (int i = startIndex; i < endIndex; i++) Addresses[i] = newAddressBlock;
        
        var splittBlock = HeapFile.GetBlock(splittingBlock.Address);
        var newBlock = HeapFile.GetBlock(newAddressBlock.Address);
        for (int i = 0; i < splittBlock.ValidCount; i++)
        {
            var record = splittBlock.GetRecord(i);
            var hash = record.GetHash();
            int newPrefix = GetPrefix(hash);
    
            if (newPrefix == splittingIndex)
            {
                splittBlock.RemoveRecord(record);
                newBlock.AddRecord(record);
            }
        }
        
        HeapFile.WriteBlock(splittBlock, splittingBlock.Address);
        HeapFile.WriteBlock(newBlock, newAddressBlock.Address);
    }

    private void IncreaseDepth()
    {
        Depth++;
        var newAddresses = new List<ExtendibleHashFileBlock<T>>();
        for (int i = 0; i < Addresses.Count; i++)
        {
            newAddresses.Add(new ExtendibleHashFileBlock<T>(Addresses[i]));
            newAddresses.Add(new ExtendibleHashFileBlock<T>(Addresses[i]));
        }
        Addresses = newAddresses;
    }
    #endregion // Private methods
}