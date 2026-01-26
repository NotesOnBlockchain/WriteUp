## 1. Import the GPG public key

First, import my public GPG key.

You can find it here:

https://github.com/NotesOnBlockchain/WriteUp/GPG.txt

## 2. Verify the signed Git tag (optional)

Each release is created from a signed Git tag.

Clone the repository:
``` sh
git clone https://github.com/NotesOnBlockchain/WriteUp.git
cd WriteUp
```

Fetch tags:
``` sh
git fetch --tags
cd WriteUp
```

Verify the release tag:
``` sh
git verify-tag v1.0.0 // use the proper version number
```

You should see output similar to:
``` sh
Good signature from ...
```

## 3. Verify release binaries
Each release includes:

 - the installer files (.msi, .deb)

- signed checksum file: SHA256SUMS.asc

First, verify the checksum signature
``` sh
gpg --verify SHA256SUMS.asc
```

Then verify file checksums
``` sh
sha256sum -c SHA256SUMS.asc
```

### Notes for Windows users

The Windows installer (.msi) may trigger SmartScreen warnings because it is not signed with a commercial Authenticode certificate.
This does not affect cryptographic verification.