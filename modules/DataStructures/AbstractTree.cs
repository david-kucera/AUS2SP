namespace DataStructures
{
	public abstract class AbstractTree<TKey, TValue> where TKey : class where TValue : class
	{
		#region Properties
		protected AbstractNode<TKey, TValue> Root { get; set; } = null!;
		public int Count { get; set; } = 0;
		#endregion //Properties

		#region Abstract functions
		public abstract void Insert(TKey key, TValue data);
		public abstract void Delete(TKey key, TValue data);
		public abstract List<TValue> Find(TKey key);
		#endregion //Abstract functions

	}
}
