using System.Text;

namespace FilesLib.Data
{
	public class Visit : IData<Visit>
	{
		#region Constants
		private const int MAX_NOTE_LENGTH = 20;
		private const double TOLERANCE = 0.0001;
		#endregion // Constants
		
		#region Class members
		private string _note = string.Empty;
		#endregion // Class members

		#region Properties
		public DateOnly Date { get; set; } = DateOnly.MinValue;
		public double Price { get; set; } = double.MinValue;
		public string Note
		{
			get => _note;
			set
			{
				if (value.Length > MAX_NOTE_LENGTH)
				{
					throw new ArgumentException($"Note length must not exceed {MAX_NOTE_LENGTH} characters.");
				}
				_note = value;
			}
		}
		#endregion // Properties

		#region Constructors
		public Visit()
        {

        }

        public Visit(DateOnly date, double price, string note)
		{
			Date = date;
			Price = price;
			_note = note;
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
            ret += sizeof(ushort) + sizeof(byte) + sizeof(byte); // Date
			ret += MAX_NOTE_LENGTH * sizeof(char); // Note
            return ret;
        }

        public Visit CreateClass()
        {
			return new Visit();
        }

        public bool Equals(Visit data)
        {
            return Date == data.Date && Math.Abs(Price - data.Price) < TOLERANCE && Note == data.Note;
        }

        public byte[] ToByteArray()
        {
			byte[] bytes = new byte[GetSize()];
            int offset = 0;

			// 8 bytes for double
			BitConverter.GetBytes(Price).CopyTo(bytes, offset);
            offset += sizeof(double);

            // 2 bytes for Year
            BitConverter.GetBytes((ushort)Date.Year).CopyTo(bytes, offset);
            offset += sizeof(ushort);

			// 1 byte for Month
			bytes[offset] = (byte)Date.Month;
			offset++;

			// 1 byte for Day
			bytes[offset] = (byte)Date.Day;
			offset++;

			// 4 bytes for note length and 20 bytes for note chars
			int noteLength = Note.Length;
			BitConverter.GetBytes(noteLength).CopyTo(bytes, offset);
			offset += sizeof(int);

			if (noteLength < MAX_NOTE_LENGTH)
			{
				Note = Note.PadRight(MAX_NOTE_LENGTH, ' ');
			}
			Encoding.ASCII.GetBytes(Note).CopyTo(bytes, offset);

            return bytes;
        }

        public Visit FromByteArray(byte[] byteArray)
        {
            int offset = 0;

			// Price
			double price = BitConverter.ToDouble(byteArray, offset);
			offset += sizeof(double);

			// Year
			ushort year = BitConverter.ToUInt16(byteArray, offset);
			offset += sizeof(ushort);

			// Month
			byte month = byteArray[offset];
			offset++;

			// Day
			byte day = byteArray[offset];
			offset++;

			// Note length
			int noteLength = BitConverter.ToInt32(byteArray, offset);
			offset += sizeof(int);

			// Note
			string note = Encoding.ASCII.GetString(byteArray, offset, noteLength);

			return new Visit(new DateOnly(year, month, day), price, note);
		}
        #endregion // Public functions
    }
}
