using System.Text;

namespace DataStructures
{
	/// <summary>
	/// Impelementation of KdTree data structure.
	/// </summary>
	/// <typeparam name="TKey">Type of key values</typeparam>
	/// <typeparam name="TValue">Type of data values</typeparam>
	public class KdTree<TKey, TValue> : AbstractTree<TKey, TValue> where TValue : class where TKey : class
	{
		#region Class members
		private readonly int _treeDimension;    // Total number of dimensions in the tree
		#endregion //Class members

		#region Constructor
		public KdTree(int treeDimension)
		{
			_treeDimension = treeDimension;
			Count = 0;
			Root = null!;
		}
		#endregion //Constructor

		#region Public functions
		/// <summary>
		/// Inserts the given node represented by key and data into the tree.
		/// </summary>
		/// <param name="key">Key of the node</param>
		/// <param name="data">Data of the node</param>
		public override void Insert(TKey key, TValue data)
		{
			if (Root == null!)
			{
				Root = new KdTreeNode<TKey, TValue>
				{
					Data = data,
					Parent = null!,
					Key = key,
					Left = null!,
					Right = null!,
				};
				Count++;
			}
			else
			{
				KdTreeNode<TKey, TValue> newNode = new()
				{
					Data = data,
					Key = key,
					Left = null!,
					Right = null!,
					Parent = null!
				};
				Count++;

				var currentNode = Root;
				int depth = 0;
				while (true)
				{
					int position = currentNode.CompareTo(newNode.Key, depth % _treeDimension);
					if (position == -1 || position == 0)
					{
						if (currentNode.Left == null!)
						{
							currentNode.Left = newNode;
							newNode.Parent = currentNode;
							break;
						}
						currentNode = currentNode.Left;
					}
					else
					{
						if (currentNode.Right == null!)
						{
							currentNode.Right = newNode;
							newNode.Parent = currentNode;
							break;
						}
						currentNode = currentNode.Right;
					}
					depth++;
				}
			}
		}

		/// <summary>
		/// Removes the node with the given key and data.
		/// </summary>
		/// <param name="key">Key of the node to delete</param>
		/// <param name="data">Data of the node to delete</param>
		public override void Remove(TKey key, TValue data)
		{
			var nodeToDelete = Find(key, data);
			if (nodeToDelete == null!) return;

			List<KdTreeNode<TKey, TValue>> duplikaty = [];
			List<KdTreeNode<TKey, TValue>> nodesToReinsert = [];
			while (nodeToDelete.Left != null! || nodeToDelete.Right != null!)
			{
				KdTreeNode<TKey, TValue> newNode;
				int dimension = GetDimension(nodeToDelete);
				if (nodeToDelete.Left != null!)
				{
					newNode = SearchMax((KdTreeNode<TKey, TValue>)nodeToDelete.Left!, dimension, double.MinValue);
				}
				else
				{
					newNode = SearchMin((KdTreeNode<TKey, TValue>)nodeToDelete.Right!, dimension, double.MaxValue);

					// Ak som vybral vrchol, ktory je v pravom podstrome, tak musim skontrolovat, ci sa vpravo nenachadzaju duplicity podla daneho kluca
					var stack = new Stack<KdTreeNode<TKey, TValue>>();
					stack.Push((KdTreeNode<TKey, TValue>)nodeToDelete.Right);

					while (stack.Count > 0)
					{
						var currentNode = stack.Pop();
						if (currentNode != newNode) nodesToReinsert.Add(currentNode);
						if (currentNode.CompareTo(newNode.Key, dimension) == 0)
						{
							duplikaty.Add(currentNode);
						}

						if (currentNode.Right != null!) stack.Push((KdTreeNode<TKey, TValue>)currentNode.Right);
						if (currentNode.Left != null!) stack.Push((KdTreeNode<TKey, TValue>)currentNode.Left);
					}

					// Ak som nasiel nejake duplicity, tak ich musim odstranit
					// Odstranim cely pravy podstrom, pretoze pomocou metody Find by som sa k prvkom po presunuti uz nedostal
					if (duplikaty.Count > 0)
					{
						nodeToDelete.Right = null!;
						Count -= nodesToReinsert.Count;
						nodeToDelete.Key = newNode.Key;
						nodeToDelete.Data = newNode.Data;
						Count--; // Odstranujem newNode, ktory je uz nahradeny za nodeToDelete
						break;
					}
				}

				// Nahradzam vrchol za jeho nasledovnika
				if (newNode == null!) throw new Exception("NewNode is null!");
				nodeToDelete.Key = newNode.Key;
				nodeToDelete.Data = newNode.Data;

				// Opakujem proces pre naslednika
				nodeToDelete = newNode;
			}

			// Ak je vrchol list, tak ho len odstranim
			if (duplikaty.Count == 0)
			{
				RemoveLeaf(nodeToDelete);
				Count--;
			}

			// Nasledne znovu vkladam pravy podstrom do stromu aby mali dobru poziciu a boli dohladatelne
			if (duplikaty.Count == 0) return;
			foreach (var node in nodesToReinsert)
			{
				Insert(node.Key, node.Data);
			}
		}

		/// <summary>
		/// Returns list of values for the given key.
		/// </summary>
		/// <param name="key">Key to look for</param>
		/// <returns></returns>
		public override List<TValue> Find(TKey key)
		{
			List<TValue> ret = [];
			var currentNode = Root;
			int depth = 0;

			while (currentNode != null!)
			{
				if (currentNode.Key.Equals(key)) ret.Add(currentNode.Data);

				int position = currentNode.CompareTo(key, depth % _treeDimension);
				if (position == -1 || position == 0)
				{
					currentNode = currentNode.Left;
				}
				else
				{
					currentNode = currentNode.Right;
				}
				depth++;
			}

			return ret;
		}

