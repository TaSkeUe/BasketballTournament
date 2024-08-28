public class Teams
{
    public string Team { get; set; } = string.Empty;
    public string ISOCode { get; set; } = string.Empty;
    public int FIBARanking { get; set; }
}

public class GroupsData
{
    public required Dictionary<string, List<Teams>> Groups { get; set; }
}

public class MatchResult
{
    public required string Team1 { get; set; }
    public required string Team2 { get; set; }
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
}

public class TeamStats
{
    public required string Team { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Points { get; set; }
    public int PointsFor { get; set; }
    public int PointsAgainst { get; set; }
    public int PointDifference { get; set; }
    public int Rank { get; set; }
}

/*public class Seed
{
    public string Name { get; set; } = string.Empty;
    public List<Teams> Teams { get; set; } = new List<Teams>();
}*/