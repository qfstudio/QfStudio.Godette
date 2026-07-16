using Godot;
using QfStudio.Godotte.ReactiveUI;

namespace QfStudio.Godotte.IntegrationTests;

public partial class HomeScene : ReactiveControl
{
    public HomeScene()
    {
        this.WhenActivated(d =>
        {
            GD.Print("activated!");
        });
    }
}
