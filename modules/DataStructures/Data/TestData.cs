using System.Text;

namespace DataStructures.Data
{
	public class TestData
	{
		#region Properties
		public TestKey Kluce { get; set; } = null!;
		public string Popis { get; set; } = string.Empty;
		public int Cislo { get; set; } = 0;
		#endregion //Properties

		#region Constructors
		public TestData() { }

		public TestData(TestKey kluce, string popis, int cislo)
		{
			Kluce = kluce;
			Popis = popis;
			Cislo = cislo;
		}
		#endregion //Constructors

		#region Public methods
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Popis: ");
			sb.Append(Popis);
			sb.Append(" Cislo: ");
			sb.Append(Cislo);
			sb.Append(Kluce.ToString());
			return sb.ToString();
		}
		#endregion //Public methods
	}
}
