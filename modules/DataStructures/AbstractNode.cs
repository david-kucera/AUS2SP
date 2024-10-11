namespace DataStructures
{
	public abstract class AbstractNode<T> where T : class
	{
		#region Properties
		public T Data { get; set; } = null!;
		public AbstractNode<T> Parent { get; set; } = null!;
		public AbstractNode<T> Left { get; set; } = null!;
		public AbstractNode<T> Right { get; set; } = null!;
		#endregion //Properties

		#region Abstract functions
		public abstract int CompareTo(T data, int comparator, int dimension);
		#endregion //Abstract functions
	}
}
