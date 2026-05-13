using System.Collections.Concurrent;
using IvfClinic.Models;

namespace IVF_Managment_Api.Services;

public static class UserCredentialStore
{
    private static readonly ConcurrentDictionary<string, UserCredential> ByUsername = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<string, UserCredential> ByEmail = new(StringComparer.OrdinalIgnoreCase);

    public record UserCredential(Guid Id, string Username, string Email, string PasswordHash, UserRole Role);

    public static void Register(UserCredential credential)
    {
        ByUsername[credential.Username] = credential;
        ByEmail[credential.Email] = credential;
    }

    public static void Remove(string username, string email)
    {
        ByUsername.TryRemove(username, out _);
        ByEmail.TryRemove(email, out _);
    }

    public static UserCredential? FindByUsernameOrEmail(string usernameOrEmail)
    {
        if (ByUsername.TryGetValue(usernameOrEmail, out var cred))
            return cred;
        if (ByEmail.TryGetValue(usernameOrEmail, out cred))
            return cred;
        return null;
    }
}