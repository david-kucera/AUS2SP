using DataStructures;
using DataStructures.Data;

namespace TestApp
{
    public class Program
	{
		static void Main(string[] args)
		{
			KdTree<Nehnutelnost> nehnutelnosti = new KdTree<Nehnutelnost>(2);
			//KdTree<Parcela> parcely = new KdTree<Parcela>(2);
			//KdTree<GeoObjekt> objekty = new KdTree<GeoObjekt>(2);


			int[] sur1S = { 157, 19, 14, 56, 17, 174, 95, 83, 166, 92 };
			int[] sur1D = { 94, 6, 123, 101, 179, 35, 85, 91, 48, 144 };

			int[] sur2S = { 142, 43, 5, 100, 79, 94, 106, 55, 49, 92 };
			int[] sur2D = { 77, 24, 167, 54, 104, 83, 48, 62, 24, 47 };

			for (int i = 0; i < sur1D.Length; i++)
			{
				GpsPos pozicia1 = new GpsPos('N', sur1S[i], 'E', sur1D[i]);
				GpsPos pozicia2 = new GpsPos('N', sur2S[i], 'E', sur2D[i]);
				Nehnutelnost nehnutelnost = new Nehnutelnost();
				nehnutelnost.Pozicie[0] = pozicia1;
				nehnutelnost.Pozicie[1] = pozicia2;
				nehnutelnosti.Insert(nehnutelnost);
				if (i == 4)
				{
					break;
				}
			}

			Console.WriteLine(nehnutelnosti.Count);
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
