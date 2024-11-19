using FilesLib.Data;

namespace FilesLib;

public class DataGenerator
{
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
            Id = _rand.Next(1000000),
            Name = GenerateString(_rand.Next(15)),
            Surname = GenerateString(_rand.Next(20)),
            Zaznamy = GenerateVisits(_rand.Next(5)),
        };
        return ret;
    }

    private string GenerateString(int length)
    {
        string ret = string.Empty;
        for (int i = 0; i < length; i++)
        {
            ret += (char)_rand.Next(65, 91);
        }
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
                Note = "note" + _rand.Next(0, 100)
            };
            ret.Add(visit);
        }

        return ret;
    }
    #endregion // Public methods
}