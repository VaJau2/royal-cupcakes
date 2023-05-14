using Godot;

namespace RoyalCupcakes.System;

/**
 * Синглтон-класс настроек, сохращяющий их в config.cfg
 */
public class Settings
{
    private const string dir = "Settings";
    
    private readonly string[] langs = { "ru", "en" };
    
    private readonly ConfigFile config = new();
    
    public int CurrentLangNum
    {
        get => (int)config.GetValue(dir, "lang", "ru");
        set
        {
            config.SetValue(dir, "lang", value);
            TranslationServer.SetLocale(langs[value]);
        }
    }

    public string PlayerName
    {
        get => (string)config.GetValue(dir, "player_name", "Player");
        set => config.SetValue(dir, "player_name", value);
    }
    
    public string Host
    {
        get => (string)config.GetValue(dir, "host");
        set => config.SetValue(dir, "host", value);
    }
    
    public int Port
    {
        get => (int)config.GetValue(dir, "port");
        set => config.SetValue(dir, "port", value);
    }
    
    private static Settings instance;
    
    private Settings()
    {
        config.Load("res://config.cfg");
    }
    
    public static Settings Instance => instance ??= new Settings();

    public void SetNextLanguage()
    {
        var langNum = CurrentLangNum;
        CurrentLangNum = langNum < langs.Length - 1 ? langNum + 1 : 0;
    }

    public void Save()
    {
        config.Save("res://config.cfg");
    }
}
