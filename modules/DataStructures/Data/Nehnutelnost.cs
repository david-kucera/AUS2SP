namespace DataStructures.Data
{
    public class Nehnutelnost : GeoObjekt
    {
        #region Properties
        public List<Parcela> Parcely { get; set; } = new List<Parcela>();           // iba referencie
        #endregion //Properties

        #region Constructor
        public Nehnutelnost()
        {
        }
		#endregion //Constructor

		#region Public functions
		public override string ToString()
		{
			return "Nehnuteľnosť: " + base.ToString();
		}
		#endregion //Public functions
	}
}
