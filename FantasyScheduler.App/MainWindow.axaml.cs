using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FantasyScheduler.Lib;

namespace FantasyScheduler.App;

public partial class MainWindow : Window
{
    private State currentState;

    private WeeksAndTeamsPage? weeksAndTeamsPage;
    private RivalryWeeksPage? rivalryWeeksPage;
    private TeamNamesPage? teamNamesPage;
    private RivalsPage? rivalsPage;
    private OutputPage? outputPage;

    private int weeksCount = 0;
    private int teamsCount = 0;
    private List<uint> rivalryWeeks = new List<uint>();

    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = this;
        this.Reset();
    }

    private void Reset()
    {
        this.currentState = State.WeeksAndTeamsNumber;
        this.Page.Content = this.weeksAndTeamsPage = new WeeksAndTeamsPage();

        this.teamNamesPage = null;

        this.NextButton.IsVisible = true;
        this.BackButton.IsVisible = false;
        this.RestartButton.IsVisible = false;
    }

    private bool Navigate(Direction direction)
    {
        switch (this.currentState)
        {
            case State.WeeksAndTeamsNumber:
                {
                    WeeksAndTeamsPage page = this.weeksAndTeamsPage!;

                    if (int.TryParse(page.TeamsTextBox.Text, out this.teamsCount) &&
                        int.TryParse(page.WeeksTextBox.Text, out this.weeksCount) &&
                        (teamsCount % 2) == 0)
                    {
                        this.Page.Content = this.rivalryWeeksPage = new RivalryWeeksPage(this.weeksCount, this.teamsCount);
                        this.currentState = State.RivalryWeeks;

                        this.BackButton.IsVisible = true;
                        return true;
                    }

                    break;
                }
            case State.RivalryWeeks:
                {
                    if (direction == Direction.Next)
                    {
                        this.rivalryWeeks.Clear();

                        RivalryWeeksPage page = this.rivalryWeeksPage!;
                        foreach (var week in page.Weeks)
                        {
                            if (string.IsNullOrEmpty(week.Week) ||
                                !int.TryParse(week.Week, out int rivalryWeek) ||
                                rivalryWeek < 1 ||
                                rivalryWeek > this.weeksCount ||
                                this.rivalryWeeks.Contains((uint)rivalryWeek))
                            {
                                return false;
                            }

                            this.rivalryWeeks.Add((uint)rivalryWeek);
                        }

                        this.currentState = State.Teams;
                        this.Page.Content = this.teamNamesPage = new TeamNamesPage(this.teamsCount);

                        this.BackButton.IsVisible = true;
                        return true;
                    }
                    else
                    {
                        this.currentState = State.WeeksAndTeamsNumber;
                        this.Page.Content = this.weeksAndTeamsPage;

                        this.BackButton.IsVisible = false;

                        return true;
                    }
                }
            case State.Teams:
                {
                    if (direction == Direction.Next)
                    {
                        TeamNamesPage page = this.teamNamesPage!;
                        List<string> teams = new List<string>();

                        foreach (var team in page.TeamNames)
                        {
                            if (string.IsNullOrEmpty(team.TeamName) || teams.Contains(team.TeamName))
                            {
                                return false;
                            }

                            teams.Add(team.TeamName);
                        }

                        this.currentState = State.Rivals;
                        this.Page.Content = this.rivalsPage = new RivalsPage(teams);

                        return true;
                    }
                    else
                    {
                        this.Page.Content = this.rivalryWeeksPage;
                        this.currentState = State.RivalryWeeks;
                        return true;
                    }
                }
            case State.Rivals:
                {
                    if (direction == Direction.Next)
                    {
                        RivalsPage page = this.rivalsPage!;
                        foreach (var team in page.Teams)
                        {
                            if (string.IsNullOrEmpty(team.Rival))
                            {
                                return false;
                            }
                        }

                        Scheduler scheduler = new Scheduler(
                            page.Teams.Select(x => new Team(x.Name, x.Rival)).ToList(),
                            (uint)this.weeksCount,
                            this.rivalryWeeks);

                        var schedule = scheduler.Schedule;

                        string output = string.Empty;
                        foreach (var week in schedule)
                        {
                            output += week;
                            output += Environment.NewLine;
                        }

                        this.Page.Content = this.outputPage = new OutputPage(output);
                        this.currentState = State.Output;
                        this.NextButton.IsVisible = false;
                        this.RestartButton.IsVisible = true;

                        return true;
                    }
                    else
                    {
                        this.Page.Content = this.teamNamesPage;
                        this.currentState = State.Teams;

                        return true;
                    }
                }
            case State.Output:
                {
                    this.Page.Content = this.rivalsPage;
                    this.currentState = State.Rivals;
                    this.NextButton.IsVisible = true;
                    this.RestartButton.IsVisible = false;
                    return true;
                }
        }

        return false;
    }

    private void OnNextButtonClick(object sender, RoutedEventArgs e)
    {
        if (this.Navigate(Direction.Next))
        {
            if (!this.BackButton.IsVisible)
            {
                this.BackButton.IsVisible = true;
            }
        }
    }

    private void OnBackButtonClick(object sender, RoutedEventArgs e)
    {
        if (this.Navigate(Direction.Back))
        {
            if (!this.NextButton.IsVisible)
            {
                this.NextButton.IsVisible = true;
            }
        }
    }

    private void OnRestartButtonClick(object sender, RoutedEventArgs e)
    {
        this.Reset();
    }
}