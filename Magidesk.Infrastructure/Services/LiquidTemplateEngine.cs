using System;
using System.Threading.Tasks;
using Fluid;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Magidesk.Infrastructure.Services;

public class LiquidTemplateEngine : ITemplateEngine
{
    private readonly FluidParser _parser;
    private readonly ILogger<LiquidTemplateEngine> _logger;
    private readonly TemplateOptions _options;

    public LiquidTemplateEngine(ILogger<LiquidTemplateEngine> logger)
    {
        _logger = logger;
        _parser = new FluidParser();
        
        // Configure options to allow all properties
        // In a strictly secure environment we might whitelist, 
        // but for a user-customizable POS ticket, binding to the DTO's properties is expected.
        _options = new TemplateOptions();
        _options.MemberAccessStrategy = new UnsafeMemberAccessStrategy(); 
    }

    public async Task<string> RenderAsync(string templateContent, object model)
    {
        if (string.IsNullOrWhiteSpace(templateContent)) return string.Empty;

        try
        {
            if (!_parser.TryParse(templateContent, out var template, out var error))
            {
                throw new InvalidOperationException($"Template parsing failed: {error}");
            }

            var context = new TemplateContext(model, _options);
            return await template.RenderAsync(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to render liquid template.");
            // We rethrow so the caller knows rendering failed (and can fallback)
            throw; 
        }
    }

    public bool Validate(string templateContent, out string error)
    {
        error = string.Empty;
        if (string.IsNullOrWhiteSpace(templateContent)) return true;

        var success = _parser.TryParse(templateContent, out _, out var parseError);
        if (!success)
        {
            error = parseError;
        }
        return success;
    }
}
