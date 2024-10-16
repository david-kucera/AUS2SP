using System.Diagnostics;
using DataStructures;
using DataStructures.Data;
using GeoLib;

namespace TestApp
{
	public class Program
	{
		#region Constants
		public static int BASE_SEED = 0;
		public static int POCET_GENEROVANYCH = 10;
		#endregion //Constants

		static void Main(string[] args)
		{
			KdTree<GeoObjekt> objekty = new KdTree<GeoObjekt>(2);

			Console.WriteLine("Generované objekty");
			Random rnd = new Random(BASE_SEED);
			DataGenerator generator = new DataGenerator(BASE_SEED);

			int pocetNehnutelnosti = 0;
			int pocetParcel = 0;
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			GeoObjekt objektToFind = null!;
			for (int i = 0; i < POCET_GENEROVANYCH; i++)
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

				Console.WriteLine(i + ". " + objekt);

				if (i == 3)
				{
					objektToFind = objekt;
				}
				objekty.Insert(objekt);
			}
			stopWatch.Stop();
			Console.WriteLine("Čas generovania a vkladania " + POCET_GENEROVANYCH + ": " + stopWatch.ElapsedMilliseconds + " ms");
			Console.WriteLine("Počet parciel: " + pocetParcel);
			Console.WriteLine("Počet nehnuteľností: " + pocetNehnutelnosti);
			Console.WriteLine("Nájdený objekt: " + objekty.Find(objektToFind));
			//objekty.Delete(objektToFind);

			Console.WriteLine("Celkový počet objektov v strome: " + objekty.Count);
			while (true)
			{
				Console.WriteLine("Stlač kláves 'd/D' pre vypísanie dát stromu.");
				Console.WriteLine("Stlač akúkoľvek klávesu pre ukočenie aplikácie");
				var key = Console.ReadKey();
				if (key.KeyChar is 'd' or 'D')
				{
					Console.Clear();
					Console.WriteLine(objekty.ToString());
					break;
				}
				if (key.KeyChar > 0 && key.KeyChar < 255)
				{
					break;
				}
			}
		}
	}
}