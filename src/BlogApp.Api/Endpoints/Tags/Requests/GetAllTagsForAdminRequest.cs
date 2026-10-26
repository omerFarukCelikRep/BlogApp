using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Tags.Requests;

public record GetAllTagsForAdminRequest(
    [FromQuery] int? Page,
    [FromQuery] int? PageSize);