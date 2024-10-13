using DataStructures.Data;

namespace GeoLib
{
	public class DataGenerator
	{
		#region Class members
		private Random _random;
		#endregion //Class members

		#region Constructors
		public DataGenerator()
		{
			_random = new Random();
		}

		public DataGenerator(int seed)
		{
			_random = new Random(seed);
		}
		#endregion //Constructor

		#region Public methods
		public void ChangeSeed(int seed)
		{
			_random = new Random(seed);
		}

		public Nehnutelnost GenerateNehnutelnost()
		{
			Nehnutelnost nehnutelnost = new Nehnutelnost
			{
				SupisneCislo = _random.Next(0, 10000),
				Pozicie =
				{
					[0] = GenerateGpsPos(),
					[1] = GenerateGpsPos()
				}
			};
			return nehnutelnost;
		}

		public Parcela GenerateParcela()
		{
			Parcela parcela = new Parcela
			{
				SupisneCislo = _random.Next(0, 10000),
				Pozicie =
				{
					[0] = GenerateGpsPos(),
					[1] = GenerateGpsPos()
				},
			};
			return parcela;
		}

		private GpsPos GenerateGpsPos()
		{
			GpsPos gpsPos = new()
			{
				Dlzka = _random.Next(0, 2) == 0 ? 'W' : 'E',
				PoziciaDlzky = _random.NextDouble() * 180,
				Sirka = _random.Next(0, 2) == 0 ? 'S' : 'N',
				PoziciaSirky = _random.NextDouble() * 90
			};
			return gpsPos;
		}
		#endregion //Public methods
	}
}