		/// <summary>
		/// Returns all values stored in tree.
		/// </summary>
		/// <returns></returns>
		public List<TValue> GetAll()
		{
			var ret = new List<TValue>();
			var stack = new Stack<KdTreeNode<TKey, TValue>>();
			var currentNode = Root;

			while (currentNode != null || stack.Count > 0)
			{
				while (currentNode != null)
				{
					stack.Push((KdTreeNode<TKey, TValue>)currentNode);
					currentNode = currentNode.Left;
				}

				currentNode = stack.Pop();
				if (currentNode.Data != null) ret.Add(currentNode.Data);

				currentNode = currentNode.Right;
			}

			return ret;
		}

		/// <summary>
		/// Returns string representation of the tree.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (Root == null!) return string.Empty;
			var builder = new StringBuilder();
			BuildString(Root, builder, 0, "");
			return builder.ToString();
		}
		#endregion //Public functions

		#region Private functions
		/// <summary>
		/// Finds the exact node with the given key and data.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		private KdTreeNode<TKey, TValue>? Find(TKey key, TValue data)
		{
			var currentNode = Root;
			int depth = 0;

			while (currentNode != null!)
			{
				if (currentNode.Key.Equals(key) && currentNode.Data.Equals(data)) return (KdTreeNode<TKey, TValue>)currentNode;

				int position = currentNode.CompareTo(key, depth % _treeDimension);
				if (position == -1 || position == 0)
				{
					currentNode = currentNode.Left;
				}
				else
				{
					currentNode = currentNode.Right;
				}
				depth++;
			}

			return null;
		}

		private void RemoveLeaf(KdTreeNode<TKey, TValue> nodeToDelete)
		{
			var parentNode = nodeToDelete.Parent;
			if (parentNode != null!)
			{
				if (parentNode.Left == nodeToDelete)
				{
					parentNode.Left = null!;
				}
				else
				{
					parentNode.Right = null!;
				}
				nodeToDelete.Parent = null!;
			}
			else
			{
				Root = null!;
			}
		}

		private static KdTreeNode<TKey, TValue> SearchMin(KdTreeNode<TKey, TValue> node, int dimensionNodeToDelete, double minimumKeyValue)
		{
			List<KdTreeNode<TKey, TValue>> nodesToSearch = [node];
			KdTreeNode<TKey, TValue> newNode = null!;
			while (nodesToSearch.Count > 0)
			{
				var currentNode = nodesToSearch[^1];
				nodesToSearch.RemoveAt(nodesToSearch.Count - 1);

				var compareValue = currentNode.GetKeyValue(dimensionNodeToDelete);
				if (compareValue <= minimumKeyValue)
				{
					minimumKeyValue = compareValue;
					newNode = currentNode;
				}

				if (currentNode.Right != null) nodesToSearch.Add((KdTreeNode<TKey, TValue>)currentNode.Right);
				if (currentNode.Left != null) nodesToSearch.Add((KdTreeNode<TKey, TValue>)currentNode.Left);
			}

			return newNode;
		}

		private static KdTreeNode<TKey, TValue> SearchMax(KdTreeNode<TKey, TValue> node, int dimensionNodeToDelete, double maximumKeyValue)
		{
			List<KdTreeNode<TKey, TValue>> nodesToSearch = [node];
			KdTreeNode<TKey, TValue> newNode = null!;
			while (nodesToSearch.Count > 0)
			{
				var currentNode = nodesToSearch[^1];
				nodesToSearch.RemoveAt(nodesToSearch.Count - 1);

				var compareValue = currentNode.GetKeyValue(dimensionNodeToDelete);
				if (compareValue >= maximumKeyValue)
				{
					maximumKeyValue = compareValue;
					newNode = currentNode;
				}

				if (currentNode.Right != null) nodesToSearch.Add((KdTreeNode<TKey, TValue>)currentNode.Right);
				if (currentNode.Left != null) nodesToSearch.Add((KdTreeNode<TKey, TValue>)currentNode.Left);
			}

			return newNode;
		}

		/// <summary>
		/// Gets the dimension of the given node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private int GetDimension(KdTreeNode<TKey, TValue> node)
		{
			int depth = 0;
			var currentNode = Root;

			while (currentNode != null && currentNode != node)
			{
				var comp = currentNode.CompareTo(node.Key, depth % _treeDimension);
				currentNode = comp < 0 ? currentNode.Left : currentNode.Right;
				if (currentNode != null) depth++;
			}

			return depth % _treeDimension;
		}

		/// <summary>
		/// Helper function to build string representation of the tree.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="builder"></param>
		/// <param name="depth"></param>
		/// <param name="position"></param>
		private static void BuildString(AbstractNode<TKey, TValue> node, StringBuilder builder, int depth, string position)
		{
			while (true)
			{
				if (node == null!) return;

				builder.AppendLine($"{new string(' ', depth * 4)}{position} {node}");
				if (node.Left != null!)
				{
					BuildString(node.Left, builder, depth + 1, "L");
				}
				if (node.Right != null!)
				{
					node = node.Right;
					depth += 1;
					position = "R";
					continue;
				}
				break;
			}
		}
		#endregion //Private functions
	}
}
