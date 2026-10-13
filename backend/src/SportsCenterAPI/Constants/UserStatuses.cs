namespace SportsCenterAPI.Constants;

/// <summary>
/// Trạng thái tài khoản, khớp CHECK constraint của cột User.status.
/// </summary>
public static class UserStatuses
{
    public const string Active = "ACTIVE";
    public const string Inactive = "INACTIVE";
    public const string Suspended = "SUSPENDED";
}
