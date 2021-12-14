
using Avalonia.Web.Blazor;

namespace zombieturkey.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<zombieturkey.App>()
            .SetupWithSingleViewLifetime();
    }
}