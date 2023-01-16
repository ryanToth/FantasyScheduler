using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;

namespace FantasyScheduler.App;

public partial class OutputPage : UserControl
{
    public string Output { get; }

    public OutputPage()
    {
        throw new NotSupportedException();
    }

    public OutputPage(string output)
    {
        InitializeComponent();

        this.Output = output;
        this.OutputTextBlock.Text = this.Output;
    }
}