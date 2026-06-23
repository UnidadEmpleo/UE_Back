using nClam;

public class ClamAntivirusScanner : IAntivirusScanner
{
    private readonly ClamClient _clamClient;

    public ClamAntivirusScanner(string clamHost = "localhost", int clamPort = 3310)
    {
        _clamClient = new ClamClient(clamHost, clamPort);
    }

    public async Task<bool> ScanAsync(Stream fileStream, CancellationToken cancellationToken)
    {
        var result = await _clamClient.SendAndScanFileAsync(fileStream);
        return result.Result != ClamScanResults.VirusDetected;
    }
}