using System.Text;

namespace FilesLib.Data
{
	public class Visit : IData<Visit>
	{
		#region Constants
		private const int MAX_NOTE_LENGTH = 20;
		#endregion // Constants

		#region Properties
		public DateOnly Date { get; set; } = DateOnly.MinValue;
		public double Price { get; set; } = double.MinValue;
		public string Note { get; set; } = string.Empty;
		#endregion // Properties

		#region Constructors
		public Visit()
        {

        }

        public Visit(DateOnly date, double price, string note)
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

		public int GetSize()
        {
            int ret = 0;
            ret += sizeof(double);	// Price
            ret += 3 * sizeof(int); // Date
            ret += MAX_NOTE_LENGTH * sizeof(char); // Note
            return ret;
        }

        public Visit CreateClass()
        {
			return new Visit();
        }

        public bool Equals(Visit data)
        {
            return Date == data.Date && Price == data.Price && Note == data.Note;
        }

        public byte[] ToByteArray()
        {
			byte[] bytes = new byte[GetSize()];
            int offset = 0;

            // TODO check dateonly size
            bytes.CopyTo(BitConverter.GetBytes(Price), offset);
            offset += sizeof(double);
            bytes.CopyTo(BitConverter.GetBytes(Date.Year), offset);
            offset += sizeof(int);
            bytes.CopyTo(BitConverter.GetBytes(Date.Month), offset);
            offset += sizeof(int);
            bytes.CopyTo(BitConverter.GetBytes(Date.Day), offset);
            offset += sizeof(int);
            bytes.CopyTo(Encoding.ASCII.GetBytes(Note), offset);
            offset += MAX_NOTE_LENGTH * sizeof(char);

            return bytes;
        }

        public Visit FromByteArray(byte[] byteArray)
        {
            throw new NotImplementedException();
        }
        #endregion // Public functions
    }
}
