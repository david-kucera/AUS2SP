using FilesLib;
using FilesLib.Data;

namespace FilesTest
{
    public class Program
    {
        public static int BLOCK_SIZE = 1024 * 4;
        public static string INIT_FILE = "../../userdata/person_init.aus";
        public static string DATA_FILE = "../../userdata/person.aus";
        public static int NUMBER_OF_PEOPLE = 0;
        public static int NUMBER_OF_REPLICATIONS = 10;
        static void Main()
        {
            int numberOperations = 100_000;
            for (int r = 0; r < NUMBER_OF_REPLICATIONS; r++)
            {
                if (File.Exists(DATA_FILE)) File.Delete(DATA_FILE);
                if (File.Exists(INIT_FILE)) File.Delete(INIT_FILE);

                HeapFile<TestRP1> heapFile = new(INIT_FILE, DATA_FILE, BLOCK_SIZE);
                DataGenerator generator = new DataGenerator(r);
                List<int> adresses = new();
                List<TestRP1> people = new();
                
                for (int x = 0; x < NUMBER_OF_PEOPLE; x++)
                {
                    TestRP1 person = generator.GenerateTestRP1();
                    var adresa = heapFile.Insert(person);
                    people.Add(new TestRP1(person));
                    adresses.Add(adresa);
                }

                // Operations
                for (int i = 0; i < numberOperations; i++)
                {
                    var operation = generator.GenerateOperation();
                    Console.WriteLine(i + ". " + operation);
                    switch (operation)
                    {
                        case OperationType.Insert:
                            var newPerson = generator.GenerateTestRP1();
                            var adresa = heapFile.Insert(newPerson);
                            people.Add(new TestRP1(newPerson));
                            adresses.Add(adresa);
                            break;
                        case OperationType.Delete:
                            if (adresses.Count == 0) break;
                            var index = generator.GenerateInt(0, adresses.Count);
                            var person = people[index];
                            var adress = adresses[index];
                            people.RemoveAt(index);
                            adresses.RemoveAt(index);
                            heapFile.Delete(adress, person);
                            break;
                        case OperationType.Find:
                            if (adresses.Count == 0) break;
                            var iindex = generator.GenerateInt(0, adresses.Count);
                            var pperson = people[iindex];
                            var aadress = adresses[iindex];
                            var foundPerson = heapFile.Find(aadress, pperson);
                            if (foundPerson == null)
                            {
                                throw new Exception("Person not found!");
                            }
                            break;
                        default:    
                            break;
                    }
                }
            
                heapFile.Close();
            }
        }
    }
}
