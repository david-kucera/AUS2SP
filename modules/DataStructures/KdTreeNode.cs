using DataStructures.Data;

namespace DataStructures
{
    public class KdTreeNode<TKey, TValue> : AbstractNode<TKey, TValue> where TKey : class where TValue : class
	{
		#region Constructor
		public KdTreeNode()
		{
			
		}

		public KdTreeNode(TValue data, TKey key)
		{
			Key = key;
			Data = data;
			Parent = null!;
			Left = null!;
			Right = null!;
		}

		public KdTreeNode(KdTreeNode<TKey, TValue> parent, KdTreeNode<TKey, TValue> left, KdTreeNode<TKey, TValue> right, TValue data, TKey key)
		{
			Data = data;
			Parent = parent;
			Left = left;
			Right = right;
			Key = key;
		}
		#endregion //Constructor

		#region Public functions
		/// <summary>
		/// Funkcia pre porovnanie dvoch kľúčov vrcholov stromu pre zistenie kam v prehliadke pokračovať.
		/// </summary>
		/// <param name="key">Kľúč objektu</param>
		/// <param name="dimension">Značka, či porovnávame dĺžku alebo šírku vrcholov</param>
		/// <returns></returns>
		public override int CompareTo(TKey key, int dimension)
		{
			var thisKey = Key as GpsPos;
			var otherKey = key as GpsPos;

			return thisKey!.CompareTo(otherKey!, dimension);
		}

		public double GetKeyValue(int dimension)
		{
			var key = Key as GpsPos;
			return key!.GetValue(dimension);
		}

		public override string ToString()
		{
			return Key.ToString()! + " " + Data.ToString();
		}
		#endregion //Public functions
	}
}
