# ProjectDTS


## Prerequisites

* [.NET SDK](https://dotnet.microsoft.com/en-us/download) 
* [PostgreSQL](https://www.postgresql.org/download/)

---

## Setup

1. **Set environment variable for PostgreSQL password**

   The application requires an environment variable `DB_PASSWORD` for PostgreSQL. 

   ### Windows (PowerShell)

   ```powershell
   $env:DB_PASSWORD="yourpassword"
   ```

   To make it permanent:

   ```powershell
   setx DB_PASSWORD "yourpassword"
   ```

   ### Linux / macOS

   ```bash
   export DB_PASSWORD="yourpassword"
   ```

   To make it permanent, add it to your shell profile (`~/.bashrc` or `~/.zshrc`):

   ```bash
   echo 'export DB_PASSWORD="yourpassword"' >> ~/.bashrc
   source ~/.bashrc
   ```

---

## Build and Compile

Restore dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```
