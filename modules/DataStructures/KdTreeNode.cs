using DataStructures.Data;

namespace DataStructures
{
    public class KdTreeNode<T> : AbstractNode<T> where T : class
	{
		#region Properties
		public int Dimension { get; set; } // Znacka, ci porovnavam prvu suradnicu, alebo druhu
		#endregion //Properties

		#region Constructor
		public KdTreeNode()
		{
			Dimension = 0;
		}

		public KdTreeNode(AbstractNode<T> parent, AbstractNode<T> left, AbstractNode<T> right, T data, int dimension)
		{
			Data = data;
			Parent = parent;
			Left = left;
			Right = right;
			Dimension = dimension;
		}
		#endregion //Constructor

		#region Public functions
		public override int CompareTo(T data, int dimension, int positionIndex)
		{
			var other = data as GeoObjekt;
			var nodeData = Data as GeoObjekt;

			return nodeData!.Pozicie[Dimension].CompareTo(other!.Pozicie[positionIndex], dimension);
		}
		#endregion //Public functions
	}
}
