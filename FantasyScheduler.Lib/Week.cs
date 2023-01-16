namespace FantasyScheduler.Lib
{
	public class Week
	{
		private List<MatchUp> matchUps = new List<MatchUp>();

		public Week(uint num)
		{
			this.Number = num;
		}

        public uint Number { get; }

        public IReadOnlyList<MatchUp> MatchUps => this.matchUps;

		public void AddMatchUp(MatchUp matchUp)
        {
			this.matchUps.Add(matchUp);
        }

        public bool IsComplete(int totalTeams)
        {
            return this.matchUps.Count == (totalTeams / 2);
        }

        public bool TeamIsScheduled(Team team)
        {
            return this.matchUps.Any(x => x.Team1.Name.Equals(team.Name) || x.Team2.Name.Equals(team.Name));
        }

        public override string ToString()
        {
            string output = "Week #" + this.Number + Environment.NewLine;

            foreach (var matchUp in this.matchUps)
            {
                output += matchUp + Environment.NewLine;
            }

            return output;
        }
    }
}

