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
			// TODO vyhladaj nehnutelnosti na pozicii
		    throw new NotImplementedException();
	    }

	    public List<Parcela> VyhladajParcely(GpsPos pozicia)
	    {
			// TODO vyhladaj parcely na pozicii
			throw new NotImplementedException();
	    }

	    public List<GeoObjekt> Vyhladaj(GpsPos poz1, GpsPos poz2)
	    {
			// TODO vyhladaj objekty na pozicii
			throw new NotImplementedException();
	    }

	    public void PridajNehnutelnost(Nehnutelnost nehnutelnost)
	    {
			_nehnutelnosti.Insert(nehnutelnost);
			_objekty.Insert(nehnutelnost);
	    }

	    public void PridajParcelu(Parcela parcela)
	    {
			_parcely.Insert(parcela);
			_objekty.Insert(parcela);
		}

	    public void EditNehnutelnost(Nehnutelnost nehnutelnost)
	    {
			// TODO edit nehnutelnost
			throw new NotImplementedException();
	    }

	    public void EditParcela(Parcela parcela)
	    {
			// TODO edit parcela
			throw new NotImplementedException();
	    }

	    public void VymazNehnutelnost(Nehnutelnost nehnutelnost)
	    {
			_nehnutelnosti.Delete(nehnutelnost);
			_objekty.Delete(nehnutelnost);
		}

	    public void VymazParcelu(Parcela parcela)
	    {
		    _parcely.Delete(parcela);
			_objekty.Delete(parcela);
		}
	    
		public bool ReadFile(string path)
		{
			// TODO nacitanie dat z csv
			return false;
		}

		public bool SaveFile(string path)
		{
			// TODO ulozenie dat do csv
			return false;
		}
		#endregion //Public functions
	}
}
