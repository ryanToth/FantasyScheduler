namespace FantasyScheduler.Lib;

public record Team
{
    public Team(string name)
    {
        this.Name = name;
    }

    public Team(string name, string rival)
    {
        this.Name = name;
        this.Rival = new Team(rival);
    }

    public string Name { get; }

    public Team? Rival { get; set; }

    public override int GetHashCode()
    {
        return this.Name.GetHashCode();
    }

    public override string ToString()
    {
        return this.Name;
    }
}

