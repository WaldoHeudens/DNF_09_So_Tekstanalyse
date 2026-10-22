namespace Tekstanalyse;

/// <summary>
/// Analyseert teksten. De methods zijn asynchroon omdat we doen alsof
/// elke tekst van een trage bron komt (een bestand of een netwerkoproep).
/// We simuleren die vertraging met Task.Delay.
///
/// De logica zit in aparte, testbare methods: los van Console.WriteLine
/// en met een CancellationToken zodat een aanroeper kan annuleren.
/// </summary>
public static class Woordenteller
{
    /// <summary>
    /// Telt de woorden in een enkele tekst. Simuleert trage I/O met Task.Delay.
    /// </summary>
    public static async Task<int> TelWoordenAsync(string tekst, CancellationToken ct = default)
    {
        // Doe alsof het ophalen van de tekst even duurt. Task.Delay honoreert
        // de token: bij annulering gooit het een OperationCanceledException.
        await Task.Delay(10, ct);

        if (string.IsNullOrWhiteSpace(tekst))
        {
            return 0;
        }

        // Split op witruimte en negeer lege stukken.
        return tekst.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    /// <summary>
    /// Telt de woorden van meerdere teksten tegelijk en geeft het totaal terug.
    /// De teksten worden parallel verwerkt met Task.WhenAll.
    /// </summary>
    public static async Task<int> TelTotaalAsync(IReadOnlyList<string> teksten, CancellationToken ct = default)
    {
        // Start alle tellingen zonder er meteen op te wachten. Zo lopen de
        // (gesimuleerde) trage oproepen door elkaar in plaats van na elkaar.
        IEnumerable<Task<int>> taken = teksten.Select(tekst => TelWoordenAsync(tekst, ct));

        // WhenAll wacht tot ze allemaal klaar zijn en geeft de resultaten
        // in dezelfde volgorde als de taken terug.
        int[] tellingen = await Task.WhenAll(taken);

        return tellingen.Sum();
    }

    /// <summary>
    /// Levert de telling van elke tekst een voor een op, zodra ze klaar is.
    /// De aanroeper verwerkt de resultaten met "await foreach".
    /// </summary>
    public static async IAsyncEnumerable<int> TelStapVoorStapAsync(
        IReadOnlyList<string> teksten,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        foreach (string tekst in teksten)
        {
            int telling = await TelWoordenAsync(tekst, ct);
            yield return telling;
        }
    }
}
