using KeyToss.Models;
using SQLite;
using System.IO;
using System.Threading.Tasks;

public class LocalDBService
{
    private const string DB_NAME = "LoaclDb.db3";
    // Name of the SQLite database file to store locally

    private readonly SQLiteAsyncConnection _connection;
    // SQLite connection object for executing commands on the database

    public LocalDBService()
    {
        _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        // Initialize the connection using a full path: AppDataDirectory + database filename

        InitializeAsync().Wait();
    }

    private async Task InitializeAsync()
    {
        await _connection.ExecuteAsync("PRAGMA foreign_keys = ON;");
        // Enable foreign key support in SQLite (it is OFF by default)

        await _connection.CreateTableAsync<User>();
        // Automatically create the User table if it doesn’t exist, using your User class definition

        await _connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Password (
                PasswordId INTEGER PRIMARY KEY AUTOINCREMENT,
                -- Auto-incrementing primary key for each password entry

                WebsiteName TEXT,
                -- Name of the website the password is for

                Username TEXT,
                -- Username used on that website

                EncryptedPassword TEXT,
                -- The actual encrypted password (stored as text)

                UserId INTEGER NOT NULL,
                -- Foreign key to the User table

                FOREIGN KEY(UserId) REFERENCES User(UserId) ON DELETE CASCADE
                -- Define the foreign key relationship: link UserId to User.UserId
                -- ON DELETE CASCADE means when a user is deleted, their passwords are also deleted
            );
        ");
        // Manually create the Password table with a foreign key to enforce relational integrity
    }


    public Task<int> AddUserAsync(User user) => _connection.InsertAsync(user);
    // Inserts a new user into the User table and returns the number of rows inserted

    public Task<int> AddPasswordAsync(Password password) => _connection.InsertAsync(password);
    // Returns all password records associated with a specific user based on UserId

    public Task<User> GetUserByUsernameAsync(string username)
        => _connection.Table<User>()
                      .Where(u => u.Username == username)
                      .FirstOrDefaultAsync();
}
