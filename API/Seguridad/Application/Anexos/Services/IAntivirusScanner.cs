public interface IAntivirusScanner
{
    /// <summary>
    /// Escanea el archivo y retorna true si está limpio, false si está infectado.
    /// </summary>
    Task<bool> ScanAsync(Stream fileStream, CancellationToken cancellationToken);
}