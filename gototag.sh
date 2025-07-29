#!/bin/bash

# This script is designed to be placed in your Unity project's root folder.
# It automates checking out a specific Git tag and launching the corresponding Unity Editor version.

# --- Configuration (Optional) ---
# If your Unity Hub Editor installations are in a non-standard location, you can uncomment and modify this line:
# UNITY_HUB_EDITORS_PATH="/Applications/Unity/Hub/Editor"
# For Windows, this would typically be handled by the PowerShell script.

# --- Script Logic ---

if [ -z "$1" ]; then
    echo "❌ Error: No tag name provided."
    echo "Usage: gototag <tag-name>"
    exit 1
fi

TAG=$1

echo "🛑 Closing Unity if running..."
pkill -x "Unity" 2>/dev/null
sleep 2

echo "🧹 Cleaning working directory (pre-checkout)..."
git reset --hard
git clean -xfd

echo "🔁 Checking out tag: $TAG"
git checkout tags/$TAG
if [ $? -ne 0 ]; then
    echo "❌ Failed to checkout tag \'$TAG\'. Does the tag exist?"
    exit 1
fi

echo "🧼 Final reset and clean after switching..."
git reset --hard
git clean -xfd

# Extract Unity version from ProjectSettings
UNITY_VERSION_FILE="ProjectSettings/ProjectVersion.txt"
if [ ! -f "$UNITY_VERSION_FILE" ]; then
    echo "❌ Cannot find $UNITY_VERSION_FILE. Is this a Unity project?"
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
    echo "❌ Unsupported operating system for direct Unity launch via this script."
    echo "Please use the Windows PowerShell script (gototag.ps1) on Windows."
    exit 1
fi

if [ ! -f "$UNITY_PATH" ]; then
    echo "❌ Unity Editor not found at: $UNITY_PATH"
    echo "Please make sure that version $UNITY_VERSION is installed via Unity Hub and the path is correct."
    exit 1
fi

echo "🚀 Launching Unity Editor version $UNITY_VERSION..."
"$UNITY_PATH" -projectPath "$(pwd)"

echo "✅ Done. Tag \'$TAG\' is now active."


