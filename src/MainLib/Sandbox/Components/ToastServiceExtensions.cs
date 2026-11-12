using Microsoft.FluentUI.AspNetCore.Components;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Sandbox.Components
{
    public static class ToastServiceExtensions
    {
        private static readonly Icon iconError = new Icons.Regular.Size20.ErrorCircle();
        private static readonly Icon iconInfo = new Icons.Regular.Size20.Info();
        private static readonly Icon iconWarning = new Icons.Regular.Size20.Warning();

        extension(INotificationService? toastService)
        {
            public async Task ShowError(string title) => await toastService.ShowMessage(title, iconError);

            public async Task ShowInfo(string title) => await toastService.ShowMessage(title, iconInfo);
            
            public async Task ShowWarning(string title) => await toastService.ShowMessage(title, iconWarning);

            public async Task ShowMessage(string title, Icon? icon)
            {
                if (toastService == null)
                    return;

                var options = new ToastOptions
                {
                    Title = title,
                    Icon = icon,
                    Lifetime = TimeSpan.FromSeconds(5),
                };

                _ = await toastService.ShowToastAsync(options);
            }
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member