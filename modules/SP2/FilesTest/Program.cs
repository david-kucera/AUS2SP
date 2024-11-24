using FilesLib.Data;
using FilesLib.Generator;
using FilesLib.Hash;
using FilesLib.Heap;

namespace FilesTest
{
    public class Program
    {
        public static int BLOCK_SIZE = 1024;
        public static string INIT_FILE = "../../userdata/person_init.aus";
        public static string DATA_FILE = "../../userdata/person.aus";
        public static int NUMBER_OF_PEOPLE = 0;
        public static int NUMBER_OF_OPERATIONS = 1_000_000;
        public static int NUMBER_OF_REPLICATIONS = 10;

        static void Main()
        {
            //TestHeapFile();
            TestHashFile();
        }
        
        static void TestHeapFile()
        {
            for (int r = 0; r < NUMBER_OF_REPLICATIONS; r++)
            {
                if (File.Exists(DATA_FILE)) File.Delete(DATA_FILE);
                if (File.Exists(INIT_FILE)) File.Delete(INIT_FILE);
                Console.WriteLine($"{r}. replication");

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
                for (int i = 0; i < NUMBER_OF_OPERATIONS; i++)
                {
                    var operation = generator.GenerateOperation();
                    Console.WriteLine(i + ". " + operation);
                    switch (operation)
                    {
                        case OperationType.Insert:
                            var countBeforeInsert = heapFile.RecordsCount;
                            var newPerson = generator.GenerateTestRP1();
                            var adresa = heapFile.Insert(newPerson);
                            var countAfterInsert = heapFile.RecordsCount;
                            if (countBeforeInsert+1 != countAfterInsert) throw new Exception("Insert failed!");
                            people.Add(new TestRP1(newPerson));
                            adresses.Add(adresa);
                            break;
                        case OperationType.Delete:
                            var countBeforeDelete = heapFile.RecordsCount;
                            if (adresses.Count == 0) break;
                            var index = generator.GenerateInt(0, adresses.Count);
                            var person = people[index];
                            var adress = adresses[index];
                            heapFile.Delete(adress, person);
                            people.RemoveAt(index);
                            adresses.RemoveAt(index);
                            var countAfterDelete = heapFile.RecordsCount;
                            if (countAfterDelete+1 != countBeforeDelete) throw new Exception("Delete failed!");
                            break;
                        case OperationType.Find:
                            if (adresses.Count == 0) break;
                            var iindex = generator.GenerateInt(0, adresses.Count);
                            var pperson = people[iindex];
                            var aadress = adresses[iindex];
                            var foundPerson = heapFile.Find(aadress, pperson);
                            if (foundPerson.Id != pperson.Id)
                            {
                                throw new Exception("Person not found!");
                            }
                            break;
                        default:    
                            break;
                    }
                }
                
                for (int i = 0; i < people.Count; i++)
                {
                    heapFile.Delete(adresses[i], people[i]);
                    Console.WriteLine($"Deleting {i}");
                }
            
                heapFile.Close();
            }
        }
        
        static void TestHashFile()
        {
            for (int r = 0; r < NUMBER_OF_REPLICATIONS; r++)
            {
                if (File.Exists(DATA_FILE)) File.Delete(DATA_FILE);
                if (File.Exists(INIT_FILE)) File.Delete(INIT_FILE);

                ExtendibleHashFile<TestRP1> extendibleHashFile = new(INIT_FILE, DATA_FILE, BLOCK_SIZE);
                DataGenerator generator = new DataGenerator(r);
                List<int> adresses = new();
                List<TestRP1> people = new();
                
                for (int x = 0; x < NUMBER_OF_PEOPLE; x++)
                {
                    TestRP1 person = generator.GenerateTestRP1();
                    var adresa = extendibleHashFile.Insert(person);
                    people.Add(new TestRP1(person));
                    adresses.Add(adresa);
                }

                // Operations
                for (int i = 0; i < NUMBER_OF_OPERATIONS; i++)
                {
                    var operation = generator.GenerateOperation();
                    if (operation == OperationType.Delete) continue;
                    if (operation == OperationType.Find && people.Count == 0) continue;
                    Console.WriteLine(i + ". " + operation);
                    if (i == 178) 
                        Console.WriteLine("tu");
                    switch (operation)
                    {
                        case OperationType.Insert:
                            var countBeforeInsert = extendibleHashFile.RecordsCount;
                            var newPerson = generator.GenerateTestRP1();
                            var adresa = extendibleHashFile.Insert(newPerson);
                            var countAfterInsert = extendibleHashFile.RecordsCount;
                            if (countBeforeInsert+1 != countAfterInsert) throw new Exception("Insert failed!");
                            people.Add(new TestRP1(newPerson));
                            adresses.Add(adresa);
                            break;
                        case OperationType.Delete:
                            var countBeforeDelete = extendibleHashFile.RecordsCount;
                            if (adresses.Count == 0) break;
                            var index = generator.GenerateInt(0, adresses.Count);
                            var person = people[index];
                            var adress = adresses[index];
                            people.RemoveAt(index);
                            adresses.RemoveAt(index);
                            extendibleHashFile.Delete(person);
                            var countAfterDelete = extendibleHashFile.RecordsCount;
                            if (countAfterDelete+1 != countBeforeDelete) throw new Exception("Delete failed!");
                            break;
                        case OperationType.Find:
                            if (adresses.Count == 0) break;
                            var iindex = generator.GenerateInt(0, adresses.Count);
                            var pperson = people[iindex];
                            var foundPerson = extendibleHashFile.Find(pperson);
                            if (foundPerson.Id != pperson.Id)
                            {
                                throw new Exception("Person not found!");
                            }
                            break;
                        default:    
                            break;
                    }
                }
            }
        }
    }
}
