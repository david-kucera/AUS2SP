using FilesLib;
using FilesLib.Data;

namespace FilesTest
{
    public class Program
    {
        public static int BLOCK_SIZE = 512;
        static void Main(string[] args)
        {
            Console.WriteLine("Test heap file");
            Heap<Person> heapFile = new("../userdata/person.aus", BLOCK_SIZE);
        }
    }
}
