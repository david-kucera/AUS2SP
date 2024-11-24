using FilesLib.Data;
using FilesLib.Hash;

namespace CarLib
{
	/// <summary>
	/// Trieda implementujúca rozhranie programu.
	/// </summary>
	public class CarSys : ICar
	{
		#region Constants
		private const int BLOCK_SIZE = 1024;
		private const string INIT_FILE = "../../userdata/person_init.aus";
		private const string DATA_FILE = "../../userdata/person.aus";
		#endregion // Constants

		#region Class members
		private readonly ExtendibleHashFile<Person> _hashFile;
		#endregion // Class members

		#region Constructor
		public CarSys()
		{
			_hashFile = new ExtendibleHashFile<Person>(INIT_FILE, DATA_FILE, BLOCK_SIZE);
		}
		#endregion // Constructor

		#region Public functions
		public Person Find(int id)
		{
			Person dummy = new()
			{
				Id = id
			};
			return _hashFile.Find(dummy);
		}

		public Person Find(string ecv)
		{
			Person dummy = new()
			{
				Ecv = ecv
			};
			return _hashFile.Find(dummy);
		}

		public List<Person> GetAllPeople()
		{
			// TODO
			throw new NotImplementedException();
		}

		public void Add(Person person)
		{
			_hashFile.Insert(person);
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
			_hashFile.Delete(person);
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
		#endregion // Public functions
	}
}
