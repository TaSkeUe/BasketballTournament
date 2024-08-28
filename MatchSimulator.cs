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

    // Rank teams across all groups
    var topTeams = allTeams.Where(t => t.Points > 0).ToList();
    var sortedTeams = topTeams.OrderByDescending(t => t.Points)
                               .ThenByDescending(t => t.PointDifference)
                               .ThenByDescending(t => t.PointsFor)
                               .ToList();

    // Assign ranks
    for (int i = 0; i < sortedTeams.Count; i++)
    {
        sortedTeams[i].Rank = i + 1;
    }

    return sortedTeams;
}

}