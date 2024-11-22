namespace FilesLib.Interfaces
{
	public interface IData<T> : IRecord<T> where T : class
	{
		T CreateClass();
		bool Equals(T data);
	}
}
