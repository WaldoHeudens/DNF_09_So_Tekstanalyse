using Tekstanalyse;

// Top-level statements mogen asynchroon zijn: gebruik gewoon "await".
// De compiler maakt er onder de motorkap een "async Task Main" van.

string[] teksten =
[
    "async en await horen bij elkaar",
    "een Task stelt werk voor dat later klaar is",
    "WhenAll wacht op meerdere taken tegelijk",
];

Console.WriteLine("Woorden per tekst (stap voor stap):");
await foreach (int telling in Woordenteller.TelStapVoorStapAsync(teksten))
{
    Console.WriteLine($"  {telling} woorden");
}

int totaal = await Woordenteller.TelTotaalAsync(teksten);
Console.WriteLine($"Totaal: {totaal} woorden.");
