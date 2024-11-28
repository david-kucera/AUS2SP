using FilesLib.Data;
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
		private const int BLOCK_SIZE = 1024;
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
            _heapFile = new HeapFile<Person>(INIT_FILE_HEAP, DATA_FILE, BLOCK_SIZE);
            _hashFileId = new ExtendibleHashFile<VisitId>(INIT_FILE_ID, _heapFile.BlockCount);
            _hashFileEcv = new ExtendibleHashFile<VisitEcv>(INIT_FILE_ECV, _heapFile.BlockCount);
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
			var heapFileAddress = obj.Address;
            Person personData = new()
			{
                Ecv = ecv
            };
            return _heapFile.Find(heapFileAddress, personData);
		}

		public List<Person> GetAllPeople()
		{
			// TODO
			throw new NotImplementedException();
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
			// TODO
			throw new NotImplementedException();
		}

		public string ZobrazTotalInfo()
		{
			// TODO basic info o strukture 
			throw new NotImplementedException();
		}

		public void Close()
        {
            _heapFile.Close();
            _hashFileId.Close();
            _hashFileEcv.Close();
        }
        #endregion // Public functions
    }
}
