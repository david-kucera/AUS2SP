using FilesLib;
using FilesLib.Data;

namespace FilesTest
{
    public class Program
    {
        public static int BLOCK_SIZE = 512;
        static void Main()
        {
            Console.WriteLine("Test heap file");
            HeapFile<Person> heapFile = new("../userdata/person_init.aus", "../userdata/person.aus", BLOCK_SIZE);
            
            Random rnd = new(0);
            var zaznamy = new List<Visit>();
            for (int i = 0; i < 5; i++)
            {
                var visit = new Visit
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(rnd.Next(-20, 20))),
                    Price = rnd.Next(0, 100),
                    Note = "note" + rnd.Next(0, 100)
                };
                zaznamy.Add(visit);
            }
            Person person = new()
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Zaznamy = zaznamy
            };

            var adresa = heapFile.Insert(person);
            Console.WriteLine("Inserted person");
            Console.WriteLine("Get person");
            var p = heapFile.Find(adresa, new Person());
            Console.WriteLine($"Found person: {p}");
            var allBlocks = heapFile.GetBlocks();
            Console.WriteLine("All Blocks");
            foreach (var b in allBlocks)
            {
                Console.WriteLine(b);
            }

            Console.WriteLine("End work with heap file");
            heapFile.Close();
        }
    }
}
