﻿using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.JobApplication.ViewModels;
using Avalonia.JobApplication.Views;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;

//[assembly: XmlnsDefinition("https://github.com/avaloniaui", "I3Jiad.AvaloniaUI.Controls")]

namespace Avalonia.JobApplication;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
