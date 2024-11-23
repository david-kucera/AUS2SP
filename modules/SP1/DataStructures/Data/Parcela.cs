namespace DataStructures.Data
{
    public class Parcela : GeoObjekt
    {
        #region Properties
        public List<Nehnutelnost> Nehnutelnosti { get; set; } = [];       // iba referencie
        #endregion //Properties

        #region Constructor
        public Parcela()
        {
	        Typ = 'P';
        }

        public Parcela(int cisloparcely, string popis, GpsPos prvaGps, GpsPos druhaGps)
        {
	        Typ = 'P';
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
