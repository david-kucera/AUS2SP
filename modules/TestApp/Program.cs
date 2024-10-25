﻿using DataStructures;
using DataStructures.Data;
using GeoLib;

namespace TestApp
{
	public class Program
	{
		#region Constants
		private const int BASE_SEED = 0;
		private const int POCET_GENEROVANYCH = 100;
		#endregion //Constants

		static void Main()
		{
			//TestObjekty();
			TestKontrolaRozpracovania2();
		}

		private static void TestKontrolaRozpracovania2()
		{
			KdTree<TestKey, TestData> tree = new(4);
			DataGenerator generator = new(BASE_SEED);

			List<TestData> naMazanie = new(); 
			for (int i = 0; i < POCET_GENEROVANYCH; i++)
			{
				TestData data = generator.GenerateTestData(i);
				tree.Insert(data.Kluce, data);
				if (generator.GenerateBool())
				{
					naMazanie.Add(data);
				}
			}

			Console.WriteLine(tree.ToString());

			Console.WriteLine("VYMAZAVANIE");
			foreach (var data in naMazanie)
			{
				Console.WriteLine("MAZEM " + data.ToString());
				tree.Remove(data.Kluce, data);
				Console.WriteLine("VYMAZAL SOM " + data.ToString());
				Console.WriteLine(tree.ToString());
				Console.WriteLine(tree.Count);
			}
			Console.WriteLine("PO VSETKYCH VYMAZANIACH");
			Console.WriteLine("POVODNE BOLO: " + POCET_GENEROVANYCH);
			Console.WriteLine("MALO SA VYMAZAT: " + naMazanie.Count);
			Console.WriteLine("V STROME OSTALO: " + tree.Count);
			Console.WriteLine(POCET_GENEROVANYCH - naMazanie.Count == tree.Count);
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