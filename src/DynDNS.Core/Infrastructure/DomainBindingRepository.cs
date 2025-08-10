using System.Text.Json;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Model;
using DomainBinding = DynDNS.Core.Model.DomainBinding;

namespace DynDNS.Core.Infrastructure;

/// <inheritdoc />
public sealed class DomainBindingRepository : IDomainBindingRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    private readonly string targetFile;

    public DomainBindingRepository()
    {
        var directory = Environment.GetEnvironmentVariable("DYNDNS_STORAGE") ?? Environment.CurrentDirectory;
        targetFile = Path.Combine(directory, "domain-bindings.json");
    }

    /// <inheritdoc />
    public async Task AddAsync(DomainBinding domainBinding)
    {
        var bindings = await ReadFileAsync();
        bindings.Add(SerializedDomainBinding.From(domainBinding));

        await WriteFileAsync(bindings);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(DomainBinding domainBinding)
    {
        var bindings = await ReadFileAsync();

        var binding = bindings.FirstOrDefault(b => b.Id == domainBinding.Id);
        binding?.Apply(domainBinding);

        await WriteFileAsync(bindings);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(DomainBinding domainBinding)
    {
        var bindings = await ReadFileAsync();
        bindings.RemoveAll(b => b.Id == domainBinding.Id);

        await WriteFileAsync(bindings);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DomainBinding>> GetAllAsync()
    {
        var bindings = await ReadFileAsync();

        return bindings.Select(static b => b.ToEntity()).ToList();
    }

    /// <inheritdoc />
    public async Task<DomainBinding?> GetByIdAsync(DomainBindingId id)
    {
        var bindings = await ReadFileAsync();
        var binding = bindings.FirstOrDefault(b => b.Id == id);

        return binding?.ToEntity();
    }

    /// <inheritdoc />
    public async Task<DomainBinding?> GetByHostnameAsync(Hostname hostname)
    {
        var bindings = await ReadFileAsync();
        var binding = bindings.FirstOrDefault(b => b.Hostname == hostname);

        return binding?.ToEntity();
    }

    private async Task<List<SerializedDomainBinding>> ReadFileAsync()
    {
        if (!File.Exists(targetFile))
        {
            return [];
        }

        await using var fileStream = File.OpenRead(targetFile);
        return await JsonSerializer.DeserializeAsync<List<SerializedDomainBinding>>(fileStream, SerializerOptions) ?? [];
    }

    private async Task WriteFileAsync(List<SerializedDomainBinding> domainBindings)
    {
        await using var fileStream = File.Open(targetFile, FileMode.Create);
        await JsonSerializer.SerializeAsync(fileStream, domainBindings, SerializerOptions);
    }

    private sealed class SerializedDomainBinding
    {
        public required string Id { get; set; }
        public required string Hostname { get; set; }

        public SerializedSubdomain[] Subdomains { get; set; } = [];
        public SerializedProviderConfiguration? ProviderConfiguration { get; set; }

        public static SerializedDomainBinding From(DomainBinding domainBinding)
        {
            return new SerializedDomainBinding
            {
                Id = domainBinding.Id,
                Hostname = domainBinding.Hostname,
                ProviderConfiguration = SerializedProviderConfiguration.From(domainBinding.ProviderConfiguration),
                Subdomains = domainBinding.Subdomains.Select(SerializedSubdomain.From).ToArray()
            };
        }

        public void Apply(DomainBinding domainBinding)
        {
            ProviderConfiguration = SerializedProviderConfiguration.From(domainBinding.ProviderConfiguration);
            Subdomains = domainBinding.Subdomains.Select(SerializedSubdomain.From).ToArray();
        }

        public DomainBinding ToEntity()
        {
            return new DomainBinding(
                id: new DomainBindingId(Id),
                hostname: Abstractions.Models.Hostname.From(Hostname),
                subdomains: Subdomains.Select(s => s.ToEntity()).ToList(),
                providerConfiguration: ProviderConfiguration?.ToEntity());
        }
    }

    private sealed class SerializedSubdomain
    {
        public required string Name { get; set; }
        public required bool CreateIPv4Record { get; set; }
        public required bool CreateIPv6Record { get; set; }

        public static SerializedSubdomain From(Subdomain subdomain)
        {
            return new SerializedSubdomain
            {
                Name = subdomain.Name,
                CreateIPv4Record = subdomain.CreateIPv4Record,
                CreateIPv6Record = subdomain.CreateIPv6Record
            };
        }

        public Subdomain ToEntity()
        {
            return new Subdomain(DomainFragment.From(Name), CreateIPv4Record, CreateIPv6Record);
        }
    }

    private sealed class SerializedProviderConfiguration
    {
        public required string Name { get; set; }
        public required Dictionary<string, string> Parameters { get; set; }

        public static SerializedProviderConfiguration? From(ProviderConfiguration? providerConfiguration)
        {
            if (providerConfiguration is null)
            {
                return null;
            }

            return new SerializedProviderConfiguration
            {
                Name = providerConfiguration.Name,
                Parameters = new Dictionary<string, string>(providerConfiguration.Parameters)
            };
        }

        public ProviderConfiguration ToEntity()
        {
            return new ProviderConfiguration(Name, new Dictionary<string, string>(Parameters));
        }
    }
}
