using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Clean.Architecture.Core.ProjectAggregate;
using Clean.Architecture.SharedKernel.Interfaces;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Clean.Architecture.Web.Endpoints.ProjectEndpoints;
#pragma warning disable CS8321 // Local function is declared but never used
public class Update : EndpointBaseAsync
    .WithRequest<UpdateProjectRequest>
    .WithActionResult<UpdateProjectResponse>
{
  private readonly IRepository<Project> _repository;

  public Update(IRepository<Project> repository)
  {
    _repository = repository;
  }

  [HttpPut(UpdateProjectRequest.Route)]
  [SwaggerOperation(
      Summary = "Updates a Project",
      Description = "Updates a Project. Only supports changing the name.",
      OperationId = "Projects.Update",
      Tags = new[] { "ProjectEndpoints" })
  ]
  public override async Task<ActionResult<UpdateProjectResponse>> HandleAsync(
    UpdateProjectRequest request,
      CancellationToken cancellationToken = new())
  {
    var result = await Validate()
      .Bind(GetProject)
      .Bind(UpdateName)
      .Bind(SaveChangesAsync)
      .Match(
        onSuccess: project => Ok(new UpdateProjectResponse(new ProjectRecord(project.Id, project.Name))),
        onFailure: ErrorHandler);

    return result;

    UnitResult<Exception> Validate()
    {
      return request.Name == null ? UnitResult.Failure<Exception>(new ArgumentNullException(nameof(request.Name))) : UnitResult.Success<Exception>();
    }

    async Task<Result<Project, Exception>> GetProject()
    {
      var existingProject = await _repository.GetByIdAsync(request.Id, cancellationToken);

      if (existingProject == null)
      {
        return Result.Failure<Project, Exception>(new NotFoundException("Id", "Project"));
      }

      return existingProject;
    }

    // usage: .Bind(UpdateNamePartial(request.Name!)
    Func<Project, Result<Project, Exception>> UpdateNamePartial(string name)
    {
      return (p) => UpdateNameImmutably(p, name);
    }

    Result<Project, Exception> UpdateName(Project existingProject)
    {
      try
      {
        existingProject.UpdateNameWithGuard(request.Name!);

        return existingProject;
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

    // No "external" side effects
    Result<Project, Exception> UpdateNameOrReturnException(Project existingProject, string name)
    {
      return existingProject.UpdateNameOrReturnException(name);
    }

    // no side effects, returns a new Project leaving original intact
    Result<Project, Exception> UpdateNameImmutably(Project existingProject, string name)
    {
      var updatedProject = existingProject.CreateProjectWithUpdatedName(name);

      return updatedProject;
    }

    async Task<Result<Project, Exception>> SaveChangesAsync(Project project)
    {
      await _repository.UpdateAsync(project, cancellationToken);

      return project;
    }

    ActionResult ErrorHandler(Exception ex) =>
    ex switch
    {
      ArgumentNullException => BadRequest(ex.Message),
      NotFoundException => NotFound(ex.Message),
      _ => Problem(ex.Message)
    };
  }
}







#pragma warning restore CS8321 // Local function is declared but never used
