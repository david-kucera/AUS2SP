namespace DataStructures.Data
{
	public class TestKey : IComparable
	{
		#region Constants
		private const double ROUNDING_ERROR = 0.001;
		#endregion //Constants

		#region Properties
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		#endregion //Properties

		#region Constructor
		public TestKey()
		{
		}

		public TestKey(int x, int y)
		{
			X = x;
			Y = y;
		}
		#endregion //Constructor

		#region Public functions
		public int CompareTo(object obj, int dimension)
		{
			var key = obj as TestKey;
			return dimension switch
			{
				0 => CompareX(key!),
				1 => CompareY(key!),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		public double GetValue(int dimension)
		{
			return dimension switch
			{
				0 => X,
				1 => Y,
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		public override bool Equals(object obj)
		{
			return obj is TestKey key &&
					Math.Abs(X - key.X) < ROUNDING_ERROR &&
				   Math.Abs(Y - key.Y) < ROUNDING_ERROR;
		}

		public override string ToString()
		{
			return $"\nX: {X}\nY: {Y}\n";
		}
		#endregion //Public functions

		#region Private functions
		private int CompareX(TestKey key)
		{
			var valueThis = X;
			var valueOther = key.X;

			if (valueThis == valueOther)
			{
				return 0; // Rovnake - dolava
			}
			if (valueThis > valueOther)
			{
				return -1; // Dolava
			}
			else
			{
				return 1; // Doprava
			}
		}

		private int CompareY(TestKey key)
		{
			var valueThis = Y;
			var valueOther = key.Y;

			if (valueThis == valueOther)
			{
				return 0; // Dolava, kedze rovnake
			}
			if (valueThis > valueOther)
			{
				return -1; // Dolava
			}
			else
			{
				return 1; // Doprava
			}
		}
		#endregion //Private functions
	}
}