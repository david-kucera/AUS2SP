using System.Text;
using FilesLib.Interfaces;

namespace CarLib.Data
{
	/// <summary>
	/// Class representing Car/Person data of system.
	/// </summary>
	public class Person : IData<Person>
	{
		#region Constants
		private const int MAX_VISITS = 5;
		private const int MAX_NAME_LENGTH = 15;
		private const int MAX_SURNAME_LENGTH = 20;
		private const int MAX_ECV_LENGTH = 10;
		#endregion // Constants
		
		#region Class members
		private string _name = string.Empty;
		private string _surname = string.Empty;
		private string _ecv = string.Empty;
		private List<Visit> _visits = new(MAX_VISITS);
		#endregion // Class members

		#region Properties
		/// <summary>
		/// Name of the person.
		/// </summary>
		/// <exception cref="ArgumentException">If name is too long.</exception>
		public string Name
		{
			get => _name;
			set
			{
				if (value.Length > MAX_NAME_LENGTH)
				{
					throw new ArgumentException($"Name is too long. Max length is {MAX_NAME_LENGTH}.");
				}
				_name = value;
			}
		}
		/// <summary>
		/// Surname of the person.
		/// </summary>
		/// <exception cref="ArgumentException">If surname is too long.</exception>
		public string Surname
		{
			get => _surname;
			set
			{
				if (value.Length > MAX_SURNAME_LENGTH)
				{
					throw new ArgumentException($"Surname is too long. Max length is {MAX_SURNAME_LENGTH}.");
				}
				_surname = value;
			}
		}
		/// <summary>
		/// ECV of person's car.
		/// </summary>
		/// <exception cref="ArgumentException">If ecv is too long.</exception>
		public string Ecv
		{
			get => _ecv;
			set
			{
				if (value.Length > MAX_ECV_LENGTH)
				{
					throw new ArgumentException($"ECV is too long. Max length is {MAX_ECV_LENGTH}.");
				}
				_ecv = value;
			}
		}
		/// <summary>
		/// ID of the person.
		/// </summary>
		public int Id { get; set; } = -1;
		/// <summary>
		/// Car repair visits.
		/// </summary>
		/// <exception cref="ArgumentException">If list is too long.</exception>
		public List<Visit> Visits
		{
			get => _visits;
			set
			{
				if (value.Count > MAX_VISITS)
				{
					throw new ArgumentException($"Number of visits must be less than {MAX_VISITS}.");
				}
				_visits = value;
			}
		}
		#endregion // Properties

		#region Constructors
		public Person()
		{
			
		}
		
		public Person(Person copy)
		{
			Name = copy.Name;
			Surname = copy.Surname;
			Ecv = copy.Ecv;
			Id = copy.Id;
			Visits = copy.Visits;
		}
		#endregion // Constructors

		#region Public functions
		/// <summary>
		/// Returns string representation of data in this class.
		/// </summary>
		/// <returns>string</returns>
		public override string ToString()
		{
			return $"[{Id}, {Ecv}] {Name} {Surname} ({Visits.Count})\n";
		}

