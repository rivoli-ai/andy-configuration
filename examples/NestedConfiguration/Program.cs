using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Andy.Configuration;
using System.ComponentModel.DataAnnotations;

var builder = Host.CreateApplicationBuilder(args);

// Configure services
builder.Services.Configure<ApplicationOptions>(
    builder.Configuration.GetSection(ApplicationOptions.SectionName));

// Add validation
builder.Services.AddSingleton<IValidateOptions<ApplicationOptions>, ConfigurationValidator<ApplicationOptions>>();

// Enable validation on startup
builder.Services.ValidateConfigurationOnStartup();

var host = builder.Build();

// Access nested configuration
var options = host.Services.GetRequiredService<IOptions<ApplicationOptions>>().Value;

Console.WriteLine("=== Nested Configuration Example ===");
Console.WriteLine($"\nApplication: {options.Name}");

// Database configuration
Console.WriteLine("\n📊 Database Configuration:");
Console.WriteLine($"  Primary Database:");
Console.WriteLine($"    Host: {options.Services.Database.Primary.Host}:{options.Services.Database.Primary.Port}");
Console.WriteLine($"    Database: {options.Services.Database.Primary.Database}");
Console.WriteLine($"    Username: {options.Services.Database.Primary.Credentials.Username}");
Console.WriteLine($"  Connection Pool:");
Console.WriteLine($"    Min Size: {options.Services.Database.ConnectionPool.MinSize}");
Console.WriteLine($"    Max Size: {options.Services.Database.ConnectionPool.MaxSize}");
Console.WriteLine($"    Timeout: {options.Services.Database.ConnectionPool.TimeoutMs}ms");

if (options.Services.Database.Replica != null)
{
    Console.WriteLine($"  Replica Database:");
    Console.WriteLine($"    Host: {options.Services.Database.Replica.Host}:{options.Services.Database.Replica.Port}");
    Console.WriteLine($"    Read Only: {options.Services.Database.Replica.ReadOnly}");
    Console.WriteLine($"    Load Balancing Weight: {options.Services.Database.Replica.LoadBalancingWeight}%");
}

// Cache configuration
Console.WriteLine("\n💾 Cache Configuration:");
Console.WriteLine($"  Provider: {options.Services.Cache.Provider}");
Console.WriteLine($"  Redis Connection: {options.Services.Cache.Redis.ConnectionString}");
Console.WriteLine($"  Redis Database: {options.Services.Cache.Redis.Database}");

if (options.Services.Cache.Redis.Cluster != null)
{
    Console.WriteLine($"  Cluster Nodes: {string.Join(", ", options.Services.Cache.Redis.Cluster.Nodes)}");
    Console.WriteLine($"  Enable Redirection: {options.Services.Cache.Redis.Cluster.EnableRedirection}");
}

// Messaging configuration
Console.WriteLine("\n📨 Messaging Configuration:");
Console.WriteLine($"  Provider: {options.Services.Messaging.Provider}");
Console.WriteLine($"  RabbitMQ Host: {options.Services.Messaging.RabbitMq.Host}:{options.Services.Messaging.RabbitMq.Port}");
Console.WriteLine($"  Virtual Host: {options.Services.Messaging.RabbitMq.VirtualHost}");

if (options.Services.Messaging.RabbitMq.Exchange != null)
{
    Console.WriteLine($"  Exchange:");
    Console.WriteLine($"    Name: {options.Services.Messaging.RabbitMq.Exchange.Name}");
    Console.WriteLine($"    Type: {options.Services.Messaging.RabbitMq.Exchange.Type}");
    Console.WriteLine($"    Durable: {options.Services.Messaging.RabbitMq.Exchange.Durable}");
}

