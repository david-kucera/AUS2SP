using DataStructures;
using DataStructures.Data;

namespace GeoLib
{
    public class KatSys : IKatSys
    {
		#region Class members
		private KdTree<Nehnutelnost> _nehnutelnosti = new(2);
		private KdTree<Parcela> _parcely = new(2);
		private KdTree<GeoObjekt> _objekty = new(2);
		#endregion //Class members

		#region Constructors
		public KatSys()
		{
		}

		public KatSys(string path)
		{
			// TODO load data from external file
			throw new System.NotImplementedException();
		}
		#endregion //Constructors

		#region Public functions
		public List<Nehnutelnost> VyhladajNehnutelnosti(GpsPos pozicia)
	    {
		    throw new NotImplementedException();
	    }

	    public List<Parcela> VyhladajParcely(GpsPos pozicia)
	    {
		    throw new NotImplementedException();
	    }

	    public List<GeoObjekt> Vyhladaj(GpsPos poz1, GpsPos poz2)
	    {
		    throw new NotImplementedException();
	    }

	    public void PridajNehnutelnost(Nehnutelnost nehnutelnost)
	    {
			_nehnutelnosti.Insert(nehnutelnost);
		    //throw new NotImplementedException();
	    }

	    public void PridajParcelu(Parcela parcela)
	    {

		    //throw new NotImplementedException();
	    }

	    public void EditNehnutelnost(Nehnutelnost nehnutelnost)
	    {
		    throw new NotImplementedException();
	    }

	    public void EditParcela(Parcela parcela)
	    {
		    throw new NotImplementedException();
	    }

	    public void VymazNehnutelnost(Nehnutelnost nehnutelnost)
	    {
		    throw new NotImplementedException();
	    }

	    public void VymazParcelu(Parcela parcela)
	    {
		    throw new NotImplementedException();
	    }
		#endregion //Public functions
	}
}
