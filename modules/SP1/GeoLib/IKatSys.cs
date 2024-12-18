﻿using DataStructures.Data;

namespace GeoLib
{
	/// <summary>
	/// Rozhranie hlavnej triedy pre prácu s údajmi.
	/// </summary>
	public interface IKatSys
	{
		IEnumerable<Nehnutelnost> VyhladajNehnutelnosti(GpsPos pozicia);
		IEnumerable<Parcela> VyhladajParcely(GpsPos pozicia);
		IEnumerable<GeoObjekt> Vyhladaj(GpsPos poz1, GpsPos poz2);
		void PridajNehnutelnost(Nehnutelnost nehnutelnost);
		void PridajParcelu(Parcela parcela);
		void EditNehnutelnost(Nehnutelnost nehnutelnost, int noveCislo, string novyPopis, GpsPos novaGpsPrva, GpsPos novaGpsDruha);
		void EditParcela(Parcela parcela, int noveCislo, string novyPopis, GpsPos novaGpsPrva, GpsPos novaGpsDruha);
		void VymazNehnutelnost(Nehnutelnost nehnutelnost);
		void VymazParcelu(Parcela parcela);
	}
}
