public static class ContentFilter
{
    private static readonly List<string> BannedWords = new()
    {
        // --- ENGLISH: GENERAL ---
        "fuck", "fucking", "fucker", "motherfucker", "shite", "shithead", "asshole", "dumbass", 
        "bitch", "bitching", "bastard", "cunt", "dick", "dickhead", "pussy", "faggot", "nigger", 
        "slut", "whore", "cock", "cocksucker", "cum", "jizz", "twat", "retard", "bollocks", 
        "piss", "wanker", "douche", "douchebag", "tit", "ballsack", "tosser", "scumbag", 
        "negro", "dyke", "coone", "jap", "kike", "spic",

        // --- NEDERLANDS: ALGEMEEN ---
        "klootzak", "lul", "luldebehangen", "eikel", "hufter", "hufterig", "slet", "sletje", 
        "hoer", "hoerenkind", "hoerig", "kut", "kutwijf", "kutzak", "neuken", "neuker", 
        "pijpen", "rukker", "aftrekken", "flikker", "nazi", "mongool", "mongooltje", 
        "debiel", "idioot", "imbeciel", "achterlijk", "kak", "stront", "schijt", 
        "pleuris", "verdomme", "godverdomme", "godvers", "straathoer", "rotzak",
        "hoerenzoon", "klootviool", "mierenneuker", "pikkie", "pisvlek", "snotneus",

        // --- NEDERLANDS: ZIEKTES ---
        "kanker", "kankerlijder", "kankeren", "tyfus", "tyfuslijer", "tering", "teringlijer", 
        "pokken", "pest", "pestlijer", "cholera", "tiefus", "tiefes",

        // --- AGGRESSIE & DREIGING ---
        "moord", "moordenaar", "kill", "killer", "death", "die", "terrorist", "terrorism", 
        "bomb", "bomber", "suicide", "zelfmoord", "verkrachten", "rape", "raper",
        "aanslag", "bloedbad", "genocide"
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