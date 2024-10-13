using System.Diagnostics;
using DataStructures;
using DataStructures.Data;

namespace TestApp
{
    public class Program
	{
		static void Main(string[] args)
		{
			KdTree<GeoObjekt> objekty = new KdTree<GeoObjekt>(2);

			Console.WriteLine("Generované nehnuteľnosti");
			Random rnd = new Random();
			DataGenerator generator = new DataGenerator(0);


			int pocetGenerovanych = 10000;
			int pocetNehnutelnosti = 0;
			int pocetParcel = 0;
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			for (int i = 0; i < pocetGenerovanych; i++)
			{
				GeoObjekt objekt;
				if (rnd.NextSingle() < 0.5)
				{
					objekt = generator.GenerateNehnutelnost();
					pocetNehnutelnosti++;
				}
				else
				{
					objekt = generator.GenerateParcela();
					pocetParcel++;
				}
				
				objekty.Insert(objekt);
			}
			stopWatch.Stop();
			Console.WriteLine("Čas generovania a vkladania " + pocetGenerovanych + ": " + stopWatch.ElapsedMilliseconds + " ms");
			Console.WriteLine("Počet parciel: " + pocetParcel);
			Console.WriteLine("Počet nehnuteľností: " + pocetNehnutelnosti);

			Console.WriteLine("Celkový počet objektov v strome: " + objekty.Count);
			while (true)
			{
				var key = Console.ReadKey();
				if (key.Key == ConsoleKey.A)
				{
					break;
				}
			}
		}
	}
}
