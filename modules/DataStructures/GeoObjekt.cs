namespace DataStructures
{
	public class GeoObjekt
	{
		#region Properties
		public int SupisneCislo { get; set; } = -1;
		public string Popis { get; set; } = string.Empty;
		public GPSPos[] Pozicie { get; set; } = new GPSPos[2];
		#endregion //Properties

		#region Constructor
		public GeoObjekt()
		{
		}
		#endregion //Constructor
	}
}
