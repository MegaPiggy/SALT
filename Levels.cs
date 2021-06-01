using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class Levels
{
    public const string MAIN_MENU = "LevelSelect";
    private static Scene replacedScene;
    private static Scene newScene;
    public static Scene LastScene => replacedScene;
    public static Scene CurrentScene => newScene;

    static Levels()
    {
        SceneManager.activeSceneChanged += new UnityAction<Scene, Scene>(Levels.OnActiveSceneChanged);
    }

    private static void OnActiveSceneChanged(Scene replaced, Scene next)
    {
        replacedScene = replaced;
        newScene = next;
        SALT.Callbacks.OnSceneLoaded();
    }

    public static string LevelName => LevelLoader.levelName;

    public static bool isMainMenu() => Levels.IsLevel(MAIN_MENU);
    
    public static bool IsLevel(string name) => SceneManager.GetActiveScene().name == name || SceneManager.GetActiveScene().buildIndex == 0;

    public static bool isOffice() => SceneManager.GetActiveScene().buildIndex == 1;
    public static bool isPopOnRocks() => SceneManager.GetActiveScene().buildIndex == 2;
}