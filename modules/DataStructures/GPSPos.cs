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
			int nasobic_other = 1;
			if (gpsPos.Dlzka != 'E')
			{
				nasobic_other = -1;
			}
			var hodnota_other = nasobic_other * gpsPos.PoziciaDlzky;

			int nasobic_this = 1;
			if (Dlzka != 'E')
			{
				nasobic_this = -1;
			}
			var hodnota_this = nasobic_this * PoziciaDlzky;

			if (hodnota_this < hodnota_other)
			{
				return 1; // Doprava
			}
			else if (hodnota_this > hodnota_other)
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
			int nasobic_other = 1;
			if (gpsPos.Sirka != 'N')
			{
				nasobic_other = -1;
			}
			var hodnota_other = nasobic_other * gpsPos.PoziciaSirky;

			int nasobic_this = 1;
			if (Sirka != 'N')
			{
				nasobic_this = -1;
			}
			var hodnota_this = nasobic_this * PoziciaSirky;

			if (hodnota_this < hodnota_other)
			{
				return 1; // Doprava
			}
			else if (hodnota_this > hodnota_other)
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
