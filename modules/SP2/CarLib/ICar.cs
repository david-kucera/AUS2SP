using FilesLib.Data;

namespace CarLib
{
	public interface ICar
	{
		Person Find(int id);
		Person Find(string ecv);
		void Add(Person person);
		void AddVisit(Person person);
		void RemoveVisit(Person person);
		void Remove(Person person);
	}
}
