using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;

namespace FantasyScheduler.App;

public partial class RivalsPage : UserControl
{
    public IReadOnlyCollection<TeamViewModel> Teams { get; }

    public RivalsPage()
    {
        throw new NotSupportedException();
    }

    public RivalsPage(IReadOnlyCollection<string> teams)
    {
        InitializeComponent();

        List<TeamViewModel> viewModels = new List<TeamViewModel>();

        foreach (var team in teams)
        {
            List<string> potentialRivals = new List<string>() { string.Empty };
            potentialRivals.AddRange(teams.Where(x => !x.Equals(team)));


            var viewModel = new TeamViewModel(
                team,
                potentialRivals,
                (team, rival) => this.OnRivalSelected(team, rival),
                (team) => this.ResetRival(team));

            viewModels.Add(viewModel);
        }

        this.Teams = viewModels;
        this.RivalsControl.Items = this.Teams;
    }

    private void OnRivalSelected(string team, string rival)
    {
        var viewModel = this.Teams.FirstOrDefault(x => x.Name.Equals(team));
        if (viewModel is not null)
        {
            var existingRival = this.Teams.FirstOrDefault(x => x.Rival.Equals(team));
            if (existingRival is not null)
            {
                existingRival.SetRival(string.Empty);
            }

            viewModel.SetRival(rival);
        }
    }

    private void ResetRival(string team)
    {
        var viewModel = this.Teams.FirstOrDefault(x => x.Name.Equals(team));
        if (viewModel is not null)
        {
            viewModel.SetRival(string.Empty);
        }
    }

    public class TeamViewModel : INotifyPropertyChanged
    {
        private string rival = string.Empty;
        private Action<string, string> onRivalSelected;
        private Action<string> resetRival;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name { get; }

        public IReadOnlyCollection<string> PotentialRivals { get; }

        public string Rival
        {
            get
            {
                return this.rival;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!string.IsNullOrEmpty(this.Rival))
                    {
                        this.resetRival(this.Rival);
                    }

                    this.onRivalSelected(value, this.Name);
                }
                else
                {
                    this.resetRival(this.rival);
                }

                this.rival = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Rival)));
            }
        }

        public TeamViewModel(
            string name,
            IReadOnlyCollection<string> potentialRivals,
            Action<string, string> onRivalSelected,
            Action<string> resetRival)
        {
            this.Name = name;
            this.PotentialRivals = potentialRivals;
            this.onRivalSelected = onRivalSelected;
            this.resetRival = resetRival;
        }

        public void SetRival(string rival)
        {
            this.rival = rival;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Rival)));
        }
    }
}