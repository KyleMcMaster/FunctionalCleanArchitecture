using Clean.Architecture.SharedKernel;

namespace Clean.Architecture.Core.Adapters;
public class ExecutionContext<T> where T : EntityBase
{
  public T Entity { get; init; }
  public IEnumerable<DomainEventBase> DomainEvents { get; init; } = new List<DomainEventBase>();

  public ExecutionContext(T entity, IEnumerable<DomainEventBase> domainEvents)
  {
    Entity = entity;
    DomainEvents = domainEvents;
  }
}
