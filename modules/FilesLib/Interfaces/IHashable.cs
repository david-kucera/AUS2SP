namespace FilesLib.Interfaces;

public interface IHashable<T> : IData<T> where T : class
{
    int GetHash();
}