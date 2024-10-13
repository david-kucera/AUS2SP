namespace DataStructures.Data
{
    public class GpsPos
    {
        #region Properties
        public char Sirka { get; set; } = char.MinValue;                // N alebo S
        public double PoziciaSirky { get; set; } = double.MinValue;     // Latitude
		public char Dlzka { get; set; } = char.MinValue;                // E alebo W
        public double PoziciaDlzky { get; set; } = double.MinValue;     // Longitude
		#endregion //Properties

		#region Constructor
		public GpsPos()
        {
        }

        public GpsPos(char sirka, double poziciaSirky, char dlzka, double poziciaDlzky)
        {
            Sirka = sirka;
            PoziciaSirky = poziciaSirky;
            Dlzka = dlzka;
            PoziciaDlzky = poziciaDlzky;
        }
        #endregion //Constructor

        #region Public functions
        public override string ToString()
		{
			return "[" + Sirka + PoziciaSirky.ToString("F2") + "; " + Dlzka + PoziciaDlzky.ToString("F2") + "]"; 
		}

		public int CompareTo(GpsPos gpsPos, int positionIndex)
        {
	        return positionIndex switch
	        {
		        0 => CompareSirka(gpsPos),
		        1 => CompareDlzka(gpsPos),
		        _ => throw new IndexOutOfRangeException()
	        };
        }
        #endregion //Public functions

        #region Private functions
        private int CompareDlzka(GpsPos gpsPos)
        {
            int multiplier = 1;
            if (gpsPos.Dlzka != 'E')
            {
                multiplier = -1;
            }
            var valueOther = multiplier * gpsPos.PoziciaDlzky;

            multiplier = 1;
            if (Dlzka != 'E')
            {
                multiplier = -1;
            }
            var valueThis = multiplier * PoziciaDlzky;

            if (Math.Abs(valueThis - valueOther) < 0 && Sirka == gpsPos.Sirka)
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

        private int CompareSirka(GpsPos gpsPos)
        {
            int multiplier = 1;
            if (gpsPos.Sirka != 'N')
            {
                multiplier = -1;
            }
            var valueOther = multiplier * gpsPos.PoziciaSirky;

            multiplier = 1;
            if (Sirka != 'N')
            {
                multiplier = -1;
            }
            var valueThis = multiplier * PoziciaSirky;

            if (Math.Abs(valueThis - valueOther) < 0 && Dlzka == gpsPos.Dlzka)
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
