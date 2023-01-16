using System;
namespace FantasyScheduler.Lib
{
	public class MatchUp
	{
		public MatchUp(Team t1, Team t2)
		{
			this.Team1 = t1;
			this.Team2 = t2;
		}

        public Team Team1 { get; }
        public Team Team2 { get; }

        public override bool Equals(object? obj)
        {
            return obj is not null &&
                obj is MatchUp matchUp &&
                this.GetHashCode() == matchUp.GetHashCode();

        }

        public override int GetHashCode()
        {
            return this.Team1.GetHashCode() +
                this.Team2.GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.Team1} vs {this.Team2}";
        }
    }
}

