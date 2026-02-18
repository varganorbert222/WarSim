# Deployment and HTTPS configuration

This document describes how to handle Development and Production configuration, HTTPS certificates, and secrets for this project.

## Files in the repository
- `appsettings.json` - common defaults (safe to commit)
- `appsettings.Development.json` - development overrides (safe to commit, don't put secrets here)
- `appsettings.Production.json` - production overrides (can contain placeholders; do NOT commit secrets here)
- `Properties/launchSettings.json` - development launch profiles (safe to commit)
- `.gitignore` - already ignores `*.pfx` files

## Development setup (local)
1. Trust the .NET developer certificate (one-time):

   ```
   dotnet dev-certs https --trust
   ```

2. Use `appsettings.Development.json` for non-sensitive dev configuration (logs etc.).

3. Store secrets (local PFX path and password) using User Secrets (recommended):

   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Kestrel:Certificates:Default:Path" "C:\path\to\dev.pfx"
   dotnet user-secrets set "Kestrel:Certificates:Default:Password" "your-password"
   ```

   Alternatively set environment variables (recommended for CI):

   Linux/macOS:
   ```bash
   export Kestrel__Certificates__Default__Path="/etc/ssl/certs/dev.pfx"
   export Kestrel__Certificates__Default__Password="your-password"
   ```

   Windows (PowerShell):
   ```powershell
   setx Kestrel__Certificates__Default__Path "C:\path\to\dev.pfx"
   setx Kestrel__Certificates__Default__Password "your-password"
   ```

4. Run the app with the `https` launch profile so Swagger opens on HTTPS: `dotnet run --launch-profile "https"` or from your IDE choose the `https` profile.

## Production setup (server / cloud)

Do NOT store PFX files or passwords in the repository.

Recommended approaches:

1. Use the hosting provider's certificate management (e.g., Azure App Service TLS/SSL settings).
2. Store the PFX and password in a secret store (Azure Key Vault, HashiCorp Vault, GitHub Secrets) and inject into the runtime environment or pipeline.
3. Set environment variables in the host environment or pipeline using the double-underscore naming convention for configuration keys, for example:

   ```bash
   export Kestrel__Certificates__Default__Path=/etc/ssl/certs/prod.pfx
   export Kestrel__Certificates__Default__Password='s3cr3t'
   ```

4. Alternatively, the application can load certificate from the Windows Certificate Store by thumbprint. Provide the thumbprint and optionally store name/location using environment variables or configuration:

   - `Kestrel:Certificates:Default:Thumbprint` (thumbprint string)
   - `Kestrel:Certificates:Default:Store` (e.g. `My`)
   - `Kestrel:Certificates:Default:StoreLocation` (either `CurrentUser` or `LocalMachine`)

## CI/CD example (GitHub Actions)

Example snippet for injecting certificate info into a workflow (use secrets):

```yaml
env:
  Kestrel__Certificates__Default__Path: ${{ secrets.CERT_PATH }}
  Kestrel__Certificates__Default__Password: ${{ secrets.CERT_PASSWORD }}

# or if you use Azure Key Vault, retrieve the values into env vars before dotnet publish/run
```

## Security best practices
- Never commit `*.pfx` files or passwords into source control.
- Use your cloud provider's managed certificate solutions whenever possible.
- Prefer Key Vault or cloud secrets for storing production secrets.

## Troubleshooting
- If the app fails to load certificate, check the logs printed to console. The `Program.cs` is resilient and will continue startup without throwing if certificate loading fails (so the app can still run in non-HTTPS mode for dev convenience).
- On Windows, ensure the process identity has access to the private key of the certificate in the certificate store.

If you want, I can also:
- Add a GitHub Actions workflow template showing how to publish with secrets.
- Add scripts to help import a PFX into the Windows Certificate Store during deployment.
