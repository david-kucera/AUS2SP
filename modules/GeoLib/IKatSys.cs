using DataStructures;
using DataStructures.Data;

namespace GeoLib
{
    public interface IKatSys
	{
		void VyhladajNehnutelnosti(GpsPos pozicia);
		void VyhladajParcely(GpsPos pozicia);
		void Vyhladaj(GpsPos poz1, GpsPos poz2);
		void PridajNehnutelnost(int supisneCislo, string popis, GpsPos[] pozicie);
		void PridajParcelu(int cisloParcely, string popis, GpsPos[] pozicie);
		void EditNehnutelnost(Nehnutelnost nehnutelnost);
		void EditParcela(Parcela parcela);
		void VymazNehnutelnost(Nehnutelnost nehnutelnost);
		void VymazParcelu(Parcela parcela);
	}
}
