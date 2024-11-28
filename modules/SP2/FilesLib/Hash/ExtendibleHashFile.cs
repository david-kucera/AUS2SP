using System.Collections;
using FilesLib.Interfaces;

namespace FilesLib.Hash;

public class ExtendibleHashFile<T> where T : class, IHashable<T>, new()
{
    #region Properties
    public List<ExtendibleHashFileBlock<T>> Addresses { get; set; }
    public int Depth { get; set; } = 1;
    public int RecordsCount { get; set; } = 0;
    public int BlockFactor { get; set; }
    #endregion // Properties

    #region Constructors
    public ExtendibleHashFile(string initFilePath, int blockFactor)
    {
        Addresses = [];
        Addresses.Add(new ExtendibleHashFileBlock<T>());
        Addresses.Add(new ExtendibleHashFileBlock<T>());
        BlockFactor = blockFactor;
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
            var block = GetBlock(data);

            if (BlockFactor == block.Values.Count)
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
                Addresses[prefix].Values.Add(data);
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
        var addressBlock = GetBlock(data);
        foreach (var record in addressBlock.Values)
        {
            if (record.Equals(data)) return (T)record;
        }
        return null!;
    }
    
    /// <summary>
    /// Vymaže dané dáta zo súboru.
    /// </summary>
    /// <param name="data">Dáta</param>
    /// <returns>True, ak sa operácia podarila, False inak</returns>
    //public void Delete(T data)
    //{
    //    var hash = data.GetHash();
    //    var prefix = GetPrefix(hash);
    //    var block = Addresses[prefix];
    //    var hBlock = HeapFile.GetBlock(block.Address);
    //    var entry = hBlock.GetRecord(data);
 
    //    if (entry != null!)
    //    {
    //        HeapFile.Delete(block.Address, entry);
    //        Addresses[prefix].ValidCount--;
    //        RecordsCount--;
    //        if (hBlock.ValidCount < hBlock.BlockFactor / 2 && block.Depth > 1)
    //        {
    //            MergeBlock(prefix);
    //        }
    //    }
    //}

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

    private ExtendibleHashFileBlock<T> GetBlock(T data)
    {
        var hash = data.GetHash();
        int prefix = GetPrefix(hash);
        return Addresses[prefix];
    }
    
    private void SplitBlock(int splittingIndex)
    {
        var groupLength = (int)Math.Pow(2, Depth - Addresses[splittingIndex].Depth);
        var addressIndex = (splittingIndex/groupLength) * groupLength;
        var splittingBlock = Addresses[addressIndex];
        var localDepth = splittingBlock.Depth;
        splittingBlock.Depth++;

        var newAddressBlock = new ExtendibleHashFileBlock<T>()
        {
            Depth = splittingBlock.Depth
        };
        var halfLength = ((int)Math.Pow(2, Depth - localDepth))/2;
        var startIndex = addressIndex + halfLength;
        var endIndex = startIndex + halfLength;
 
        for (int i = startIndex; i < endIndex; i++) Addresses[i] = newAddressBlock;
        
        var oldBlockItems = new List<IHashable<T>>();
        var newBlockItems = new List<IHashable<T>>();
        for (int i = 0; i < splittingBlock.Values.Count; i++)
        {
            var record = splittingBlock.Values[i];
            var hash = record.GetHash();
            int newPrefix = GetPrefix(hash);
            
            if (newPrefix != addressIndex) newBlockItems.Add(record);
            else oldBlockItems.Add(record);
        }
        splittingBlock.Values = oldBlockItems;
        newAddressBlock.Values = newBlockItems;
    }

    private void IncreaseDepth()
    {
        Depth++;
        List<ExtendibleHashFileBlock<T>> newAddresses = [];
        foreach (var address in Addresses)
        {
            newAddresses.Add(new ExtendibleHashFileBlock<T>(address));
            newAddresses.Add(new ExtendibleHashFileBlock<T>(address));
        }
        Addresses = newAddresses;
    }

    //private void DecreaseDepth()
    //{
    //    Depth--;
    //    int size = Addresses.Count;
    //    var newAdresses = new List<ExtendibleHashFileBlock<T>>(size / 2);
    //    for (int i = 0; i < size; i += 2)
    //    {
    //        newAdresses.Add(Addresses[i]);
    //    }
    //    Addresses = newAdresses;
    //}
    
    //private void MergeBlock(int blockIndex)
    //    {
    //        var length = (int)Math.Pow(2, Depth - Addresses[blockIndex].Depth);
    //        var actualMergeIndex = (blockIndex / length) * length;
    //        var block = Addresses[actualMergeIndex];
    //        var hBlock = HeapFile.GetBlock(block.Address);
 
    //        var neighborIndex = actualMergeIndex + length;
    //        var neighborLength = 0;
    //        if (actualMergeIndex < Addresses.Count/2)
    //        {
    //            if (neighborIndex >= Addresses.Count/2)
    //            {
    //                neighborIndex = actualMergeIndex - 1;
    //                neighborLength = (int)Math.Pow(2, Depth - Addresses[neighborIndex].Depth);
    //                neighborIndex = (neighborIndex / neighborLength) * neighborLength;
    //            }
    //            else
    //            {
    //                neighborIndex = actualMergeIndex + length;
    //                neighborLength = (int)Math.Pow(2, Depth - Addresses[neighborIndex].Depth);
    //                neighborIndex = (neighborIndex / neighborLength) * neighborLength;
    //            }
    //        }
    //        else
    //        {
    //            if (neighborIndex >= Addresses.Count)
    //            {
    //                neighborIndex = actualMergeIndex - 1;
    //                neighborLength = (int)Math.Pow(2, Depth - Addresses[neighborIndex].Depth);
    //                neighborIndex = (neighborIndex / neighborLength) * neighborLength;
    //            }
    //            else
    //            {
    //                neighborIndex = actualMergeIndex + length;
    //                neighborLength = (int)Math.Pow(2, Depth - Addresses[neighborIndex].Depth);
    //                neighborIndex = (neighborIndex / neighborLength) * neighborLength;
    //            }
    //        }
 
    //        var neighbor = Addresses[neighborIndex];
    //        var neighborBlock = HeapFile.GetBlock(neighbor.Address);
    //        if (neighbor.Depth == block.Depth && neighborBlock.ValidCount + hBlock.ValidCount <= hBlock.BlockFactor)
    //        {
    //            var entries = new List<T>(hBlock.ValidCount);
    //            List<T> validRecords = new List<T>();
    //            for (int i = 0; i < hBlock.ValidCount; i++)
    //            {
    //                validRecords.Add(hBlock.GetRecord(i));
    //            }
    //            entries.AddRange(validRecords);
    //            HeapFile.Update(block.Address, entries);
    //            HeapFile.Update(neighbor.Address, new List<T>());
    //            block.Depth--;
 
    //            var endIndex = neighborIndex + neighborLength;
 
    //            for (int i = neighborIndex; i < endIndex; i++)
    //            {
    //                Addresses[i] = block;
    //            }
 
    //            var maxDepth = int.MinValue;
    //            foreach (var address in Addresses)
    //            {
    //                if (maxDepth < address.Depth)
    //                {
    //                    maxDepth = address.Depth;
    //                }
    //            }
    //            if (maxDepth < Depth)
    //            {
    //                DecreaseDepth();
    //            }
    //        }
    //    }
    
    #endregion // Private methods
}