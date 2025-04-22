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
        await _connection.CreateTableAsync<Password>();

        //Cleaning up from previous runs can delete after first run
        await _connection.ExecuteAsync("DROP TABLE IF EXISTS User;");
    }

    // Inserts a new password entry into the Password table and returns the number of rows inserted
    public Task<int> AddPasswordAsync(Password password) => _connection.InsertAsync(password);

    //Grabs all password objects from the Password table and returns them as a list
    public Task<List<Password>> GetAllPasswordsAsync() =>
    _connection.Table<Password>().ToListAsync();


}
