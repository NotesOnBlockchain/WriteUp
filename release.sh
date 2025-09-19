#!/usr/bin/env bash
set -euo pipefail

APP_NAME="WriteUpProject"
PROJECT_PATH="src/WriteUpProject.Desktop/WriteUpProject.Desktop.csproj"
VERSION="1.0.0"
RUNTIME="linux-x64"
PUBLISH_DIR="dist/${RUNTIME}"

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
cd publish
tar -czf "${APP_NAME}-${VERSION}-${RUNTIME}.tar.gz" "${RUNTIME}"
cd ..

# --- DEB package ---
echo "=== Creating deb package ==="
PKG_DIR="deb_pkg"
DEB_NAME="${APP_NAME,,}_${VERSION}_amd64.deb"   # lowercase name

rm -rf "${PKG_DIR}"
mkdir -p "${PKG_DIR}/DEBIAN"
mkdir -p "${PKG_DIR}/usr/local/bin"

# Control file
cat > "${PKG_DIR}/DEBIAN/control" <<EOF
Package: ${APP_NAME}
Version: ${VERSION}
Section: utils
Priority: optional
Architecture: amd64
Maintainer: Your Name <you@example.com>
Description: ${APP_NAME} desktop app
 A small desktop app for signing and creating PSBTs.
EOF

# Copy published files
cp -r "${PUBLISH_DIR}/"* "${PKG_DIR}/usr/local/bin/"

# Build the .deb
dpkg-deb --build "${PKG_DIR}" "publish/${DEB_NAME}"

echo "=== Packages created ==="
ls -lh publish/*.tar.gz publish/*.deb
