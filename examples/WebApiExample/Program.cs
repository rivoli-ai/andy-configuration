using Andy.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Andy Configuration API", 
        Version = "v1",
        Description = "Example API demonstrating Andy.Configuration library"
    });
});

// Add Andy configuration with validation
builder.Services.AddAndyConfiguration(builder.Configuration);

// Validate configuration on startup
builder.Services.ValidateConfigurationOnStartup();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Add a simple endpoint to demonstrate configuration usage
app.MapGet("/", () => "Andy Configuration Web API Example - Navigate to /swagger for API documentation");

// Configuration status endpoint
app.MapGet("/api/configuration/status", (IOptions<AndyOptions> options) =>
{
    var config = options.Value;
    return Results.Ok(new
    {
        Status = "Configuration loaded successfully",
        Model = new
        {
            config.Model.Name,
            config.Model.Temperature,
            config.Model.MaxTokens
        },
        UI = new
        {
            config.UI.Theme,
            config.UI.EnableSyntaxHighlighting
        },
        Timestamp = DateTime.UtcNow
    });
})
.WithName("GetConfigurationStatus")
;

// Model configuration endpoint
app.MapGet("/api/configuration/model", (IOptions<ModelOptions> options) =>
{
    return Results.Ok(options.Value);
})
.WithName("GetModelConfiguration")
;

// UI configuration endpoint
app.MapGet("/api/configuration/ui", (IOptions<UIOptions> options) =>
{
    return Results.Ok(options.Value);
})
.WithName("GetUIConfiguration")
;

// Configuration validation test endpoint
app.MapPost("/api/configuration/validate", (ValidateRequest request) =>
{
    var validationErrors = new List<string>();
    
    // Simulate validation
    if (request.Temperature < 0 || request.Temperature > 2)
    {
        validationErrors.Add("Temperature must be between 0 and 2");
    }
    
    if (string.IsNullOrWhiteSpace(request.ApiKey))
    {
        validationErrors.Add("API key is required");
    }
    
    if (request.MaxTokens < 1 || request.MaxTokens > 10000)
    {
        validationErrors.Add("Max tokens must be between 1 and 10000");
    }
    
    if (validationErrors.Any())
    {
        return Results.BadRequest(new { Errors = validationErrors });
    }
    
    return Results.Ok(new { Message = "Configuration is valid" });
})
.WithName("ValidateConfiguration")
;

app.Run();

// Request model for validation endpoint
public record ValidateRequest(
    double Temperature,
    string ApiKey,
    int MaxTokens
);