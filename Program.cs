using System.Security.Cryptography.X509Certificates;
using WarSim.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

// Register application services (moved to extension method)
builder.Services.AddApplicationServices();

// Configure Kestrel to load a certificate if provided in configuration
builder.WebHost.ConfigureKestrel((context, options) =>
{
    var config = context.Configuration;
    var certPath = config["Kestrel:Certificates:Default:Path"];
    var certPassword = config["Kestrel:Certificates:Default:Password"];
    if (!string.IsNullOrEmpty(certPath))
    {
        // If a certificate path is configured, attempt to load it for HTTPS
        try
        {
            var cert = new X509Certificate2(certPath, certPassword);
            options.ConfigureHttpsDefaults(https => https.ServerCertificate = cert);
        }
        catch (Exception ex)
        {
            // Log to console - don't throw during startup to keep developer experience simple
            Console.WriteLine($"Failed to load HTTPS certificate from {certPath}: {ex.Message}");
        }
    }
    else
    {
        // If no file path provided, attempt to load certificate from certificate store by thumbprint.
        var thumbprint = config["Kestrel:Certificates:Default:Thumbprint"];
        if (!string.IsNullOrEmpty(thumbprint))
        {
            try
            {
                // Normalize thumbprint (remove spaces)
                var tp = thumbprint.Replace(" ", string.Empty).ToUpperInvariant();
                var storeNameStr = config["Kestrel:Certificates:Default:Store"] ?? "My";
                var storeLocationStr = config["Kestrel:Certificates:Default:StoreLocation"] ?? "CurrentUser";

                var storeName = (System.Security.Cryptography.X509Certificates.StoreName)Enum.Parse(typeof(System.Security.Cryptography.X509Certificates.StoreName), storeNameStr, true);
                var storeLocation = storeLocationStr.Equals("LocalMachine", StringComparison.OrdinalIgnoreCase)
                    ? System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine
                    : System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;

                using var store = new X509Store(storeName, storeLocation);
                store.Open(OpenFlags.ReadOnly);
                var certs = store.Certificates.Find(X509FindType.FindByThumbprint, tp, validOnly: false);
                if (certs.Count > 0)
                {
                    var cert = certs[0];
                    options.ConfigureHttpsDefaults(https => https.ServerCertificate = cert);
                    Console.WriteLine($"Loaded HTTPS certificate from store '{storeName}' (location: {storeLocation}) with thumbprint {tp}.");
                }
                else
                {
                    Console.WriteLine($"No certificate found in store '{storeName}' (location: {storeLocation}) with thumbprint {tp}.");
                }

                store.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load HTTPS certificate from store by thumbprint {thumbprint}: {ex.Message}");
            }
        }
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS middleware before authorization and endpoint mapping
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
