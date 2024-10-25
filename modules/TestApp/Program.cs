using System.Diagnostics;
using DataStructures;
using DataStructures.Data;
using GeoLib;

namespace TestApp
{
	public class Program
	{
		#region Constants
		private const int BASE_SEED = 0;
		private const int POCET_GENEROVANYCH = 10_000;
		private const int POCET_OPERACII = 1_000_000;
		#endregion //Constants

		static void Main()
		{
			//TestObjekty();
			TestKontrolaRozpracovania2_NaplnenaStruktura(true);
			//TestKontrolaRozpracovania2_NenaplnenaStruktura();
		}

		private static void TestKontrolaRozpracovania2_NaplnenaStruktura(bool vypis)
		{
			KdTree<TestKey, TestData> tree = new(4);
			DataGenerator generator = new(BASE_SEED);

			List<TestData> vsetkyPrvky = new();
			Console.WriteLine("Vkladam " + POCET_GENEROVANYCH + " prvkov do stromu.");
			for (int i = 0; i < POCET_GENEROVANYCH; i++)
			{
				TestData data = generator.GenerateTestData(i);
				vsetkyPrvky.Add(data);
				tree.Insert(data.Kluce, data);
			}
			Console.WriteLine("Prvky boli uspesne vlozene:");

			/////// Testovanie /////////
			int pocetHladani = 0;
			int pocetChybPriHladani = 0;
			List<TestData> chybyPriHladani = new();
			int pocetMazani = 0;
			int pocetChybPriMazani = 0;
			List<TestData> chybyPriMazani = new();
			int pocetVkladani = 0;
			int pocetChybPriVkladani = 0;
			List<TestData> chybyPriVkladani = new();
			Stopwatch sw = new();
			for (int i = 0; i < POCET_OPERACII; i++)
			{
				sw.Start();
				Console.WriteLine(i);
				int typOperacie = generator.GenerateInt(0, 3);
				switch (typOperacie)
				{
					case 0:
						// Vkladanie
						pocetVkladani++;
						TestData data = generator.GenerateTestData(i);
						if (vypis) Console.WriteLine("Vkladam novy prvok " + data);
						vsetkyPrvky.Add(data);
						tree.Insert(data.Kluce, data);
						if (tree.Count != tree.GetAll().Count)
						{
							pocetChybPriVkladani++;
							chybyPriVkladani.Add(data);
						}
						if (vypis) Console.WriteLine("Prvok bol vlozeny do stromu, aktualny pocet prvkov v strome je: " + tree.Count);
						break;
					case 1:
						// Hladanie
						pocetHladani++;
						if (vsetkyPrvky.Count == 0)
						{
							break;
						}
						var hladanyPrvok = vsetkyPrvky[generator.GenerateInt(0, vsetkyPrvky.Count)];
						if (vypis) Console.WriteLine("Hladam prvok " + hladanyPrvok.ToString());
						var res = tree.Find(hladanyPrvok.Kluce);

						bool najdeny = false;
						foreach (var found in res)
						{
							if (found.Kluce.Equals(hladanyPrvok.Kluce))
							{
								najdeny = true;
								break;
							}
						}
						if (vypis) Console.WriteLine(!najdeny ? "Nenasiel som dany prvok!" : "Nasiel som dany prvok!");
						if (!najdeny)
						{
							pocetChybPriHladani++;
							chybyPriHladani.Add(hladanyPrvok);
						}
						break;
					case 2:
						// Mazanie
						pocetMazani++;
						if (vsetkyPrvky.Count == 0)
						{
							break;
						}
						var prvokNaZmazanie = vsetkyPrvky[generator.GenerateInt(0, vsetkyPrvky.Count)];
						if (vypis) Console.WriteLine("Mazem prvok " + prvokNaZmazanie);
						vsetkyPrvky.Remove(prvokNaZmazanie);
						tree.Remove(prvokNaZmazanie.Kluce, prvokNaZmazanie);
						if (vypis)
						{
							Console.WriteLine("Uspesne som vymazal dany prvok");
							Console.WriteLine("---");
							Console.WriteLine("POVODNE BOLO: " + vsetkyPrvky.Count + 1);
							Console.WriteLine("V STROME OSTALO: " + tree.Count);
							Console.WriteLine("V STROME REALNE JE: " + tree.GetAll().Count + " prvkov");
						}

						if (tree.GetAll().Count != tree.Count)
						{
							pocetChybPriMazani++;
							chybyPriMazani.Add(prvokNaZmazanie);
						}
						break;
				}

				if (vypis) Console.WriteLine("--------------------------------------------------------------------");
			}
			sw.Stop();
			Console.WriteLine("==========================================================================");
			Console.WriteLine("Pocet sekund: " + sw.Elapsed.TotalSeconds);
			Console.WriteLine("Po vsetkych operaciach: ");
			Console.WriteLine("Pocet vkladani: " + pocetVkladani);
			Console.WriteLine("Pocet chyb pri vkladani: " + pocetChybPriVkladani);
			for (int i = 0; i < chybyPriVkladani.Count; i++)
			{
				var testData = chybyPriVkladani[i];
				Console.WriteLine(i + " " + testData.ToString());
			}
			Console.WriteLine("Pocet hladani: " + pocetHladani);
			Console.WriteLine("Pocet chyb pri hladani: " + pocetChybPriHladani);
			for (int i = 0; i < chybyPriHladani.Count; i++)
			{
				var testData = chybyPriHladani[i];
				Console.WriteLine(i + " " + testData.ToString());
			}
			Console.WriteLine("Pocet mazani: " + pocetMazani);
			Console.WriteLine("Pocet chyb pri mazani: " + pocetChybPriMazani);
			for (int i = 0; i < chybyPriMazani.Count; i++)
			{
				var testData = chybyPriMazani[i];
				Console.WriteLine(i + " " + testData.ToString());
			}

			var allPrvky = tree.GetAll();
			var pocetRealne = allPrvky.Count;
			Console.WriteLine("Strom zacinal s potom prvkov: " + POCET_GENEROVANYCH);
			Console.WriteLine("Pocet prvkov v liste uchovavanom: " + vsetkyPrvky.Count);
			Console.WriteLine("Pocet prvkov v strome v atribute Count: " + tree.Count);
			Console.WriteLine("Pocet prvkov v strome realne: " + pocetRealne);
		}

		private static void TestKontrolaRozpracovania2_NenaplnenaStruktura()
		{
			throw new NotImplementedException();
		}

		private static void TestObjekty()
		{
			KdTree<GpsPos, Nehnutelnost> objekty = new(2);

			Console.WriteLine("Generované nehnutelnosti");
			DataGenerator generator = new(BASE_SEED);

			int pocetNehnutelnosti = 0;
			Nehnutelnost objektToFind = null!;
			for (int i = 0; i < POCET_GENEROVANYCH; i++)
			{
				Nehnutelnost objekt;
				objekt = generator.GenerateNehnutelnost(i);
				pocetNehnutelnosti++;

				if (i == 0)
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
			objekty.Remove(objektToFind.Pozicie[1], objektToFind);

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
		}
	}
}