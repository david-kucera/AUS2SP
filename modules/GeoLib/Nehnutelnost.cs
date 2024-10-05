namespace GeoLib
{
	public class Nehnutelnost
	{
		#region Properties
		public int SupisneCislo { get; set; } = 0;
		public string Popis { get; set; } = string.Empty;
		public List<Parcela> Parcely { get; set; } = new List<Parcela>(); // iba referencie
		public GPSPos[] Pozicia { get; set; } = new GPSPos[2];
		#endregion //Properties

		#region Constructor
		public Nehnutelnost()
		{
		}
		#endregion //Constructor
	}
}
