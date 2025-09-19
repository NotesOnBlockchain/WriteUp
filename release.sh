#!/usr/bin/env bash
set -euo pipefail

APP_NAME="WriteUpProject"
PROJECT_PATH="WriteUpProject.Desktop/WriteUpProject.Desktop.csproj"
VERSION="1.0.0"
RUNTIME="linux-x64"
PUBLISH_DIR="dist/${RUNTIME}"

echo "=== Cleaning up ==="
rm -rf "/home/adam/Desktop/WriteUpApp/${PUBLISH_DIR}"
mkdir "/home/adam/Desktop/WriteUpApp/dist"
mkdir "/home/adam/Desktop/WriteUpApp/${PUBLISH_DIR}"

echo "=== Publishing ${APP_NAME} for ${RUNTIME} ==="
dotnet publish "${PROJECT_PATH}" \
    -c Release \
    -r ${RUNTIME} \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:PublishTrimmed=true \
    -o "${PUBLISH_DIR}"

# --- TAR.GZ package ---
echo "=== Creating tar.gz package ==="
# cd "$PUBLISH_DIR"
tar -czf dist/"${APP_NAME}-${VERSION}-${RUNTIME}.tar.gz" "$PUBLISH_DIR"/*
cd ..

# --- DEB package ---
echo "=== Creating deb package ==="
PKG_DIR="deb_pkg"
DEB_NAME="${APP_NAME}_${VERSION}_amd64.deb"

rm -rf "/home/adam/Desktop/WriteUpApp/dist/${PKG_DIR}"
mkdir -p "/home/adam/Desktop/WriteUpApp/dist/${PKG_DIR}/DEBIAN"
mkdir -p "/home/adam/Desktop/WriteUpApp/dist/${PKG_DIR}/usr/local/bin/${APP_NAME}"

# Control file
cat > "/home/adam/Desktop/WriteUpApp/dist/${PKG_DIR}/DEBIAN/control" <<EOF
Package: ${APP_NAME}
Version: ${VERSION}
Section: utils
Priority: optional
Architecture: amd64
Maintainer: Petho Adam <petho.adam911@gmail.com>
Description: Write unchangeable message onto the Blockchain.
 A small desktop app for creating PSBTs with custom OP_RETURN outputs.
EOF

# Copy published files
cp -r "/home/adam/Desktop/WriteUpApp/dist/${RUNTIME}" "/home/adam/Desktop/WriteUpApp/dist/${PKG_DIR}/usr/local/bin/${APP_NAME}"

# Build the .deb
dpkg-deb --build "/home/adam/Desktop/WriteUpApp/dist/${PKG_DIR}" "/home/adam/Desktop/WriteUpApp/dist/${APP_NAME}.deb"

echo "=== Packages created ==="
