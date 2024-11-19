using System.Text;

namespace FilesLib.Data
{
	public class Person : IData<Person>
	{
		#region Constants
		private const int MAX_VISITS = 5;
		private const int MAX_NAME_LENGTH = 15;
		private const int MAX_SURNAME_LENGTH = 20;
		#endregion // Constants
		
		#region Class members
		private string _name = string.Empty;
		private string _surname = string.Empty;
		private List<Visit> _zaznamy = new List<Visit>(MAX_VISITS);
		#endregion // Class members

		#region Properties
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
		public int Id { get; set; } = -1;
		public List<Visit> Zaznamy
		{
			get => _zaznamy;
			set
			{
				if (value.Count > MAX_VISITS)
				{
					throw new ArgumentException($"Number of visits must be less than {MAX_VISITS}.");
				}
				_zaznamy = value;
			}
		}
		#endregion // Properties

		#region Constructors
		public Person()
		{
			
		}

		public Person(int id, string name, string surname)
		{
			Name = name;
			Surname = surname;
			Id = id;
			Zaznamy = new List<Visit>(MAX_VISITS);
		}
		#endregion // Constructors

		#region Public functions
		public override string ToString()
		{
			return $"[{Id}] {Name} {Surname} {Zaznamy.Count}: {Zaznamy}";
		}

		public void Add(Visit visit)
		{
			Zaznamy.Add(visit);
		}

		public Visit Get(int i)
		{
			return Zaznamy.ElementAt(i);
		}

		public void Remove(Visit visit)
		{
			Zaznamy.Remove(visit);
		}

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
			if (Name.Length < MAX_NAME_LENGTH)
			{
				Name = Name.PadRight(MAX_NAME_LENGTH, ' ');
			}
			Encoding.ASCII.GetBytes(Name).CopyTo(bytes, offset);
            offset += sizeof(char) * MAX_NAME_LENGTH;

			// Surname length
			int surnameLength = Surname.Length;
			BitConverter.GetBytes(surnameLength).CopyTo(bytes, offset);
			offset += sizeof(int);

			// Surname
			if (Surname.Length < MAX_SURNAME_LENGTH)
			{
				Surname = Surname.PadRight(MAX_SURNAME_LENGTH, ' ');
			}
			Encoding.ASCII.GetBytes(Surname).CopyTo(bytes, offset);
            offset += sizeof(char) * MAX_SURNAME_LENGTH;

			// Zaznamy count
			int zaznamyCount = Zaznamy.Count;
			BitConverter.GetBytes(zaznamyCount).CopyTo(bytes, offset);
			offset += sizeof(int);

			// Zaznamy
			int zaznamSize = new Visit().GetSize();
			for (int i = 0; i < zaznamyCount; i++)
			{
				Zaznamy[i].ToByteArray().CopyTo(bytes, offset);
				offset += zaznamSize;
			}
            return bytes;
        }

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

			// Zaznamy count
			int zaznamyCount = BitConverter.ToInt32(byteArray, offset);
			offset += sizeof(int);

			// Zaznamy
			Zaznamy = new List<Visit>(MAX_VISITS);
			int zaznamSize = new Visit().GetSize();
			for (int i = 0; i < zaznamyCount; i++)
			{
				var bytes = byteArray.Skip(offset).Take(zaznamSize).ToArray();
				Zaznamy.Add(new Visit().FromByteArray(bytes));
				offset += zaznamSize;
			}
			
			return this;
		}

		public int GetSize()
        {
            int ret = 0;
            ret += sizeof(int); // Id
            ret += sizeof(int); // Name length
            ret += sizeof(char) * MAX_NAME_LENGTH;
			ret += sizeof(int); // Surname length
			ret += sizeof(char) * MAX_SURNAME_LENGTH;
			ret += sizeof(int); // Zaznamy length
			var visit = new Visit();
			ret += visit.GetSize() * MAX_VISITS;
            return ret;
        }

        public Person CreateClass()
        {
            return new Person();
        }

        public bool Equals(Person data)
        {
            return Id == data.Id && Name == data.Name && Surname == data.Surname && Zaznamy.Equals(data.Zaznamy);
        }
        #endregion // Public functions
    }
}
