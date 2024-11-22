using FilesLib.Data;
using FilesLib.Generator;
using FilesLib.Hash;
using FilesLib.Heap;

namespace FilesTest
{
    public class Program
    {
        public static int BLOCK_SIZE = 1024 * 4;
        public static string INIT_FILE = "../../userdata/person_init.aus";
        public static string DATA_FILE = "../../userdata/person.aus";
        public static int NUMBER_OF_PEOPLE = 10_000;
        public static int NUMBER_OF_OPERATIONS = 100_000;

        static void Main()
        {
            TestHeapFile();
            //TestHashFile();
        }
        
        static void TestHeapFile()
        {
            int numberOfReplications = 10;
            for (int r = 0; r < numberOfReplications; r++)
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
                            people.RemoveAt(index);
                            adresses.RemoveAt(index);
                            heapFile.Delete(adress, person);
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
            
                heapFile.Close();
            }
        }
        
        static void TestHashFile()
        {
            int numberOfReplications = 10;
            for (int r = 0; r < numberOfReplications; r++)
            {
                if (File.Exists(DATA_FILE)) File.Delete(DATA_FILE);
                if (File.Exists(INIT_FILE)) File.Delete(INIT_FILE);

                HashFile<TestRP1> hashFile = new(INIT_FILE, DATA_FILE, BLOCK_SIZE);
                DataGenerator generator = new DataGenerator(r);
                List<int> adresses = new();
                List<TestRP1> people = new();
                
                for (int x = 0; x < NUMBER_OF_PEOPLE; x++)
                {
                    TestRP1 person = generator.GenerateTestRP1();
                    var adresa = hashFile.Insert(person);
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
                            var countBeforeInsert = hashFile.RecordsCount;
                            var newPerson = generator.GenerateTestRP1();
                            var adresa = hashFile.Insert(newPerson);
                            var countAfterInsert = hashFile.RecordsCount;
                            if (countBeforeInsert+1 != countAfterInsert) throw new Exception("Insert failed!");
                            people.Add(new TestRP1(newPerson));
                            adresses.Add(adresa);
                            break;
                        case OperationType.Delete:
                            var countBeforeDelete = hashFile.RecordsCount;
                            if (adresses.Count == 0) break;
                            var index = generator.GenerateInt(0, adresses.Count);
                            var person = people[index];
                            var adress = adresses[index];
                            people.RemoveAt(index);
                            adresses.RemoveAt(index);
                            hashFile.Delete(person);
                            var countAfterDelete = hashFile.RecordsCount;
                            if (countAfterDelete+1 != countBeforeDelete) throw new Exception("Delete failed!");
                            break;
                        case OperationType.Find:
                            if (adresses.Count == 0) break;
                            var iindex = generator.GenerateInt(0, adresses.Count);
                            var pperson = people[iindex];
                            var foundPerson = hashFile.Find(pperson);
                            if (foundPerson.Id != pperson.Id)
                            {
                                throw new Exception("Person not found!");
                            }
                            break;
                        default:    
                            break;
                    }
                }
            
                //hashFile.Close();
            }
        }
    }
}
