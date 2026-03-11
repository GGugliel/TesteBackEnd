using System.Reflection;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data;

public class SqlDbContext : DbContext
{
    private readonly IDomainEventHandler? _domainEventService;

    // Construtor usado pelo EF Core em tempo de design (migrations)
    public SqlDbContext(DbContextOptions<SqlDbContext> options)
        : base(options)
    {
    }

    // Construtor usado em runtime pela aplicação
    public SqlDbContext(DbContextOptions<SqlDbContext> options, IDomainEventHandler domainEventService)
        : base(options)
    {
        _domainEventService = domainEventService;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        if (_domainEventService is not null)
            await DispatchEvents();

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(dateTimeConverter);
            }
        }

        // Impedir que o EF tente mapear classes do FluentValidation
        modelBuilder.Ignore<FluentValidation.Results.ValidationFailure>();
        modelBuilder.Ignore<FluentValidation.Results.ValidationResult>();

        base.OnModelCreating(modelBuilder);
    }

    private async Task DispatchEvents()
    {
        while (true)
        {
            var domainEventEntity = ChangeTracker.Entries<IHasDomainEvent>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .Where(domainEvent => !domainEvent.IsPublished)
                .FirstOrDefault();

            if (domainEventEntity is null)
                break;

            domainEventEntity.IsPublished = true;

            if (_domainEventService is not null)
                await _domainEventService.Publish(domainEventEntity);
        }
    }
}