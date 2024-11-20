﻿using FilesLib;
using FilesLib.Data;

namespace FilesTest
{
    public class Program
    {
        public static int BLOCK_SIZE = 1024;
        public static string INIT_FILE = "../../userdata/person_init.aus";
        public static string DATA_FILE = "../../userdata/person.aus";
        public static int NUMBER_OF_PEOPLE = 1000;
        public static int NUMBER_OF_REPLICATIONS = 10;
        static void Main()
        {
            for (int r = 0; r < NUMBER_OF_REPLICATIONS; r++)
            {
                for (int x = 0; x < NUMBER_OF_PEOPLE; x++)
                {
                    Console.WriteLine(x + ". Testing heap file...");
                    if (File.Exists(DATA_FILE)) File.Delete(DATA_FILE);
                    if (File.Exists(INIT_FILE)) File.Delete(INIT_FILE);
            
                    HeapFile<TestRP1> heapFile = new(INIT_FILE, DATA_FILE, BLOCK_SIZE);
                    DataGenerator generator = new DataGenerator(r);
                    List<int> adresses = new(x);
                    List<TestRP1> people = new(x);
                    for (int i = 0; i < x; i++)
                    {
                        TestRP1 person = generator.GenerateTestRP1();
                        var adresa = heapFile.Insert(person);
                        people.Add(new TestRP1(person));
                        adresses.Add(adresa);
                    }
                
                    for (int i = 0; i < x; i++)
                    {
                        var index = generator.GenerateInt(0, adresses.Count);
                        var person = people[index];
                        var adress = adresses[index];
                        people.RemoveAt(index);
                        adresses.RemoveAt(index);
                        heapFile.Delete(adress, person);
                    }
                
                    heapFile.Close();
                }
            }
            
            

            // Console.WriteLine("Number of blocks:");
            // Console.WriteLine(heapFile.GetBlocks().Count);
            // Console.WriteLine("------------------------");

            // for (int i = 0; i < NUMBER_OF_PEOPLE; i++)
            // {
            //     var pers = heapFile.Find(adresses[i], people[i]);
            //     if (pers == null) throw new Exception("Data not found!");
            // }
            
            // Console.WriteLine("All blocks:");
            // var blocks = heapFile.GetBlocks();
            // for (int i = 0; i < blocks.Count; i++)
            // {
            //     Console.WriteLine($"{i + 1}. Block {blocks[i]}");
            // }

            // Console.WriteLine(heapFile.NextEmptyBlockAddress);
            // Console.WriteLine(heapFile.NextFreeBlockAddress);
            
            // for (int i = NUMBER_OF_PEOPLE-1; i >= 0; i--)
            // {
            //     heapFile.Delete(adresses[i], people[i]);
            //     Console.WriteLine($"{i}. Deleted person");
            //     Console.WriteLine(heapFile.GetBlocks().Count);
            // }
            
            // for (int i = 0; i < NUMBER_OF_PEOPLE; i++)
            // {
            //     heapFile.Delete(adresses[i], people[i]);
            //     Console.WriteLine($"{i}. Deleted person");
            //     Console.WriteLine(heapFile.GetBlocks().Count);
            // }
            
            // var allBlocks = heapFile.GetBlocks();
            // Console.WriteLine("------------------------");
            // Console.WriteLine("All Blocks");
            // int poradie = 1;
            // foreach (var b in allBlocks)
            // {
            //     Console.WriteLine("------------------------");
            //     Console.WriteLine(poradie + ". block\n" + b);
            //     Console.WriteLine("------------------------");
            //     poradie++;
            // }
            
            
        }
    }
}
