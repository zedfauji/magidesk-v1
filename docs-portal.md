# Magidesk POS Documentation Portal

## Overview

The Magidesk POS documentation is published as a static site using MkDocs with the Material theme. This portal provides comprehensive documentation for developers, operators, and contributors to the Magidesk Point of Sale system.

## Chosen Stack

**MkDocs with Material Theme** was selected because:
- Works perfectly with pure Markdown files
- Generates static sites compatible with GitHub Pages
- Excellent search capabilities and navigation
- Built-in code highlighting and diagram support
- Minimal configuration required
- Professional appearance with responsive design

## Repository Structure

```
Magidesk/
├── docs/                          # Source documentation
│   ├── README.md                  # Main portal entry
│   ├── system/                    # System documentation
│   ├── architecture/              # Architecture docs
│   ├── ui/                        # UI documentation
│   ├── backend/                   # Backend documentation
│   ├── data/                      # Data model docs
│   ├── workflows/                 # Business workflows
│   ├── configuration/             # Configuration guides
│   ├── operations/                # Operations docs
│   ├── runbooks/                  # Troubleshooting guides
│   └── contributing/              # Development guides
├── mkdocs.yml                     # MkDocs configuration
├── requirements.txt               # Python dependencies
├── run-docs-locally.ps1          # Windows local runner
├── run-docs-locally.sh           # Linux/Mac local runner
└── .github/workflows/docs.yml    # GitHub Pages deployment
```

## How to Run Portal Locally

### Prerequisites

- Python 3.8 or higher
- pip (Python package manager)

### Quick Start

#### Windows
```powershell
# Run the provided PowerShell script
.\run-docs-locally.ps1
```

#### Linux/Mac
```bash
# Make script executable and run
chmod +x run-docs-locally.sh
./run-docs-locally.sh
```

#### Manual Setup
```bash
# Install dependencies
pip install -r requirements.txt

# Start development server
mkdocs serve

# Or specify address and port
mkdocs serve --dev-addr=0.0.0.0:8000
```

### Access

Once running, the documentation portal is available at:
- **Local**: http://127.0.0.1:8000

The development server includes:
- Live reload (pages refresh automatically)
- Search functionality
- Full navigation
- Code highlighting
- Diagram rendering

### Self-Hosted Runner Configuration
The documentation portal is configured to use a self-hosted runner named "WIDOWSVAIL" for building and deployment.

### For Repository Owner
Repository URLs have been updated to use zedfauji/magidesk-v1

## How to Update Documentation Safely

### Adding New Documentation

1. **Create Markdown Files**
   ```bash
   # Add new file to appropriate section
   touch docs/section/new-document.md
   ```

2. **Update Navigation**
   ```yaml
   # Edit mkdocs.yml to add to nav section
   nav:
     - Section:
       - New Document: section/new-document.md
   ```

3. **Test Locally**
   ```bash
   mkdocs serve
   # Verify navigation and content
   ```

4. **Commit Changes**
   ```bash
   git add docs/new-document.md mkdocs.yml
   git commit -m "docs: Add new documentation section"
   git push
   ```

### Modifying Existing Documentation

1. **Edit Markdown Files**
   - Use any Markdown editor
   - Maintain existing formatting
   - Test code examples

2. **Verify Links**
   ```bash
   # Build site to check for broken links
   mkdocs build
   ```

3. **Test Changes**
   ```bash
   mkdocs serve
   # Review changes locally
   ```

4. **Commit Changes**
   ```bash
   git add docs/modified-file.md
   git commit -m "docs: Update section with new information"
   git push
   ```

### Adding New Sections

1. **Create Directory Structure**
   ```bash
   mkdir docs/new-section
   touch docs/new-section/index.md
   ```

2. **Add to Navigation**
   ```yaml
   nav:
     - New Section:
       - Overview: new-section/index.md
   ```

3. **Update Configuration**
   - Add section-specific settings if needed
   - Configure section navigation

### Best Practices

#### Content Guidelines
- **Preserve Depth**: Maintain comprehensive documentation
- **No Placeholders**: Replace placeholder content with actual documentation
- **Code Examples**: Ensure all code examples are tested and accurate
- **Cross-References**: Link between related documentation

#### Formatting Standards
- **Markdown**: Use standard Markdown syntax
- **Headings**: Use proper heading hierarchy (H1, H2, H3)
- **Code Blocks**: Specify language for syntax highlighting
- **Links**: Use relative links for internal documentation

#### Navigation Organization
- **Logical Grouping**: Group related documentation together
- **Consistent Structure**: Maintain consistent section organization
- **Searchable**: Use descriptive titles and headings
- **User-Friendly**: Organize by user journey and task

### Automated Deployment

The documentation portal is automatically deployed to GitHub Pages when:

1. **Push to Main Branch**
   ```bash
   git push origin main
   ```

2. **Pull Request Merged**
   - Documentation builds automatically
   - Deployed to GitHub Pages
   - Available within 1-2 minutes

3. **Manual Trigger** (if needed)
   - Go to Actions tab in GitHub
   - Select "Documentation" workflow
   - Click "Run workflow"

### Troubleshooting

#### Common Issues

**Build Failures**
```bash
# Check for Markdown syntax errors
mkdocs build --strict

# Verify all files exist
mkdocs build --verbose
```

**Navigation Issues**
```bash
# Validate configuration
mkdocs validate
```

**Missing Dependencies**
```bash
# Reinstall dependencies
pip install -r requirements.txt --upgrade
```

**Search Not Working**
- Ensure search plugin is enabled in mkdocs.yml
- Check that pages have proper headings
- Verify site is built and deployed

#### Getting Help

1. **Check Logs**: Review GitHub Actions logs for build errors
2. **Local Testing**: Always test changes locally before pushing
3. **Documentation**: Refer to MkDocs and Material theme documentation
4. **Community**: Use GitHub Issues for portal-specific problems

## Portal Features

### Navigation
- **Sidebar Navigation**: Hierarchical menu structure
- **Tab Navigation**: Top-level section tabs
- **Search**: Full-text search with highlighting
- **Breadcrumbs**: Navigation path indicator
- **Back to Top**: Quick navigation to page top

### Content Features
- **Code Highlighting**: Syntax highlighting for multiple languages
- **Diagrams**: Mermaid diagram support
- **Tables**: Responsive table formatting
- **Tooltips**: Hover tooltips for additional information
- **Callouts**: Admonition blocks for notes and warnings

### User Experience
- **Responsive Design**: Works on desktop, tablet, and mobile
- **Dark Mode**: Toggle between light and dark themes
- **Print Support**: Optimized printing styles
- **Accessibility**: WCAG compliant navigation and content
- **Performance**: Fast loading with optimized assets

## Maintenance

### Regular Tasks
- **Link Checking**: Verify internal and external links monthly
- **Content Review**: Update outdated documentation quarterly
- **Dependency Updates**: Update MkDocs and plugins regularly
- **Performance Monitoring**: Monitor site speed and user experience

### Security
- **Dependencies**: Keep Python dependencies updated
- **Content**: Review documentation for sensitive information
- **Access**: Control GitHub repository access appropriately

---

This documentation portal serves as the definitive source of truth for the Magidesk POS system. All contributions should follow the guidelines above to maintain consistency and quality.