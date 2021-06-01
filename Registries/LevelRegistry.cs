using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SALT.Registries
{
    public static class LevelRegistry
    {
        public static Scene CreateLevel(string name, CreateSceneParameters parameters) => SceneManager.CreateScene(name, parameters);
    }
}
