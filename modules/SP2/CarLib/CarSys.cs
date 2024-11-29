using FilesLib.Data;
using FilesLib.Generator;
using FilesLib.Hash;
using FilesLib.Heap;

namespace CarLib
{
	/// <summary>
	/// Trieda implementujúca rozhranie programu.
	/// </summary>
	public class CarSys : ICar
	{
		#region Constants
		private const int BLOCK_SIZE = 4096 * 2;
		private const string INIT_FILE_HEAP = "../../userdata/heap_init.aus";
        private const string INIT_FILE_ID = "../../userdata/hashId_init.aus";
        private const string INIT_FILE_ECV = "../../userdata/hashEcv_init.aus";
        private const string DATA_FILE = "../../userdata/person.aus";
		#endregion // Constants

		#region Class members
		private HeapFile<Person> _heapFile;
        private readonly ExtendibleHashFile<VisitId> _hashFileId;
		private readonly ExtendibleHashFile<VisitEcv> _hashFileEcv;
        #endregion // Class members

        #region Constructor
        public CarSys()
		{
			if (File.Exists(DATA_FILE)) File.Delete(DATA_FILE);
			if (File.Exists(INIT_FILE_HEAP)) File.Delete(INIT_FILE_HEAP);
			if (File.Exists(INIT_FILE_ID)) File.Delete(INIT_FILE_ID);
			if (File.Exists(INIT_FILE_ECV)) File.Delete(INIT_FILE_ECV);

			_heapFile = new HeapFile<Person>(INIT_FILE_HEAP, DATA_FILE, BLOCK_SIZE);
            _hashFileId = new ExtendibleHashFile<VisitId>(INIT_FILE_ID, _heapFile.BlockFactor);
            _hashFileEcv = new ExtendibleHashFile<VisitEcv>(INIT_FILE_ECV, _heapFile.BlockFactor);
        }
		#endregion // Constructor

		#region Public functions
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

		public void AddVisit(Person person, Visit visit)
		{
			// TODO
			throw new NotImplementedException();
		}

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

		public void RemoveVisit(Person person, Visit visit)
		{
			// TODO
			throw new NotImplementedException();
		}

		public void Remove(Person person)
		{
            // TODO
            //_hashFileId.Delete(person);
        }

        public void GenerujData(int count)
		{
			var generator = new DataGenerator(count % 100);
			for (int i = 0; i < count; i++)
			{
				var person = generator.GeneratePerson();
				Add(person);
			}
		}

		public void Close()
        {
            _heapFile.Close();
            _hashFileId.Close();
            _hashFileEcv.Close();
        }

		public void Clear()
		{
			_heapFile.Clear();
			_hashFileId.Clear();
			_hashFileEcv.Clear();
		}

		public string ZobrazHeapFileInfo()
        {
			string result = _heapFile.ToString();
			result += "\n\n";
			result += _heapFile.SequentialOutput();
			return result;
		}

        public string ZobrazHashFileIdInfo()
        {
	        return _hashFileId.SequentialOutput();
        }

        public string ZobrazHashFileEcvInfo()
        {
            return _hashFileEcv.SequentialOutput();
		}

        public void CheckId(int id)
        {
	        var person = Find(id);
	        if (person != null)
	        {
		        throw new Exception("Osoba s ID " + id + " bola nájdená!");
	        }
        }

        public void CheckEcv(string ecv)
        {
	        var person = Find(ecv);
	        if (person != null)
	        {
		        throw new Exception("Osoba s ECV " + ecv + " bola nájdená!");
	        }
        }
		#endregion // Public functions
	}
}
