using System;
using System.Collections.Generic;
using Godot;

namespace Artifactor.App.Core
{
    public class Global
    {
        public static Global Instance => _instance ?? (_instance = new Global());
        private static Global _instance;

        private Viewport _root;
        
        public enum Scenes
        {
            Search,
            DataSync
        }

        private Scenes _startScene = Scenes.Search;
        private Dictionary<string, string> _scenePaths = new Dictionary<string, string>
        {
            {"Search", "SearchScene.tscn"},
            {"DataSync", "DataSyncScene.tscn"}
        };

        private string _sceneDirectory = "res://Data/Scenes";

        public void Setup()
        {
            _root = ((SceneTree) Engine.GetMainLoop()).GetRoot();
            GD.Print("Setup");
        }

        public static Viewport Root()
        {
            return Instance._root ?? throw new Exception("The root viewport hasn't been set");
        }

        public static App App()
        {
            return Root().GetNode<App>("App");
        }
        
        public static Node CurrentScene()
        {
            return Root().GetChild(Root().GetChildCount() - 1);
        }

        public void LoadStartScene()
        {
            ChangeScene(_startScene);
        }

        public void ChangeScene(Scenes scene)
        {
            App().SwitchScene(GetScenePath(scene));
        }

        private string GetScenePath(Scenes scene)
        {
            var path = _scenePaths[scene.ToString("G")];
            return $"{_sceneDirectory}/{path}";
        }
    }
}