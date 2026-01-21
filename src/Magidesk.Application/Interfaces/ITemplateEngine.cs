using System.Threading.Tasks;

namespace Magidesk.Application.Interfaces;

public interface ITemplateEngine
{
    /// <summary>
    /// Renders a template string using the provided model.
    /// </summary>
    /// <param name="templateContent">The Liquid template source.</param>
    /// <param name="model">The context/model to bind parameters from.</param>
    /// <returns>Rendered string output.</returns>
    Task<string> RenderAsync(string templateContent, object model);

    /// <summary>
    /// Validates the syntax of a template.
    /// </summary>
    /// <param name="templateContent">Template source to check.</param>
    /// <param name="error">Error message if invalid.</param>
    /// <returns>True if valid, False if syntax errors exist.</returns>
    bool Validate(string templateContent, out string error);
}
