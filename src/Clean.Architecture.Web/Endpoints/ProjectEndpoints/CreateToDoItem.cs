using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Clean.Architecture.Core.Adapters;
using Clean.Architecture.Core.ProjectAggregate;
using Clean.Architecture.Core.ProjectAggregate.Specifications;
using Clean.Architecture.SharedKernel.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Clean.Architecture.Web.Endpoints.ProjectEndpoints;
#pragma warning disable CS8321 // Local function is declared but never used
public class CreateToDoItem : EndpointBaseAsync
  .WithRequest<CreateToDoItemRequest>
  .WithActionResult
{
  private readonly IRepository<Project> _repository;

  public CreateToDoItem(IRepository<Project> repository)
  {
    _repository = repository;
  }

  [HttpPost(CreateToDoItemRequest.Route)]
  [SwaggerOperation(
    Summary = "Creates a new ToDo Item for a Project",
    Description = "Creates a new ToDo Item for a Project",
    OperationId = "Project.CreateToDoItem",
    Tags = new[] { "ProjectEndpoints" })
  ]
  public override async Task<ActionResult> HandleAsync(
    CreateToDoItemRequest request,
    CancellationToken cancellationToken = new())
  {
    return await GetProject()
      .Bind(CreateToDoItem)
      .Bind(SaveChangesAsync)
      .Match(
        onSuccess: _ => Created(GetProjectByIdRequest.BuildRoute(request.ProjectId), null),
        onFailure: ErrorHandler);




    async Task<Result<Project, Exception>> GetProject()
    {
      var spec = new ProjectByIdWithItemsSpec(request.ProjectId);
      var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

      if (entity is null)
      {
        return Result.Failure<Project, Exception>(new NotFoundException("Id", "Project"));
      }

      return entity;
    }


    Result<Project, Exception> CreateToDoItem(Project entity)
    {
      var newItem = new ToDoItem
      {
        Title = request.Title!,
        Description = request.Description!
      };

      if (request.ContributorId.HasValue)
      {
        newItem.AddContributor(request.ContributorId.Value);
      }

      entity.AddItem(newItem);

      return entity;
    }

    async Task<Result<Project, Exception>> SaveChangesAsync(Project entity)
    {
      await _repository.UpdateAsync(entity, cancellationToken);

      return entity;
    }

    ActionResult ErrorHandler(Exception ex) =>
      ex switch
      {
        ArgumentNullException => BadRequest(ex.Message),
        NotFoundException => NotFound(ex.Message),
        _ => Problem(ex.Message)
      };




    Result<ExecutionContext<Project>, Exception> CreateToDoItemAndDomainEvents(Project entity)
    {
      var newItem = new ToDoItem
      {
        Title = request.Title!,
        Description = request.Description!
      };

      if (request.ContributorId.HasValue)
      {
        newItem.AddContributor(request.ContributorId.Value);
      }

      return entity.AddItemToProject(newItem);
    }

    Result<ExecutionContext<Project>, Exception> FireDomainEventsAsync(ExecutionContext<Project> context)
    {
      foreach (var domainEvent in context.DomainEvents)
      {
        //await _mediator.Publish(domainEvent);
      }

      return context;
    }

    //return await GetProject()
    //  .Bind(CreateToDoItemAndDomainEvents)
    //  .Bind(SaveChangesAsync)
    //  .Bind(FireDomainEventsAsync)
    //  .Match(
    //    onSuccess: _ => Created(GetProjectByIdRequest.BuildRoute(request.ProjectId), null),
    //    onFailure: ErrorHandler);
  }
}
#pragma warning restore CS8321 // Local function is declared but never used
