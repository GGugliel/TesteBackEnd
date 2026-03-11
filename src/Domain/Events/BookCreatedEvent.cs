using Domain.Entities;
using Domain.Events;

namespace Domain.Events;

public sealed class BookCreatedEvent : DomainEvent
{
    public Book Book { get; }

    public BookCreatedEvent(Book book)
    {
        Book = book;
    }
}