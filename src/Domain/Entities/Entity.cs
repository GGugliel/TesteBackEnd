using Domain.Events;
using Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace Domain.Entities;

public abstract class Entity<TEntity> : AbstractValidator<TEntity>, IHasDomainEvent where TEntity : Entity<TEntity>
{
    public long Id { get; protected set; }
    public DateTime CreatedDate { get; protected set; }
    public DateTime? LastUpdatedDate { get; protected set; }

    public abstract bool IsValid();

    public ValidationResult ValidationResult { get; protected set; }
    public List<DomainEvent> DomainEvents { get; set; }

    protected Entity()
    {
        ValidationResult = new ValidationResult();
        DomainEvents = [];
    }

    public void SetLastAction()
    {
        if (Id > 0)
            LastUpdatedDate = DateTime.Now;
        else
            CreatedDate = DateTime.Now;
    }
}