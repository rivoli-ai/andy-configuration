using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Andy.Configuration;

namespace WebApiExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IOptions<AndyOptions> _andyOptions;
    private readonly IOptionsSnapshot<AndyOptions> _optionsSnapshot;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(
        IOptions<AndyOptions> andyOptions,
        IOptionsSnapshot<AndyOptions> optionsSnapshot,
        IConfiguration configuration,
        ILogger<ConfigurationController> logger)
    {
        _andyOptions = andyOptions;
        _optionsSnapshot = optionsSnapshot;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Get the complete Andy configuration
    /// </summary>
    [HttpGet]
    public ActionResult<AndyOptions> GetConfiguration()
    {
        _logger.LogInformation("Retrieving Andy configuration");
        return Ok(_andyOptions.Value);
    }

    /// <summary>
    /// Get current configuration (supports hot reload)
    /// </summary>
    [HttpGet("current")]
    public ActionResult<AndyOptions> GetCurrentConfiguration()
    {
        _logger.LogInformation("Retrieving current Andy configuration with snapshot");
        return Ok(_optionsSnapshot.Value);
    }

    /// <summary>
    /// Get a specific configuration section
    /// </summary>
    [HttpGet("section/{sectionName}")]
    public ActionResult<object> GetConfigurationSection(string sectionName)
    {
        var section = _configuration.GetSection($"Andy:{sectionName}");
        
        if (!section.Exists())
        {
            return NotFound(new { Message = $"Configuration section 'Andy:{sectionName}' not found" });
        }

        // Return the section as a dictionary
        var result = section.GetChildren()
            .ToDictionary(x => x.Key, x => x.Value);

        return Ok(result);
    }

    /// <summary>
    /// Test configuration validation
    /// </summary>
    [HttpPost("test-validation")]
    public ActionResult TestValidation([FromBody] TestModelOptions testModel)
    {
        var validator = new ConfigurationValidator<TestModelOptions>();

        var validationResult = validator.Validate(null, testModel);

        if (validationResult.Succeeded)
        {
            return Ok(new { Message = "Validation passed", Model = testModel });
        }

        return BadRequest(new 
        { 
            Message = "Validation failed", 
            Errors = validationResult.FailureMessage 
        });
    }

    /// <summary>
    /// Get configuration metadata
    /// </summary>
    [HttpGet("metadata")]
    public ActionResult GetMetadata()
    {
        var config = _andyOptions.Value;
        
        return Ok(new
        {
            ConfigurationSections = new[]
            {
                "Model",
                "Authentication",
                "UI",
                "Tools",
                "Logging"
            },
            Themes = new[] { "light", "dark", "auto" },
            AuthenticationMethods = Enum.GetNames<AuthenticationMethod>(),
            CurrentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        });
    }
}

public class TestModelOptions
{
    [System.ComponentModel.DataAnnotations.Required]
    public string Name { get; set; } = string.Empty;
    
    [System.ComponentModel.DataAnnotations.Range(0, 2)]
    public double Temperature { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(1, 10000)]
    public int MaxTokens { get; set; }
}