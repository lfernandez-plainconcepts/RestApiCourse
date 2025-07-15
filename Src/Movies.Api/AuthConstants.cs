namespace Movies.Api;

public static class AuthConstants
{
    public static class Policies
    {
        public const string Admin = "Admin";
        public const string TrustedMember = "TrustedMember";
    }

    public static class Claims
    {
        public const string Admin = "admin";
        public const string TrustedMember = "trusted_member";
    }
}
