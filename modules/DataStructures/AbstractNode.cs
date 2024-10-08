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

		#region Constructor
		protected AbstractNode()
		{
		}

		protected AbstractNode(AbstractNode<T> parent, AbstractNode<T> left, AbstractNode<T> right, T data)
		{
			Data = data;
			Parent = parent;
			Left = left;
			Right = right;
		}
		#endregion //Constructor

		#region Public functions
		public abstract int CompareTo(AbstractNode<T> node, int dimensionIndex);
		#endregion //Public functions
	}
}
