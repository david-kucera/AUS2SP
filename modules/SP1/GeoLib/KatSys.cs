﻿using DataStructures;
using DataStructures.Data;

namespace GeoLib
{
	/// <summary>
	/// Trieda implementujúca rozhranie programu.
	/// </summary>
	public class KatSys : IKatSys
    {
		#region Constants
		public const string HLAVICKA_CSV = "TYP;SUPISNE_CISLO;POPIS;GPS[0]SIRKA;GPS[0]POZICIA_SIRKY;GPS[0]DLZKA;GPS[0]POZICIA_DLZKY;GPS[1]SIRKA;GPS[1]POZICIA_SIRKY;GPS[1]DLZKA;GPS[1]POZICIA_DLZKY";
		#endregion //Constants

		#region Class members
		private readonly KdTree<GpsPos, Nehnutelnost> _nehnutelnosti = new(2);	// Strom nehnuteľností
		private readonly KdTree<GpsPos, Parcela> _parcely = new(2);				// Strom parciel
		private readonly KdTree<GpsPos, GeoObjekt> _objekty = new(2);			// Strom oboch objektov
		private readonly DataGenerator _generator = new(0);						// Generátor dát
		#endregion //Class members

		#region Constructors
		public KatSys()
		{
		}
		#endregion //Constructors

		#region Public functions
		/// <summary>
		/// Vráti zoznam nehnuteľností na základe zadanej GPS súradnice.
		/// </summary>
		/// <param name="pozicia"></param>
		/// <returns></returns>
		public IEnumerable<Nehnutelnost> VyhladajNehnutelnosti(GpsPos pozicia)
	    {
			return _nehnutelnosti.Find(pozicia).Distinct();
		}

		/// <summary>
		/// Vráti zoznam parciel na základe zadanej GPS súradnice.
		/// </summary>
		/// <param name="pozicia"></param>
		/// <returns></returns>
		public IEnumerable<Parcela> VyhladajParcely(GpsPos pozicia)
	    {
			return _parcely.Find(pozicia).Distinct();
	    }

		/// <summary>
		/// Vráti zoznam všetkých objektov na základe zadaných GPS súradníc.
		/// </summary>
		/// <param name="poz1"></param>
		/// <param name="poz2"></param>
		/// <returns></returns>
		public IEnumerable<GeoObjekt> Vyhladaj(GpsPos poz1, GpsPos poz2)
	    {
			var ret = new List<GeoObjekt>();
			var objekty1 = _objekty.Find(poz1);
			var objekty2 = _objekty.Find(poz2);
			ret.AddRange(objekty1);
			ret.AddRange(objekty2);

			return ret.Distinct();
		}

		/// <summary>
		/// Pridá nehnuteľnosť do stromu nehnuteľností a stromu všetkých objektov.
		/// </summary>
		/// <param name="nehnutelnost"></param>
		public void PridajNehnutelnost(Nehnutelnost nehnutelnost)
	    {
			for (int i = 0; i < nehnutelnost.Pozicie.Length; i++)
		    {
				_nehnutelnosti.Insert(nehnutelnost.Pozicie[i], nehnutelnost);
				_objekty.Insert(nehnutelnost.Pozicie[i], nehnutelnost);
			}

			var nachadzaSaNaParcelach = _parcely.Find(nehnutelnost.Pozicie[0]);
			nachadzaSaNaParcelach.AddRange(_parcely.Find(nehnutelnost.Pozicie[1]));
			nehnutelnost.Parcely = nachadzaSaNaParcelach; // Naplnenie referencií pridávanej nehnutelnosti

			// Naplnenie každej parcele referenciu na pridávanú nehnutelnosti
			for (int i = 0; i < nachadzaSaNaParcelach.Count; i++)
			{
				nachadzaSaNaParcelach[i].Nehnutelnosti.Add(nehnutelnost);
			}
		}

		/// <summary>
		/// Pridá parcelu do stromu parciel a stromu všetkých objektov.
		/// </summary>
		/// <param name="parcela"></param>
		public void PridajParcelu(Parcela parcela)
	    {
			for (int i = 0; i < parcela.Pozicie.Length; i++)
		    {
				_parcely.Insert(parcela.Pozicie[i], parcela);
				_objekty.Insert(parcela.Pozicie[i], parcela);
			}

			var nachadzajuSaNaNejNehnutelnosti = _nehnutelnosti.Find(parcela.Pozicie[0]);
			nachadzajuSaNaNejNehnutelnosti.AddRange(_nehnutelnosti.Find(parcela.Pozicie[1]));
			parcela.Nehnutelnosti = nachadzajuSaNaNejNehnutelnosti; // Naplnenie referencií na nehnutelnosti

			// Naplnenie každej nehnutelnosti referenciu na pridávanú parcelu
			for (int i = 0; i < nachadzajuSaNaNejNehnutelnosti.Count; i++)
			{
				nachadzajuSaNaNejNehnutelnosti[i].Parcely.Add(parcela);
			}
		}

