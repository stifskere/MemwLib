using System.Text.RegularExpressions;

namespace MemwLib.Http.Types.Content.Views;

public readonly struct Location
{
    public required int Line { get; init; }
    public required int Character { get; init; }

    public static Location Find(Match match, string on)
    {
        if (on.Length < match.Index + match.Length)
            throw new ArgumentOutOfRangeException(nameof(match));
        
        int line = 1;
        int character = 0;

        for (int i = 0; i < match.Index; i++)
        {
            switch (on[i])
            {
                case '\n':
                    character = 0;
                    line++;
                    break;
                default:
                    character++;
                    break;
            }
        }

        return new Location { Line = line, Character = character };
    }
    
    public override string ToString() 
        => $"{Line}:{Character}";

    public Location Align(int at, string based)
    {
        int newLine = Line;
        int newChar = Character;

        if (based.StartsWith("${"))
        {
            based = based[2..^1];
            newChar += 2;
            at += 2;
        }

        for (int i = 0; i < based.Length && i < at; i++)
        {
            switch (based[i])
            {
                case '\n':
                    newChar = 0;
                    newLine++;
                    break;
                default:
                    newChar++;
                    break;
            }
        }
        
        return new Location { Line = newLine, Character = newChar };
    }
}