namespace DataStructures.Data
{
    public class Parcela : GeoObjekt
    {
        #region Properties
        public List<Nehnutelnost> Nehnutelnosti { get; set; } = new List<Nehnutelnost>();       // iba referencie
        #endregion //Properties

        #region Constructor
        public Parcela()
        {
        }

        public Parcela(int cisloparcely, string popis, GpsPos prvaGps, GpsPos druhaGps)
        {
	        SupisneCislo = cisloparcely;
			Popis = popis;
			Pozicie[0] = prvaGps;
			Pozicie[1] = druhaGps;
        }
		#endregion //Constructor

		#region Public functions
		public override string ToString()
		{
			return "Parcela: " + base.ToString();
		}
		#endregion //Public functions
	}
}
