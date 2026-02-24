using Duende.IdentityServer.Test;

namespace TestApi.Identity;

public static class TestUsers
{
    public static List<TestUser> Users =>
        new()
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "alice",
                Password = "password"
            }
        };
}

