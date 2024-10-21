using System.Text;

namespace DataStructures
{
	public class KdTree<TKey, TValue> : AbstractTree<TKey, TValue> where TValue : class where TKey : class
	{
		#region Class members
		private readonly int _treeDimension;
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

		public override void Delete(TKey key, TValue data)
		{
			var nodeToDelete = Find(key, data);
			if (nodeToDelete == null!) return;

			var parentNode = nodeToDelete.Parent;
			int depth = 0;

			if (nodeToDelete.Left == null! && nodeToDelete.Right == null!)
			{
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
				}
				else
				{
					Root = null!;
				}

				Count--;
				return;
			}

			if (nodeToDelete.Right != null!)
			{
				var minNode = FindMin(nodeToDelete.Right, depth % _treeDimension);
				nodeToDelete.Key = minNode.Key;
				nodeToDelete.Data = minNode.Data;

				Delete(minNode.Key, minNode.Data);
			}
			else if (nodeToDelete.Left != null!)
			{
				var minNode = FindMin(nodeToDelete.Left, depth % _treeDimension);
				nodeToDelete.Key = minNode.Key;
				nodeToDelete.Data = minNode.Data;

				Delete(minNode.Key, minNode.Data);
			}

			Count--;
		}

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

		public override string ToString()
		{
			if (Root == null!) return string.Empty;
			var builder = new StringBuilder();
			BuildString(Root, builder, 0, "");
			return builder.ToString();
		}
		#endregion //Public functions
		
		#region Private functions
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

		private KdTreeNode<TKey, TValue> FindMin(AbstractNode<TKey, TValue> node, int axis)
		{
			var currentNode = node;
			var minNode = node;
			int depth = 0;

			while (currentNode != null!)
			{
				if (depth % _treeDimension == axis)
				{
					if (currentNode.Left != null!)
					{
						minNode = currentNode.Left;
						currentNode = currentNode.Left;
					}
					else
					{
						return (KdTreeNode<TKey, TValue>)minNode;
					}
				}
				else
				{
					if (currentNode.Left != null! && currentNode.CompareTo(minNode.Key, axis) < 0) // mozno pridat rovna sa
					{
						minNode = currentNode.Left;
					}

					if (currentNode.Right != null!)
					{
						currentNode = currentNode.Right;
					}
					else
					{
						currentNode = currentNode.Left;
					}
				}

				depth++;
			}

			return (KdTreeNode<TKey, TValue>)minNode;
		}

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
