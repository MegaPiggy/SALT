using Enum = System.Enum;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using SALT;

public static class Levels
{
    public const string MAIN_MENU = "LevelSelect";
    private static Scene replacedScene;
    private static Scene newScene;
    public static Scene LastScene => replacedScene;
    public static Scene CurrentScene => newScene;

    static Levels()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private static void OnActiveSceneChanged(Scene replaced, Scene next)
    {
        replacedScene = replaced;
        newScene = next;
        Callbacks.OnSceneLoaded();
    }
    
    public static Level CurrentLevel => (Level)Enum.ToObject(typeof(Level), SceneManager.GetActiveScene().buildIndex);

    public static string LevelName => LevelLoader.levelName;

    public static bool isMainMenu() => Levels.IsLevel(MAIN_MENU) || Levels.IsLevel(Level.MAIN_MENU);

    public static bool isTitleScreen() => isMainMenu() && MainScript.title;

    public static bool IsLevel(string name) => SceneManager.GetActiveScene().name == name;

    public static bool IsLevel(int index) => SceneManager.GetActiveScene().buildIndex == index;

    public static bool IsLevel(Level level) => CurrentLevel == level;

    public static bool isOffice() => Levels.IsLevel(Level.OFFICE);//=> SceneManager.GetActiveScene().buildIndex == 1;
    public static bool isPopOnRocks() => Levels.IsLevel(Level.POP_ON_ROCKS);//=> SceneManager.GetActiveScene().buildIndex == 2;
    public static bool isRedHeart() => Levels.IsLevel(Level.RED_HEART);// => SceneManager.GetActiveScene().buildIndex == 3;
    public static bool isPekoland() => Levels.IsLevel(Level.PEKO_LAND);//SceneManager.GetActiveScene().buildIndex == 4;
    public static bool isOfficeReversed() => Levels.IsLevel(Level.OFFICE_REVERSED);//SceneManager.GetActiveScene().buildIndex == 5;
    public static bool isToTheMoon() => Levels.IsLevel(Level.TO_THE_MOON);//SceneManager.GetActiveScene().buildIndex == 6;
    public static bool isNothing() => Levels.IsLevel(Level.NOTHING);//SceneManager.GetActiveScene().buildIndex == 7;
    public static bool isMoguMogu() => Levels.IsLevel(Level.MOGU_MOGU);//SceneManager.GetActiveScene().buildIndex == 8;
    public static bool isInumore() => Levels.IsLevel(Level.INUMORE);//SceneManager.GetActiveScene().buildIndex == 9;
    public static bool isRushia() => Levels.IsLevel(Level.RUSHIA);//SceneManager.GetActiveScene().buildIndex == 10;
    public static bool isInascapableMadness() => Levels.IsLevel(Level.INASCAPABLE_MADNESS);//SceneManager.GetActiveScene().buildIndex == 11;
    public static bool isHereComesHope() => Levels.IsLevel(Level.HERE_COMES_HOPE);//SceneManager.GetActiveScene().buildIndex == 12;
    public static bool isReflect() => Levels.IsLevel(Level.REFLECT);//SceneManager.GetActiveScene().buildIndex == 13;
}
