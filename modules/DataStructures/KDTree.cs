namespace DataStructures
{
	public class KdTree<T> where T : class
	{
		#region Properties
		public Node<T> Root { get; set; }
		public int TotalTreeDimension { get; set; }
		public int CurrentTreeDimension { get; set; }
		public int Count { get; set; }
		#endregion //Properties

		#region Constructor
		public KdTree(int totalTreeDimension)
		{
			TotalTreeDimension = totalTreeDimension;
			CurrentTreeDimension = 0;
			Count = 0;
			Root = null!;
		}
		#endregion //Constructor

		#region Public functions
		public void Insert(T data)
		{
			if (Root == null!)
			{
				Root = new Node<T>
				{
					Data = data,
					Parent = null!,
					Dimension = CurrentTreeDimension // 0
				};
				Count++;
				CurrentTreeDimension++;

				int position = Root.CompareTo(data, CurrentTreeDimension);
				if (position == -1)
				{
					Root.Left = new Node<T>
					{
						Data = data,
						Parent = Root,
						Dimension = CurrentTreeDimension++
					};
					Count++;
				}
				else if (position == 1)
				{
					Root.Right = new Node<T>
					{
						Data = data,
						Parent = Root,
						Dimension = CurrentTreeDimension++
					};
					Count++;
				}
				else
				{
					Root.Left = new Node<T>
					{
						Data = data,
						Parent = Root,
						Dimension = CurrentTreeDimension++
					};
					Count++;
				}

				CurrentTreeDimension--;
			}
			else
			{
				
				
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