// Environment configuration
Console.WriteLine("\n🌍 Environment Configuration:");
foreach (var env in new[] { 
    ("Development", options.Environments.Development),
    ("Staging", options.Environments.Staging),
    ("Production", options.Environments.Production)
})
{
    Console.WriteLine($"  {env.Item1}:");
    Console.WriteLine($"    API URL: {env.Item2.ApiUrl}");
    Console.WriteLine($"    Debug Enabled: {env.Item2.EnableDebug}");
    Console.WriteLine($"    Log Level: {env.Item2.LogLevel}");
    
    if (env.Item2.CustomSettings.Any())
    {
        Console.WriteLine($"    Custom Settings:");
        foreach (var setting in env.Item2.CustomSettings)
        {
            Console.WriteLine($"      {setting.Key}: {setting.Value}");
        }
    }
}

Console.WriteLine("\n✅ All nested configuration validated successfully!");

// Example of deeply nested configuration structure
public class ApplicationOptions
{
    public const string SectionName = "Application";

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public ServicesOptions Services { get; set; } = new();

    [Required]
    public EnvironmentOptions Environments { get; set; } = new();
}

public class ServicesOptions
{
    [Required]
    public DatabaseServiceOptions Database { get; set; } = new();

    [Required]
    public CacheServiceOptions Cache { get; set; } = new();

    [Required]
    public MessagingOptions Messaging { get; set; } = new();
}

public class DatabaseServiceOptions
{
    [Required]
    public PrimaryDatabaseOptions Primary { get; set; } = new();

    public ReplicaDatabaseOptions? Replica { get; set; }

    [Required]
    public ConnectionPoolOptions ConnectionPool { get; set; } = new();
}

public class PrimaryDatabaseOptions
{
    [Required]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; } = 5432;

    [Required]
    public string Database { get; set; } = string.Empty;

    [Required]
    public CredentialsOptions Credentials { get; set; } = new();
}

public class ReplicaDatabaseOptions : PrimaryDatabaseOptions
{
    public bool ReadOnly { get; set; } = true;

    [Range(0, 100)]
    public int LoadBalancingWeight { get; set; } = 50;
}

public class CredentialsOptions
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class ConnectionPoolOptions
{
    [Range(1, 1000)]
    public int MinSize { get; set; } = 5;

    [Range(1, 1000)]
    public int MaxSize { get; set; } = 100;

    [Range(1000, 300000)]
    public int TimeoutMs { get; set; } = 30000;
}

public class CacheServiceOptions
{
    [Required]
    public string Provider { get; set; } = "Redis";

    [Required]
    public RedisOptions Redis { get; set; } = new();
}

public class RedisOptions
{
    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Range(0, 15)]
    public int Database { get; set; } = 0;

    public ClusterOptions? Cluster { get; set; }
}

public class ClusterOptions
{
    [Required]
    public List<string> Nodes { get; set; } = new();

    public bool EnableRedirection { get; set; } = true;
}

public class MessagingOptions
{
    [Required]
    public string Provider { get; set; } = "RabbitMQ";

    [Required]
    public RabbitMqOptions RabbitMq { get; set; } = new();
}

public class RabbitMqOptions
{
    [Required]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; } = 5672;

    [Required]
    public string VirtualHost { get; set; } = "/";

    [Required]
    public CredentialsOptions Credentials { get; set; } = new();

    public ExchangeOptions? Exchange { get; set; }
}

public class ExchangeOptions
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = "topic";

    public bool Durable { get; set; } = true;
}

public class EnvironmentOptions
{
    [Required]
    public EnvironmentConfig Development { get; set; } = new();

    [Required]
    public EnvironmentConfig Staging { get; set; } = new();

    [Required]
    public EnvironmentConfig Production { get; set; } = new();
}

public class EnvironmentConfig
{
    [Required]
    [Url]
    public string ApiUrl { get; set; } = string.Empty;

    public bool EnableDebug { get; set; }

    [Range(1, 10)]
    public int LogLevel { get; set; } = 3;

    public Dictionary<string, string> CustomSettings { get; set; } = new();
}

