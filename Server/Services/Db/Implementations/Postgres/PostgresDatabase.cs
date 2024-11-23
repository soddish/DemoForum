using System.Diagnostics.Eventing.Reader;
using DemoForum.Models;
using Microsoft.VisualBasic;
using Npgsql;

namespace DemoForum.Services.Db.Implementations.Postgres;

public class PostgresDatabase(IConfiguration configuration) : 
    ISubforumStore
    //IReplyStore, IThreadStore, IUserStore
{
    async ValueTask<NpgsqlDataSource> CreateDataSource()
    {
        var connectionString = configuration.GetValue<string>("ConnectionStrings:DemoForum")!;
        var datasource = NpgsqlDataSource.Create(connectionString);

        var command = datasource.CreateCommand(@"
CREATE TABLE IF NOT EXISTS
    subforums (
        id INT NOT NULL PRIMARY KEY, 
        name TEXT NOT NULL, 
        newThreadsOpen BOOLEAN NOT NULL
    );
    
CREATE TABLE IF NOT EXISTS
    replies (
        id INT NOT NULL PRIMARY KEY, 
        userId INT NOT NULL,
        creationTime INT NOT NULL,
        parentThreadId INT NOT NULL,
        content TEXT NOT NULL
    );
        
CREATE TABLE IF NOT EXISTS
    threads (
        id INT NOT NULL PRIMARY KEY,
        userId INT NOT NULL,
        subforumId INT NOT NULL,
        creationTime INT NOT NULL,
        title TEXT NOT NULL,
        content TEXT NOT NULL,
        repliesOpen BOOLEAN NOT NULL
    );
        
CREATE TABLE IF NOT EXISTS
    users (
        id INT NOT NULL PRIMARY KEY,
        creationTime INT NOT NULL,
        userName TEXT NOT NULL,
        isAdmin BOOLEAN NOT NULL,
        bannedUntil INT);");

        await command.ExecuteNonQueryAsync();

        return datasource;
    }

    private NpgsqlDataSource? _dataSource;

    async ValueTask<NpgsqlDataSource> GetDataSource()
    {
        _dataSource ??= await CreateDataSource();

        return _dataSource;
    }

    async ValueTask<NpgsqlCommand> CreateCommand(string commandText)
    {
        var dataSource = await GetDataSource();

        var command = dataSource.CreateCommand(commandText);

        return command;
    }

    async Task<bool> ISubforumStore.Add(Subforum subforum)
    {
        var command = await CreateCommand(@"
INSERT INTO
    subforums (name, newThreadsOpen)
VALUES
    $name, $newThreadsOpen
RETURNING
    id;");

        command.Parameters.AddWithValue("name", NpgsqlTypes.NpgsqlDbType.Text, subforum.Name);
        command.Parameters.AddWithValue("newThreadsOpen", NpgsqlTypes.NpgsqlDbType.Boolean, subforum.NewThreadsOpen);

        var result = await command.ExecuteScalarAsync();
        if(result is int id)
        {
            subforum.Id = id;
            return true;
        }
        else
            return false;
    }

    async Task<Subforum[]> ISubforumStore.GetAll()
    {
        var command = await CreateCommand(@"
SELECT 
    id,
    name,
    newThreadsOpen
FROM
    subforums;");

        await using var reader = await command.ExecuteReaderAsync();

        List<Subforum> results = [];
        while (await reader.ReadAsync())
        {
            var subforum = new Subforum
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                NewThreadsOpen = reader.GetBoolean(2)
            };
        }

        return [.. results];
    }

    Task<Subforum?> ISubforumStore.GetById(int subforumId)
    {
        throw new NotImplementedException();
    }

    Task<bool> ISubforumStore.Update(int id, Subforum subforum)
    {
        throw new NotImplementedException();
    }
}