namespace DataStructures
{
	public class KdTree<T> where T : class
	{
		#region Properties
		public Node<T> Root { get; set; }
		public int TreeDimension { get; set; }
		public int Count { get; set; }
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
		public void Insert(T data)
		{
			int depth = 0;
			if (Root == null!)
			{
				Root = new Node<T>
				{
					Data = data,
					Parent = null!,
					Dimension = depth // 0
				};
				Count++;
				depth++;

				int position = Root.CompareTo(data, depth % TreeDimension);
				if (position == -1 || position == 0)
				{
					Root.Left = new Node<T>
					{
						Data = data,
						Parent = Root,
						Dimension = depth % TreeDimension // 1
					};
				}
				else
				{
					Root.Right = new Node<T>
					{
						Data = data,
						Parent = Root,
						Dimension = depth % TreeDimension // 1
					};
				}
				Count++;
				return;
			}

			for (int i = 0; i < TreeDimension; i++)
			{
				Node<T> currentNode = Root;
				Node<T> parentNode = null!;
				depth = 0;
				int currentTreeDimension = i;

				while (currentNode != null!)
				{
					parentNode = currentNode;

					int position = currentNode.CompareTo(data, currentTreeDimension);
					if (position == -1 || position == 0)
					{
						currentNode = currentNode.Left;
					}
					else
					{
						currentNode = currentNode.Right;
					}

					depth++;
					currentTreeDimension = depth % TreeDimension;
				}

				Node<T> newNode = new Node<T>
				{
					Data = data,
					Parent = parentNode,
					Dimension = depth % TreeDimension
				};

				//currentTreeDimension = depth % TreeDimension;
				int newPosition = parentNode.CompareTo(data, currentTreeDimension);
				if (newPosition == -1 || newPosition == 0)
				{
					parentNode.Left = newNode;
				}
				else
				{
					parentNode.Right = newNode;
				}
				Count++;
			}
		}

		public void Delete(T data)
		{
			throw new NotImplementedException();
		}

		public Node<T> Find(object value)
		{
			throw new NotImplementedException();
		}
		#endregion //Public functions
	}
}
