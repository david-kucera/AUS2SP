namespace GeoLib
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
		#endregion //Constructor
	}
}
