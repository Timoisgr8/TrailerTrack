namespace TrailerTrack.Application.Common;

public static class EnumExtensions
{
    public static string ToLabel(this Enum value)
    => string.Concat(value.ToString().Select(c => char.IsUpper(c) ? " " + c : c.ToString())).Trim();
}