#!/bin/bash
# Run Documentation Portal Locally
# This script sets up and runs the MkDocs documentation portal locally

echo "Setting up Magidesk POS Documentation Portal..."

# Check if Python is installed
if ! command -v python3 &> /dev/null; then
    echo "Error: Python 3 is not installed"
    echo "Please install Python 3.8 or higher"
    exit 1
fi

echo "Python found: $(python3 --version)"

# Check if pip is available
if ! command -v pip3 &> /dev/null; then
    echo "Error: pip3 is not installed"
    exit 1
fi

echo "Pip found: $(pip3 --version)"

# Install requirements
echo "Installing Python dependencies..."
pip3 install -r requirements.txt

if [ $? -ne 0 ]; then
    echo "Error: Failed to install dependencies"
    exit 1
fi

# Build and serve documentation
echo "Starting documentation server..."
echo "Documentation will be available at: http://127.0.0.1:8000"
echo "Press Ctrl+C to stop the server"
echo ""

# Start MkDocs development server
mkdocs serve --dev-addr=0.0.0.0:8000