using DataStructures;

namespace TestApp
{
	public class Program
	{
		static void Main(string[] args)
		{
			KdTree<Nehnutelnost> nehnutelnosti = new KdTree<Nehnutelnost>(2);
			//KdTree<Parcela> parcely = new KdTree<Parcela>(2);
			//KdTree<GeoObjekt> objekty = new KdTree<GeoObjekt>(2);
			GPSPos pozicia1 = new GPSPos('N',17,'E', 20);
			GPSPos pozicia2 = new GPSPos('N', 11, 'E', 10);
			Nehnutelnost nehnutelnost = new Nehnutelnost();
			nehnutelnost.Pozicie[0] = pozicia1;
			nehnutelnost.Pozicie[1] = pozicia2;
			
			nehnutelnosti.Insert(nehnutelnost);
			Console.WriteLine(nehnutelnosti.Count);
		}
	}
}
