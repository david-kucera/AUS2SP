namespace FilesLib
{
	public interface IRecord<T> where T : class
	{
		byte[] ToByteArray();
		T FromByteArray(byte[] byteArray);
		int GetSize();
	}
}
