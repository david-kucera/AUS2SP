namespace DataStructures
{
	public abstract class AbstractTree<T> where T : class
	{
		#region Properties
		public AbstractNode<T> Root { get; set; } = null!;
		public int Count { get; set; } = 0;
		#endregion //Properties

		#region Abstract functions
		public abstract void Insert(T data);
		public abstract void Delete(T data);
		public abstract AbstractNode<T> Find(T data);
		#endregion //Abstract functions

	}
}
