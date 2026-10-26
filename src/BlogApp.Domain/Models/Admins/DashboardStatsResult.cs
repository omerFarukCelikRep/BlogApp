namespace BlogApp.Domain.Models.Admins;

public record DashboardStatsResult(
    int TotalUsers,
    int TotalBlogs,
    int TotalComments,
    int TotalLikes,
    int TotalTags,
    int TotalCategories,
    int NewUsersThisWeek,
    int NewBlogsThisWeek,
    int NewCommentsThisWeek,
    int PublishedBlogs,
    int DraftBlogs,
    int ArchivedBlogs,
    List<RecentActivityResult> RecentActivity,
    List<DailyCountResult> BlogsPerDay,
    List<DailyCountResult> UsersPerDay);