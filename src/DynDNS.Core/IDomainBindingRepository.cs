using DynDNS.Core.Abstractions.Models;
using DomainBinding = DynDNS.Core.Model.DomainBinding;

namespace DynDNS.Core;

/// <summary>
/// Persistent storage for all <em>domain bindings</em>.
/// </summary>
public interface IDomainBindingRepository
{
    /// <summary>
    /// Add a new domain binding to the storage.
    /// </summary>
    /// <param name="domainBinding">The domain binding to add.</param>
    Task AddAsync(DomainBinding domainBinding);
    
    /// <summary>
    /// Updates the domain binding in the storage.
    /// </summary>
    /// <param name="domainBinding">The domain binding to update.</param>
    Task UpdateAsync(DomainBinding domainBinding);
    
    /// <summary>
    /// Remove a domain binding from the storage.
    /// </summary>
    /// <param name="domainBinding">The domain binding to remove.</param>
    Task RemoveAsync(DomainBinding domainBinding);
    
    /// <summary>
    /// Return all stored domain bindings.
    /// </summary>
    /// <returns>A collection of all domain bindings that have been added by <see cref="AddAsync"/>.</returns>
    Task<IReadOnlyCollection<DomainBinding>> GetAllAsync();
    
    /// <summary>
    /// Retrieve a specific domain binding - if it exists.
    /// </summary>
    /// <param name="id"><see cref="DomainBindingId"/> of the domain binding to retrieve.</param>
    /// <returns>The found <see cref="DomainBinding"/> or <c>null</c>.</returns>
    Task<DomainBinding?> GetByIdAsync(DomainBindingId id);
    
    /// <summary>
    /// Retrieve a specific domain binding - if it exists.
    /// </summary>
    /// <param name="hostname"><see cref="Hostname"/> of the domain binding to retrieve.</param>
    /// <returns>The found <see cref="DomainBinding"/> or <c>null</c>.</returns>
    Task<DomainBinding?> GetByHostnameAsync(Hostname hostname);
    
}
