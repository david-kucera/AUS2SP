using FilesLib.Data;

namespace CarLib
{
	/// <summary>
	/// Rozhranie hlavnej triedy pre prácu s údajmi.
	/// </summary>
	public interface ICar
	{
		Person Find(int id);
		Person Find(string ecv);
		void Add(Person person);
		void AddVisit(Person person, Visit visit);
		void RemoveVisit(Person person, Visit visit);
		void Remove(Person person);
	}
}
