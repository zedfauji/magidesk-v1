# Self-Hosted Runner Setup Guide

## Overview

This guide helps you set up your self-hosted runner "WIDOWSVAIL" for GitHub Actions documentation deployment.

## Prerequisites

### Windows Machine Requirements
- **OS**: Windows 10/11 or Windows Server 2019+
- **PowerShell**: 5.1 or later
- **Network**: Internet connection for GitHub access
- **Permissions**: Administrator rights for software installation

### Software Requirements
- **Git**: Latest version (2.40+)
- **Python**: 3.11 or later
- **GitHub CLI**: gh command line tool

## Setup Instructions

### 1. Install GitHub Actions Runner

Download and install the runner:

```powershell
# Create runner directory
mkdir C:\actions-runner

# Download runner
Invoke-WebRequest -Uri "https://github.com/actions/runner/releases/download/v2.311.0/actions-runner-win-x64-2.311.0.zip" -OutFile "actions-runner-win-x64-2.311.0.zip"

# Extract runner
Expand-Archive -Path "actions-runner-win-x64-2.311.0.zip" -DestinationPath "C:\actions-runner"

# Configure runner
cd C:\actions-runner
.\config.cmd --url https://github.com/zedfauji/magidesk-v1 --token YOUR_TOKEN --name WIDOWSVAIL
```

### 2. Install Python 3.11

#### Option A: Official Installer (Recommended)
```powershell
# Download Python 3.11
Invoke-WebRequest -Uri "https://www.python.org/ftp/python/3.11.9/python-3.11.9-amd64.exe" -OutFile "python-3.11.9-amd64.exe"

# Install Python silently
Start-Process -FilePath "python-3.11.9-amd64.exe" -ArgumentList "/quiet InstallAllUsers=1 PrependPath=1" -Wait

# Add to PATH
[Environment]::SetEnvironmentVariable("PATH", "$env:PATH;C:\Program Files\Python311", "Machine")
```

#### Option B: Microsoft Store
```powershell
# Install from Microsoft Store
winget install Python.Python.3.11
```

#### Option C: Chocolatey
```powershell
# Install via Chocolatey
choco install python311
```

### 3. Configure PowerShell Execution Policies

```powershell
# Set execution policies for GitHub Actions
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process -Force
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force

# Verify policies
Get-ExecutionPolicy -List
```

### 4. Install Additional Dependencies

```powershell
# Install pip if not included
python -m ensurepip --upgrade

# Install required packages
pip install mkdocs mkdocs-material pymdown-extensions
```

### 5. Test Runner Configuration

```powershell
# Test runner connection
.\run.cmd

# Verify Python installation
python --version

# Test MkDocs
mkdocs --version
```

### 6. Configure as Service (Optional)

```powershell
# Install as Windows service
.\config.cmd --url https://github.com/zedfauji/magidesk-v1 --token YOUR_TOKEN --name WIDOWSVAIL --runasservice

# Start service
net start actions.runner.WIDOWSVAIL
```

## Troubleshooting

### Common Issues

#### Python Installation Fails
**Problem**: "File cannot be loaded due to execution policy"
**Solution**: 
```powershell
# Temporarily bypass execution policy
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force
# Install Python
# Run installation script
# Reset policy
Set-ExecutionPolicy -ExecutionPolicy Restricted -Scope Process -Force
```

#### Runner Registration Fails
**Problem**: "Runner is not reachable"
**Solution**:
```powershell
# Check network connectivity
Test-NetConnection -ComputerName github.com

# Verify token
.\config.cmd --url https://github.com/zedfauji/magidesk-v1 --token YOUR_TOKEN --name WIDOWSVAIL --unattended
```

#### Build Failures
**Problem**: "mkdocs command not found"
**Solution**:
```powershell
# Check Python PATH
where python

# Add to PATH if needed
[Environment]::SetEnvironmentVariable("PATH", "$env:PATH;C:\Program Files\Python311\Scripts", "Machine")

# Restart runner service
net stop actions.runner.WIDOWSVAIL
net start actions.runner.WIDOWSVAIL
```

### Performance Optimization

#### Improve Build Speed
```powershell
# Use local cache
pip install --cache-dir "C:\actions-runner\.cache" mkdocs mkdocs-material

# Pre-install dependencies
pip download mkdocs mkdocs-material pymdown-extensions
pip install --no-index --find-links file:*.whl
```

#### Reduce Memory Usage
```powershell
# Limit parallel processes
$env:MKDOCS_MULTIPROCESSING = 0

# Use minimal theme
mkdocs build --theme material --clean
```

## Security Considerations

### Runner Security
- Use dedicated service account
- Restrict repository permissions
- Regularly update runner software
- Monitor runner logs

### Network Security
- Configure firewall rules for GitHub
- Use HTTPS for all communications
- Rotate access tokens regularly

## Maintenance

### Update Runner
```powershell
# Stop runner service
net stop actions.runner.WIDOWSVAIL

# Update runner
.\config.cmd --url https://github.com/zedfauji/magidesk-v1 --token YOUR_TOKEN --name WIDOWSVAIL --replace

# Restart service
net start actions.runner.WIDOWSVAIL
```

### Clean Up
```powershell
# Remove old runners
.\config.cmd --url https://github.com/zedfauji/magidesk-v1 --token YOUR_TOKEN --name WIDOWSVAIL --remove

# Clean cache
Remove-Item -Recurse -Force "C:\actions-runner\.cache"
```

## Verification

### Test Workflow
1. Push a change to trigger the workflow
2. Check GitHub Actions tab for progress
3. Verify documentation builds successfully
4. Confirm deployment to GitHub Pages

### Monitor Performance
- Check runner resource usage
- Monitor build times
- Review workflow logs for errors
- Optimize based on usage patterns

## Support

### Getting Help
- GitHub Actions Documentation: https://docs.github.com/en/actions
- Runner Documentation: https://github.com/actions/runner
- MkDocs Documentation: https://www.mkdocs.org/

### Common Commands
```powershell
# Check runner status
.\config.cmd --url https://github.com/zedfauji/magidesk-v1 --token YOUR_TOKEN --name WIDOWSVAIL --list

# Stop runner
.\config.cmd --url https://github.com/zedfauji/magidesk-v1 --token YOUR_TOKEN --name WIDOWSVAIL --stop

# Restart runner
net stop actions.runner.WIDOWSVAIL
net start actions.runner.WIDOWSVAIL
```

---

This setup should resolve the Python installation issues and provide a reliable self-hosted runner for your documentation deployment.