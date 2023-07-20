using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using Clean.Architecture.Core.ProjectAggregate;
using Clean.Architecture.Core.ProjectAggregate.Specifications;
using Clean.Architecture.SharedKernel.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Clean.Architecture.Web.Endpoints.ProjectEndpoints;

public class GetById : EndpointBaseAsync
  .WithRequest<GetProjectByIdRequest>
  .WithActionResult<GetProjectByIdResponse>
{
  private readonly IRepository<Project> _repository;

  public GetById(IRepository<Project> repository)
  {
    _repository = repository;
  }

  [HttpGet(GetProjectByIdRequest.Route)]
  [SwaggerOperation(
    Summary = "Gets a single Project",
    Description = "Gets a single Project by Id",
    OperationId = "Projects.GetById",
    Tags = new[] { "ProjectEndpoints" })
  ]
  public override async Task<ActionResult<GetProjectByIdResponse>> HandleAsync(
    [FromRoute] GetProjectByIdRequest request,
    CancellationToken cancellationToken = new()) =>
    await GetProject(request, cancellationToken)
      .ToResult(new NotFoundException("Id", "Project"))
      .Map(MapProjectToResponse)
      .Match(
        onSuccess: Ok,
        onFailure: ErrorHandler);

  private async Task<Maybe<Project>> GetProject(GetProjectByIdRequest request, CancellationToken cancellationToken)
  {
    var spec = new ProjectByIdWithItemsSpec(request.ProjectId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (entity is null)
    {
      return Maybe<Project>.None;
    }

    return entity;
  }

  private GetProjectByIdResponse MapProjectToResponse(Project entity) =>
    new GetProjectByIdResponse(
        id: entity.Id,
        name: entity.Name,
        items: entity.Items.Select(
          item => new ToDoItemRecord(item.Id, 
            item.Title,
            item.Description,
            item.IsDone,
            item.ContributorId))
          .ToList()
      );

  private ActionResult ErrorHandler(Exception ex) =>
    ex switch
    {
      NotFoundException => NotFound(ex.Message),
      _ => Problem(ex.Message)
    };
}
