using DataStructures.Data;

namespace DataStructures
{
    public class KdTreeNode<T> : AbstractNode<T> where T : class
	{
		#region Properties
		public int Comparator { get; set; } = 0;
		#endregion //Properties

		#region Constructor
		public KdTreeNode()
		{
			
		}

		public KdTreeNode(T data)
		{
			Comparator = 0;
			Data = data;
			Parent = null!;
			Left = null!;
			Right = null!;
		}

		public KdTreeNode(KdTreeNode<T> parent, KdTreeNode<T> left, KdTreeNode<T> right, T data, int comparator)
		{
			Data = data;
			Parent = parent;
			Left = left;
			Right = right;
			Comparator = comparator;
		}
		#endregion //Constructor

		#region Public functions
		/// <summary>
		/// Funkcia pre porovnanie dvoch kľúčov vrcholov stromu pre zistenie kam v prehliadke pokračovať.
		/// </summary>
		/// <param name="data">Dáta objektu</param>
		/// <param name="comparator">Index GPS súradnice</param>
		/// <param name="dimension">Značka, či porovnávame dĺžku alebo šírku vrcholov</param>
		/// <returns></returns>
		public override int CompareTo(T data, int comparator, int dimension)
		{
			var other = data as GeoObjekt;
			var nodeData = Data as GeoObjekt;
			
			return nodeData!.Pozicie[Comparator].CompareTo(other!.Pozicie[comparator], dimension);
		}

		public override string ToString()
		{
			return Data.ToString()! + " " + "Comparator: " + Comparator;
		}
		#endregion //Public functions
	}
}
