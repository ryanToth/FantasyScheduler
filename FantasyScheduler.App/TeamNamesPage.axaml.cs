using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace FantasyScheduler.App;

public partial class TeamNamesPage : UserControl
{
    public List<TeamNameItem> TeamNames { get; } = new List<TeamNameItem>();

    public TeamNamesPage()
    {
        throw new NotSupportedException();
    }

    public TeamNamesPage(int teamsCount)
    {
        InitializeComponent();

        for (int i = 0; i < teamsCount; i++)
        {
            if (i < 9 && teamsCount >= 10)
            {
                this.TeamNames.Add(new TeamNameItem($"Team #0{i + 1}: "));
            }
            else
            {
                this.TeamNames.Add(new TeamNameItem($"Team #{i + 1}: "));
            }
        }

        this.TeamNamesControl.Items = this.TeamNames;
    }

    public class TeamNameItem
    {
        public string TeamNameLabel { get; }

        public string TeamName { get; set; } = string.Empty;

        public TeamNameItem(string label)
        {
            this.TeamNameLabel = label;
        }
    }
}