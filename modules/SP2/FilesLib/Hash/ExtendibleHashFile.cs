using System.Collections;
using FilesLib.Heap;
using FilesLib.Interfaces;

namespace FilesLib.Hash;

/// <summary>
/// Class of Extendible hashing implementation.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ExtendibleHashFile<T> where T : class, IHashable<T>, new()
{
	#region Class members
	private int _depth = 1;
	private List<ExtendibleHashFileBlock<T>> _addresses = [];
    private HeapFile<T> _heapFile = null!;
    private readonly string _initFilePath = string.Empty;
	#endregion // Class members

	#region Properties
	/// <summary>
	/// Number of records stored in hash file.
	/// </summary>
	public int RecordsCount { get; set; } = 0;
    #endregion // Properties

    #region Constructors
    public ExtendibleHashFile(string initFilePath, string initHeapFilePath, string dataHeapFilePath, int blockSize)
    {
	    if (!File.Exists(initFilePath)) File.Create(initFilePath).Close();
        if (!File.Exists(initHeapFilePath)) File.Create(initHeapFilePath).Close();
        if (!File.Exists(dataHeapFilePath)) File.Create(dataHeapFilePath).Close();

        _initFilePath = initFilePath;
        _heapFile = new HeapFile<T>(initHeapFilePath, dataHeapFilePath, blockSize);
        if (File.ReadAllBytes(initFilePath).Length > 0)
		{
            LoadFromInit();
            return;
		}

        _addresses = [];
        _addresses.Add(new ExtendibleHashFileBlock<T>(_heapFile.GetEmptyBlock()));
        _addresses.Add(new ExtendibleHashFileBlock<T>(_heapFile.GetEmptyBlock()));
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
            var heapFileBlock = _heapFile.GetBlock(block.Address);

            if (heapFileBlock.BlockFactor == heapFileBlock.ValidCount)
            {
                if (_addresses[prefix].Depth == _depth)
                {
                    IncreaseDepth();
                    SplitBlock(GetPrefix(hash));
                }
                else SplitBlock(prefix);
            }
            else
            {
                heapFileBlock.AddRecord(data);
                _heapFile.WriteBlock(heapFileBlock, block.Address);
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
        var heapFileBlock = _heapFile.GetBlock(addressBlock.Address);
        foreach (var record in heapFileBlock.Records)
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
    public void Delete(T data)
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// Vráti reťazec informácií o súbore.
    /// </summary>
    /// <returns>String</returns>
    public override string ToString()
    {
	    return $"Depth: {_depth}, RecordsCount: {RecordsCount}";
    }

    /// <summary>
    /// Vypíše sekvenčne všetky dáta o adresách.
    /// </summary>
    /// <returns>Reťazec informácií o adresách.</returns>
    public string SequentialOutput()
    {
        string ret = "Addresses:" + Environment.NewLine;
        int index = 0;
        foreach (var address in _addresses)
        {
            ret += index + ". address: ";
            ret += address.ToString();
            ret += Environment.NewLine;
            index++;
        }
        ret += Environment.NewLine;
        ret += _heapFile.SequentialOutput();
        return ret;
    }

    /// <summary>
    /// Metóda potrebná pri skončení práce.
    /// </summary>
    public void Close()
    {
        SaveToInit();
        _heapFile.Close();
    }

    /// <summary>
	/// Vyčistí súbor.
	/// </summary>
	public void Clear()
    {
        _heapFile.Clear();
	    _addresses = [];
	    _addresses.Add(new ExtendibleHashFileBlock<T>());
	    _addresses.Add(new ExtendibleHashFileBlock<T>());
	    _depth = 1;
	    RecordsCount = 0;
    }
	#endregion // Public methods

	#region Private methods
	private void LoadFromInit()
	{
		long fileSize = new FileInfo(_initFilePath).Length;
		byte[] buffer = new byte[fileSize];

		using (var initFile = new FileStream(_initFilePath, FileMode.Open, FileAccess.Read))
		{
			initFile.Read(buffer, 0, buffer.Length);
            initFile.Close();
        }

		FromByteArray(buffer);
	}

	private void SaveToInit()
	{
		var size = GetSize();
		byte[] buffer = ToByteArray(size);

		using var initFile = new FileStream(_initFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		initFile.SetLength(0);
		initFile.Seek(0, SeekOrigin.Begin);
		initFile.Write(buffer, 0, size);
		initFile.Flush();
		initFile.Close();
	}

	private ExtendibleHashFile<T> FromByteArray(byte[] data)
	{
        int offset = 0;

		_depth = BitConverter.ToInt32(data, offset);
		offset += sizeof(int);

        RecordsCount = BitConverter.ToInt32(data, offset);
        offset += sizeof(int);

        int addressesCount = BitConverter.ToInt32(data, offset);
		offset += sizeof(int);

        _addresses.Clear();

		for (int i = 0; i < addressesCount; i++)
		{
			ExtendibleHashFileBlock<T> address = new();
            var addressSize = address.GetSize();

            byte[] addressData = data[offset..(offset + addressSize)];
			address.FromByteArray(addressData);
			_addresses.Add(address);
			offset += address.GetSize();
		}

		return this;
	}

	private byte[] ToByteArray(int size)
	{
		byte[] bytes = new byte[size];
		int offset = 0;

		BitConverter.GetBytes(_depth).CopyTo(bytes, offset);
		offset += sizeof(int);

        BitConverter.GetBytes(RecordsCount).CopyTo(bytes, offset);
        offset += sizeof(int);

        BitConverter.GetBytes(_addresses.Count).CopyTo(bytes, offset);
		offset += sizeof(int);

		foreach (var address in _addresses)
		{
			address.ToByteArray().CopyTo(bytes, offset);
			offset += address.GetSize();
		}

		return bytes;
	}

	private int GetSize()
	{
		return sizeof(int) + sizeof(int) + sizeof(int) + _addresses.Sum(address => address.GetSize());
	}

	private int GetPrefix(BitArray hash)
    {
        if (hash.Length > 32) throw new ArgumentException("BitArray must be less than 32 bits!");

		var reversedHash = new BitArray(_depth);
        for (int i = 0; i < _depth; i++) reversedHash[reversedHash.Length - i - 1] = hash[i];

        return BitArrayToInt(reversedHash);
    }
    
    private static int BitArrayToInt(BitArray bitArray)
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
        return _addresses[prefix];
    }
    
    private void SplitBlock(int splittingIndex)
    {
        var groupLength = (int)Math.Pow(2, _depth - _addresses[splittingIndex].Depth);
        var addressIndex = (splittingIndex/groupLength) * groupLength;
        var splittingBlock = _addresses[addressIndex];
        var splittingBlockHeapFile = _heapFile.GetBlock(splittingBlock.Address);
        var localDepth = splittingBlock.Depth;
        splittingBlock.Depth++;

        var newAddressBlock = new ExtendibleHashFileBlock<T>()
        {
            Depth = splittingBlock.Depth,
            Address = _heapFile.GetEmptyBlock()
        };
        var newBlockHeapFile = _heapFile.GetBlock(newAddressBlock.Address);

        var halfLength = ((int)Math.Pow(2, _depth - localDepth))/2;
        var startIndex = addressIndex + halfLength;
        var endIndex = startIndex + halfLength;
 
        for (int i = startIndex; i < endIndex; i++) _addresses[i] = newAddressBlock;
        
        var recordsToRehash = new List<T>(splittingBlockHeapFile.Records);
        splittingBlockHeapFile.ClearRecords();
        newBlockHeapFile.ClearRecords();
        foreach (var record in recordsToRehash)
        {
	        var hash = record.GetHash();
	        int newPrefix = GetPrefix(hash);

            if (newPrefix != addressIndex) newBlockHeapFile.AddRecord(record);
            else splittingBlockHeapFile.AddRecord(record);
        }

        _heapFile.WriteBlock(splittingBlockHeapFile, splittingBlock.Address);
        _heapFile.WriteBlock(newBlockHeapFile, newAddressBlock.Address);
    }

    private void IncreaseDepth()
    {
        _depth++;
        List<ExtendibleHashFileBlock<T>> newAddresses = [];
        foreach (var address in _addresses)
        {
            newAddresses.Add(new ExtendibleHashFileBlock<T>(address));
            newAddresses.Add(new ExtendibleHashFileBlock<T>(address));
        }
        _addresses = newAddresses;
    }
    #endregion // Private methods
}