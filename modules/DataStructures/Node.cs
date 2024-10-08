namespace DataStructures
{
	public class Node<T> where T : class
	{
		#region Properties
		public T Data { get; set; } = null!;
		public Node<T> Parent { get; set; } = null!;
		public Node<T> Left { get; set; } = null!;
		public Node<T> Right { get; set; } = null!;
		public int Dimension { get; set; }
		#endregion //Properties

		#region Constructor
		public Node()
		{
		}

		public Node(Node<T> parent, Node<T> left, Node<T> right, T data, int dimension)
		{
			Data = data;
			Parent = parent;
			Left = left;
			Right = right;
			Dimension = dimension;
		}
		#endregion //Constructor

		#region Public functions
		public int CompareTo(T data, int dimension)
		{
			var other = data as GeoObjekt;
			var nodeData = Data as GeoObjekt;

			return nodeData!.Pozicie[dimension].CompareTo(other!.Pozicie[dimension], dimension);
		}
		#endregion //Public functions
	}
}
