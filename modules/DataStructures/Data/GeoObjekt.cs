namespace DataStructures.Data
{
    public class GeoObjekt
    {
        #region Properties
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
			return Pozicie[0].Sirka.ToString() + Pozicie[0].PoziciaSirky + "," + Pozicie[0].Dlzka.ToString() + Pozicie[0].PoziciaDlzky + "\n " +
			       Pozicie[1].Sirka.ToString() + Pozicie[1].PoziciaSirky + "," + Pozicie[1].Dlzka.ToString() + Pozicie[1].PoziciaDlzky + "\n\n";
		}
		#endregion //Public functions
	}
}
