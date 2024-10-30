using DataStructures;
using DataStructures.Data;

namespace GeoLib
{
	/// <summary>
	/// Trieda implementujúca rozhranie programu.
	/// </summary>
	public class KatSys : IKatSys
    {
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

		public KatSys(string path)
		{
			// TODO load data from external file
			throw new System.NotImplementedException();
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
		/// Edituje zvolenú nehnuteľnosť => vymaže ju a pridá novú s novými dátami.
		/// </summary>
		/// <param name="nehnutelnost"></param>
		/// <param name="noveCislo"></param>
		/// <param name="novyPopis"></param>
		/// <param name="novaGpsPrva"></param>
		/// <param name="novaGpsDruha"></param>
		public void EditNehnutelnost(Nehnutelnost nehnutelnost, int noveCislo, string novyPopis, GpsPos novaGpsPrva, GpsPos novaGpsDruha)
	    {
			VymazNehnutelnost(nehnutelnost);
			var novaNehnutelnost = new Nehnutelnost(noveCislo, novyPopis, novaGpsPrva, novaGpsDruha);
			PridajNehnutelnost(novaNehnutelnost);
		}

		/// <summary>
		/// Edituje zvolenú parcelu => vymaže ju a pridá novú s novými dátami.
		/// </summary>
		/// <param name="parcela"></param>
		/// <param name="noveCislo"></param>
		/// <param name="novyPopis"></param>
		/// <param name="novaGpsPrva"></param>
		/// <param name="novaGpsDruha"></param>
		public void EditParcela(Parcela parcela, int noveCislo, string novyPopis, GpsPos novaGpsPrva, GpsPos novaGpsDruha)
	    {
			VymazParcelu(parcela);
			var novaParcela = new Parcela(noveCislo, novyPopis, novaGpsPrva, novaGpsDruha);
			PridajParcelu(novaParcela);
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
		/// <param name="path"></param>
		/// <returns></returns>
		public bool ReadFile(string path)
		{
			// TODO nacitanie dat z csv
			return false;
		}

		/// <summary>
		/// Metóda na uloženie dát do súboru.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public bool SaveFile(string path)
		{
			var allObjects = GetAllObjects();
			string saveData = string.Empty;
			foreach (var obj in allObjects)
			{
				if (obj == null) continue;
				saveData += obj.ToFile() + "\n";
			}

			try
			{
				File.WriteAllText(path, saveData);
				return true;
			}
			catch (Exception e)
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

		public void GenerujParcely(int count)
		{
			for (int i = 0; i < count; i++)
			{
				var parcela = _generator.GenerateParcela(i);
				PridajParcelu(parcela);
			}
		}

		public void GenerujNehnutelnosti(int count)
		{
			for (int i = 0; i < count; i++)
			{
				var nehnutelnost = _generator.GenerateNehnutelnost(i);
				PridajNehnutelnost(nehnutelnost);
			}
		}
		#endregion //Public functions
	}
}
