using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Tags.Requests;

public record GetAllCategoriesForAdminRequest(
    [FromQuery] int? Page,
    [FromQuery] int? PageSize);