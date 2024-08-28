public static class DrawPrinter
{
    public static void PrintSeeds(Dictionary<string, List<Teams>> seeds)
    {
        Console.WriteLine("Šeširi:");
        foreach (var seed in seeds)
        {
            Console.WriteLine($"  Šešir {seed.Key}");
            foreach (var team in seed.Value)
            {
                Console.WriteLine($"    {team.Team}");
            }
            Console.WriteLine();
        }
    }

    public static void PrintQuarterfinals(List<(Teams, Teams)> quarterfinals)
    {
        Console.WriteLine("Eliminaciona faza:");
        foreach (var match in quarterfinals)
        {
            Console.WriteLine($"  {match.Item1.Team} - {match.Item2.Team}");
        }
        Console.WriteLine();
    }
}