		/// <summary>
		/// Edituje zvolenú nehnuteľnosť => vymaže ju a pridá novú s novými dátami => alebo len upraví dáta zvolenej nehnuteľnosti ak neboli menené kľúčové prvky.
		/// </summary>
		/// <param name="nehnutelnost"></param>
		/// <param name="noveCislo"></param>
		/// <param name="novyPopis"></param>
		/// <param name="novaGpsPrva"></param>
		/// <param name="novaGpsDruha"></param>
		public void EditNehnutelnost(Nehnutelnost nehnutelnost, int noveCislo, string novyPopis, GpsPos novaGpsPrva, GpsPos novaGpsDruha)
	    {
			if (nehnutelnost.Pozicie[0].Equals(novaGpsPrva) && nehnutelnost.Pozicie[1].Equals(novaGpsDruha))
			{
				nehnutelnost.SupisneCislo = noveCislo;
				nehnutelnost.Popis = novyPopis;
			}
			else
			{
				VymazNehnutelnost(nehnutelnost);
				var novaNehnutelnost = new Nehnutelnost(noveCislo, novyPopis, novaGpsPrva, novaGpsDruha);
				PridajNehnutelnost(novaNehnutelnost);
			}
		}

		/// <summary>
		/// Edituje zvolenú parcelu => vymaže ju a pridá novú s novými dátami => alebo len upraví dáta zvolenej parcely ak neboli menené kľúčové prvky.
		/// </summary>
		/// <param name="parcela"></param>
		/// <param name="noveCislo"></param>
		/// <param name="novyPopis"></param>
		/// <param name="novaGpsPrva"></param>
		/// <param name="novaGpsDruha"></param>
		public void EditParcela(Parcela parcela, int noveCislo, string novyPopis, GpsPos novaGpsPrva, GpsPos novaGpsDruha)
	    {
			if (parcela.Pozicie[0].Equals(novaGpsPrva) && parcela.Pozicie[1].Equals(novaGpsDruha))
			{
				parcela.SupisneCislo = noveCislo;
				parcela.Popis = novyPopis;
			}
			else
			{
				VymazParcelu(parcela);
				var novaParcela = new Parcela(noveCislo, novyPopis, novaGpsPrva, novaGpsDruha);
				PridajParcelu(novaParcela);
			}
		}

		/// <summary>
		/// Vymaže zvolenú nehnuteľnosť zo stromu nehnuteľností a stromu všetkých objektov.
		/// </summary>
		/// <param name="nehnutelnost"></param>
		public void VymazNehnutelnost(Nehnutelnost nehnutelnost)
	    {
			// Odstránenie každej parcele referenciu na odstraňovanú nehnutelnosť
			for (int i = 0; i < nehnutelnost.Parcely.Count; i++)
			{
				nehnutelnost.Parcely[i].Nehnutelnosti.Remove(nehnutelnost);
			}

			for (int i = 0; i < nehnutelnost.Pozicie.Length; i++)
		    {
				_nehnutelnosti.Remove(nehnutelnost.Pozicie[i], nehnutelnost);
				_objekty.Remove(nehnutelnost.Pozicie[i], nehnutelnost);
			}
		}

		/// <summary>
		/// Vymaže zvolenú parcelu zo stromu parciel a stromu všetkých objektov.
		/// </summary>
		/// <param name="parcela"></param>
		public void VymazParcelu(Parcela parcela)
	    {
			// Odstránenie každej nehnutelnosti referenciu na odstraňovanú parcelu
			for (int i = 0; i < parcela.Nehnutelnosti.Count; i++)
			{
				parcela.Nehnutelnosti[i].Parcely.Remove(parcela);
			}

			for (int i = 0; i < parcela.Pozicie.Length; i++)
		    {
			    _parcely.Remove(parcela.Pozicie[i], parcela);
			    _objekty.Remove(parcela.Pozicie[i], parcela);
		    }
		}

