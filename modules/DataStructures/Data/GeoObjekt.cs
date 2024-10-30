namespace DataStructures.Data
{
    public class GeoObjekt
    {
        #region Properties
        public char Typ { get; set; } = ' ';
		public int SupisneCislo { get; set; } = -1;
        public string Popis { get; set; } = string.Empty;
        public GpsPos[] Pozicie { get; set; } = new GpsPos[2];
		#endregion //Properties

		#region Constructor
		public GeoObjekt()
        {
        }
		#endregion //Constructor

		#region Public functions
		public override string ToString()
		{
			return SupisneCislo + " " + Popis + " " + Pozicie[0].ToString() + " " + Pozicie[1].ToString();
		}

		public string ToFile()
		{
			return Typ + ";" + SupisneCislo + ";" + Popis + ";" + Pozicie[0].ToFile() + ";" + Pozicie[1].ToFile();
		}
		#endregion //Public functions
	}
}
