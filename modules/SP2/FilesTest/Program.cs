using CarLib.Data;
using CarLib.Generator;
using FilesLib.Hash;
using FilesLib.Heap;
using CarLib.Data.TestingData;

namespace FilesTest
{
    public static class Program
    {
        #region Constants
        public static int BLOCK_SIZE = 4096;
		private const string INIT_FILE = "../../userdata/t_person_init.aus";
        private const string DATA_FILE = "../../userdata/t_person.aus";
        
        private const string INIT_FILE_HASH = "../../userdata/t_hash_init.aus";
        private const string INIT_FILE_HEAP_HASH = "../../userdata/t_hash_init_heap.aus";
        private const string DATA_FILE_HEAP_HASH = "../../userdata/t_hash_heap.aus";
		public static int NUMBER_OF_PEOPLE = 0;
        public static int NUMBER_OF_OPERATIONS = 1_000_000;
        public static int NUMBER_OF_REPLICATIONS = 10;
        #endregion // Constants

        #region Main
        static void Main()
        {
            Console.WriteLine("STRUCTURE TESTER");
            Console.WriteLine("Current configuration");
            Console.WriteLine($"Block size: {BLOCK_SIZE}, Number of people: {NUMBER_OF_PEOPLE}, Number of operations: {NUMBER_OF_OPERATIONS}, Number of repllications: {NUMBER_OF_REPLICATIONS}");
            Console.WriteLine("Choose what you want to run:");
            Console.WriteLine("1 - Test heap file with TestRP1");
            Console.WriteLine("2 - Test hash file with TestRP1");
            Console.WriteLine("3 - Test heap file with Person class !!!Block size must be larger than 2500!!!");
            Console.Write("Choice: ");
            var choice = Console.ReadLine();
            if (choice == "1") TestHeapFile();
            else if (choice == "2") TestHashFile();
            else if (choice == "3") TestHeapFilePerson();
            else throw new Exception("Invalid choice");
        }
        #endregion // Main
        
        #region Heap file
        static void TestHeapFile()
        {
            for (int r = 0; r < NUMBER_OF_REPLICATIONS; r++)
            {
                if (File.Exists(DATA_FILE)) File.Delete(DATA_FILE);
                if (File.Exists(INIT_FILE)) File.Delete(INIT_FILE);
                Console.WriteLine($"{r}. replication");

                HeapFile<TestRP1> heapFile = new(INIT_FILE, DATA_FILE, BLOCK_SIZE);
                DataGenerator generator = new DataGenerator(r);
                List<int> adresses = [];
                List<TestRP1> people = [];
                
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
                    //Console.WriteLine(i + ". " + operation);
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
                    }
                }
                
                for (int i = 0; i < people.Count; i++)
                {
                    heapFile.Delete(adresses[i], people[i]);
                    Console.WriteLine($"Deleting {i}");
                }

