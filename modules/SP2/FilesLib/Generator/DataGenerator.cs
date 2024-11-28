using FilesLib.Data;

namespace FilesLib.Generator;

/// <summary>
/// Generator of random operations and random data.
/// </summary>
public class DataGenerator
{
    #region Constants
    private const int MAX_COUNT_VISIT = 10;
    private const int MAX_LENGTH_VISIT = 20;
    private int NEXT_ID = 0; // Ensures unique ID and ECV
    #endregion // Constants
    
    #region Class members   
    private readonly Random _rand;
    #endregion // Class members
    
    #region Constructors
    public DataGenerator(int seed)
    {
        _rand = new Random(seed);
    }
    #endregion // Constructors
    
    #region Public methods
    /// <summary>
    /// Generates Person with random values.
    /// </summary>
    /// <returns></returns>
    public Person GeneratePerson()
    {
        int i = NEXT_ID++;
        Person ret = new()
        {
            Id = i,
            Name = GenerateString(_rand.Next(1,15)),
            Surname = GenerateString(_rand.Next(1,20)),
            Ecv = GenerateUniqueString(i),
            Visits = GenerateVisits(_rand.Next(1,5)),
        };
        return ret;
    }
    
    /// <summary>
    /// Generates TestRP1 with random values.
    /// </summary>
    /// <returns></returns>
    public TestRP1 GenerateTestRP1()
    {
        int i = NEXT_ID++;
        TestRP1 ret = new()
        {
            Id = i,
            Name = GenerateString(_rand.Next(1,15)),
            Surname = GenerateString(_rand.Next(1,20)),
            Ecv = GenerateUniqueString(i)
        };
        return ret;
    }

    /// <summary>
    /// Generates list of Visits with random values.
    /// </summary>
    /// <param name="count">Number of visits.</param>
    /// <returns></returns>
    public List<Visit> GenerateVisits(int count)
    {
        var ret = new List<Visit>();
        for (int i = 0; i < count; i++)
        {
            var visit = new Visit
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(_rand.Next(-20, 20))),
                Price = _rand.Next(0, 100),
                Notes = GenerateNotes(_rand.Next(1,MAX_COUNT_VISIT))
            };
            ret.Add(visit);
        }

        return ret;
    }

    /// <summary>
    /// Generates list of random notes.
    /// </summary>
    /// <param name="count">Count of notes.</param>
    /// <returns></returns>
    private List<string> GenerateNotes(int count)
    {
        List<string> ret = new List<string>();
        
        for (int i = 0; i < count; i++)
        {
            string noteString = GenerateString(_rand.Next(1,MAX_LENGTH_VISIT));
            ret.Add(noteString);
        }

        return ret;
    }
    
    /// <summary>
    /// Generates random integer in given interval.
    /// </summary>
    /// <param name="min">Minimum</param>
    /// <param name="max">Maximum</param>
    /// <returns></returns>
    public int GenerateInt(int min, int max)
    {
        return _rand.Next(min, max);
    }
    
    /// <summary>
    /// Generates random operation type.
    /// </summary>
    /// <returns></returns>
    public OperationType GenerateOperation()
    {
        var ret = (OperationType)_rand.Next(0, 3);
        return ret;
    }
    #endregion // Public methods
    
    #region Private methods
    private string GenerateString(int length)
    {
        string ret = string.Empty;
        for (int i = 0; i < length; i++)
        {
            ret += (char)_rand.Next(65, 91);
        }
        return ret;
    }

    private string GenerateUniqueString(int index)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        int baseValue = chars.Length;
        string result = string.Empty;

        do
        {
            result = chars[index % baseValue] + result;
            index = (index / baseValue) - 1;
        } while (index >= 0);

        return result;
    }
    #endregion // Private methods
}