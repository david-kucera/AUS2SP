using FilesLib.Data;

namespace FilesLib.Generator;

public class DataGenerator
{
    #region Constants
    private const int MAX_COUNT_VISIT = 10;
    private const int MAX_LENGTH_VISIT = 20;
    private int NEXT_ID = 0;
    #endregion // Constants
    
    #region Class members   
    private Random _rand;
    #endregion // Class members
    
    #region Constructors
    public DataGenerator(int seed)
    {
        _rand = new Random(seed);
    }
    #endregion // Constructors
    
    #region Public methods
    public Person GeneratePerson()
    {
        Person ret = new()
        {
            Id = NEXT_ID++,
            Name = GenerateString(_rand.Next(1,15)),
            Surname = GenerateString(_rand.Next(1,20)),
            Ecv = GenerateString(_rand.Next(1,10)), // TODO unikatne generovat ECV
            Zaznamy = GenerateVisits(_rand.Next(1,5)),
        };
        return ret;
    }
    
    public TestRP1 GenerateTestRP1()
    {
        TestRP1 ret = new()
        {
            Id = NEXT_ID++,
            Name = GenerateString(_rand.Next(1,15)),
            Surname = GenerateString(_rand.Next(1,20)),
            Ecv = GenerateString(_rand.Next(1,10)), // TODO treba unikatne generovat ECV
        };
        return ret;
    }

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
    
    public int GenerateInt(int min, int max)
    {
        return _rand.Next(min, max);
    }
    
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
    #endregion // Private methods
}