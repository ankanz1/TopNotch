using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TopNotch.Data;

/// <summary>
/// Database service for local data persistence
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Initializes the database
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Gets a table by type
    /// </summary>
    ITable<T> GetTable<T>() where T : class, new();

    /// <summary>
    /// Saves changes to the database
    /// </summary>
    Task SaveChangesAsync();
}

/// <summary>
/// Table interface for type-safe database access
/// </summary>
public interface ITable<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);
}

/// <summary>
/// SQLite database service implementation
/// </summary>
public class DatabaseService : IDatabaseService
{
    private SqliteConnection? _connection;
    private readonly string _databasePath;
    private bool _initialized;

    public DatabaseService()
    {
        // Cross-platform path to user's data directory
        var appDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TopNotch"
        );

        // On Linux, ApplicationData returns the home directory
        // Create the data directory if it doesn't exist
        Directory.CreateDirectory(appDataDir);
        _databasePath = Path.Combine(appDataDir, "topnotch.db");
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        var connectionString = $"Data Source={_databasePath}";
        _connection = new SqliteConnection(connectionString);
        await _connection.OpenAsync().ConfigureAwait(false);

        // Create tables
        await CreateTablesAsync().ConfigureAwait(false);

        // Run migrations
        await RunMigrationsAsync().ConfigureAwait(false);

        _initialized = true;
    }

    private async Task CreateTablesAsync()
    {
        var commands = new[]
        {
            // Settings table
            @"CREATE TABLE IF NOT EXISTS Settings (
                Key TEXT PRIMARY KEY,
                Value TEXT
            )",

            // Clipboard history table
            @"CREATE TABLE IF NOT EXISTS ClipboardHistory (
                Id TEXT PRIMARY KEY,
                Content TEXT,
                ContentType TEXT,
                Timestamp INTEGER,
                IsPinned INTEGER,
                Application TEXT
            )",

            // Reminders table
            @"CREATE TABLE IF NOT EXISTS Reminders (
                Id TEXT PRIMARY KEY,
                Title TEXT,
                TriggerTime INTEGER,
                IsRecurring INTEGER,
                Recurrence INTEGER,
                IsCompleted INTEGER,
                SnoozeMinutes INTEGER
            )",

            // File shelf table
            @"CREATE TABLE IF NOT EXISTS FileShelf (
                Id TEXT PRIMARY KEY,
                Name TEXT,
                Path TEXT,
                Size INTEGER,
                CreatedTime INTEGER,
                ModifiedTime INTEGER,
                IconPath TEXT
            )",

            // Paired devices table
            @"CREATE TABLE IF NOT EXISTS PairedDevices (
                Id TEXT PRIMARY KEY,
                DisplayName TEXT,
                IpAddress TEXT,
                SessionToken TEXT,
                PairedAt INTEGER,
                LastUsed INTEGER
            )",

            // Widgets table
            @"CREATE TABLE IF NOT EXISTS Widgets (
                Id TEXT PRIMARY KEY,
                Type TEXT,
                PositionX REAL,
                PositionY REAL,
                Width REAL,
                Height REAL,
                IsVisible INTEGER
            )",

            // Media info table (current media only)
            @"CREATE TABLE IF NOT EXISTS CurrentMedia (
                Id TEXT PRIMARY KEY DEFAULT 'current',
                Title TEXT,
                Artist TEXT,
                Album TEXT,
                AppName TEXT,
                Position REAL,
                Duration REAL,
                PlaybackState INTEGER,
                UpdatedAt INTEGER
            )"
        };

        foreach (var command in commands)
        {
            using var cmd = new SqliteCommand(command, _connection!);
            await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
    }

    private async Task RunMigrationsAsync()
    {
        // Check if migration version table exists, create if not
        using var checkCmd = new SqliteCommand(
            "SELECT value FROM Settings WHERE key = 'MigrationVersion'",
            _connection!
        );

        var result = await checkCmd.ExecuteScalarAsync().ConfigureAwait(false);
        var version = result?.ToString() ?? "0";

        // Apply migrations based on version
        while (version != "3") // Current version
        {
            version = await ApplyMigrationAsync(version).ConfigureAwait(false);
        }
    }

    private async Task<string> ApplyMigrationAsync(string currentVersion)
    {
        // Migration logic would go here
        // For now, just update the version
        var nextVersion = (int.Parse(currentVersion) + 1).ToString();

        using var cmd = new SqliteCommand(
            "INSERT OR REPLACE INTO Settings (Key, Value) VALUES ('MigrationVersion', @version)",
            _connection!
        );
        cmd.Parameters.AddWithValue("@version", nextVersion);
        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

        return nextVersion;
    }

    public ITable<T> GetTable<T>() where T : class, new()
    {
        if (!_initialized) throw new InvalidOperationException("Database not initialized");
        return new Table<T>(_connection!);
    }

    public async Task SaveChangesAsync()
    {
        // SQLite auto-commits, but we can optimize
        await Task.CompletedTask.ConfigureAwait(false);
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.CloseAsync().ConfigureAwait(false);
            await _connection.DisposeAsync().ConfigureAwait(false);
        }
    }
}

