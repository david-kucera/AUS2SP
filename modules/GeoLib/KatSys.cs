using DataStructures;
using DataStructures.Data;

namespace GeoLib
{
    public class KatSys : IKatSys
    {
		#region Class members
		private KdTree<GpsPos, Nehnutelnost> _nehnutelnosti = new(2);
		private KdTree<GpsPos, Parcela> _parcely = new(2);
		private KdTree<GpsPos, GeoObjekt> _objekty = new(2);
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
			// TODO duplicity
			return _nehnutelnosti.Find(pozicia);
		}

	    public List<Parcela> VyhladajParcely(GpsPos pozicia)
	    {
			// TODO duplicity
		    return _parcely.Find(pozicia);
	    }

	    public List<GeoObjekt> Vyhladaj(GpsPos poz1, GpsPos poz2)
	    {
			// TODO duplicity
			var ret = new List<GeoObjekt>();
			ret.AddRange(_objekty.Find(poz1));
			ret.AddRange(_objekty.Find(poz2));
			return ret;
		}

	    public void PridajNehnutelnost(Nehnutelnost nehnutelnost)
	    {
			for (int i = 0; i < nehnutelnost.Pozicie.Length; i++)
		    {
				_nehnutelnosti.Insert(nehnutelnost.Pozicie[i], nehnutelnost);
				_objekty.Insert(nehnutelnost.Pozicie[i], nehnutelnost);
			}

			var nachadzaSaNa = _parcely.Find(nehnutelnost.Pozicie[0]);
			nachadzaSaNa.AddRange(_parcely.Find(nehnutelnost.Pozicie[1]));
			nehnutelnost.Parcely = nachadzaSaNa;
			// TODO refresh pri kazdom pridani
		}

	    public void PridajParcelu(Parcela parcela)
	    {
			for (int i = 0; i < parcela.Pozicie.Length; i++)
		    {
				_parcely.Insert(parcela.Pozicie[i], parcela);
				_objekty.Insert(parcela.Pozicie[i], parcela);
			}

			var nachadzajuSaNaNej = _nehnutelnosti.Find(parcela.Pozicie[0]);
			nachadzajuSaNaNej.AddRange(_nehnutelnosti.Find(parcela.Pozicie[1]));
			parcela.Nehnutelnosti = nachadzajuSaNaNej;
			// TODO refresh pri kazdom pridani
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
		    for (int i = 0; i < nehnutelnost.Pozicie.Length; i++)
		    {
				_nehnutelnosti.Delete(nehnutelnost.Pozicie[i], nehnutelnost);
				_objekty.Delete(nehnutelnost.Pozicie[i], nehnutelnost);
			}
		}

		public void VymazParcelu(Parcela parcela)
	    {
		    for (int i = 0; i < parcela.Pozicie.Length; i++)
		    {
			    _parcely.Delete(parcela.Pozicie[i], parcela);
			    _objekty.Delete(parcela.Pozicie[i], parcela);
		    }
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
