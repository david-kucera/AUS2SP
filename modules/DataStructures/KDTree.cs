using System.Text;

namespace DataStructures
{
	public class KdTree<T> : AbstractTree<T> where T : class
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
		public override void Insert(T data)
		{
			if (Root == null!)
			{
				Root = new KdTreeNode<T>
				{
					Data = data,
					Parent = null!,
					Comparator = 0,
					Left = null!,
					Right = null!,
				};
				Count++;

				KdTreeNode<T> secondRoot = new()
				{
					Data = data,
					Parent = Root,
					Comparator = 1,
					Left = null!,
					Right = null!,
				};
				Count++;

				int position = Root.CompareTo(secondRoot.Data, secondRoot.Comparator, 0);
				if (position == -1 || position == 0)
				{
					Root.Left = secondRoot;
				}
				else
				{
					Root.Right = secondRoot;
				}
			}
			else
			{
				for (int i = 0; i < TreeDimension; i++)
				{
					KdTreeNode<T> newNode = new()
					{
						Data = data,
						Comparator = i,
						Left = null!,
						Right = null!,
						Parent = null!
					};
					Count++;

					var currentNode = Root;
					int depth = 0;
					while (true)
					{
						int position = currentNode.CompareTo(newNode.Data, newNode.Comparator, depth % TreeDimension);
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
		}

		public override void Delete(T data)
		{
			for (int i = 0; i < 2; i++)
			{
				KdTreeNode<T> nodeToDelete = Find(data);

				if (nodeToDelete == null!) return;

				if (nodeToDelete == Root)
				{
					Root = null!;
					Count = 0;
					return;
				}
				if (nodeToDelete.Left == null! && nodeToDelete.Right == null!)
				{
					// TODO 
				}

				// TODO - implement delete
			}
		}

		public override KdTreeNode<T> Find(T data)
		{
			var currentNode = Root;
			int depth = 0;

			while (true)
			{
				if (currentNode == null!) return null!;

				if (currentNode.Data.Equals(data)) return (KdTreeNode<T>?)currentNode!;

				int position = currentNode.CompareTo(data, 0, depth % TreeDimension);
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
		private static void BuildString(AbstractNode<T> node, StringBuilder builder, int depth, string position)
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
