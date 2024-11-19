using FilesLib;
using FilesLib.Data;

namespace FilesTest
{
    public class Program
    {
        public static int BLOCK_SIZE = 512;
        public static string INIT_FILE = "../../userdata/person_init.aus";
        public static string DATA_FILE = "../../userdata/person.aus";
        static void Main()
        {
            Console.WriteLine("Test heap file");
            if (File.Exists(DATA_FILE)) File.Delete(DATA_FILE);
            if (File.Exists(INIT_FILE)) File.Delete(INIT_FILE);
            
            HeapFile<Person> heapFile = new(INIT_FILE, DATA_FILE, BLOCK_SIZE);
            DataGenerator generator = new DataGenerator(0);
            Person person = generator.GeneratePerson();

            var adresa = heapFile.Insert(person);
            Console.WriteLine("Inserted person");
            Console.WriteLine("Get person");
            var pers = heapFile.Find(adresa, person);
            Console.WriteLine($"Found person: {pers.ToString()}");
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
