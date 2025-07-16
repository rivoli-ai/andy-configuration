# Configuration Guide

This guide covers all configuration options available in Andy.Configuration and provides examples for common scenarios.

## Table of Contents

- [Configuration Sources](#configuration-sources)
- [Configuration Structure](#configuration-structure)
- [Complete Configuration Example](#complete-configuration-example)
- [Option Details](#option-details)
- [Environment Variables](#environment-variables)
- [Validation Rules](#validation-rules)
- [Common Scenarios](#common-scenarios)

## Configuration Sources

Andy.Configuration supports all standard .NET configuration sources:

1. **JSON files** (appsettings.json, appsettings.{Environment}.json)
2. **Environment variables**
3. **Command line arguments**
4. **Azure Key Vault**
5. **User secrets** (for development)
6. **In-memory collections**

### Configuration Priority

Configuration sources are loaded in order, with later sources overriding earlier ones:

1. appsettings.json
2. appsettings.{Environment}.json
3. User secrets (Development only)
4. Environment variables
5. Command line arguments

## Configuration Structure

The configuration follows a hierarchical structure under the "Andy" root section:

```
Andy
├── Model
│   ├── Name
│   ├── Temperature
│   ├── MaxTokens
│   ├── TopP
│   └── TopK
├── Authentication
│   ├── Method
│   ├── CacheTokens
│   ├── ApiKey
│   ├── DefaultProvider
│   ├── OAuth
│   │   ├── ClientId
│   │   ├── ClientSecret
│   │   └── RedirectUri
│   └── VertexAI
│       ├── ProjectId
│       ├── Region
│       └── ServiceAccountKeyPath
├── UI
│   ├── Theme
│   ├── EnableSyntaxHighlighting
│   ├── EnableAutoScroll
│   └── MaxDisplayMessages
├── Tools
│   ├── RequireConfirmation
│   ├── DefaultTimeoutMs
│   └── MaxFileSizeBytes
└── Logging
    ├── MinimumLevel
    ├── WriteToFile
    ├── FilePath
    ├── WriteToConsole
    └── ShowUserFriendlyApiMessages
```

## Complete Configuration Example

### appsettings.json

```json
{
  "Andy": {
    "Model": {
      "Name": "gpt-4",
      "Temperature": 0.7,
      "MaxTokens": 2048,
      "TopP": 0.95,
      "TopK": 40
    },
    "Authentication": {
      "Method": "ApiKey",
      "CacheTokens": true,
      "ApiKey": "your-api-key-here",
      "DefaultProvider": "OpenAI",
      "OAuth": {
        "ClientId": "your-client-id",
        "ClientSecret": "your-client-secret",
        "RedirectUri": "http://localhost:8080/callback"
      },
      "VertexAI": {
        "ProjectId": "your-gcp-project",
        "Region": "us-central1",
        "ServiceAccountKeyPath": "/path/to/service-account.json"
      }
    },
    "UI": {
      "Theme": "dark",
      "EnableSyntaxHighlighting": true,
      "EnableAutoScroll": true,
      "MaxDisplayMessages": 100
    },
    "Tools": {
      "RequireConfirmation": true,
      "DefaultTimeoutMs": 30000,
      "MaxFileSizeBytes": 10485760
    },
    "Logging": {
      "MinimumLevel": "Information",
      "WriteToFile": true,
      "FilePath": "logs/andy.log",
      "WriteToConsole": true,
      "ShowUserFriendlyApiMessages": true
    }
  }
}
```

## Option Details

### Model Options

| Option | Type | Default | Range | Description |
|--------|------|---------|-------|-------------|
| Name | string | "gemini-2.0-flash-exp" | Required | AI model name |
| Temperature | double | 0.7 | 0.0 - 2.0 | Controls randomness in responses |
| MaxTokens | int? | null | 1 - 32768 | Maximum response length |
| TopP | double? | null | 0.0 - 1.0 | Nucleus sampling parameter |
| TopK | int? | null | 1 - 100 | Top-K sampling parameter |

### Authentication Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| Method | enum | OAuth | Authentication method (OAuth, ApiKey, VertexAI) |
| CacheTokens | bool | true | Whether to cache authentication tokens |
| ApiKey | string? | null | API key for direct authentication |
| DefaultProvider | string | "OpenAI" | Default AI provider |

### OAuth Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| ClientId | string? | null | OAuth client ID |
| ClientSecret | string? | null | OAuth client secret |
| RedirectUri | string | "http://localhost:8080/callback" | OAuth redirect URI |

### VertexAI Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| ProjectId | string? | null | Google Cloud project ID |
| Region | string | "us-central1" | Google Cloud region |
| ServiceAccountKeyPath | string? | null | Path to service account key file |

### UI Options

| Option | Type | Default | Range | Description |
|--------|------|---------|-------|-------------|
| Theme | string | "default" | Required | UI theme name |
| EnableSyntaxHighlighting | bool | true | - | Enable code syntax highlighting |
| EnableAutoScroll | bool | true | - | Auto-scroll to new content |
| MaxDisplayMessages | int | 100 | 10 - 1000 | Maximum messages to display |

### Tool Options

| Option | Type | Default | Range | Description |
|--------|------|---------|-------|-------------|
| RequireConfirmation | bool | true | - | Require confirmation for dangerous operations |
| DefaultTimeoutMs | int | 30000 | 1000 - 300000 | Default tool execution timeout |
| MaxFileSizeBytes | long | 10485760 (10MB) | 1024 - 104857600 | Maximum file size for operations |

### Logging Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| MinimumLevel | string | "Information" | Minimum log level (Trace, Debug, Information, Warning, Error, Critical) |
| WriteToFile | bool | true | Write logs to file |
| FilePath | string | "logs/andy-.log" | Log file path pattern |
| WriteToConsole | bool | true | Write logs to console |
| ShowUserFriendlyApiMessages | bool | true | Show simplified API messages |

## Environment Variables

All configuration options can be set via environment variables using the pattern:

```
Andy__{Section}__{Property}
```

Double underscore (`__`) is used as the hierarchy separator.

### Examples

```bash
# Model configuration
export Andy__Model__Name="gpt-4"
export Andy__Model__Temperature="0.8"
export Andy__Model__MaxTokens="2048"

# Authentication configuration
export Andy__Authentication__Method="ApiKey"
export Andy__Authentication__ApiKey="sk-..."
export Andy__Authentication__CacheTokens="true"

# OAuth configuration
export Andy__Authentication__OAuth__ClientId="your-client-id"
export Andy__Authentication__OAuth__ClientSecret="your-client-secret"

# UI configuration
export Andy__UI__Theme="dark"
export Andy__UI__EnableSyntaxHighlighting="true"

# Tool configuration
export Andy__Tools__RequireConfirmation="false"
export Andy__Tools__DefaultTimeoutMs="60000"

# Logging configuration
export Andy__Logging__MinimumLevel="Debug"
export Andy__Logging__WriteToFile="true"
```

## Validation Rules

### Required Fields

The following fields must be provided:
- Model.Name
- Authentication.Method
- UI.Theme
- Logging.MinimumLevel

### Range Validation

| Field | Min | Max |
|-------|-----|-----|
| Model.Temperature | 0.0 | 2.0 |
| Model.MaxTokens | 1 | 32768 |
| Model.TopP | 0.0 | 1.0 |
| Model.TopK | 1 | 100 |
| UI.MaxDisplayMessages | 10 | 1000 |
| Tools.DefaultTimeoutMs | 1000 | 300000 |
| Tools.MaxFileSizeBytes | 1024 | 104857600 |

### Enum Values

**AuthenticationMethod:**
- OAuth
- ApiKey
- VertexAI

## Common Scenarios

### Development Configuration

```json
{
  "Andy": {
    "Model": {
      "Name": "gpt-3.5-turbo",
      "Temperature": 0.7,
      "MaxTokens": 1000
    },
    "Authentication": {
      "Method": "ApiKey",
      "ApiKey": "$(UserSecretsId:ApiKey)"
    },
    "Logging": {
      "MinimumLevel": "Debug",
      "WriteToConsole": true,
      "WriteToFile": false
    }
  }
}
```

### Production Configuration

```json
{
  "Andy": {
    "Model": {
      "Name": "gpt-4",
      "Temperature": 0.3,
      "MaxTokens": 4096
    },
    "Authentication": {
      "Method": "OAuth",
      "CacheTokens": true
    },
    "UI": {
      "Theme": "light",
      "EnableSyntaxHighlighting": true
    },
    "Tools": {
      "RequireConfirmation": true,
      "DefaultTimeoutMs": 30000
    },
    "Logging": {
      "MinimumLevel": "Warning",
      "WriteToFile": true,
      "FilePath": "/var/log/andy/andy.log",
      "WriteToConsole": false
    }
  }
}
```

### Minimal Configuration

```json
{
  "Andy": {
    "Model": {
      "Name": "gemini-2.0-flash-exp"
    },
    "Authentication": {
      "Method": "ApiKey",
      "ApiKey": "your-api-key"
    },
    "UI": {
      "Theme": "default"
    },
    "Logging": {
      "MinimumLevel": "Information"
    }
  }
}
```

### Using Multiple Authentication Methods

```json
{
  "Andy": {
    "Authentication": {
      "Method": "$(ANDY_AUTH_METHOD)",
      "ApiKey": "$(ANDY_API_KEY)",
      "OAuth": {
        "ClientId": "$(ANDY_OAUTH_CLIENT_ID)",
        "ClientSecret": "$(ANDY_OAUTH_CLIENT_SECRET)"
      },
      "VertexAI": {
        "ProjectId": "$(ANDY_GCP_PROJECT)",
        "ServiceAccountKeyPath": "$(ANDY_GCP_KEY_PATH)"
      }
    }
  }
}
```

### High-Performance Configuration

```json
{
  "Andy": {
    "Model": {
      "Name": "gpt-4-turbo",
      "Temperature": 0.2,
      "MaxTokens": 8192,
      "TopP": 0.9
    },
    "Tools": {
      "RequireConfirmation": false,
      "DefaultTimeoutMs": 120000,
      "MaxFileSizeBytes": 52428800
    },
    "UI": {
      "EnableSyntaxHighlighting": false,
      "EnableAutoScroll": false,
      "MaxDisplayMessages": 50
    }
  }
}
```

## Troubleshooting

### Common Validation Errors

1. **Missing required field:**
   ```
   Configuration validation failed: The Name field is required.
   ```
   Solution: Ensure all required fields have values.

2. **Value out of range:**
   ```
   Configuration validation failed: The field Temperature must be between 0 and 2.
   ```
   Solution: Check that numeric values are within allowed ranges.

3. **Invalid enum value:**
   ```
   Failed to convert configuration value at 'Andy:Authentication:Method' to type 'AuthenticationMethod'
   ```
   Solution: Use one of the valid enum values (OAuth, ApiKey, VertexAI).

### Configuration Loading Issues

1. **File not found:**
   Ensure appsettings.json is in the application root directory.

2. **Invalid JSON:**
   Validate JSON syntax using a JSON validator.

3. **Environment variable conflicts:**
   Check for environment variables that might override your JSON configuration.

### Best Practices

1. **Use environment-specific files:**
   - appsettings.Development.json
   - appsettings.Production.json

2. **Secure sensitive data:**
   - Use User Secrets in development
   - Use Azure Key Vault or similar in production
   - Never commit API keys to source control

3. **Validate early:**
   - Enable startup validation to catch errors immediately
   - Test configuration changes in a development environment first

4. **Document custom settings:**
   - Add comments in appsettings.json (using JSON5 if supported)
   - Maintain a configuration reference for your team