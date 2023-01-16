using System;
using System.Diagnostics.CodeAnalysis;

namespace FantasyScheduler.Lib
{
	public class TeamMatchups
	{
		private Dictionary<string, List<uint>> matchups = new Dictionary<string, List<uint>>();

		public TeamMatchups(Team team, IReadOnlyList<Team> teams)
		{
			this.Team = team;

			foreach (var otherTeam in teams.Where(x => !x.Name.Equals(this.Team.Name)))
            {
				this.matchups.Add(otherTeam.Name, new List<uint>());
            }
		}

		public Team Team { get; }

		public bool TryGetMatchUp(uint week, [NotNullWhen(returnValue: true)] out MatchUp? matchUp)
        {
			foreach (var item in this.matchups)
            {
				if (item.Value.Any(x => x == week))
                {
					matchUp = new MatchUp(this.Team, new Team(item.Key));
					return true;
                }
            }

			matchUp = null;
			return false;
        }

		public bool CanScheduleMatchUp(Team team, uint week)
        {
			if (this.CanScheduleAgainstTeam(team, week))
            {
				return true;
			}

			return false;
        }

		public void ScheduleMatchUp(Team team, uint week)
		{
			this.matchups[team.Name].Add(week);
		}

		private bool CanScheduleAgainstTeam(Team team, uint week)
        {
			if (this.matchups.TryGetValue(team.Name, out List<uint>? meetings))
            {
				foreach (var matchup in this.matchups)
                {
					if (meetings.Count > matchup.Value.Count ||
						meetings.Any(x => x == (week - 1)))
                    {
						return false;
                    }
                }

				return true;
            }

			return false;
        }
	}
}

