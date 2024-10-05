namespace GeoLib
{
	public class GPSOhranicenie
	{
		#region Properties
		public GPSPos Hranica1 { get; set; } = new GPSPos();
		public GPSPos Hranica2 { get; set; } = new GPSPos();
		#endregion //Properties

		#region Constructor
		public GPSOhranicenie()
		{
		}
		#endregion //Constructor
	}
}
