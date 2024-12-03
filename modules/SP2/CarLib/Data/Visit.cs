using System.Text;
using FilesLib.Interfaces;

namespace CarLib.Data
{
	public class Visit : IData<Visit>
	{
		#region Constants
		private const int MAX_NOTES_COUNT = 10;
		private const int MAX_NOTE_LENGTH = 20;
		private const double TOLERANCE = 0.0001;
		#endregion // Constants
		
		#region Class members
		private List<string> _notes = new(MAX_NOTES_COUNT);
		#endregion // Class members

		#region Properties
		/// <summary>
		/// Date of visit.
		/// </summary>
		public DateOnly Date { get; set; } = DateOnly.MinValue;
		/// <summary>
		/// Price of visit.
		/// </summary>
		public double Price { get; set; } = double.MinValue;
		/// <summary>
		/// Notes of visit.
		/// </summary>
		/// <exception cref="ArgumentException">If count of notes is more than MAX_NOTES_COUNT</exception>
		public List<string> Notes
		{
			get => _notes;
			set
			{
				if (value.Count > MAX_NOTES_COUNT)
				{
					throw new ArgumentException($"Notes count must not exceed {MAX_NOTES_COUNT} characters.");
				}

				foreach (var note in value)
				{
					if (note.Length > MAX_NOTE_LENGTH)
						throw new ArgumentException($"Note size must not exceed {MAX_NOTE_LENGTH} characters.");
				}

				_notes = value;
			}
		}
		#endregion // Properties

		#region Constructors
		public Visit()
        {
			Notes = new List<string>();
        }

        public Visit(DateOnly date, double price, List<string> notes)
		{
			Date = date;
			Price = price;
			Notes = notes;
		}
		#endregion // Constructors

		#region Public functions
		/// <summary>
		/// Returns string representation of class data.
		/// </summary>
		/// <returns>String</returns>
		public override string ToString()
		{
			var notes = "\n";
			foreach (var note in Notes)
			{
				notes += note + "\n";
			}
			return $"Date: {Date}, Price: {Price}, Notes({Notes.Count}): {notes}";
		}

		/// <summary>
		/// Returns the size of class data in bytes.
		/// </summary>
		/// <returns>Integer</returns>
		public int GetSize()
        {
            int ret = 0;
            ret += sizeof(double);	// Price
            ret += sizeof(ushort) + sizeof(byte) + sizeof(byte); // Date
            ret += sizeof(int); // Notes lengths - that are real
            ret += sizeof(int) * MAX_NOTES_COUNT; // Note length for each individual note - how long is each note
            ret += sizeof(char) * MAX_NOTE_LENGTH * MAX_NOTES_COUNT; // Note data
            return ret;
        }

		/// <summary>
		/// Creates a new dummy class.
		/// </summary>
		/// <returns>Dummy class.</returns>
        public Visit CreateClass()
        {
			return new Visit();
        }
		
        public bool Equals(Visit data)
        {
            return Date == data.Date && Math.Abs(Price - data.Price) < TOLERANCE && Notes == data.Notes; // This method is not necessary...
        }

        /// <summary>
        /// Serializes the class to byte array.
        /// </summary>
        /// <returns>Byte array of class data.</returns>
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

			// Notes length
			BitConverter.GetBytes(Notes.Count).CopyTo(bytes, offset);
			offset += sizeof(int);
			
			// Inidivudial notes lengths
			var emptyNotes = MAX_NOTES_COUNT - Notes.Count;
			for (int i = 0; i < Notes.Count; i++)
			{
				BitConverter.GetBytes(Notes[i].Length).CopyTo(bytes, offset);
				offset += sizeof(int);
			}

			for (int i = 0; i < emptyNotes; i++)
			{
				BitConverter.GetBytes(0).CopyTo(bytes, offset);
				offset += sizeof(int);
			}
			
			// Note data
			for (int i = 0; i < Notes.Count; i++)
			{
				var noteString = Notes[i];
				if (noteString.Length <= MAX_NOTE_LENGTH)
				{
					noteString = noteString.PadRight(MAX_NOTE_LENGTH, ' ');
				}
				Encoding.ASCII.GetBytes(noteString).CopyTo(bytes, offset);
				offset += MAX_NOTE_LENGTH;
			}

			for (int i = 0; i < emptyNotes; i++)
			{
				var noteString = string.Empty;
				noteString = noteString.PadRight(MAX_NOTE_LENGTH, ' ');
				Encoding.ASCII.GetBytes(noteString).CopyTo(bytes, offset);
				offset += MAX_NOTE_LENGTH;
			}

            return bytes;
        }

        /// <summary>
        /// Returns new class object from given byte array.
        /// </summary>
        /// <param name="byteArray">Byte array of data.</param>
        /// <returns>New class object.</returns>
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

			// Notes count
			int notesCount = BitConverter.ToInt32(byteArray, offset);
			offset += sizeof(int);

			// Individual note lengths
			int[] noteLengths = new int[MAX_NOTES_COUNT];
			for (int i = 0; i < MAX_NOTES_COUNT; i++)
			{
				noteLengths[i] = BitConverter.ToInt32(byteArray, offset);
				offset += sizeof(int);
			}
			
			// Note data
			var notes = new List<string>();
			for (int i = 0; i < notesCount; i++)
			{
				var noteString = Encoding.ASCII.GetString(byteArray, offset, noteLengths[i]);
				notes.Add(noteString);
				offset += MAX_NOTE_LENGTH;
			}

			return new Visit(new DateOnly(year, month, day), price, notes);
		}
        #endregion // Public functions
    }
}
