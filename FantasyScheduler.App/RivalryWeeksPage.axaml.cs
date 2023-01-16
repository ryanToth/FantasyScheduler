using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace FantasyScheduler.App;

public partial class RivalryWeeksPage : UserControl
{
    public List<RivalryWeek> Weeks { get; } = new List<RivalryWeek>();

    public RivalryWeeksPage()
    {
        throw new NotSupportedException();
    }

    public RivalryWeeksPage(int weeks, int teams)
    {
        InitializeComponent();

        int rivalryWeeks = (int)Math.Ceiling((double)weeks / teams);
        for (int i = 0; i < rivalryWeeks; i++)
        {
            this.Weeks.Add(new RivalryWeek($"Rivalry Weel #{i + 1}: "));
        }

        this.RivalryWeeksControl.Items = this.Weeks;
    }

    public class RivalryWeek
    {
        public string WeekLabel { get; }

        public string Week { get; set; } = string.Empty;

        public RivalryWeek(string label)
        {
            this.WeekLabel = label;
        }
    }
}