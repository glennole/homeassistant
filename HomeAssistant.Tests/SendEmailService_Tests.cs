using System.Collections.Generic;
using HomeAssistant.Service.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HomeAssistant.Tests;

public class SendEmailService_Tests
{
    private IConfiguration CreateConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Smtp2Go:ApiKey"] = "api-22E0C0AFDC4D4407BC4566F23555F166"
        }!);
        return configurationBuilder.Build();
    }

    
    [Fact]
    public async void SendTestEmail_ShouldBeSentToDefaultEmailAddress()
    {
        var configuration = CreateConfiguration();
        var emailService = new EmailService(configuration);
        
        var result = await emailService.SendEmail(
            "Test Subject", 
            "Test plain text content", 
            "<p>Test HTML content</p>");

        // Assert
        Assert.True(result);

    }
}