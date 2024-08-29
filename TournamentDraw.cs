public static class TournamentDraw{
    public static Dictionary<string, List<Teams>> CreateSeeds(List<TeamStats> rankedTeams, Dictionary<string, List<Teams>> groups)
    {
        var seeds = new Dictionary<string, List<Teams>>
        {
            { "D", rankedTeams.Take(2).Select(t => groups.Values.SelectMany(g => g).FirstOrDefault(team => team.Team == t.Team)).Where(t => t != null).Cast<Teams>().ToList() },
            { "E", rankedTeams.Skip(2).Take(2).Select(t => groups.Values.SelectMany(g => g).FirstOrDefault(team => team.Team == t.Team)).Where(t => t != null).Cast<Teams>().ToList() },
            { "F", rankedTeams.Skip(4).Take(2).Select(t => groups.Values.SelectMany(g => g).FirstOrDefault(team => team.Team == t.Team)).Where(t => t != null).Cast<Teams>().ToList() },
            { "G", rankedTeams.Skip(6).Take(2).Select(t => groups.Values.SelectMany(g => g).FirstOrDefault(team => team.Team == t.Team)).Where(t => t != null).Cast<Teams>().ToList() }
        };

        return seeds;
    }

    public static List<(Teams, Teams)> DrawQuarterfinals(Dictionary<string, List<Teams>> seeds, Dictionary<string, List<MatchResult>> matchResults)
    {
        var random = new Random();
        List<(Teams, Teams)> quarterfinals;

        do
        {
            quarterfinals = new List<(Teams, Teams)>();

            var potD = seeds["D"].OrderBy(_ => random.Next()).ToList();
            var potE = seeds["E"].OrderBy(_ => random.Next()).ToList();
            var potF = seeds["F"].OrderBy(_ => random.Next()).ToList();
            var potG = seeds["G"].OrderBy(_ => random.Next()).ToList();

            // Pokušavamo da napravimo validan raspored
            bool isValid = true;

            foreach (var teamD in potD)
            {
                var opponent = potG.FirstOrDefault(t => !HasPlayedBefore(teamD, t, matchResults));
                if (opponent != null)
                {
                    quarterfinals.Add((teamD, opponent));
                    potG.Remove(opponent);
                }
                else
                {
                    isValid = false;
                    break;
                }
            }

            if (!isValid) continue; // Pokušaj ponovo ako postoji konflikt

            foreach (var teamE in potE)
            {
                var opponent = potF.FirstOrDefault(t => !HasPlayedBefore(teamE, t, matchResults));
                if (opponent != null)
                {
                    quarterfinals.Add((teamE, opponent));
                    potF.Remove(opponent);
                }
                else
                {
                    isValid = false;
                    break;
                }
            }

            // Ako i dalje imamo nevalidan raspored, pokušaj ponovo
            if (!isValid || potG.Count > 0 || potF.Count > 0)
            {
                continue;
            }

            break; // Pronašli smo validan raspored, izlazimo iz petlje

        } while (true);

        return quarterfinals;
    }
    private static bool HasPlayedBefore(Teams team1, Teams team2, Dictionary<string, List<MatchResult>> matchResults)
    {
        foreach (var groupMatches in matchResults.Values)
        {
            foreach (var match in groupMatches)
            {
                if ((match.Team1 == team1.Team && match.Team2 == team2.Team) || 
                    (match.Team1 == team2.Team && match.Team2 == team1.Team))
                {
                    return true;
                }
            }
        }
        return false;
    }
}