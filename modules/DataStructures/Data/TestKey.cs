namespace DataStructures.Data
{
	public class TestKey : IComparable
	{
		#region Constants
		private const double ROUNDING_ERROR = 0.001;
		#endregion //Constants

		#region Properties
		public double A { get; set; } = 0.0;
		public string B { get; set; } = string.Empty;
		public int C { get; set; } = 0;
		public double D { get; set; } = 0.0;
		#endregion //Properties

		#region Constructor
		public TestKey()
		{
		}

		public TestKey(double a, string b, int c, double d)
		{
			A = a;
			B = b;
			C = c;
			D = d;
		}
		#endregion //Constructor

		#region Public functions
		public int CompareTo(object obj, int dimension)
		{
			var key = obj as TestKey;
			return dimension switch
			{
				0 => Compare1(key!),
				1 => Compare2(key!),
				2 => Compare3(key!),
				3 => Compare4(key!),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		public double GetValue(int dimension)
		{
			return dimension switch
			{
				0 => A,
				1 => GetVal(B),
				2 => C,
				3 => D,
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		public override bool Equals(object obj)
		{
			return obj is TestKey key &&
					Math.Abs(A - key.A) < ROUNDING_ERROR &&
				   Math.Abs(GetVal(B) - GetVal(key.B)) < ROUNDING_ERROR &&
				   C == key.C &&
				   Math.Abs(D - key.D) < ROUNDING_ERROR;
		}

		public override string ToString()
		{
			return $"\nA: {A}\nB: {B}\nC: {C}\nD: {D}\n";
		}
		#endregion //Public functions

		#region Private functions
		private int Compare1(TestKey key)
		{
			var valueThis = A;
			var valueOther = key.A;

			if (Math.Abs(valueThis - valueOther) < ROUNDING_ERROR)
			{
				// Rovnake, porovnavame podla atributu B
				valueThis = GetVal(B);
				valueOther = GetVal(key.B);
				if (Math.Abs(valueThis - valueOther) < ROUNDING_ERROR)
				{
					return -1; // Dolava, kedze rovnake
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
			if (valueThis > valueOther)
			{
				return -1; // Dolava
			}
			else
			{
				return 1; // Doprava
			}
		}

		private int Compare2(TestKey key)
		{
			var valueThis = C;
			var valueOther = key.C;

			if (valueThis == valueOther)
			{
				return -1; // Dolava, kedze rovnake
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

		private int Compare3(TestKey key)
		{
			var valueThis = D;
			var valueOther = key.D;
			if (Math.Abs(valueThis - valueOther) < ROUNDING_ERROR)
			{
				return -1; // Dolava, kedze rovnake
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

		private int Compare4(TestKey key)
		{
			var valueThis = GetVal(B);
			var valueOther = GetVal(key.B);
			if (Math.Abs(valueThis - valueOther) < ROUNDING_ERROR)
			{
				// Rovnake, porovnavame atribut C
				valueThis = C;
				valueOther = key.C;

				if (Math.Abs(valueThis - valueOther) < ROUNDING_ERROR)
				{
					return -1; // Dolava, kedze rovnake
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
			if (valueThis > valueOther)
			{
				return -1; // Dolava
			}
			else
			{
				return 1; // Doprava
			}
		}

		private double GetVal(string chars)
		{
			double ret = 0.0;
			foreach (var c in chars)
			{
				ret += c;
			}
			return ret;
		}
		#endregion //Private functions
	}
}