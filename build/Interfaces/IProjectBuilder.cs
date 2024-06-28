namespace _build.Interfaces;

public interface IProjectBuilder
{
    void Setup();
    void Clean();
    void Restore();
    void Compile();
}
