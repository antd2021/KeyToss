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

    public async Task SeedDataAsync()
    {
        // Check if the database already has some data, and if not, insert some default records.
        var existingPasswords = await _connection.Table<Password>().ToListAsync();
        if (!existingPasswords.Any()) // If no data exists
        {
            var defaultPasswords = new List<Password>
        {
            new Password { WebsiteName = "ExampleSite1", Username = "user1", EncryptedPassword = "encrypted1", LastModified = DateTime.Now, ExpirationDate = DateTime.Now.AddYears(1) },
            new Password { WebsiteName = "ExampleSite2", Username = "user2", EncryptedPassword = "encrypted2", LastModified = DateTime.Now, ExpirationDate = DateTime.Now.AddYears(1) }
        };

            foreach (var password in defaultPasswords)
            {
                await _connection.InsertAsync(password); // Insert each default password
            }
        }
    }
    private async Task InitializeAsync()
    {
        await _connection.CreateTableAsync<Password>();

        //Cleaning up from previous runs can delete after first run
        await _connection.ExecuteAsync("DROP TABLE IF EXISTS User;");
        SeedDataAsync().Wait();
    }

    public async Task<List<Password>> GetCustomers()
    {
        return await _connection.Table<Password>().ToListAsync();
    }

    public async Task<Password> GetById(int id)
    {
        return await _connection.Table<Password>().Where(x => x.PasswordId == id).FirstOrDefaultAsync();
    }
    public async Task Create (Password password)
    {
        await _connection.InsertAsync(password);
    }

    public async Task Update(Password password)
    {
        await _connection.UpdateAsync(password);
    }

    public async Task Delete(Password password)
    {
        await _connection.DeleteAsync(password);
    }
}
