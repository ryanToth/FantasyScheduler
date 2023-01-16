namespace FantasyScheduler.Lib.Tests;

public class SchedulerTests
{
    [Fact]
    public void CanMakeSchedule()
    {
        const uint Weeks = 11;

        const string TeamA = "a";
        const string TeamB = "b";
        const string TeamC = "c";
        const string TeamD = "d";
        const string TeamE = "e";
        const string TeamF = "f";

        List<Team> teams = new List<Team>()
        {
            new Team(TeamA),
            new Team(TeamB),
            new Team(TeamC),
            new Team(TeamD),
            new Team(TeamE),
            new Team(TeamF),
        };

        teams[0].Rival = teams[1];
        teams[1].Rival = teams[0];

        teams[2].Rival = teams[3];
        teams[3].Rival = teams[2];

        teams[4].Rival = teams[5];
        teams[5].Rival = teams[4];

        List<uint> rivalryWeeks = new List<uint>() { 3, 8, 11 };

        Scheduler scheduler = new Scheduler(teams, Weeks, rivalryWeeks);

        Assert.Equal(Weeks, (uint)scheduler.Schedule.Count);

        uint aGames = 0;
        uint bGames = 0;
        uint cGames = 0;
        uint dGames = 0;
        uint eGames = 0;
        uint fGames = 0;

        for (int i = 0; i < scheduler.Schedule.Count; i++)
        {
            Week week = scheduler.Schedule[i];

            Assert.Equal(i + 1, (int)week.Number);
            Assert.Equal(3, week.MatchUps.Count);

            if (rivalryWeeks.Any(x => x == (i + 1)))
            {
                Assert.Equal(week.MatchUps[0].Team1.Name, TeamA);
                Assert.Equal(week.MatchUps[0].Team2.Name, TeamB);

                Assert.Equal(week.MatchUps[1].Team1.Name, TeamC);
                Assert.Equal(week.MatchUps[1].Team2.Name, TeamD);

                Assert.Equal(week.MatchUps[2].Team1.Name, TeamE);
                Assert.Equal(week.MatchUps[2].Team2.Name, TeamF);
            }

            for (int j = 0; j < week.MatchUps.Count; j++)
            {
                MatchUp matchUp1 = week.MatchUps[j];

                Assert.NotEqual(matchUp1.Team1, matchUp1.Team2);

                for (int k = 0; k < week.MatchUps.Count; k++)
                {
                    if (j == k)
                    {
                        continue;
                    }

                    MatchUp matchUp2 = week.MatchUps[k];

                    Assert.NotEqual(matchUp1.Team1, matchUp2.Team1);
                    Assert.NotEqual(matchUp1.Team1, matchUp2.Team2);
                    Assert.NotEqual(matchUp1.Team2, matchUp2.Team2);
                }

                if (matchUp1.Team1.Name == TeamA || matchUp1.Team2.Name == TeamA)
                {
                    aGames++;
                }

                if (matchUp1.Team1.Name == TeamB || matchUp1.Team2.Name == TeamB)
                {
                    bGames++;
                }

                if (matchUp1.Team1.Name == TeamC || matchUp1.Team2.Name == TeamC)
                {
                    cGames++;
                }

                if (matchUp1.Team1.Name == TeamD || matchUp1.Team2.Name == TeamD)
                {
                    dGames++;
                }

                if (matchUp1.Team1.Name == TeamE || matchUp1.Team2.Name == TeamE)
                {
                    eGames++;
                }

                if (matchUp1.Team1.Name == TeamF || matchUp1.Team2.Name == TeamF)
                {
                    fGames++;
                }
            }
        }

        Assert.Equal(Weeks, aGames);
        Assert.Equal(Weeks, bGames);
        Assert.Equal(Weeks, cGames);
        Assert.Equal(Weeks, dGames);
        Assert.Equal(Weeks, eGames);
        Assert.Equal(Weeks, fGames);
    }
}
