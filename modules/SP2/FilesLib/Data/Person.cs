using System.Collections;
using System.Text;
using FilesLib.Interfaces;

namespace FilesLib.Data
{
	public class Person : IHashable<Person>
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
		private List<Visit> _zaznamy = new(MAX_VISITS);
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
		/// Number of car repair visits.
		/// </summary>
		public int VisitCount => Zaznamy.Count;
		/// <summary>
		/// Car repair visits.
		/// </summary>
		/// <exception cref="ArgumentException">If list is too long.</exception>
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

		public Person(int id, string ecv, string name, string surname)
		{
			Name = name;
			Surname = surname;
			Ecv = ecv;
			Id = id;
			Zaznamy = new List<Visit>(MAX_VISITS);
		}

		public Person(Person p)
		{
			Name = p.Name;
			Surname = p.Surname;
			Ecv = p.Ecv;
			Id = p.Id;
			Zaznamy = p.Zaznamy;
		}
		#endregion // Constructors

		#region Public functions
		public BitArray GetHash()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			var zaznamy = string.Empty;
			int i = 1;
			foreach (var visit in Zaznamy)
			{
				zaznamy += $"{i}. {visit.ToString()}\n";
				i++;
			}
			return $"[{Id}, {Ecv}] {Name} {Surname} ({Zaznamy.Count}):\n{zaznamy}";
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
			var nameToFile = string.Empty;
			if (Name.Length < MAX_NAME_LENGTH)
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
			if (Surname.Length < MAX_SURNAME_LENGTH)
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
            if (Ecv.Length < MAX_ECV_LENGTH)
            {
	            ecvToFile = Ecv.PadRight(MAX_ECV_LENGTH, ' ');
            }
            Encoding.ASCII.GetBytes(ecvToFile).CopyTo(bytes, offset);
            offset += sizeof(char) * MAX_ECV_LENGTH;

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
			ret += sizeof(int); // ECV length
			ret += sizeof(char) * MAX_ECV_LENGTH;
			ret += sizeof(int); // Zaznamy length
			var visitLength = new Visit().GetSize();
			ret += visitLength * MAX_VISITS;
            return ret;
        }

        public Person CreateClass()
        {
            return new Person();
        }

        public bool Equals(Person data)
        {
	        if (Zaznamy.Count != data.Zaznamy.Count) return false;
	        
	        bool equal = true;
	        for (int i = 0; i < Zaznamy.Count; i++)
	        {
		        for (int j = 0; j < Zaznamy[i].Notes.Count; j++)
		        {
			        if (!Zaznamy[i].Notes[j].Equals(data.Zaznamy[i].Notes[j]))
			        {
				        equal = false;
				        break;
			        }
			        
		        }
	        }
	        return Id == data.Id && Name == data.Name && Surname == data.Surname && Ecv == data.Ecv && equal;
        }
        #endregion // Public functions
    }
}
