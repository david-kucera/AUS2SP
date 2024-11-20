using FilesLib;
using FilesLib.Data;

namespace FilesTest
{
    public class Program
    {
        public static int BLOCK_SIZE = 1024;
        public static string INIT_FILE = "../../userdata/person_init.aus";
        public static string DATA_FILE = "../../userdata/person.aus";
        public static int NUMBER_OF_PEOPLE = 20;
        static void Main()
        {
            Console.WriteLine("Test heap file");
            if (File.Exists(DATA_FILE)) File.Delete(DATA_FILE);
            if (File.Exists(INIT_FILE)) File.Delete(INIT_FILE);
            
            HeapFile<TestRP1> heapFile = new(INIT_FILE, DATA_FILE, BLOCK_SIZE);
            DataGenerator generator = new DataGenerator(0);
            List<int> adresses = new(NUMBER_OF_PEOPLE);
            List<TestRP1> people = new(NUMBER_OF_PEOPLE);
            for (int i = 0; i < NUMBER_OF_PEOPLE; i++)
            {
                TestRP1 person = generator.GenerateTestRP1();
                var adresa = heapFile.Insert(person);
                Console.WriteLine($"{i}. Inserted person");
                people.Add(new TestRP1(person));
                adresses.Add(adresa);
            }

            Console.WriteLine("Number of blocks:");
            Console.WriteLine(heapFile.GetBlocks().Count);
            Console.WriteLine("------------------------");

            for (int i = 0; i < NUMBER_OF_PEOPLE; i++)
            {
                var pers = heapFile.Find(adresses[i], people[i]);
                Console.WriteLine("Get person");
                Console.WriteLine($"Found person: {pers}");
            }

            for (int i = NUMBER_OF_PEOPLE-1; i >= 0; i--)
            {
                heapFile.Delete(adresses[i], people[i]);
                Console.WriteLine($"{i}. Deleted person");
                Console.WriteLine(heapFile.GetBlocks().Count);
            }
            
            var allBlocks = heapFile.GetBlocks();
            Console.WriteLine("------------------------");
            Console.WriteLine("All Blocks");
            int poradie = 1;
            foreach (var b in allBlocks)
            {
                Console.WriteLine("------------------------");
                Console.WriteLine(poradie + ". block\n" + b);
                Console.WriteLine("------------------------");
                poradie++;
            }
            
            heapFile.Close();
            Console.WriteLine("End work with heap file");
        }
    }
}
