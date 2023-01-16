using FantasyScheduler.Lib;

Console.Write("How many weeks are there: ");
uint weeksCount = uint.Parse(Console.ReadLine() ?? string.Empty);

Console.Write("How many teams are there: ");

uint teamCount = uint.Parse(Console.ReadLine() ?? string.Empty);

if (teamCount % 2 != 0)
{
    throw new InvalidOperationException("Number of teams must be even.");
}

List<Team> teams = new List<Team>();

while (teams.Count < teamCount)
{
    Console.Write($"Enter Team #{teams.Count + 1}: ");
    string teamName = Console.ReadLine() ?? string.Empty;

    if (string.IsNullOrEmpty(teamName) || teams.Any(x => x.Name.Equals(teamName)))
    {
        throw new InvalidOperationException("Invalid team name.");
    }

    teams.Add(new Team(teamName));
}

foreach (var team in teams)
{
    if (team.Rival is not null)
    {
        continue;
    }

    Console.Write($"Enter the rival for {team.Name}: ");
    string rivalName = Console.ReadLine() ?? string.Empty;

    Team? rival = teams.FirstOrDefault(x => x.Name.Equals(rivalName));

    if (string.IsNullOrEmpty(rivalName) || rival is null || team.Name.Equals(rivalName))
    {
        throw new InvalidOperationException("Invalid rival name.");
    }

    team.Rival = rival;
    rival.Rival = team;
}

uint rivalryWeeksCount = (uint)Math.Ceiling((double)weeksCount / (teamCount - 1));
Console.WriteLine($"There will be {rivalryWeeksCount} rivalry week(s)");

List<uint> rivalryWeeks = new List<uint>();
while (rivalryWeeks.Count < rivalryWeeksCount)
{
    Console.Write($"Enter rivalry week #{rivalryWeeks.Count + 1}: ");
    uint rivalryWeek = uint.Parse(Console.ReadLine() ?? string.Empty);

    if (rivalryWeeks.Any(x => x == rivalryWeek))
    {
        throw new InvalidOperationException("Week already set.");
    }

    rivalryWeeks.Add(rivalryWeek);
}

var scheduler = new Scheduler(teams, weeksCount, rivalryWeeks);

Console.WriteLine(Environment.NewLine);

foreach (var week in scheduler.Schedule)
{
    Console.WriteLine(week);
}