using Artifactor.App.Core;
using Artifactor.App.UI;
using Godot;

namespace Artifactor.App.Scripts
{
    public class Menu : Control
    {
        private Label _backLabel;
        private Label _syncLabel;

        public LabelHover BackLabelHover;
        public LabelHover SyncLabelHover;

        public override void _Ready()
        {
            _backLabel = GetNode<Label>("MenuBar/Back");
            _syncLabel = GetNode<Label>("MenuBar/Sync");
            
            _backLabel.MouseFilter = 0;
            _syncLabel.MouseFilter = 0;
            
            BackLabelHover = new LabelHover(_backLabel, "#6b6b6b", "#FFFFFF");
            SyncLabelHover = new LabelHover(_syncLabel, "#6b6b6b", "#FFFFFF");
        }

        public override void _Input(InputEvent @event)
        {
            if(@event is InputEventMouseButton input && input.IsPressed() && input.ButtonIndex == (int) ButtonList.Left)
            {
                HandleLabelClick();
            }
        }

        public void HideBackButton()
        {
            _backLabel.Hide();
        }

        public void ShowBackButton()
        {
            _backLabel.Show();
        }

        private void HandleLabelClick()
        {
            // go back to search scene
            if(BackLabelHover.IsHovering && _backLabel.Visible && !BackLabelHover.ClickDisabled)
            {
                Global.Instance.ChangeScene(Global.Scenes.Search);
            }

            // go to sync scene
            if(SyncLabelHover.IsHovering && !SyncLabelHover.ClickDisabled)
            {
                Global.Instance.ChangeScene(Global.Scenes.DataSync);
            }
        }
    }
}