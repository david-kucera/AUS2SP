namespace DataStructures
{
	public abstract class AbstractTree<T> where T : class
	{
		#region Properties
		public AbstractNode<T> Root { get; set; } = null!;
		#endregion //Properties

		#region Constructor
		protected AbstractTree()
		{
		}

		protected AbstractTree(AbstractNode<T> root)
		{
			Root = root;
		}
		#endregion //Constructor

		#region Public functions
		public abstract void Insert(T data);
		public abstract void Delete(T data);
		public abstract AbstractNode<T> Find(T data);
		#endregion //Public functions
	}
}
