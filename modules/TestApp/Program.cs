using System.Diagnostics;
using DataStructures;
using DataStructures.Data;
using GeoLib;

namespace TestApp
{
    public class Program
	{
		public static int BASE_SEED = 0;
		static void Main(string[] args)
		{
			KdTree<GeoObjekt> objekty = new KdTree<GeoObjekt>(2);

			Console.WriteLine("Generované objekty");
			Random rnd = new Random(BASE_SEED);
			DataGenerator generator = new DataGenerator(BASE_SEED);

			int pocetGenerovanych = 10;
			int pocetNehnutelnosti = 0;
			int pocetParcel = 0;
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			GeoObjekt objektToFind = null!;
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

				Console.WriteLine(i + ". " + objekt.ToString());

				if (i == 6)
				{
					objektToFind = objekt;
				}

				objekty.Insert(objekt);
			}
			stopWatch.Stop();
			Console.WriteLine("Čas generovania a vkladania " + pocetGenerovanych + ": " + stopWatch.ElapsedMilliseconds + " ms");
			Console.WriteLine("Počet parciel: " + pocetParcel);
			Console.WriteLine("Počet nehnuteľností: " + pocetNehnutelnosti);

			Console.WriteLine("Nájdený objekt: " + objekty.Find(objektToFind));

			Console.WriteLine("Celkový počet objektov v strome: " + objekty.Count);
			//Console.WriteLine(objekty.ToString());
			while (true)
			{
				Console.WriteLine("Stlač akúkoľvek klávesu pre ukočenie aplikácie");
				var key = Console.ReadKey();
				if (key.KeyChar > 0 && key.KeyChar < 255)
				{
					break;
				}
			}
		}
	}
}
