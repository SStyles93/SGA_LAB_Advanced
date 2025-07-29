#!/bin/bash

# This script is designed to be placed in your Unity project's root folder.
# It automates checking out a specific Git tag and launching the corresponding Unity Editor version.

# --- Configuration (Optional) ---
# If your Unity Hub Editor installations are in a non-standard location, you can uncomment and modify this line:
# UNITY_HUB_EDITORS_PATH="/Applications/Unity/Hub/Editor"
# For Windows, this would typically be handled by the PowerShell script.

# --- Script Logic ---

# Function to display usage
usage() {
    echo "Usage: gototag <tag-name> [--clean]"
    echo ""
    echo "Arguments:"
    echo "  <tag-name>  The Git tag to checkout (e.g., v1.0, development)"
    echo ""
    echo "Options:"
    echo "  --clean     Performs a 'git clean -xfd' before checking out the tag."
    echo "              Use with caution: this will remove untracked files and directories."
    echo ""
    echo "Example:"
    echo "  gototag v2.5 --clean"
    echo "  gototag feature/new-ui"
    exit 1
}

# Parse arguments
TAG=""
CLEAN_FLAG=false

for arg in "$@"; do
    case $arg in
        --clean)
        CLEAN_FLAG=true
        shift
        ;;
        *)
        if [ -z "$TAG" ]; then
            TAG=$arg
        else
            echo "Error: Too many arguments provided."
            usage
        fi
        shift
        ;;
    esac
done

if [ -z "$TAG" ]; then
    usage
fi

echo "Closing Unity if running..."
pkill -x "Unity" 2>/dev/null
sleep 2

echo "Cleaning working directory (pre-checkout)..."
git reset --hard

if [ "$CLEAN_FLAG" = true ]; then
    echo "Performing git clean -xfd..."
    git clean -xfd
else
    echo "Skipping git clean -xfd. Use --clean to enable."
fi

echo "Checking out tag: $TAG"
git checkout tags/$TAG
if [ $? -ne 0 ]; then
    echo "Failed to checkout tag \'$TAG\'. Does the tag exist?"
    exit 1
fi

echo "Final reset and clean after switching..."
git reset --hard

if [ "$CLEAN_FLAG" = true ]; then
    echo "Performing git clean -xfd..."
    git clean -xfd
else
    echo "Skipping git clean -xfd. Use --clean to enable."
fi

# Extract Unity version from ProjectSettings
UNITY_VERSION_FILE="ProjectSettings/ProjectVersion.txt"
if [ ! -f "$UNITY_VERSION_FILE" ]; then
    echo "Cannot find $UNITY_VERSION_FILE. Is this a Unity project?"
    exit 1
fi

UNITY_VERSION=$(grep -oP 'm_EditorVersion: \K.*' "$UNITY_VERSION_FILE" || grep -o 'm_EditorVersion: .*' "$UNITY_VERSION_FILE" | cut -d' ' -f2)

# Determine Unity Editor path based on OS
if [[ "$(uname)" == "Darwin" ]]; then
    # macOS path
    UNITY_PATH="${UNITY_HUB_EDITORS_PATH:-/Applications/Unity/Hub/Editor}/$UNITY_VERSION/Unity.app/Contents/MacOS/Unity"
elif [[ "$(uname -s)" == "Linux" ]]; then
    # Linux path (common for Unity on Linux, adjust if needed)
    UNITY_PATH="${UNITY_HUB_EDITORS_PATH:-/opt/Unity/Hub/Editor}/$UNITY_VERSION/Editor/Unity"
else
    echo "Unsupported operating system for direct Unity launch via this script."
    echo "Please use the Windows PowerShell script (gototag.ps1) on Windows."
    exit 1
fi

if [ ! -f "$UNITY_PATH" ]; then
    echo "Unity Editor not found at: $UNITY_PATH"
    echo "Please make sure that version $UNITY_VERSION is installed via Unity Hub and the path is correct."
    exit 1
fi

echo "Launching Unity Editor version $UNITY_VERSION..."
"$UNITY_PATH" -projectPath "$(pwd)"

echo "Done. Tag \'$TAG\' is now active."


