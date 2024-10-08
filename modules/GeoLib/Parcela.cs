namespace GeoLib
{
	public class Parcela
	{
		#region Properties
		public int Cislo { get; set; } = -1;
		public string Popis { get; set; } = string.Empty;
		public List<Nehnutelnost> Nehnutelnosti { get; set; } = new List<Nehnutelnost>();		// iba referencie
		public GPSPos[] Pozicie { get; set; } = new GPSPos[2];
		#endregion //Properties

		#region Constructor
		public Parcela()
		{
		}
		#endregion //Constructor
	}
}
