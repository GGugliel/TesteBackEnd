using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Domain.Entities;

public class Book : Entity<Book>, IHasDomainEvent
{
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public List<DomainEvent> DomainEvents { get; } = new();

    private Book() { }

    public Book(string title, string author, string description)
    {
        Validate(title, author, description);

        Title = title;
        Author = author;
        Description = description;

        AddDomainEvent(new BookCreatedEvent(this));
    }

    public void Update(string title, string author, string description)
    {
        Validate(title, author, description);

        Title = title;
        Author = author;
        Description = description;

        SetLastAction();
    }

    private void Validate(string title, string author, string description)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length < 10 || title.Length > 100)
            throw new DomainException("Título deve ter entre 10 e 100 caracteres.");

        if (string.IsNullOrWhiteSpace(author) || author.Length < 10 || author.Length > 100)
            throw new DomainException("Autor deve ter entre 10 e 100 caracteres.");

        if (description.Length > 1024)
            throw new DomainException("Descrição deve ter no máximo 1024 caracteres.");
    }

    public override bool IsValid()
    {
        try
        {
            Validate(Title, Author, Description);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void AddDomainEvent(DomainEvent @event)
        => DomainEvents.Add(@event);
}