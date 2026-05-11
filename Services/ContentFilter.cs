public static class ContentFilter
{
    private static readonly List<string> BannedWords = new()
    {
        "badword1", "badword2", "badword3", "poop", "damn", "hell", "stupid", "idiot", "dumb", "ugly", "hate", "kill", "terrorist"
    };

    public static bool ContainsInappropriateContent(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;

        var words = text.Split(new char[] { ' ', '.', ',', '!', '?', '\t', '\n' }, 
                               StringSplitOptions.RemoveEmptyEntries);

        return words.Any(word => BannedWords.Contains(word, StringComparer.OrdinalIgnoreCase));
    }

    public static string Sanitize(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        foreach (var word in BannedWords)
        {
            string stars = new string('*', word.Length);
            text = text.Replace(word, stars, StringComparison.OrdinalIgnoreCase);
        }

        return text;
    }
}