using Godot;

namespace Artifactor.App.Core
{
    public class App : Node
    {
        private Node _scene;
        
        public override void _Ready()
        {
            // this singleton lets us access the root from everywhere in our application
            Global.Instance.Setup();
 
            // grab the current scene that's loaded
            _scene = Global.CurrentScene();
        }
 
        public void SwitchScene(string path)
        {
            CallDeferred(nameof(SwitchCurrentScene), path);
        }

        public void QuitApp()
        {
            CallDeferred(nameof(Quit));
        }

        public void Quit()
        {
            GetTree().Quit(); // default behavior
        }
 
        private void SwitchCurrentScene(string path)
        {
            _scene.Free();
            _scene = ((PackedScene) GD.Load(path)).Instance();
             
            Global.Root().AddChild(_scene);
            GetTree().SetCurrentScene(_scene);
        }
    }
}