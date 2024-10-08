using DataStructures;
using DataStructures.Data;

namespace GeoLib
{
    public interface IKatSys
	{
		void VyhladajNehnutelnosti(GPSPos pozicia);
		void VyhladajParcely(GPSPos pozicia);
		void Vyhladaj(GPSPos poz1, GPSPos poz2);
		void PridajNehnutelnost(int supisneCislo, string popis, GPSPos[] pozicie);
		void PridajParcelu(int cisloParcely, string popis, GPSPos[] pozicie);
		void EditNehnutelnost(Nehnutelnost nehnutelnost);
		void EditParcela(Parcela parcela);
		void VymazNehnutelnost(Nehnutelnost nehnutelnost);
		void VymazParcelu(Parcela parcela);
	}
}
