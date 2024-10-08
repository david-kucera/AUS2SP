namespace DataStructures
{
	public class KDTree<T> : AbstractTree<T> where T : class
	{
		#region Properties
		public int Dimension;
		#endregion //Properties

		#region Constructor
		public KDTree(int dimension)
		{
			Dimension = dimension;
		}
		#endregion //Constructor

		#region Public functions
		public override void Insert(T data)
		{
			throw new NotImplementedException();
		}

		public override void Delete(T data)
		{
			throw new NotImplementedException();
		}

		public override AbstractNode<T> Find(T data)
		{
			throw new NotImplementedException();
		}
		#endregion //Public functions
	}
}
