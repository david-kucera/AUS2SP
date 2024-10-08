namespace DataStructures
{
	public class GPSPos
	{
		#region Properties
		public char Sirka { get; set; } = char.MinValue;              // N alebo S
		public double PoziciaSirky { get; set; } = Double.MinValue;
		public char Dlzka { get; set; } = char.MinValue;              // E alebo W
		public double PoziciaDlzky { get; set; } = Double.MinValue;
		#endregion //Properties

		#region Constructor
		public GPSPos()
		{
		}

		public GPSPos(char sirka, double poziciaSirky, char dlzka, double poziciaDlzky)
		{
			Sirka = sirka;
			PoziciaSirky = poziciaSirky;
			Dlzka = dlzka;
			PoziciaDlzky = poziciaDlzky;
		}
		#endregion //Constructor

		public int CompareTo(GPSPos gpsPos, int dimension)
		{
			switch (dimension)
			{
				case 0:
					return CompareSirka(gpsPos);
				case 1:
					return CompareDlzka(gpsPos);
				default:
					throw new NotSupportedException();
			}
		}

		private int CompareDlzka(GPSPos gpsPos)
		{
			int multiplierOther = 1;
			if (gpsPos.Dlzka != 'E')
			{
				multiplierOther = -1;
			}
			var valueOther = multiplierOther * gpsPos.PoziciaDlzky;

			int multiplierThis = 1;
			if (Dlzka != 'E')
			{
				multiplierThis = -1;
			}
			var valueThis = multiplierThis * PoziciaDlzky;

			if (valueThis < valueOther)
			{
				return 1; // Doprava
			}
			else if (valueThis > valueOther)
			{
				return -1; // Dolava
			}
			else
			{
				return 0; // Dolava, kedze rovnake
			}
		}

		private int CompareSirka(GPSPos gpsPos)
		{
			int multiplierOther = 1;
			if (gpsPos.Sirka != 'N')
			{
				multiplierOther = -1;
			}
			var valueOther = multiplierOther * gpsPos.PoziciaSirky;

			int multiplierThis = 1;
			if (Sirka != 'N')
			{
				multiplierThis = -1;
			}
			var valueThis = multiplierThis * PoziciaSirky;

			if (valueThis < valueOther)
			{
				return 1; // Doprava
			}
			else if (valueThis > valueOther)
			{
				return -1; // Dolava
			}
			else
			{
				return 0; // Dolava, kedze rovnake
			}
		}
	}
}
