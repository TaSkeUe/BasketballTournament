public static class MatchSimulator{
    public static Dictionary<string, List<MatchResult>> SimulateGroupMatches(Dictionary<string, List<Teams>> groups)
    {
        var random = new Random();
        var results = new Dictionary<string, List<MatchResult>>();

        foreach (var group in groups)
        {
            var matchResults = new List<MatchResult>();

            for (int i = 0; i < group.Value.Count; i++)
            {
                for (int j = i + 1; j < group.Value.Count; j++)
                {
                    var team1 = group.Value[i];
                    var team2 = group.Value[j];

                    // Linearna verovatnoća pobede bazirana na FIBA rangiranju sa ograničenjem
                    double rankDifference = team2.FIBARanking - team1.FIBARanking;
                    double team1WinProbability = 0.5 + (rankDifference / 200.0);
                    team1WinProbability = Math.Clamp(team1WinProbability, 0.1, 0.9); // Ograničava verovatnoću između 0.1 i 0.9

                    int team1Score = random.Next(70, 100);
                    int team2Score = random.Next(70, 100);

                    if (random.NextDouble() < team1WinProbability)
                    {
                        team1Score += random.Next(5, 20);
                    }
                    else
                    {
                        team2Score += random.Next(5, 20);
                    }

                    matchResults.Add(new MatchResult
                    {
                        Team1 = team1.Team,
                        Team2 = team2.Team,
                        Team1Score = team1Score,
                        Team2Score = team2Score
                    });
                }
            }

            results[group.Key] = matchResults;
        }

        return results;
    }

    public static List<TeamStats> RankTeams(Dictionary<string, List<MatchResult>> matchResults, Dictionary<string, List<Teams>> groups)
    {
        var allTeams = new List<TeamStats>();

        foreach (var group in groups)
        {
            var teamStats = new List<TeamStats>();

            foreach (var team in group.Value)
            {
                var stats = new TeamStats { Team = team.Team };

                foreach (var match in matchResults[group.Key])
                {
                    if (match.Team1 == team.Team)
                    {
                        stats.PointsFor += match.Team1Score;
                        stats.PointsAgainst += match.Team2Score;

                        if (match.Team1Score > match.Team2Score)
                        {
                            stats.Wins++;
                            stats.Points += 2;
                        }
                        else
                        {
                            stats.Losses++;
                            stats.Points += 1;
                        }
                    }
                    else if (match.Team2 == team.Team)
                    {
                        stats.PointsFor += match.Team2Score;
                        stats.PointsAgainst += match.Team1Score;

                        if (match.Team2Score > match.Team1Score)
                        {
                            stats.Wins++;
                            stats.Points += 2;
                        }
                        else
                        {
                            stats.Losses++;
                            stats.Points += 1;
                        }
                    }
                }

                stats.PointDifference = stats.PointsFor - stats.PointsAgainst;
                teamStats.Add(stats);
            }

            var rankedTeams = teamStats.OrderByDescending(t => t.Points)
                                    .ThenByDescending(t => t.PointDifference)
                                    .ThenByDescending(t => t.PointsFor)
                                    .ToList();

            allTeams.AddRange(rankedTeams);
        }

        // Rangiranje timova po grupama
        var topTeams = allTeams.Where(t => t.Points > 0).ToList();
        var sortedTeams = topTeams.OrderByDescending(t => t.Points)
                                .ThenByDescending(t => t.PointDifference)
                                .ThenByDescending(t => t.PointsFor)
                                .ToList();

        // Dodela rangova
        for (int i = 0; i < sortedTeams.Count; i++)
        {
            sortedTeams[i].Rank = i + 1;
        }

        return sortedTeams;
    }

    public static void SimulateEliminationPhase(List<(Teams, Teams)> quarterfinals)
    {
        var random = new Random();

        // Cetvrtfinale
        Console.WriteLine("Četvrtfinale:");
        var semifinalists = new List<Teams>();

        foreach (var match in quarterfinals)
        {
            var winner = SimulateMatch(match.Item1, match.Item2);
            Console.WriteLine($"    {match.Item1.Team} - {match.Item2.Team} ({match.Item1.Score}:{match.Item2.Score})");
            semifinalists.Add(winner);
        }

        Console.WriteLine();

        // Polufinale
        Console.WriteLine("Polufinale:");
        var finalists = new List<Teams>();
        var thirdPlaceContenders = new List<Teams>();

        for (int i = 0; i < semifinalists.Count; i += 2)
        {
            var winner = SimulateMatch(semifinalists[i], semifinalists[i + 1]);
            Console.WriteLine($"    {semifinalists[i].Team} - {semifinalists[i + 1].Team} ({semifinalists[i].Score}:{semifinalists[i + 1].Score})");
            finalists.Add(winner);
            thirdPlaceContenders.Add(semifinalists[i] == winner ? semifinalists[i + 1] : semifinalists[i]);
        }

        Console.WriteLine();

        // Za trece mesto
        Console.WriteLine("Utakmica za treće mesto:");
        var thirdPlaceWinner = SimulateMatch(thirdPlaceContenders[0], thirdPlaceContenders[1]);
        Console.WriteLine($"    {thirdPlaceContenders[0].Team} - {thirdPlaceContenders[1].Team} ({thirdPlaceContenders[0].Score}:{thirdPlaceContenders[1].Score})");

        Console.WriteLine();

        // Finale
        Console.WriteLine("Finale:");
        var champion = SimulateMatch(finalists[0], finalists[1]);
        Console.WriteLine($"    {finalists[0].Team} - {finalists[1].Team} ({finalists[0].Score}:{finalists[1].Score})");

        Console.WriteLine();

        // Dodela medalja
        Console.WriteLine("Medalje:");
        Console.WriteLine($"    1. {champion.Team}");
        Console.WriteLine($"    2. {(finalists[0] == champion ? finalists[1].Team : finalists[0].Team)}");
        Console.WriteLine($"    3. {thirdPlaceWinner.Team}");
    }

    private static Teams SimulateMatch(Teams team1, Teams team2)
    {
        team1.Score = new Random().Next(60, 100);
        team2.Score = new Random().Next(60, 100);
    
        return team1.Score > team2.Score ? team1 : team2;
    }
}