#!/bin/bash

set -e

echo "Building the dll"
dotnet restore
dotnet build -c Release

echo "Coping the built dll to the thunderstore folder"
BUILT_DLL_PATH="$(find . -type f -name 'PersistentPurchasesRewritten.dll' -path '*bin**Release*')"
cp -f "$BUILT_DLL_PATH" "./Thunderstore/PersistentPurchasesRewritten.dll"

CS_PROJ_PATH="$(find . -type f -name '*.csproj')"
VERSION="$(grep -oP '<Version>\K[^<]+' "$CS_PROJ_PATH")"

echo "Replacing the version number from the thunderstore manifest"
MANIFEST_PATH="./Thunderstore/manifest.json"
sed -i -E "s/\"version_number\": \"[0-9]+\.[0-9]+\.[0-9]+\"/\"version_number\": \"${VERSION}\"/" "$MANIFEST_PATH"

echo "Creating the zip"
zip -j -r "PersistentPurchasesRewritten-${VERSION}.zip" Thunderstore/*

echo "Removing the now useless file"
rm -f "./Thunderstore/PersistentPurchasesRewritten.dll"

echo "Done"