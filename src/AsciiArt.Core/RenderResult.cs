namespace AsciiArt.Core;

/// <summary>
/// Represents the output of an ASCII rendering operation.
/// </summary>
public sealed class RenderResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RenderResult"/> class.
    /// </summary>
    /// <param name="lines">Rendered lines.</param>
    /// <param name="warnings">Warning or error messages.</param>
    /// <param name="success">Whether the render succeeded.</param>
    public RenderResult(IReadOnlyList<string> lines, IReadOnlyList<string> warnings, bool success)
    {
        Lines = lines ?? throw new ArgumentNullException(nameof(lines));
        Warnings = warnings ?? throw new ArgumentNullException(nameof(warnings));
        Success = success;
    }

    /// <summary>
    /// Gets the rendered lines.
    /// </summary>
    public IReadOnlyList<string> Lines { get; }

    /// <summary>
    /// Gets the warning messages.
    /// </summary>
    public IReadOnlyList<string> Warnings { get; }

    /// <summary>
    /// Gets a value indicating whether the render completed successfully.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="lines">Rendered lines.</param>
    /// <param name="warnings">Optional warnings.</param>
    /// <returns>A successful result.</returns>
    public static RenderResult SuccessResult(IReadOnlyList<string> lines, IReadOnlyList<string>? warnings = null)
    {
        return new RenderResult(lines, warnings ?? Array.Empty<string>(), true);
    }

    /// <summary>
    /// Creates a failed result with a single message.
    /// </summary>
    /// <param name="message">Failure message.</param>
    /// <returns>A failed result.</returns>
    public static RenderResult Failure(string message)
    {
        return new RenderResult(Array.Empty<string>(), new[] { message }, false);
    }
}
