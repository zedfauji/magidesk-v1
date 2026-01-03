# Deploy to GitHub Pages

## Quick Setup for GitHub Pages

### 1. Push to GitHub

First, commit all changes and push to your repository:

```bash
git add .
git commit -m "docs: Add MkDocs documentation portal with GitHub Pages deployment"
git push origin main
```

### 2. Enable GitHub Pages

1. Go to your repository: https://github.com/zedfauji/magidesk-v1
2. Click **Settings** tab
3. Scroll down to **Pages** section
4. Under "Build and deployment", select **GitHub Actions**
5. GitHub Pages will now be automatically deployed when you push to main

### 3. Verify Deployment

After pushing, GitHub Actions will automatically:
- Build the documentation
- Deploy to GitHub Pages
- Make it available at: https://zedfauji.github.io/magidesk-v1/

### 4. Check Status

- Go to **Actions** tab in your repository
- Look for the "Documentation" workflow
- Wait for it to complete (usually 1-2 minutes)
- Click on the workflow to see deployment details

## Manual Deployment (Alternative)

If GitHub Actions doesn't work, you can deploy manually:

```bash
# Build the documentation
mkdocs build

# Install gh-pages CLI
npm install -g gh-pages

# Deploy to GitHub Pages
gh-pages --dist site --branch gh-pages --repo https://github.com/zedfauji/magidesk-v1.git
```

Then enable GitHub Pages from the `gh-pages` branch in repository settings.

## Troubleshooting

### Common Issues

1. **GitHub Actions fails**
   - Check the workflow file syntax
   - Ensure repository has Actions enabled
   - Verify secrets are available

2. **404 Error on GitHub Pages**
   - Wait a few minutes for DNS propagation
   - Check that GitHub Pages is enabled in settings
   - Verify the correct branch is selected

3. **Build fails**
   - Check for syntax errors in mkdocs.yml
   - Ensure all referenced files exist
   - Check Python dependencies

### Getting Help

- Check GitHub Actions logs for detailed error messages
- Review the workflow file: `.github/workflows/docs.yml`
- Verify mkdocs.yml configuration
- Test locally with `mkdocs build` first

## Success Indicators

✅ **GitHub Actions workflow completes successfully**
✅ **Documentation available at https://zedfauji.github.io/magidesk-v1/**
✅ **All navigation links work**
✅ **Search functionality works**
✅ **Code highlighting displays correctly**

## Next Steps

Once deployed:
1. Share the documentation URL with your team
2. Test all navigation and features
3. Set up regular documentation updates
4. Consider adding Google Analytics (optional)

The documentation portal will automatically update whenever you push changes to the main branch.