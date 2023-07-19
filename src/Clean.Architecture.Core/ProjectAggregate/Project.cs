using Ardalis.GuardClauses;
using Clean.Architecture.Core.Adapters;
using Clean.Architecture.Core.ProjectAggregate.Events;
using Clean.Architecture.SharedKernel;
using Clean.Architecture.SharedKernel.Interfaces;
using CSharpFunctionalExtensions;

namespace Clean.Architecture.Core.ProjectAggregate;

public class Project : EntityBase, IAggregateRoot
{
  public string Name { get; private set; }

  private readonly List<ToDoItem> _items = new();
  public IEnumerable<ToDoItem> Items => _items.AsReadOnly();
  public ProjectStatus Status => _items.All(i => i.IsDone) ? ProjectStatus.Complete : ProjectStatus.InProgress;

  public PriorityStatus Priority { get; }

  /// <summary>
  /// Constructor for creating a new Project with no items
  /// </summary>
  /// <param name="name"></param>
  /// <param name="priority"></param>
  public Project(string name, PriorityStatus priority)
  {
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
    Priority = priority;
  }

  /// <summary>
  /// Alternative take on Copy Constructor for creating a Project from an existing one
  /// </summary>
  /// <param name="name"></param>
  /// <param name="priority"></param>
  /// <param name="toDoItems"></param>
  /// <param name="domainEvents"></param>
  private Project(string name, PriorityStatus priority, IEnumerable<ToDoItem> toDoItems, IEnumerable<DomainEventBase> domainEvents)
  {
    _items.AddRange(toDoItems);
    Name = name;
    Priority = priority;

    foreach (var domainEvent in domainEvents)
    {
      RegisterDomainEvent(domainEvent);
    }
  }

  public void UpdateNameWithGuard(string newName)
  {
    Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
  }

  public Result<Project, Exception> UpdateNameOrReturnException(string newName)
  {
    if (string.IsNullOrEmpty(newName))
    {
      return new ArgumentException("Name is required");
    }

    Name = newName;

    return this;
  }

  public Result<Project, Exception> CreateProjectWithUpdatedName(string newName)
  {
    if (string.IsNullOrEmpty(newName))
    {
      return new ArgumentException("Cannot update Project.Name with null or empty value");
    }

    var project = new Project(newName, Priority, _items, DomainEvents)
    {
      Id = Id,
    };

    return project;
  }

  public void AddItem(ToDoItem newItem)
  {
    Guard.Against.Null(newItem, nameof(newItem));
    _items.Add(newItem);

    var newItemAddedEvent = new NewItemAddedEvent(this, newItem);
    base.RegisterDomainEvent(newItemAddedEvent);
  }

  public Result<ExecutionContext<Project>, Exception> AddItemToNewProject(ToDoItem newItem)
  {
    if (newItem is null)
    {
      return new ArgumentNullException(nameof(ToDoItem));
    }

    var items = _items.Union(new[] { newItem });

    var newItemAddedEvent = new NewItemAddedEvent(this, newItem);

    var project = new Project(Name, Priority, items, Enumerable.Empty<DomainEventBase>())
    {
      Id = Id,
    };

    return new ExecutionContext<Project>(project, new[] { newItemAddedEvent });
  }
}
