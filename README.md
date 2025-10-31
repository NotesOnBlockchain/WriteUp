# WriteUp

An open-source application to create self-spend BTC transactions with a custom output. The custom output can hold up to 80 bytes as a message. You can write anything up on the blockchain.

# [Download WriteUp](https://github.com/NotesOnBlockchain/LookUp.Core/releases)

<br>

# Build From Source Code

## Requirements

- [Git](https://git-scm.com/downloads)
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)

## Clone Repo @ Build and Run
```sh
   git clone https://github.com/NotesOnBlockchain/WriteUpApp.git
   cd WriteUpApp
   dotnet build && dotnet run
```

# Example OP_RETURN

If a transaction contains:

```sh
OP_RETURN 48656c6c6f20576f726c64
```

The decoded message will be:
```sh
"Hello World"
```