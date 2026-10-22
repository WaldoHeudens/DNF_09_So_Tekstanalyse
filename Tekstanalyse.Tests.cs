using Tekstanalyse;

namespace Tekstanalyse.Tests;

public class WoordentellerTests
{
    // Async tests geven een Task terug. xUnit wacht die af, dus je test
    // gewoon het resultaat van je await, niet de timing.
    [Theory]
    [InlineData("een twee drie", 3)]
    [InlineData("woord", 1)]
    [InlineData("  veel   spaties   ertussen  ", 3)]
    public async Task TelWoordenAsync_TeltDeWoorden(string tekst, int verwacht)
    {
        int aantal = await Woordenteller.TelWoordenAsync(tekst);

        Assert.Equal(verwacht, aantal);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task TelWoordenAsync_LegeTekst_GeeftNul(string tekst)
    {
        Assert.Equal(0, await Woordenteller.TelWoordenAsync(tekst));
    }

    [Fact]
    public async Task TelTotaalAsync_TeltAlleTekstenSamen()
    {
        string[] teksten = ["een twee", "drie vier vijf", "zes"];

        int totaal = await Woordenteller.TelTotaalAsync(teksten);

        Assert.Equal(6, totaal);
    }

    [Fact]
    public async Task TelStapVoorStapAsync_GeeftTellingPerTekstInVolgorde()
    {
        string[] teksten = ["een", "twee drie", "vier vijf zes"];

        List<int> tellingen = [];
        await foreach (int telling in Woordenteller.TelStapVoorStapAsync(teksten))
        {
            tellingen.Add(telling);
        }

        Assert.Equal(new[] { 1, 2, 3 }, tellingen);
    }

    [Fact]
    public async Task TelWoordenAsync_GeannuleerdeToken_GooitException()
    {
        using CancellationTokenSource cts = new();
        cts.Cancel();

        // We testen het gedrag (er wordt geannuleerd), niet de timing.
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => Woordenteller.TelWoordenAsync("een twee", cts.Token));
    }
}
