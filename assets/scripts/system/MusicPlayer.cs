using Godot;
using Godot.Collections;

public partial class MusicPlayer : AudioStreamPlayer
{
    [Export] private Array<AudioStream> music = new();
    private int musicI;

    public override void _Ready()
    {
        music.Shuffle();
        PlayNext();
        Finished += PlayNext;
    }

    private void PlayNext()
    {
        if (musicI < music.Count - 1)
        {
            Stream = music[musicI];
            Play();
            musicI++;
        }
        else
        {
            musicI = 0;
            music.Shuffle();
        }
    }
}
