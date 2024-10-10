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
			int depth = 0;
			int positionIndex = 0;
			if (Root == null!)
			{
				Root = new KdTreeNode<T>
				{
					Data = data,
					Parent = null!,
					Dimension = positionIndex
				};
				Count++;
				depth++;
				positionIndex++;

				int position = Root.CompareTo(data, depth % TreeDimension, positionIndex);
				KdTreeNode<T> newKdTreeNode = new KdTreeNode<T>
				{
					Data = data,
					Parent = Root,
					Dimension = positionIndex
				};
				if (position == -1 || position == 0)
				{
					Root.Left = newKdTreeNode;
				}
				else
				{
					Root.Right = newKdTreeNode;
				}

				Count++;
				return;
			}

			for (int i = 0; i < TreeDimension; i++)
			{
				var currentKdTreeNode = Root;
				var parentKdTreeNode = Root.Parent;
				depth = 0;

				while (currentKdTreeNode != null!)
				{
					parentKdTreeNode = currentKdTreeNode;

					int position = currentKdTreeNode.CompareTo(data, depth % TreeDimension, i);
					if (position == -1 || position == 0)
					{
						currentKdTreeNode = currentKdTreeNode.Left;
					}
					else
					{
						currentKdTreeNode = currentKdTreeNode.Right;
					}

					depth++;
				}

				KdTreeNode<T> newKdTreeNode = new KdTreeNode<T>
				{
					Data = data,
					Parent = parentKdTreeNode,
					Dimension = i
				};

				int newPosition = parentKdTreeNode.CompareTo(data, depth % TreeDimension, i);
				if (newPosition == -1 || newPosition == 0)
				{
					parentKdTreeNode.Left = newKdTreeNode;
				}
				else
				{
					parentKdTreeNode.Right = newKdTreeNode;
				}
				Count++;
			}
		}

		public override void Delete(T data)
		{
			throw new NotImplementedException();
		}

		public override KdTreeNode<T> Find(object value)
		{
			throw new NotImplementedException();
		}
		#endregion //Public functions
	}
}
