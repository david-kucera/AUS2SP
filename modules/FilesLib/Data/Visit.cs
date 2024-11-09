namespace FilesLib.Data
{
	public class Visit
	{
		#region Constants
		private const int MAX_NOTE_LENGTH = 20;
		#endregion // Constants

		#region Properties
		public DateTime Date { get; set; } = DateTime.Now;
		public double Price { get; set; } = double.MinValue;
		public string Note { get; set; } = string.Empty;
		#endregion // Properties

		#region Constructors
		public Visit(DateTime date, double price, string note)
		{
			if (note.Length > MAX_NOTE_LENGTH)
			{
				throw new ArgumentException($"Note length must be less than {MAX_NOTE_LENGTH} characters.");
			}

			Date = date;
			Price = price;
			Note = note;
		}
		#endregion // Constructors

		#region Public functions
		public override string ToString()
		{
			return $"Date: {Date}, Price: {Price}, Note: {Note}";
		}
		#endregion // Public functions

	}
}
