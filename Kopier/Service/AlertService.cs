﻿namespace Kopier.Service;

public interface IAlertService
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");
    Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No");

    void ShowAlert(string title, string message, string cancel = "OK");
    void ShowConfirmation(string title, string message, Action<bool> callback, string accept = "Yes", string cancel = "No");
}

internal class AlertService : IAlertService
{
    public Task ShowAlertAsync(string title, string message, string cancel = "OK")
        => Application.Current.MainPage.DisplayAlert(title, message, cancel);

    public Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No")
        => Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);


    public void ShowAlert(string title, string message, string cancel = "OK")
        => Application.Current.MainPage.Dispatcher.Dispatch(async () => await ShowAlertAsync(title, message, cancel));

    public void ShowConfirmation(string title, string message, Action<bool> callback, string accept = "Yes", string cancel = "No")
        => Application.Current.MainPage.Dispatcher.Dispatch(async () => callback(await ShowConfirmationAsync(title, message, accept, cancel)));
}