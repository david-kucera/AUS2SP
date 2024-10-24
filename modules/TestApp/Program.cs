using DataStructures;
using DataStructures.Data;
using GeoLib;

namespace TestApp
{
	public class Program
	{
		#region Constants
		private const int BASE_SEED = 0;
		private const int POCET_GENEROVANYCH = 10;
		#endregion //Constants

		static void Main()
		{
			KdTree<GpsPos, Nehnutelnost> objekty = new(2);

			Console.WriteLine("Generované nehnutelnosti");
			Random rnd = new(BASE_SEED);
			DataGenerator generator = new(BASE_SEED);

			int pocetNehnutelnosti = 0;
			Nehnutelnost objektToFind = null!;
			for (int i = 0; i < POCET_GENEROVANYCH; i++)
			{
				Nehnutelnost objekt;
				objekt = generator.GenerateNehnutelnost(i);
				pocetNehnutelnosti++;

				if (i == 2)
				{
					objektToFind = objekt;
				}
				objekty.Insert(objekt.Pozicie[0], objekt);
				objekty.Insert(objekt.Pozicie[1], objekt);
			}
			Console.WriteLine("POCET CELKOVO: " + objekty.Count);
			Console.WriteLine(objekty.ToString());

			Console.WriteLine($"Nájdené objekty na pozícií {objektToFind.Pozicie[0]}");
			var objs = objekty.Find(objektToFind.Pozicie[0]);
			foreach (var obj in objs)
			{
				Console.WriteLine(obj.ToString());
			}
			Console.WriteLine("----------------");

			Console.WriteLine($"Nájdené objekty na pozícií {objektToFind.Pozicie[1]}");
			var objss = objekty.Find(objektToFind.Pozicie[1]);
			foreach (var obj in objss)
			{
				Console.WriteLine(obj.ToString());
			}
			Console.WriteLine("----------------");

			Console.WriteLine($"Objekty po vymazaní objektu {objektToFind}");
			objekty.Remove(objektToFind.Pozicie[0], objektToFind);
			//objekty.Delete(objektToFind.Pozicie[1], objektToFind);

			Console.WriteLine($"Nájdené objekty na pozícií {objektToFind.Pozicie[0]}");
			objs = objekty.Find(objektToFind.Pozicie[0]);
			foreach (var obj in objs)
			{
				Console.WriteLine(obj.ToString());
			}
			Console.WriteLine("----------------");

			Console.WriteLine($"Nájdené objekty na pozícií {objektToFind.Pozicie[1]}");
			objs = objekty.Find(objektToFind.Pozicie[1]);
			foreach (var obj in objs)
			{
				Console.WriteLine(obj.ToString());
			}
			Console.WriteLine("----------------");

			Console.WriteLine("Celkový počet objektov v strome: " + objekty.Count);
			Console.WriteLine(objekty.ToString());


			//while (true)
			//{
			//	Console.WriteLine("Stlač kláves 'd/D' pre vypísanie dát stromu.");
			//	Console.WriteLine("Stlač akúkoľvek klávesu pre ukočenie aplikácie");
			//	var key = Console.ReadKey();
			//	if (key.KeyChar is 'd' or 'D')
			//	{
			//		Console.Clear();
			//		Console.WriteLine(objekty.ToString());
			//		break;
			//	}
			//	if (key.KeyChar > 0 && key.KeyChar < 255)
			//	{
			//		break;
			//	}
			//}
		}
	}
}