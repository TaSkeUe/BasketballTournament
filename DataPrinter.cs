public static class DataPrinter{
    public static void PrintGroupData(Dictionary<string, List<Teams>> groups)
    {
        foreach (var group in groups)
        {
            Console.WriteLine($"Grupa {group.Key}:");
            foreach (var team in group.Value)
            {
                Console.WriteLine($"  - Tim: {team.Team}, ISO Kod: {team.ISOCode}, FIBA Rang: {team.FIBARanking}");
            }
            Console.WriteLine();
        }
    }

    public static void PrintRankings(Dictionary<string, List<Teams>> groups, List<TeamStats> rankedTeams)
    {
        Console.WriteLine("Konačan plasman u grupama:");

        foreach (var group in groups)
        {
            Console.WriteLine($"Grupa {group.Key}:");
            var groupTeams = rankedTeams.Where(t => group.Value.Any(g => g.Team == t.Team)).OrderBy(t => t.Rank);

            foreach (var team in groupTeams)
            {
                Console.WriteLine($"  {team.Rank}. {team.Team} - {team.Wins} / {team.Losses} / {team.Points} / {team.PointsFor} / {team.PointsAgainst} / {team.PointDifference}");
            }

            Console.WriteLine();
        }
    }
}