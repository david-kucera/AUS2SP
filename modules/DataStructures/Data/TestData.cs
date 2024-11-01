using System.Text;

namespace DataStructures.Data
{
	public class TestData
	{
		#region Properties
		public TestKey Kluce { get; set; } = null!;
		public string Popis { get; set; } = string.Empty;
		#endregion //Properties

		#region Constructors
		public TestData() { }

		public TestData(TestKey kluce, string popis)
		{
			Kluce = kluce;
			Popis = popis;
		}
		#endregion //Constructors

		#region Public methods
		public override string ToString()
		{
			return Kluce.ToString() + " " + Popis;
		}
		#endregion //Public methods
	}
}
