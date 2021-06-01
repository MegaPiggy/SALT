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

    public static bool isTitleScreen() => isMainMenu() && MainScript.title;
    
    public static bool IsLevel(string name) => SceneManager.GetActiveScene().name == name || SceneManager.GetActiveScene().buildIndex == 0;

    public static bool isOffice() => SceneManager.GetActiveScene().buildIndex == 1;
    public static bool isPopOnRocks() => SceneManager.GetActiveScene().buildIndex == 2;
    public static bool isRedHeart() => SceneManager.GetActiveScene().buildIndex == 3;
    public static bool isPekoland() => SceneManager.GetActiveScene().buildIndex == 4;
    public static bool isOfficeReversed() => SceneManager.GetActiveScene().buildIndex == 5;
    public static bool isToTheMoon() => SceneManager.GetActiveScene().buildIndex == 6;
    public static bool isNothing() => SceneManager.GetActiveScene().buildIndex == 7;
    public static bool isMoguMogu() => SceneManager.GetActiveScene().buildIndex == 8;
    public static bool isInumore() => SceneManager.GetActiveScene().buildIndex == 9;
}