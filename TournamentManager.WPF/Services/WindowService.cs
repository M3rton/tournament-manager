﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Windows;
using TournamentManager.Core.Interfaces.Navigation;

namespace TournamentManager.WPF.Services;

internal class WindowService : IWindowService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, Type> _viewModelToViewMap;
    private readonly Dictionary<object, Window> _openWindows = new Dictionary<object, Window>();

    public WindowService(IServiceProvider serviceProvider, Dictionary<Type, Type> viewModelToViewMap)
    {
        _serviceProvider = serviceProvider;
        _viewModelToViewMap = viewModelToViewMap;
    }

    public void ShowWindow<TViewModel>(Action<TViewModel> setup, object? owner = null) where TViewModel : class
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

        setup(viewModel);

        if (_viewModelToViewMap.ContainsKey(viewModel.GetType()))
        {
            Type viewType = _viewModelToViewMap[viewModel.GetType()];
            if (viewType == null)
            {
                return;
            }

            var window = Activator.CreateInstance(viewType) as Window;
            if (window == null)
            {
                return;
            } 

            window.DataContext = viewModel;
            _openWindows[viewModel] = window;
            window.Show();

            if (owner != null)
            {
                window.Owner = _openWindows[owner];
            }
        }
    }

    public Task<string?> ShowSaveFileDialog(string? title = null, string? filter = null, string? defaultFileName = null) => Task.Run(() =>
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = title,
            Filter = filter,
            FileName = defaultFileName
        };

        bool? result = saveFileDialog.ShowDialog();
        return result == true ? saveFileDialog.FileName : null;
    });

    public void CloseWindow<TViewModel>(TViewModel viewModel) where TViewModel : class
    {
        if (_openWindows.ContainsKey(viewModel))
        {
            var window = _openWindows[viewModel];

            window.Close();
            _openWindows.Remove(viewModel);
        }
    }
}