                Console.WriteLine(heapFile.ToString());
                Console.WriteLine(heapFile.SequentialOutput());
                heapFile.Close();
            }
        }
        
        static void TestHeapFilePerson()
        {
            if (BLOCK_SIZE < 2500) throw new Exception("Block size is too small!");
            for (int r = 0; r < NUMBER_OF_REPLICATIONS; r++)
            {
                if (File.Exists(DATA_FILE)) File.Delete(DATA_FILE);
                if (File.Exists(INIT_FILE)) File.Delete(INIT_FILE);
                Console.WriteLine($"{r}. replication");

                HeapFile<Person> heapFile = new(INIT_FILE, DATA_FILE, BLOCK_SIZE);
                DataGenerator generator = new DataGenerator(r);
                List<int> adresses = [];
                List<Person> people = [];
                
                for (int x = 0; x < NUMBER_OF_PEOPLE; x++)
                {
                    Person person = generator.GeneratePerson();
                    var adresa = heapFile.Insert(person);
                    people.Add(new Person(person));
                    adresses.Add(adresa);
                }

                // Operations
                for (int i = 0; i < NUMBER_OF_OPERATIONS; i++)
                {
                    var operation = generator.GenerateOperation();
                    //Console.WriteLine(i + ". " + operation);
                    switch (operation)
                    {
                        case OperationType.Insert:
                            var countBeforeInsert = heapFile.RecordsCount;
                            var newPerson = generator.GeneratePerson();
                            var adresa = heapFile.Insert(newPerson);
                            var countAfterInsert = heapFile.RecordsCount;
                            if (countBeforeInsert+1 != countAfterInsert) throw new Exception("Insert failed!");
                            people.Add(new Person(newPerson));
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
                    }
                }
                
                for (int i = 0; i < people.Count; i++)
                {
                    heapFile.Delete(adresses[i], people[i]);
                    Console.WriteLine($"Deleting {i}");
                }

                Console.WriteLine(heapFile.ToString());
                Console.WriteLine(heapFile.SequentialOutput());
                heapFile.Close();
            }
        }
        #endregion // Heap file
        
        #region Hash file
        static void TestHashFile()
        {
            for (int r = 0; r < NUMBER_OF_REPLICATIONS; r++)
            {
                if (File.Exists(DATA_FILE)) File.Delete(DATA_FILE);
                if (File.Exists(INIT_FILE)) File.Delete(INIT_FILE);
                if (File.Exists(INIT_FILE_HASH)) File.Delete(INIT_FILE_HASH);
				Console.WriteLine($"{r}. replication");

                HeapFile<TestRP1> heapFile = new(INIT_FILE, DATA_FILE, BLOCK_SIZE);
                ExtendibleHashFile<TestRP1Id> extendibleHashFile = new(INIT_FILE_HASH, INIT_FILE_HEAP_HASH, DATA_FILE_HEAP_HASH, BLOCK_SIZE);
                DataGenerator generator = new(r);
                List<TestRP1> people = [];
                
                for (int x = 0; x < NUMBER_OF_PEOPLE; x++)
                {
                    TestRP1 person = generator.GenerateTestRP1();
                    var address = heapFile.Insert(person);
                    TestRP1Id personId = new()
                    {
                        Id = person.Id,
                        Address = address
                    };
                    extendibleHashFile.Insert(personId);
                    people.Add(new TestRP1(person));
                }

                // Operations
                int inserts = 0;
                int deletes = 0;
                int searches = 0;
                for (int i = 0; i < NUMBER_OF_OPERATIONS; i++)
                {
                    var operation = generator.GenerateOperation();
                    if (operation == OperationType.Delete && people.Count == 0) continue;
                    if (operation == OperationType.Find && people.Count == 0) continue;
                    //Console.WriteLine(i + ". " + operation);
                    //if (i == 267131)
                    //    Console.WriteLine("TU");
                    switch (operation)
                    {
                        case OperationType.Insert:
                            var countBeforeInsert = extendibleHashFile.RecordsCount;
                            var newPerson = generator.GenerateTestRP1();
                            var address = heapFile.Insert(newPerson);
                            TestRP1Id newPersonId = new()
                            {
                                Id = newPerson.Id,
                                Address = address
                            };
                            extendibleHashFile.Insert(newPersonId);
                            var countAfterInsert = heapFile.RecordsCount;
                            if (countBeforeInsert+1 != countAfterInsert) throw new Exception("Insert failed!");
                            people.Add(new TestRP1(newPerson));
                            inserts++;
                            break;
                        case OperationType.Delete:
                            var countBeforeDelete = extendibleHashFile.RecordsCount;
                            if (people.Count == 0) break;
                            var index = generator.GenerateInt(0, people.Count);
                            var person = people[index];
                            people.RemoveAt(index);
                            if (i == 267128)
                            {
                                continue;
                            }

                            TestRP1Id personIid = new()
                            {
                                Id = person.Id
                            };
                            var addr = extendibleHashFile.Find(personIid);
                            var aa = addr.Address;
                            extendibleHashFile.Delete(personIid);
                            heapFile.Delete(aa, person);
                            deletes++;
                            var countAfterDelete = extendibleHashFile.RecordsCount;
                            if (countAfterDelete + 1 != countBeforeDelete) throw new Exception("Delete failed!");
                            break;
                        case OperationType.Find:
                            if (people.Count == 0) break;
                            var iindex = generator.GenerateInt(0, people.Count);
                            var pperson = people[iindex];
                            TestRP1Id personId = new()
                            {
                                Id = pperson.Id
                            };
                            TestRP1Id a = extendibleHashFile.Find(personId);
                            var foundPerson = heapFile.Find(a.Address, pperson);
                            searches++;
                            if (foundPerson == null) throw new Exception("Person not found!");
                            break;
                    }
                }

                Console.WriteLine($"Inserted {inserts} inserts, {deletes} deletes, {searches} searches");
                extendibleHashFile.Close();
				heapFile.Close();
			}
        }
        #endregion // Hash file
    }
}