		/// <summary>
		/// Serializes the class to byte array.
		/// </summary>
		/// <returns>Byte array of the class.</returns>
        public byte[] ToByteArray()
        {
			byte[] bytes = new byte[GetSize()];
			int offset = 0;
			
			// Id
			BitConverter.GetBytes(Id).CopyTo(bytes, offset);
            offset += sizeof(int);

			// Name length
			int nameLength = Name.Length;
			BitConverter.GetBytes(nameLength).CopyTo(bytes, offset);
			offset += sizeof(int);

			// Name
			var nameToFile = string.Empty;
			if (Name.Length <= MAX_NAME_LENGTH)
			{
				nameToFile = Name.PadRight(MAX_NAME_LENGTH, ' ');
			}
			Encoding.ASCII.GetBytes(nameToFile).CopyTo(bytes, offset);
            offset += sizeof(char) * MAX_NAME_LENGTH;

			// Surname length
			int surnameLength = Surname.Length;
			BitConverter.GetBytes(surnameLength).CopyTo(bytes, offset);
			offset += sizeof(int);

			// Surname
			var surnameToFile = string.Empty;
			if (Surname.Length <= MAX_SURNAME_LENGTH)
			{
				surnameToFile = Surname.PadRight(MAX_SURNAME_LENGTH, ' ');
			}
			Encoding.ASCII.GetBytes(surnameToFile).CopyTo(bytes, offset);
            offset += sizeof(char) * MAX_SURNAME_LENGTH;
            
            // ECV length
            var ecvLength = Ecv.Length;
            BitConverter.GetBytes(ecvLength).CopyTo(bytes, offset);
            offset += sizeof(int);
            
            // ECV
            var ecvToFile = string.Empty;
            if (Ecv.Length <= MAX_ECV_LENGTH)
            {
	            ecvToFile = Ecv.PadRight(MAX_ECV_LENGTH, ' ');
            }
            Encoding.ASCII.GetBytes(ecvToFile).CopyTo(bytes, offset);
            offset += sizeof(char) * MAX_ECV_LENGTH;

			// Zaznamy count
			int zaznamyCount = Visits.Count;
			BitConverter.GetBytes(zaznamyCount).CopyTo(bytes, offset);
			offset += sizeof(int);

			// Zaznamy
			int zaznamSize = new Visit().GetSize();
			for (int i = 0; i < zaznamyCount; i++)
			{
				Visits[i].ToByteArray().CopyTo(bytes, offset);
				offset += zaznamSize;
			}
            return bytes;
        }

		/// <summary>
		/// Returns the class object from given byte array.
		/// </summary>
		/// <param name="byteArray">Byte array of class data.</param>
		/// <returns>New class object.</returns>
        public Person FromByteArray(byte[] byteArray)
        {
            int offset = 0;

			// Id
			Id = BitConverter.ToInt32(byteArray, offset);
			offset += sizeof(int);

			// Name length
			int nameLength = BitConverter.ToInt32(byteArray, offset);
			offset += sizeof(int);

			// Name
			Name = Encoding.ASCII.GetString(byteArray, offset, nameLength);
			offset += sizeof(char) * MAX_NAME_LENGTH;

			// Surname length
			int surnameLength = BitConverter.ToInt32(byteArray, offset);
			offset += sizeof(int);

			// Surname
			Surname = Encoding.ASCII.GetString(byteArray, offset, surnameLength);
			offset += sizeof(char) * MAX_SURNAME_LENGTH;
			
			// ECV length
			int ecvLength = BitConverter.ToInt32(byteArray, offset);
			offset += sizeof(int);
			
			// ECV
			Ecv = Encoding.ASCII.GetString(byteArray, offset, ecvLength);
			offset += sizeof(char) * MAX_ECV_LENGTH;

			// Zaznamy count
			int zaznamyCount = BitConverter.ToInt32(byteArray, offset);
			offset += sizeof(int);

			// Zaznamy
			Visits = new List<Visit>(MAX_VISITS);
			int zaznamSize = new Visit().GetSize();
			for (int i = 0; i < zaznamyCount; i++)
			{
				var bytes = byteArray.Skip(offset).Take(zaznamSize).ToArray();
				Visits.Add(new Visit().FromByteArray(bytes));
				offset += zaznamSize;
			}
			
			return this;
		}

		/// <summary>
		/// Returns the size of this class in bytes.
		/// </summary>
		/// <returns>Integer</returns>
		public int GetSize()
        {
            int ret = 0;
            ret += sizeof(int); // Id
            ret += sizeof(int); // Name length
            ret += sizeof(char) * MAX_NAME_LENGTH;
			ret += sizeof(int); // Surname length
			ret += sizeof(char) * MAX_SURNAME_LENGTH;
			ret += sizeof(int); // ECV length
			ret += sizeof(char) * MAX_ECV_LENGTH;
			ret += sizeof(int); // Zaznamy length
			var visitLength = new Visit().GetSize();
			ret += visitLength * MAX_VISITS;
            return ret;
        }

		/// <summary>
		/// Creates a new dummy class.
		/// </summary>
		/// <returns>Dummy class.</returns>
        public Person CreateClass()
        {
            return new Person();
        }

		/// <summary>
		/// Checks wether this and other data match Id or Ecv.
		/// </summary>
		/// <param name="data">Other class data.</param>
		/// <returns>True if equal, false other.</returns>
        public bool Equals(Person data)
        {
	        return Id == data.Id || Ecv == data.Ecv;
        }
        #endregion // Public functions
    }
}
