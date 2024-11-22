namespace DataStructures
{
	public abstract class AbstractNode<TKey, TValue> where TKey : class where TValue : class
	{
		#region Properties
		public TValue Data { get; set; } = null!;
		public TKey Key { get; set; } = null!;
		public AbstractNode<TKey, TValue> Parent { get; set; } = null!;
		public AbstractNode<TKey, TValue> Left { get; set; } = null!;
		public AbstractNode<TKey, TValue> Right { get; set; } = null!;
		#endregion //Properties

		#region Abstract functions
		public abstract int CompareTo(TKey key, int dimension);
		#endregion //Abstract functions
	}
}
