namespace DataStructures
{
	public interface IComparable
	{
		int CompareTo(object key, int dimension);
		double GetValue(int dimension);
		bool Equals(object obj);
	}
}
