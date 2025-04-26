# Key Toss

## Overview

Key Toss is an offline-first password manager built with .NET MAUI and the MVVM pattern. It enables users to:

- **Securely store** their website credentials on-device using AES-256 encryption.
- **Generate** strong, random passwords with a built‑in generator.
- **Add**, **edit**, **delete**, and **search** password entries.
- **Show/Hide** stored passwords for quick access or privacy.
- **Track** each entry’s **expiration date** and **last modified** timestamp.

By relying solely on local storage (via SecureStorage and JSON serialization), Key Toss never sends your data over the internet, minimizing the risk of leaks.

## Features

- **User Registration & Login** with BCrypt-hashed master password stored in SecureStorage.
- **AES-256 Encryption** of all stored passwords (key & IV generated at signup).
- **Local JSON Persistence** using `SecureStorage` for cross-platform compatibility.
- **Password Management**: add new entries, modify existing ones, remove unwanted records.
- **Search** by website name with case‑insensitive filtering.
- **Show/Hide** toggle to reveal or mask plaintext passwords in the list.
- **Strong Password Generator** for one‑click creation of random 16‑character passwords.
- **Expiration & Audit**: each entry records when it was last modified and when it will expire.

## Technology Stack

- [.NET MAUI](https://learn.microsoft.com/dotnet/maui/) (cross-platform UI)
- MVVM architecture with [CommunityToolkit.MVVM](https://github.com/CommunityToolkit/MVVM)
- AES encryption via `System.Security.Cryptography`
- BCrypt hashing via `BCrypt.Net-Next`
- Local storage using `Microsoft.Maui.Storage.SecureStorage` + JSON
- C# 11, .NET 8

## Prerequisites

- **Visual Studio 2022** (17.6 or later) or **Visual Studio for Mac** with the **.NET MAUI workload** installed.
- **.NET 8 SDK**
- Android Emulator or iOS Simulator (or physical device)

## Building and Running

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/keytoss.git
   cd keytoss
   ```
2. **Open** the `KeyToss.sln` solution in Visual Studio.
3. **Restore** NuGet packages (Visual Studio should do this automatically).
4. **Select** your target (Android, iOS, or Windows) and **run** the project.

> **Tip:** On first launch, tap **"Sign Up"** to create an account (this also generates and stores your AES encryption key/IV). Then log in with your new credentials.

## Usage

- **Add a Password**: Tap the **+** button on the home screen, fill in website details, password, confirm, then save.
- **Edit**: In the list, tap **Edit** next to an entry to update its website name, password, or expiration date.
- **Delete**: Tap **Delete** and confirm to remove an entry permanently.
- **Show/Hide**: Toggle the **Show** button to reveal or mask the stored password.
- **Search**: Use the magnifier icon to filter entries by website name.
- **Home**: Tap the house icon to reset any active filters or search results.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

