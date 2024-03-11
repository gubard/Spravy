namespace _build.Interfaces;

public interface IProjectBuilder
{
    void Setup(string host);
    void Clean();
    void Restore();
    void Compile();
}