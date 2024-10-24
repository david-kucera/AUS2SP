namespace DataStructures
{
	/// <summary>
	/// Trieda representujúca vrchol stromu.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class KdTreeNode<TKey, TValue> : AbstractNode<TKey, TValue> where TKey : class where TValue : class
	{
		#region Constructors
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
		#endregion //Constructors

		#region Public functions
		/// <summary>
		/// Funkcia pre porovnanie dvoch kľúčov vrcholov stromu pre zistenie kam v prehliadke pokračovať.
		/// </summary>
		/// <param name="key">Kľúč objektu</param>
		/// <param name="dimension">Značka, či porovnávame dĺžku alebo šírku vrcholov</param>
		/// <returns></returns>
		public override int CompareTo(TKey key, int dimension)
		{
			if (Key is IComparable comparableKey)
			{
				return comparableKey.CompareTo(key, dimension);
			}
			throw new ArgumentException("Key must implement IComparable interface!");
		}

		/// <summary>
		/// Vráti hodnotu kľúča podľa zadanej dimenzie - dĺžka alebo šírka.
		/// </summary>
		/// <param name="dimension">Dimenzia</param>
		/// <returns></returns>
		public double GetKeyValue(int dimension)
		{
			if (Key is IComparable comparableKey)
			{
				return comparableKey.GetValue(dimension);
			}
			throw new ArgumentException("Key must implement IComparable interface!");
		}

		public override string ToString()
		{
			return Key.ToString()! + " " + Data.ToString();
		}
		#endregion //Public functions
	}
}