		/// <summary>
		/// Metóda na načítanie dát zo súboru.
		/// </summary>
		/// <param name="path">Cesta k súboru na načítanie</param>
		/// <returns>True, ak boli dáta úspšene načítané</returns>
		public bool ReadFile(string path)
		{
			string[] lines;
			try
			{
				lines = File.ReadAllLines(path);
			}
			catch (Exception)
			{
				return false;
			}
			
			if (lines.Length == 0) return false;
			if (lines[0] != HLAVICKA_CSV) return false;

			for (int i = 1; i < lines.Length; i++)
			{
				var line = lines[i];
				var parts = line.Split(';');
				if (parts.Length != 11) return false;

				var typ = parts[0];
				var supisneCislo = int.Parse(parts[1]);
				var popis = parts[2];
				var gps0 = new GpsPos(parts[3].First(), double.Parse(parts[4]), parts[5].First(), double.Parse(parts[6]));
				var gps1 = new GpsPos(parts[7].First(), double.Parse(parts[8]), parts[9].First(), double.Parse(parts[10]));

				if (typ == "N")
				{
					var nehnutelnost = new Nehnutelnost(supisneCislo, popis, gps0, gps1);
					PridajNehnutelnost(nehnutelnost);
				}
				else if (typ == "P")
				{
					var parcela = new Parcela(supisneCislo, popis, gps0, gps1);
					PridajParcelu(parcela);
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Metóda na uloženie dát do súboru.
		/// </summary>
		/// <param name="path">Cesta k súbpru</param>
		/// <returns>True, ak sa úspešne dáta uložia</returns>
		public bool SaveFile(string path)
		{
			var allObjects = GetAllObjects();
			string saveData = string.Empty;
			saveData += HLAVICKA_CSV + "\n";
			foreach (var obj in allObjects)
			{
				if (obj == null!) continue;
				saveData += obj.ToFile() + "\n";
			}

			try
			{
				File.WriteAllText(path, saveData);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Vráti zoznam všetkých objektov v strome všetkých objektov.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<GeoObjekt> GetAllObjects()
		{
			return _objekty.GetAll().Distinct();
		}

		/// <summary>
		/// Vráti reťazec s informáciami o strome.
		/// </summary>
		/// <returns></returns>
		public string ZobrazTotalInfo()
		{
			return $"STROM PARCIEL\n" +
				$"Počet prvkov: {_parcely.Count}" +
				$"\n" +
				$"STROM NEHNUTELNOSTI\n" +
				$"Počet prvkov: {_nehnutelnosti.Count}" +
				$"\n" +
				$"STROM OBOCH\n" +
				$"Počet prvkov: {_objekty.Count}" +
				$"\n";
		}

		/// <summary>
		/// Vygeneruje zadaný počet parciel a vloží ich do stromu.
		/// </summary>
		/// <param name="count">Počet parciel na vygenerovanie</param>
		public void GenerujParcely(int count)
		{
			for (int i = 0; i < count; i++)
			{
				var parcela = _generator.GenerateParcela(i);
				PridajParcelu(parcela);
			}
		}

		/// <summary>
		/// Vygeneruje zadaný počet nehnuteľností a vloží ich do stromu.
		/// </summary>
		/// <param name="count">Počet nehnuteľností na vygenerovanie</param>
		public void GenerujNehnutelnosti(int count)
		{
			for (int i = 0; i < count; i++)
			{
				var nehnutelnost = _generator.GenerateNehnutelnost(i);
				PridajNehnutelnost(nehnutelnost);
			}
		}

		/// <summary>
		/// Vygeneruje zadaný počet nehnuteľností a parciel a vloží ich do stromu.
		/// </summary>
		/// <param name="countNehn">Počet nehnuteľností na vygenerovanie</param>
		/// <param name="countParc">Počet parciel na vygenerovanie</param>
		/// <param name="perc">Percentuálny prekryv nehnuteľností a parciel</param>
		public void GenerujData(int countNehn, int countParc, double perc)
		{
			List<Parcela> parcely = [];
			List<Nehnutelnost> nehnutelnosti = [];
			for (int i = 0; i < countParc; i++)
			{
				parcely.Add(_generator.GenerateParcela(i));
			}

			// Generujem nehnutelnosti, ktore sa budu nachadzat na nejakej parcele
			double pocet = countNehn * (perc / 100);
			for (int i = 0; i < (int)pocet; i++)
			{
				var nahodnaGpsParcely = parcely[i % countParc].Pozicie[i % 2];
				var nehnutelnost = _generator.GenerateNehnutelnost(i);
				nehnutelnost.Pozicie[i % 2] = nahodnaGpsParcely;
				nehnutelnosti.Add(nehnutelnost);
			}

			// Generujem ostatne nehnutelnosti
			for (int i = 0; i < countNehn * (1 - (perc / 100)); i++)
			{
				nehnutelnosti.Add(_generator.GenerateNehnutelnost(i));
			}

			foreach (var parcela in parcely)
			{
				PridajParcelu(parcela);
			}

			foreach (var nehnutelnost in nehnutelnosti)
			{
				PridajNehnutelnost(nehnutelnost);
			}
		}
		#endregion //Public functions
	}
}
