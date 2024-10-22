using DataStructures;
using DataStructures.Data;

namespace GeoLib
{
    public class KatSys : IKatSys
    {
		#region Class members
		private readonly KdTree<GpsPos, Nehnutelnost> _nehnutelnosti = new(2);
		private readonly KdTree<GpsPos, Parcela> _parcely = new(2);
		private readonly KdTree<GpsPos, GeoObjekt> _objekty = new(2);
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
			var ret = _nehnutelnosti.Find(pozicia);

			for (int i = ret.Count - 1; i > 0; i--)
			{
				if (ret[i].Equals(ret[i - 1]))
				{
					ret.RemoveAt(i);
				}
			}

			return ret;
		}

	    public List<Parcela> VyhladajParcely(GpsPos pozicia)
	    {
		    var ret = _parcely.Find(pozicia);

			for (int i = ret.Count - 1; i > 0; i--)
			{
				if (ret[i].Equals(ret[i - 1]))
				{
					ret.RemoveAt(i);
				}
			}

			return ret;
	    }

	    public List<GeoObjekt> Vyhladaj(GpsPos poz1, GpsPos poz2)
	    {
			var ret = new List<GeoObjekt>();
			var objekty1 = _objekty.Find(poz1);
			var objekty2 = _objekty.Find(poz2);
			ret.AddRange(objekty1);
			ret.AddRange(objekty2);

			for (int i = ret.Count - 1; i > 0; i--)
			{
				if (ret[i].Equals(ret[i - 1]))
				{
					ret.RemoveAt(i);
				}
			}

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

	    public void EditNehnutelnost(Nehnutelnost nehnutelnost, int noveCislo, string novyPopis, GpsPos novaGpsPrva, GpsPos novaGpsDruha)
	    {
			//VymazNehnutelnost(nehnutelnost);
			var novaNehnutelnost = new Nehnutelnost(noveCislo, novyPopis, novaGpsPrva, novaGpsDruha);
			PridajNehnutelnost(novaNehnutelnost);
		}

	    public void EditParcela(Parcela parcela, int noveCislo, string novyPopis, GpsPos novaGpsPrva, GpsPos novaGpsDruha)
	    {
			//VymazParcelu(parcela);
			var novaParcela = new Parcela(noveCislo, novyPopis, novaGpsPrva, novaGpsDruha);
			PridajParcelu(novaParcela);
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
