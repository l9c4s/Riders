using System;

namespace Aplication.Contracts;


/// <summary>
/// Define um contrato para notificar a camada de apresentação sobre mudanças de permissão de localização.
/// </summary>
public interface ILocationPermissionNotifier
{
    Task PermissionGrantedAsync(string observerId, string sharerId);
    Task PermissionRevokedAsync(string observerId, string sharerId);
}
