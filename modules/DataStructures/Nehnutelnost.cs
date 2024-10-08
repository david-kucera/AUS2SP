namespace DataStructures
{
	public class Nehnutelnost : GeoObjekt
	{
		#region Properties
		public List<Parcela> Parcely { get; set; } = new List<Parcela>();			// iba referencie
		#endregion //Properties

		#region Constructor
		public Nehnutelnost()
		{
		}
		#endregion //Constructor
	}
}
