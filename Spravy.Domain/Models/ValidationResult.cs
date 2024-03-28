namespace Spravy.Domain.Models
{
    public abstract class ValidationResult
    {
        protected ValidationResult(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }
    }
}