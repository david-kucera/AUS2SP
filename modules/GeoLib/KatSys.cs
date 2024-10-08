using DataStructures;
using DataStructures.Data;

namespace GeoLib
{
    class KatSys : IKatSys
    {
		#region Properties
		public KdTree<Nehnutelnost> Nehnutelnosti { get; private set; } = new KdTree<Nehnutelnost>(2);
		public KdTree<Parcela> Parcely { get; private set; } = new KdTree<Parcela>(2);
		public KdTree<GeoObjekt> Objekty { get; private set; } = new KdTree<GeoObjekt>(2);
		#endregion //Properties

		#region Public functions
		public void VyhladajNehnutelnosti(GPSPos pozicia)
	    {
		    throw new NotImplementedException();
	    }

	    public void VyhladajParcely(GPSPos pozicia)
	    {
		    throw new NotImplementedException();
	    }

	    public void Vyhladaj(GPSPos poz1, GPSPos poz2)
	    {
		    throw new NotImplementedException();
	    }

	    public void PridajNehnutelnost(int supisneCislo, string popis, GPSPos[] pozicie)
	    {
		    throw new NotImplementedException();
	    }

	    public void PridajParcelu(int cisloParcely, string popis, GPSPos[] pozicie)
	    {
		    throw new NotImplementedException();
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
