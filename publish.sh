#!/bin/bash

# .NET 8.0 Project Packager
# Creates .deb and .tar.gz packages

# Configuration
PROJECT_NAME="WriteUpProject"
PROJECT_PATH="/WriteUpProject.Desktop/WriteUpProject.Desktop.csproj" # Path to .csproj file
OUTPUT_DIR="dist"       # Output directory for packages
BUILD_DIR="build"       # Temporary build directory
VERSION="1.0.0"
DESCRIPTION="Small application to create PSBT transactions with a custom message output to save messages on the Blockchain."
AUTHOR="Petho Adam"
EXECUTABLE_NAME="WriteUpProject.Desktop.exe"

# Debian package specific settings
DEB_PACKAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr ' ' '-')
MAINTAINER_EMAIL="petho.adam911@gmail.com"
LICENSE="MIT"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

print_status() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}! $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Check prerequisites
check_prerequisites() {
    print_status "Checking prerequisites..."
    
    if ! command_exists dotnet; then
        print_error "dotnet command not found. Please install .NET 8.0 SDK."
        exit 1
    fi
    
    # Check .NET version
    DOTNET_VERSION=$(dotnet --version 2>/dev/null || echo "unknown")
    if [[ ! "$DOTNET_VERSION" =~ ^8\. ]]; then
        print_warning "Detected .NET version: $DOTNET_VERSION. Script is optimized for .NET 8.0."
    else
        print_status "Found .NET version: $DOTNET_VERSION"
    fi
    
    # Check for dpkg-deb (for .deb creation)
    if ! command_exists dpkg-deb; then
        print_warning "dpkg-deb not found. Skipping .deb package creation."
        CREATE_DEB=false
    else
        CREATE_DEB=true
        print_status "Found dpkg-deb for .deb package creation"
    fi
}

# Clean previous builds
clean() {
    print_status "Cleaning previous builds..."
    rm -rf "$BUILD_DIR" "$OUTPUT_DIR"
    mkdir -p "$BUILD_DIR" "$OUTPUT_DIR"
}

# Build the project
build_project() {
    print_status "Building project..."
    
    # Restore dependencies
    dotnet restore "$PROJECT_PATH"
    
    # Build and publish for Linux x64
    dotnet publish "$PROJECT_PATH" \
        --output "$BUILD_DIR" \
        --runtime linux-x64 \
        --self-contained true \
        --configuration Release \
        -p:PublishSingleFile=true \
        -p:IncludeNativeLibrariesForSelfExtract=true \
        -p:DebugType=none \
        -p:AssemblyVersion="$VERSION" \
        -p:FileVersion="$VERSION"
    
    # Make executable
    chmod +x "$BUILD_DIR/$EXECUTABLE_NAME"
    
    print_status "Project built successfully"
}

# Create .tar.gz package
create_tarball() {
    print_status "Creating .tar.gz package..."
    
    local tarball_name="${PROJECT_NAME}_${VERSION}_linux-x64.tar.gz"
    local tarball_path="$OUTPUT_DIR/$tarball_name"
    
    # Create directory structure
    local tarball_dir="$BUILD_DIR/tarball/$PROJECT_NAME"
    mkdir -p "$tarball_dir"
    
    # Copy executable
    cp "$BUILD_DIR/$EXECUTABLE_NAME" "$tarball_dir/"
    
    # Create basic README
    cat > "$tarball_dir/README.md" << EOF
# $PROJECT_NAME

Version: $VERSION

## Installation
1. Extract this archive
2. Run ./$EXECUTABLE_NAME

## Requirements
- Linux x64 system
EOF

    # Create basic installation script
    cat > "$tarball_dir/install.sh" << EOF
#!/bin/bash
# Simple installation script

echo "Installing $PROJECT_NAME..."
sudo cp $EXECUTABLE_NAME /usr/local/bin/
echo "Installation complete. Run '$EXECUTABLE_NAME' to start."
EOF
    chmod +x "$tarball_dir/install.sh"
    
    # Create tar.gz
    cd "$BUILD_DIR/tarball"
    tar -czf "$tarball_path" "$PROJECT_NAME"
    cd - >/dev/null
    
    print_status "Created tarball: $tarball_name"
}

# Create .deb package
create_deb() {
    if [ "$CREATE_DEB" = false ]; then
        print_warning "Skipping .deb creation (dpkg-deb not available)"
        return
    fi
    
    print_status "Creating .deb package..."
    
    local deb_name="${DEB_PACKAGE_NAME}_${VERSION}_amd64.deb"
    local deb_path="$OUTPUT_DIR/$deb_name"
    local deb_build_dir="$BUILD_DIR/deb"
    local deb_control_dir="$deb_build_dir/DEBIAN"
    local deb_usr_dir="$deb_build_dir/usr"
    
    # Create directory structure
    mkdir -p "$deb_control_dir"
    mkdir -p "$deb_usr_dir/bin"
    mkdir -p "$deb_usr_dir/share/doc/$DEB_PACKAGE_NAME"
    
    # Copy executable
    cp "$BUILD_DIR/$EXECUTABLE_NAME" "$deb_usr_dir/bin/"
    chmod 0755 "$deb_usr_dir/bin/$EXECUTABLE_NAME"
    
    # Create control file
    cat > "$deb_control_dir/control" << EOF
Package: $DEB_PACKAGE_NAME
Version: $VERSION
Section: utils
Priority: optional
Architecture: amd64
Maintainer: $AUTHOR <$MAINTAINER_EMAIL>
Description: $DESCRIPTION
Depends: libc6
EOF

    # Create copyright file
    cat > "$deb_usr_dir/share/doc/$DEB_PACKAGE_NAME/copyright" << EOF
Format: https://www.debian.org/doc/packaging-manuals/copyright-format/1.0/
Upstream-Name: $PROJECT_NAME
Source: https://your-project-url.com

Files: *
Copyright: $(date +%Y) $AUTHOR
License: $LICENSE
EOF

    # Create changelog
    cat > "$deb_usr_dir/share/doc/$DEB_PACKAGE_NAME/changelog.Debian" << EOF
$DEB_PACKAGE_NAME ($VERSION) unstable; urgency=low

  * Initial release.

 -- $AUTHOR <$MAINTAINER_EMAIL>  $(date -R)
EOF
    gzip -9 "$deb_usr_dir/share/doc/$DEB_PACKAGE_NAME/changelog.Debian"

    # Create md5sums
    ( cd "$deb_build_dir" && find usr -type f -exec md5sum {} \; ) > "$deb_control_dir/md5sums"
    
    # Set permissions
    chmod 0755 "$deb_control_dir"
    chmod 0644 "$deb_control_dir/control"
    chmod 0644 "$deb_control_dir/md5sums"
    
    # Build package
    dpkg-deb --build "$deb_build_dir" "$deb_path"
    
    print_status "Created Debian package: $deb_name"
}

# Main execution
main() {
    print_status "Starting packaging process for $PROJECT_NAME v$VERSION"
    
    check_prerequisites
    clean
    build_project
    create_tarball
    create_deb
    
    print_status "Packaging complete!"
    echo "Packages created in: $OUTPUT_DIR"
    ls -la "$OUTPUT_DIR"
}

# Run main function
main "$@"