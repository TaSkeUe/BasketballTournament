using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Program
{
    public static void Main()
    {
        string jsonFilePath = "groups.json";
        var jsonData = File.ReadAllText(jsonFilePath);
        var groupsData = JsonConvert.DeserializeObject<Dictionary<string, List<Teams>>>(jsonData);

        // Provera učitanih podataka
        if (groupsData != null)
        {
            //DataPrinter.PrintGroupData(groupsData);

            var matchResults = MatchSimulator.SimulateGroupMatches(groupsData);

            // Ispis svih rezultata utakmica
            foreach (var group in matchResults)
            {
                Console.WriteLine($"Grupa {group.Key} - Rezultati utakmica:");
                foreach (var match in group.Value)
                {
                    Console.WriteLine($"  {match.Team1} - {match.Team2} ({match.Team1Score}:{match.Team2Score})");
                }
                Console.WriteLine();
            }

            // Rangiranje timova u grupama
            var rankedTeams = MatchSimulator.RankTeams(matchResults, groupsData);

            DataPrinter.PrintRankings(groupsData, rankedTeams);

            // Formiranje šešira i žreb
            var seeds = TournamentDraw.CreateSeeds(rankedTeams, groupsData);
            var quarterfinals = TournamentDraw.DrawQuarterfinals(seeds, matchResults);

            // Ispis šešira i žreba
            DrawPrinter.PrintSeeds(seeds);
            DrawPrinter.PrintQuarterfinals(quarterfinals);

            //Formiranje i ispis eliminacione faze 
            MatchSimulator.SimulateEliminationPhase(quarterfinals);
        }
        else
        {
            Console.WriteLine("Podaci iz JSON fajla nisu uspešno učitani.");
        }
    }
}