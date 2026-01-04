# Manual GitHub Pages Deployment

## Trigger Workflow Manually

If automatic deployment doesn't work, you can trigger the workflow manually:

### Option 1: GitHub Web Interface

1. Go to: https://github.com/zedfauji/magidesk-v1/actions
2. Click on "Documentation" workflow
3. Click "Run workflow" button
4. Select branch: `master`
5. Click "Run workflow"

### Option 2: GitHub CLI

```bash
# Install GitHub CLI (if not already installed)
# Windows: winget install GitHub.cli
# Mac: brew install gh
# Linux: sudo apt install gh

# Trigger workflow
gh workflow run "Documentation" --field branch=master
```

### Option 3: Local Deployment

If GitHub Actions completely fails, deploy manually:

```bash
# Build documentation
mkdocs build

# Install gh-pages (if not already installed)
npm install -g gh-pages

# Deploy to gh-pages branch
gh-pages --dist site --branch gh-pages --repo https://github.com/zedfauji/magidesk-v1.git

# Then enable GitHub Pages from gh-pages branch in settings
```

## Verify Deployment

After triggering:
1. Go to Actions tab to see workflow progress
2. Wait for completion (usually 2-3 minutes)
3. Check deployment at: https://zedfauji.github.io/magidesk-v1/

## Common Issues

### Workflow Not Found
- Ensure `.github/workflows/docs.yml` exists
- Check file permissions
- Verify YAML syntax

### Permission Denied
- Check repository Actions permissions
- Verify GITHUB_TOKEN is available
- Ensure workflow has write permissions

### Build Failures
- Check Python version (needs 3.8+)
- Verify requirements.txt is correct
- Check for syntax errors in mkdocs.yml
- Test locally with `mkdocs build`

### Deployment Failures
- Check GitHub Pages settings
- Verify destination directory is correct
- Ensure gh-pages branch exists (for manual deployment)

## Success Indicators

✅ Workflow completes without errors
✅ Site appears at https://zedfauji.github.io/magidesk-v1/
✅ All pages load correctly
✅ Navigation works properly
✅ Search functions correctly