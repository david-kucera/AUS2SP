namespace GeoLib
{
	public class Parcela
	{
		#region Properties
		public int Cislo { get; set; } = 0;
		public string Popis { get; set; } = string.Empty;
		public List<Nehnutelnost> Nehnutelnosti { get; set; } = new List<Nehnutelnost>(); // iba referencie
		public GPSOhranicenie Ohranicenie { get; set; } = new GPSOhranicenie();
		#endregion //Properties

		#region Constructor
		public Parcela()
		{
		}
		#endregion //Constructor
	}
}
