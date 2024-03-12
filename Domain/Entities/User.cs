namespace Domain.Entities;

public class User
{
    private User(string email, string passwordHash, string salt)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        Salt = salt;
    }
    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }
    
    public string Salt { get; private set; }

    public static User Create(string email, string passwordHash, string salt)
    {
        return new User(email, passwordHash, salt);
    }
}