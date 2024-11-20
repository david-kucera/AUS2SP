using FilesLib;
using FilesLib.Data;

namespace FilesTest
{
    public class Program
    {
        public static int BLOCK_SIZE = 4096;
        public static string INIT_FILE = "../../userdata/person_init.aus";
        public static string DATA_FILE = "../../userdata/person.aus";
        public static int NUMBER_OF_PEOPLE = 1;
        static void Main()
        {
            Console.WriteLine("Test heap file");
            if (File.Exists(DATA_FILE)) File.Delete(DATA_FILE);
            if (File.Exists(INIT_FILE)) File.Delete(INIT_FILE);
            
            HeapFile<Person> heapFile = new(INIT_FILE, DATA_FILE, BLOCK_SIZE);
            DataGenerator generator = new DataGenerator(0);
            List<int> adresses = new(NUMBER_OF_PEOPLE);
            List<Person> people = new(NUMBER_OF_PEOPLE);
            for (int i = 0; i < NUMBER_OF_PEOPLE; i++)
            {
                Person person = generator.GeneratePerson();
                var adresa = heapFile.Insert(person);
                Console.WriteLine("Inserted person");
                people.Add(person);
                adresses.Add(adresa);
            }

            for (int i = 0; i < NUMBER_OF_PEOPLE; i++)
            {
                var pers = heapFile.Find(adresses[i], people[i]);
                Console.WriteLine("Get person");
                Console.WriteLine($"Found person: {pers.ToString()}");
            }
            
            var allBlocks = heapFile.GetBlocks();
            Console.WriteLine("All Blocks");
            foreach (var b in allBlocks)
            {
                Console.WriteLine(b);
            }
            
            heapFile.Close();
            Console.WriteLine("End work with heap file");
        }
    }
}
