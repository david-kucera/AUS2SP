using DataStructures.Data;
using System.Text;

namespace GeoLib
{
	/// <summary>
	/// Trieda pre generovanie dát.
	/// </summary>
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
		/// <summary>
		/// Metóda pre zmenu násady.
		/// </summary>
		/// <param name="seed"></param>
		public void ChangeSeed(int seed)
		{
			_random = new Random(seed);
		}

		/// <summary>
		/// Metóda pre generovanie zvoleného počtu nehnuteľností.
		/// </summary>
		/// <param name="i">Počet nehnuteľností pre generovanie</param>
		/// <returns></returns>
		public Nehnutelnost GenerateNehnutelnost(int i)
		{
			Nehnutelnost nehnutelnost = new()
			{
				SupisneCislo = i,
				Popis = "Nehnutelnost " + i,
				Pozicie =
				{
					[0] = GenerateGpsPos(),
					[1] = GenerateGpsPos()
				}
			};
			return nehnutelnost;
		}

		/// <summary>
		/// Metóda pre generovanie zvoleného počtu parciel.
		/// </summary>
		/// <param name="i"></param>
		/// <returns>Počet parciel na generovanie</returns>
		public Parcela GenerateParcela(int i)
		{
			Parcela parcela = new()
			{
				SupisneCislo = i,
				Popis = "Parcela " + i,
				Pozicie =
				{
					[0] = GenerateGpsPos(),
					[1] = GenerateGpsPos()
				},
			};
			return parcela;
		}

		/// <summary>
		/// Metóda pre generovanie zvoleného počtu objektov dát. 50% šanca na vygenerovanie nehnuteľnosti alebo parcely.
		/// </summary>
		/// <param name="i">Počet objektov na generovanie</param>
		/// <returns></returns>
		public GeoObjekt GenerateGeoObjekt(int i)
		{
			if (GenerateBool())
			{
				return GenerateNehnutelnost(i);
			}
			return GenerateParcela(i);
		}

		public TestData GenerateTestData(int i)
		{
			TestData testData = new()
			{
				Kluce = new TestKey
				{
					A = _random.NextDouble() * 10_000,
					B = GenerateString(_random.Next(1,10)),
					C = _random.Next(),
					D = _random.NextDouble() * 10_000
				},
				Popis = "TestData " + i,
				Cislo = i
			};
			return testData;
		}

		/// <summary>
		/// Metóda na generovanie náhodnej boolovskej hodnoty.
		/// </summary>
		/// <returns></returns>
		public bool GenerateBool()
		{
			return _random.Next(0, 2) == 0;
		}

		/// <summary>
		/// Metóda na generovanie náhodnej double hodnoty.
		/// </summary>
		/// <returns></returns>
		public double GenerateDouble()
		{
			return _random.NextDouble();
		}

		/// <summary>
		/// Metóda na generovanie náhodnej int hodnoty.
		/// </summary>
		/// <param name="v1">Minimum od</param>
		/// <param name="v2">Maximum do</param>
		/// <returns></returns>
		public int GenerateInt(int v1, int v2)
		{
			return _random.Next(v1, v2);
		}
		#endregion //Public methods

		#region Private methods
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

		private string GenerateString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var stringBuilder = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				int index = _random.Next(chars.Length);
				stringBuilder.Append(chars[index]);
			}

			return stringBuilder.ToString();
		}
		#endregion //Private methods
	}
}
