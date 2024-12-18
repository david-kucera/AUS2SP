﻿namespace DataStructures.Data
{
    public class Nehnutelnost : GeoObjekt
    {
        #region Properties
        public List<Parcela> Parcely { get; set; } = [];           // iba referencie
        #endregion //Properties

        #region Constructor
        public Nehnutelnost()
        {
	        Typ = 'N';
        }

        public Nehnutelnost(int supisneCislo, string popis, GpsPos prvaGps, GpsPos druhaGps)
		{
			Typ = 'N';
			SupisneCislo = supisneCislo;
			Popis = popis;
			Pozicie[0] = prvaGps;
			Pozicie[1] = druhaGps;
		}
		#endregion //Constructor

		#region Public functions
		public override string ToString()
		{
			return "Nehnuteľnosť: " + base.ToString();
		}
		#endregion //Public functions
	}
}
