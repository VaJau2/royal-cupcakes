using Godot;

namespace RoyalCupcakes.System;

/**
 * Синглтон-класс настроек, сохращяющий их в config.cfg
 */
public class Settings
{
    private enum Bus
    {
        Audio = 1,
        Music = 2
    };
    
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

    public float SoundVolume
    {
        get => (float)config.GetValue(dir, "sound", AudioServer.GetBusVolumeDb((int)Bus.Audio));
        set
        {
            config.SetValue(dir, "sound", value);
            AudioServer.SetBusVolumeDb((int)Bus.Audio, value);
        }
    }
    
    public float MusicVolume
    {
        get => (float)config.GetValue(dir, "music", AudioServer.GetBusVolumeDb((int)Bus.Music));
        set
        {
            config.SetValue(dir, "music", value);
            AudioServer.SetBusVolumeDb((int)Bus.Music, value);
        }
    }

    public string PlayerName
    {
        get => (string)config.GetValue(dir, "player_name", "Player");
        set => config.SetValue(dir, "player_name", value);
    }
    
    public string Host
    {
        get => (string)config.GetValue(dir, "host", "localhost");
        set => config.SetValue(dir, "host", value);
    }
    
    public int Port
    {
        get => (int)config.GetValue(dir, "port", 8080);
        set => config.SetValue(dir, "port", value);
    }
    
    public int CakesCount
    {
        get => (int)config.GetValue(dir, "cakes_count", 3);
        set => config.SetValue(dir, "cakes_count", value);
    }
    
    public int NpcCount
    {
        get => (int)config.GetValue(dir, "npc_count", 10);
        set => config.SetValue(dir, "npc_count", value);
    }
    
    public double MainTimer
    {
        get => (double)config.GetValue(dir, "main_timer", 120f);
        set => config.SetValue(dir, "main_timer", value);
    }

    public double AppendMainTime
    {
        get => (double)config.GetValue(dir, "append_main_time", 20f);
        set => config.SetValue(dir, "append_main_time", value);
    }

    public int CaughtNpcCount
    {
        get => (int)config.GetValue(dir, "caught_npc_count", 3);
        set => config.SetValue(dir, "caught_npc_count", value);
    }
    
    public bool MinimapOn
    {
        get => (bool)config.GetValue(dir, "minimap_on", false);
        set => config.SetValue(dir, "minimap_on", value);
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

    public void LoadLanguage()
    {
        TranslationServer.SetLocale(langs[CurrentLangNum]);
    }

    public void Save()
    {
        config.Save("res://config.cfg");
    }
}
