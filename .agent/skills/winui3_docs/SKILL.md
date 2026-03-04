---
name: Consult WinUI Gallery
description: Forces the agent to use the WinUI-Gallery-Docs MCP server as the primary knowledge source for WinUI 3 implementation, identifying the correct tools to verify code.
---

# WinUI 3 Implementation Guidelines

## 🚨 MANDATORY REQUIREMENT 🚨

When you need to implement, debug, or research any **WinUI 3** component, control, style, or framework feature, you **MUST** consult the official **`WinUI-Gallery-Docs`** MCP server **BEFORE** relying on your internal knowledge.

## Verified Tool Workflow

The `WinUI-Gallery-Docs` server provides access to the `microsoft/WinUI-Gallery` repository, which contains the authoritative code samples for WinUI 3.

### 1. Find Authoritative Code
Use `mcp_WinUI-Gallery-Docs_search_WinUI_Gallery_code` to find the actual implementation files.

**Do NOT** rely on `search_WinUI_Gallery_docs` for specific control implementation details, as it often returns empty results or generic READMEs.

*   **Tool:** `mcp_WinUI-Gallery-Docs_search_WinUI_Gallery_code`
*   **Args:** `query="<ControlName>"` (e.g., "Button", "DataGrid", "NavigationView")
*   **Goal:** Obtain the `Path` of the relevant `.xaml` or `.cs` file from the search results.
    *   *Look for paths like:* `WinUIGallery/Controls/MyControl.xaml` or `WinUIGallery/Samples/ControlPages/MyControlPage.xaml`.

### 2. Fetch Raw Content (CRITICAL STEP)
The search results provide a GitHub blob URL (HTML). **Do NOT** fetch this URL directly as it contains UI noise. You must construct a **Raw URL** to get cleaner headers and content.

*   **Tool:** `mcp_WinUI-Gallery-Docs_fetch_generic_url_content`
*   **Args:** `url="https://raw.githubusercontent.com/microsoft/WinUI-Gallery/main/<PATH_FROM_STEP_1>"`
*   **Example:**
    *   Search Result Path: `WinUIGallery/Controls/CopyButton.xaml`
    *   Raw URL to Fetch: `https://raw.githubusercontent.com/microsoft/WinUI-Gallery/main/WinUIGallery/Controls/CopyButton.xaml`

### 3. Analyze & Implement
Use the fetched XAML/C# code as the **primary reference** for your implementation.
*   Copy styles, templates, and binding patterns exactly as shown in the gallery.
*   Note the use of `ThemeResource`, `StaticResource`, and specific Visual States.

## When to Use This Skill

*   **Implementing Controls:** `mcp_WinUI-Gallery-Docs_search_WinUI_Gallery_code(query="DataGrid")`
*   **Styling:** `mcp_WinUI-Gallery-Docs_search_WinUI_Gallery_code(query="Button Style")`
*   **Debugging:** When layout or binding behaves unexpectedly, compare your code against the Gallery's implementation.

## What NOT to Do
*   ❌ Do not use `fetch_WinUI_Gallery_documentation` for specific controls (it returns the repo README).
*   ❌ Do not use `fetch_generic_url_content` on the `html` URL from search results (use the `raw` URL pattern described above).
*   ❌ Do not guess XAML syntax for WinUI 3 based on WPF knowledge.
