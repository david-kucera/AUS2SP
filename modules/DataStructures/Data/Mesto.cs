namespace DataStructures.Data
{
	public class Mesto
	{
		public GpsPos Pozicia { get; set; }
		public string Nazov { get; set; }

		public Mesto(string nazov, GpsPos pozicia)
		{
			Nazov = nazov;
			Pozicia = pozicia;
		}

		public override string ToString()
		{
			return Nazov;
		}
	}
}
