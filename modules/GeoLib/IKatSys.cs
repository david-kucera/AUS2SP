using DataStructures.Data;

namespace GeoLib
{
    public interface IKatSys
	{
		List<Nehnutelnost> VyhladajNehnutelnosti(GpsPos pozicia);
		List<Parcela> VyhladajParcely(GpsPos pozicia);
		List<GeoObjekt> Vyhladaj(GpsPos poz1, GpsPos poz2);
		void PridajNehnutelnost(Nehnutelnost nehnutelnost);
		void PridajParcelu(Parcela parcela);
		void EditNehnutelnost(Nehnutelnost nehnutelnost, int noveCislo, string novyPopis, GpsPos novaGpsPrva, GpsPos novaGpsDruha);
		void EditParcela(Parcela parcela, int noveCislo, string novyPopis, GpsPos novaGpsPrva, GpsPos novaGpsDruha);
		void VymazNehnutelnost(Nehnutelnost nehnutelnost);
		void VymazParcelu(Parcela parcela);
	}
}
