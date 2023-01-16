using System;
namespace FantasyScheduler.Lib
{
	public class Scheduler
	{
		private const int MAX_RETRIES = 10;

		private uint numberOfWeeks;
		private IReadOnlyList<Team> teams;
		private IReadOnlyList<uint> rivalryWeeks;

		private IReadOnlyDictionary<string, TeamMatchups> matchupsCache = new Dictionary<string, TeamMatchups>();

		public Scheduler(IReadOnlyList<Team> teams, uint numberOfWeeks, IReadOnlyList<uint> rivalryWeeks)
		{
			this.teams = teams;
			this.numberOfWeeks = numberOfWeeks;
			this.rivalryWeeks = rivalryWeeks;

			if (numberOfWeeks < rivalryWeeks.Count)
            {
				throw new ArgumentException();
            }

			this.Schedule = this.CreateSchedule();
		}

		public IReadOnlyList<Week> Schedule { get; }

		private IReadOnlyList<Week> CreateSchedule()
        {
			int totalRetries = 0;

			while (true)
			{
				try
				{
					var cache = new Dictionary<string, TeamMatchups>();
					foreach (var team in teams)
					{
						cache.Add(team.Name, new TeamMatchups(team, this.teams));
					}

					this.matchupsCache = cache;

					SortedDictionary<uint, Week> weeks = this.ScheduleRivalryWeeks();

					int retries = 0;
					uint currentWeek = 1;

					while (currentWeek <= this.numberOfWeeks)
					{
						if (weeks.ContainsKey(currentWeek))
						{
							currentWeek++;
							continue;
						}

						Week week = new Week(currentWeek);
						foreach (var team1 in this.teams)
						{
							if (week.TeamIsScheduled(team1))
							{
								continue;
							}

							var shuffledTeams = new List<Team>(this.teams.Where(x => !x.Name.Equals(team1.Name)));
							shuffledTeams.Shuffle();

							foreach (var team2 in shuffledTeams)
							{
								if (!week.TeamIsScheduled(team2))
								{
									MatchUp matchUp = new MatchUp(team1, team2);
									if (this.CanScheduleMatchUp(matchUp, currentWeek))
									{
										week.AddMatchUp(matchUp);
										break;
									}
								}
							}
						}

						if (week.IsComplete(this.teams.Count))
						{
							weeks.Add(currentWeek++, week);

							foreach (var matchUp in week.MatchUps)
							{
								this.AddMatchUpToCache(matchUp, week.Number);
							}

							retries = 0;
						}
						else if (retries >= MAX_RETRIES)
						{
							throw new TaskCanceledException($"Failed to create a valid schedule after {MAX_RETRIES} retries.");
						}
						else
						{
							retries++;
						}
					}

					return weeks.Values.ToList();
				}
				catch (TaskCanceledException)
                {
					if (totalRetries >= MAX_RETRIES)
                    {
						throw;
                    }

					if (totalRetries == 0)
                    {
						Console.WriteLine(string.Empty);
                    }

					Console.WriteLine($"Attempt {totalRetries + 1}. Failed to create a valid schedule. Retrying...");
					totalRetries++;
                }
			}
        }

		private SortedDictionary<uint, Week> ScheduleRivalryWeeks()
        {
			SortedDictionary<uint, Week> weeks = new SortedDictionary<uint, Week>();

			foreach (var weekNum in this.rivalryWeeks)
			{
				Week week = new Week(weekNum);

				foreach (var team in this.teams)
				{
					if (!week.TeamIsScheduled(team))
					{
						if (team.Rival is null)
						{
							throw new InvalidOperationException();
						}

						MatchUp matchUp = new MatchUp(team, team.Rival);

						this.AddMatchUpToCache(matchUp, weekNum);
						week.AddMatchUp(matchUp);
					}
				}

				weeks.Add(weekNum, week);
			}

			return weeks;
		}

		private void AddMatchUpToCache(MatchUp matchUp, uint week)
        {
			if (this.matchupsCache.TryGetValue(matchUp.Team1.Name, out TeamMatchups? item1))
            {
				item1.ScheduleMatchUp(matchUp.Team2, week);
            }

			if (this.matchupsCache.TryGetValue(matchUp.Team2.Name, out TeamMatchups? item2))
			{
				item2.ScheduleMatchUp(matchUp.Team1, week);
			}
		}

		private bool CanScheduleMatchUp(MatchUp matchUp, uint week)
        {
			return this.matchupsCache.TryGetValue(matchUp.Team1.Name, out TeamMatchups? teamMatchups1)
			   && teamMatchups1.CanScheduleMatchUp(matchUp.Team2, week)
			   && this.matchupsCache.TryGetValue(matchUp.Team2.Name, out TeamMatchups? teamMatchups2)
			   && teamMatchups2.CanScheduleMatchUp(matchUp.Team1, week);
        }

		// This method works but its pretty unnecessary
		private IReadOnlyList<Week> GenerateScheduleFromCache()
        {
			List<Week> weeks = new List<Week>();

			while (weeks.Count < this.numberOfWeeks)
            {
				Week week = new Week((uint)weeks.Count + 1);

				foreach (var team in this.matchupsCache)
                {
					if (team.Value.TryGetMatchUp(week.Number, out MatchUp? matchUp))
                    {
						if (!week.TeamIsScheduled(matchUp!.Team1) && !week.TeamIsScheduled(matchUp!.Team2))
                        {
							week.AddMatchUp(matchUp!);
						}
                    }
                }

				weeks.Add(week);
            }

			return weeks;
        }
	}
}