/// <summary>
/// Generic table implementation
/// </summary>
public class Table<T> : ITable<T> where T : class, new()
{
    private readonly SqliteConnection _connection;

    public Table(SqliteConnection connection)
    {
        _connection = connection;
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var tableName = typeof(T).Name;
        var command = $"SELECT * FROM {tableName} WHERE Id = @id LIMIT 1";

        using var cmd = new SqliteCommand(command, _connection);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
        if (await reader.ReadAsync().ConfigureAwait(false))
        {
            return MapFromReader(reader);
        }
        return null;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var tableName = typeof(T).Name;
        var command = $"SELECT * FROM {tableName}";

        using var cmd = new SqliteCommand(command, _connection);
        using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

        var results = new List<T>();
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            results.Add(MapFromReader(reader));
        }

        return results;
    }

    public async Task AddAsync(T entity)
    {
        var tableName = typeof(T).Name;
        var columns = GetColumns<T>();

        var columnList = string.Join(", ", columns.Select(c => c.Key));
        var parameterList = string.Join(", ", columns.Select(c => $"@{c.Key}"));

        var command = $"INSERT OR REPLACE INTO {tableName} ({columnList}) VALUES ({parameterList})";

        using var cmd = new SqliteCommand(command, _connection);
        AddParameters(cmd, entity, columns);

        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task UpdateAsync(T entity)
    {
        await AddAsync(entity).ConfigureAwait(false); // SQLite upsert
    }

    public async Task DeleteAsync(T entity)
    {
        var tableName = typeof(T).Name;
        var id = GetIdValue(entity);

        var command = $"DELETE FROM {tableName} WHERE Id = @id";

        using var cmd = new SqliteCommand(command, _connection);
        cmd.Parameters.AddWithValue("@id", id);

        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate)
    {
        var items = await GetAllAsync().ConfigureAwait(false);
        return items.Where(predicate);
    }

    private T MapFromReader(SqliteDataReader reader)
    {
        var entity = new T();
        var properties = typeof(T).GetProperties();

        foreach (var prop in properties)
        {
            try
            {
                var ordinal = reader.GetOrdinal(prop.Name);
                if (!reader.IsDBNull(ordinal))
                {
                    var value = reader.GetValue(ordinal);
                    prop.SetValue(entity, value);
                }
            }
            catch
            {
                // Ignore columns that don't exist in the object
            }
        }

        return entity;
    }

    private Dictionary<string, object?> GetColumns<TEntity>() where TEntity : class
    {
        var columns = new Dictionary<string, object?>();
        var properties = typeof(TEntity).GetProperties();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(new TEntity());
            if (value != null || prop.CanWrite)
            {
                columns[prop.Name] = value;
            }
        }

        return columns;
    }

    private void AddParameters(SqliteCommand command, object entity, Dictionary<string, object?> columns)
    {
        var properties = entity.GetType().GetProperties();

        foreach (var prop in properties)
        {
            if (columns.ContainsKey(prop.Name))
            {
                var value = prop.GetValue(entity);
                if (value != null)
                {
                    command.Parameters.AddWithValue($"@{prop.Name}", value);
                }
            }
        }
    }

    private string GetIdValue(object entity)
    {
        var idProperty = entity.GetType().GetProperty("Id");
        return idProperty?.GetValue(entity)?.ToString() ?? Guid.NewGuid().ToString();
    }
}