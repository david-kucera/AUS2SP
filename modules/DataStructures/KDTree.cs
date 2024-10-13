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
			throw new NotImplementedException();
		}

		public override T Find(T data)
		{
			throw new NotImplementedException();
		}
		#endregion //Public functions
	}
}
