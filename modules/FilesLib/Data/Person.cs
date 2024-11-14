using FilesLib.Helpers;

namespace FilesLib.Data
{
	public class Person : IData<Person>
	{
		#region Constants
		private const int MAX_VISITS = 5;
		private const int MAX_NAME_LENGTH = 15;
		private const int MAX_SURNAME_LENGTH = 20;
		#endregion // Constants

		#region Properties
		public string Name { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
		public int Id { get; set; } = -1;
		public Visit[] Zaznamy { get; set; } = new Visit[MAX_VISITS];
		#endregion // Properties

		#region Constructors
		public Person()
		{
		}

		public Person(int id, string name, string surname)
		{
			if (name.Length > MAX_NAME_LENGTH)
			{
				throw new ArgumentException($"Name is too long. Max length is {MAX_NAME_LENGTH}.");
			}
			if (surname.Length > MAX_SURNAME_LENGTH)
			{
				throw new ArgumentException($"Surname is too long. Max length is {MAX_SURNAME_LENGTH}.");
			}
			
			Name = name;
			Surname = surname;
			Id = id;
		}
		#endregion // Constructors

		#region Public functions
		public override string ToString()
		{
			return $"[{Id}] {Name} {Surname} ";
		}

        public byte[] ToByteArray()
        {
            return Serializator.Serialize(this);
        }

        public Person FromByteArray(byte[] byteArray)
        {
            return Serializator.Deserialize<Person>(byteArray);
        }

        public int GetSize()
        {
            int ret = 0;
            ret += sizeof(int);
            ret += sizeof(char) * MAX_NAME_LENGTH;
            ret += sizeof(char) * MAX_SURNAME_LENGTH;
            ret += Zaznamy[0].GetSize() * MAX_VISITS;
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
