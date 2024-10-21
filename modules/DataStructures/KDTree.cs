using System.Text;

namespace DataStructures
{
	public class KdTree<TKey, TValue> : AbstractTree<TKey, TValue> where TValue : class where TKey : class
	{
		#region Properties
		public int TreeDimension { get; set; }
		#endregion //Properties

		#region Constructor
		public KdTree(int treeDimension)
		{
			TreeDimension = treeDimension;
			Count = 0;
			Root = null!;
		}
		#endregion //Constructor

		#region Public functions
		public override void Insert(TKey key, TValue data)
		{
			// TODO - PRE NEHUTELNOSTI - vyhladat vsetky parcely na ktorych sa nachadza
			// TODO - PRE PARCELY - vyhladat vsetky nehnutelnosti ktore sa na nej nachadzaju

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
					int position = currentNode.CompareTo(newNode.Key, depth % TreeDimension);
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

		public override void Delete(TKey key)
		{
			List<TValue> foundNodes = Find(key);

			for (int i = 0; i < 2; i++)
			{
				// TODO - implement delete
			}
		}

		public override List<TValue> Find(TKey key)
		{
			List<TValue> ret = [];
			var currentNode = Root;
			int depth = 0;

			while (currentNode != null!)
			{
				if (currentNode.Key.Equals(key)) ret.Add(currentNode.Data);

				int position = currentNode.CompareTo(key, depth % TreeDimension);
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
