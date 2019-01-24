using Godot;

namespace Artifactor.App.Scripts
{
    public class MessageBoxScene : Control
    {
        [Signal]
        public delegate void WindowClosed();

        private Button _okButton;

        public override void _Ready()
        {
            _okButton = GetNode<Button>("Panel/OkButton");
            _okButton.Connect("pressed", this, nameof(CloseWindow));
        }

        public void CloseWindow()
        {
            EmitSignal(nameof(WindowClosed));
        }
    }
}