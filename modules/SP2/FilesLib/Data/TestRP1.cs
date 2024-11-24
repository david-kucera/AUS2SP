using System.Collections;
using System.Text;
using FilesLib.Interfaces;

namespace FilesLib.Data;

public class TestRP1 : IHashable<TestRP1>
	{
		#region Constants
		private const int MAX_NAME_LENGTH = 15;
		private const int MAX_SURNAME_LENGTH = 20;
		private const int MAX_ECV_LENGTH = 10;
		#endregion // Constants
		
		#region Class members
		private string _name = string.Empty;
		private string _surname = string.Empty;
		private string _ecv = string.Empty;
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
		#endregion // Properties

		#region Constructors
		public TestRP1()
		{
			
		}

		public TestRP1(int id, string ecv, string name, string surname)
		{
			Name = name;
			Surname = surname;
			Ecv = ecv;
			Id = id;
		}

		public TestRP1(TestRP1 p)
		{
			Name = p.Name;
			Surname = p.Surname;
			Ecv = p.Ecv;
			Id = p.Id;
		}
		#endregion // Constructors

		#region Public functions
		public BitArray GetHash()
		{
			return new BitArray(BitConverter.GetBytes(Id));
		}

		public override string ToString()
		{
			return $"[{Id}, {Ecv}] {Name} {Surname}";
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
            
            return bytes;
        }

        public TestRP1 FromByteArray(byte[] byteArray)
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
            return ret;
        }

        public TestRP1 CreateClass()
        {
            return new TestRP1();
        }

        public bool Equals(TestRP1 data)
        {
	        return Id == data.Id && Ecv == data.Ecv;
        }
        #endregion // Public functions
    }