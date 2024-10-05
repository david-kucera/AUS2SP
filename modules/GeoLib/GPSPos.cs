namespace GeoLib
{
	public class GPSPos
	{
		#region Properties
		public char Sirka { get; set; } = 'N';              // N alebo S
		public double PoziciaSirky { get; set; } = 0.0;
		public char Dlzka { get; set; } = 'E';              // E alebo W
		public double PoziciaDlzky { get; set; } = 0.0;
		#endregion //Properties

		#region Constructor
		public GPSPos()
		{
		}
		#endregion //Constructor
	}
}
