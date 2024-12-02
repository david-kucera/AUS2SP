using FilesLib.Data;
using FilesLib.Generator;
using FilesLib.Hash;
using FilesLib.Heap;

namespace CarLib
{
	/// <summary>
	/// Trieda implementujúca rozhranie programu.
	/// </summary>
	public class CarSys
	{
		#region Constants
		private const int BLOCK_SIZE = 5000;
		private const string INIT_FILE_HEAP = "../../userdata/heap_init.aus";
        private const string DATA_FILE = "../../userdata/person.aus";

        private const string INIT_FILE_HASH_ID = "../../userdata/hash_init_id.aus";
        private const string INIT_FILE_HEAP_HASH_ID = "../../userdata/hash_init_heap_id.aus";
        private const string DATA_FILE_HEAP_HASH_ID = "../../userdata/hash_heap_id.aus";

        private const string INIT_FILE_HASH_ECV = "../../userdata/hash_init_ecv.aus";
        private const string INIT_FILE_HEAP_HASH_ECV = "../../userdata/hash_init_heap_ecv.aus";
        private const string DATA_FILE_HEAP_HASH_ECV = "../../userdata/hash_heap_ecv.aus";
        #endregion // Constants

        #region Class members
        private HeapFile<Person> _heapFile;
        private readonly ExtendibleHashFile<VisitId> _hashFileId;
		private readonly ExtendibleHashFile<VisitEcv> _hashFileEcv;
        #endregion // Class members

        #region Constructor
        public CarSys()
		{
			_heapFile = new HeapFile<Person>(INIT_FILE_HEAP, DATA_FILE, BLOCK_SIZE);
            _hashFileId = new ExtendibleHashFile<VisitId>(INIT_FILE_HASH_ID, INIT_FILE_HEAP_HASH_ID, DATA_FILE_HEAP_HASH_ID, BLOCK_SIZE);
            _hashFileEcv = new ExtendibleHashFile<VisitEcv>(INIT_FILE_HASH_ECV, INIT_FILE_HEAP_HASH_ECV, DATA_FILE_HEAP_HASH_ECV, BLOCK_SIZE);
        }
		#endregion // Constructor

		#region Public functions
		/// <summary>
		/// Metóda na vyhľadanie osoby podľa ID.
		/// </summary>
		/// <param name="id">Id osoby</param>
		/// <returns>Person</returns>
		public Person Find(int id)
		{
			VisitId dummy = new()
			{
				Id = id
			};
			var obj = _hashFileId.Find(dummy);
			if (obj == null!) return null!;

			var heapFileAddress = obj.Address;
            Person personData = new()
            {
                Id = id
            };
            return _heapFile.Find(heapFileAddress, personData);
		}

		/// <summary>
		/// Metóda na vyhľadanie osoby podľa EČV.
		/// </summary>
		/// <param name="ecv">EČV vozidla</param>
		/// <returns>Person</returns>
		public Person Find(string ecv)
		{
			VisitEcv dummy = new()
			{
				Ecv = ecv
			};
			var obj = _hashFileEcv.Find(dummy);
			if (obj == null!) return null!;

			var heapFileAddress = obj.Address;
            Person personData = new()
			{
                Ecv = ecv
            };
            return _heapFile.Find(heapFileAddress, personData);
		}

		/// <summary>
		/// Metóda na pridanie osoby do súboru.
		/// </summary>
		/// <param name="person">Person</param>
		public void Add(Person person)
		{
			var address = _heapFile.Insert(person);
			VisitId visitId = new()
			{
				Address = address,
				Id = person.Id
			};
			VisitEcv visitEcv = new()
			{
				Address = address,
				Ecv = person.Ecv
			};
            _hashFileId.Insert(visitId);
			_hashFileEcv.Insert(visitEcv);
        }

		/// <summary>
		/// Metóda na aktualizáciu dát osoby.
		/// </summary>
		/// <param name="updatedPerson">Person</param>
		/// <exception cref="Exception">Ak nastane chyba pri upravovaní osoby.</exception>
		public void Update(Person updatedPerson)
		{
			var address = _hashFileId.Find(new VisitId { Id = updatedPerson.Id }).Address;
			try
			{
				_heapFile.Update(address, updatedPerson);
			}
			catch (Exception ex)
			{
				throw new Exception("Chyba pri upravovaní osoby: " + ex.Message);
			}
		}

		public void UpdateKeyChanged(Person editedPerson, int oldId, string oldEcv)
		{
			// TODO - najskor zmazat z indexov, heap file a potom vlozit s novymi hodnotami
			throw new NotImplementedException();
		}

		public void Remove(Person person)
		{
			// TODO
			try
			{
				var visitId = new VisitId { Id = person.Id };
				var address = _hashFileId.Find(visitId).Address;
				_hashFileId.Delete(visitId);
				var visitEcv = new VisitEcv { Ecv = person.Ecv };
				_hashFileEcv.Delete(visitEcv);

				_heapFile.Delete(address, person);
			}
			catch (Exception ex)
			{
				throw new Exception("Chyba pri mazaní osoby: " + ex.Message);
			}
		}

		/// <summary>
		/// Metóda na generovanie náhodných dát.
		/// </summary>
		/// <param name="count">Integer</param>
		public void GenerujData(int count)
		{
			var generator = new DataGenerator(count % 100);
			for (int i = 0; i < count; i++)
			{
				var person = generator.GeneratePerson();
				Add(person);
			}
		}

		/// <summary>
		/// Metóda na zatvorenie súborov.
		/// </summary>
		public void Close()
        {
            _heapFile.Close();
            _hashFileId.Close();
            _hashFileEcv.Close();
        }

		/// <summary>
		/// Metóda na vymazanie všetkých dát zo súborov.
		/// </summary>
		public void Clear()
		{
			_heapFile.Clear();
			_hashFileId.Clear();
			_hashFileEcv.Clear();
		}

		/// <summary>
		/// Metóda na zobrazenie informácií o heap file.
		/// </summary>
		/// <returns>string</returns>
		public string ZobrazHeapFileInfo()
        {
			return _heapFile.ToString() + "\n\n" + _heapFile.SequentialOutput();
		}

		/// <summary>
		/// Metóda na zobrazenie informácií o hash file ID.
		/// </summary>
		/// <returns>string</returns>
		public string ZobrazHashFileIdInfo()
        {
			return _hashFileId.ToString() + "\n\n" + _hashFileId.SequentialOutput();
        }

		/// <summary>
		/// Metóda na zobrazenie informácií o hash file EČV.
		/// </summary>
		/// <returns>string</returns>
		public string ZobrazHashFileEcvInfo()
        {
			return _hashFileEcv.ToString() + "\n\n" + _hashFileEcv.SequentialOutput();
		}

		/// <summary>
		/// Metóda na kontrolu unikátnosti ID.
		/// </summary>
		/// <param name="id">ID</param>
		/// <exception cref="Exception">Ak osoba s daným ID bola nájdená.</exception>
		public void CheckId(int id)
        {
	        var person = Find(id);
	        if (person != null) throw new Exception("Osoba s ID " + id + " bola nájdená!");
        }

		/// <summary>
		/// Metóda na kontrolu unikátnosti EČV.
		/// </summary>
		/// <param name="ecv">EČV</param>
		/// <exception cref="Exception">Ak osoba s daným EČV bola nájdená.</exception>
		public void CheckEcv(string ecv)
        {
	        var person = Find(ecv);
	        if (person != null) throw new Exception("Osoba s ECV " + ecv + " bola nájdená!");
        }
		#endregion // Public functions
	}
}
