using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// A C# implementation of the Burglish (Burmese-English) transliteration logic
/// </summary>
public class BurglishIME
{
    public string Version { get; } = "1.9.12.090705";

    // --- Data Structures ---

    private readonly List<string[]> wI;
    private readonly Dictionary<string, string> knownWordsMap;
    private readonly HashSet<string> burmeseDictionary; // Based on the original Python file's dictionary
    private readonly Dictionary<string, string[]> wB;
    private readonly Dictionary<string, string[]> wU;
    private readonly string[][] wj;
    private readonly string[][] w9;

    // wS data structure split for C#
    private readonly string[] wS_Units;
    private readonly string[][] wS_Tens;
    private readonly string wS_Digits;

    /// <summary>
    /// Initializes a new instance of the BurglishIME class, loading all transliteration rules.
    /// </summary>
    public BurglishIME()
    {
        // Equivalent to Python's wI
        wI = new List<string[]>
        {
          new[] {"kyarr", "က်္ား", "-1"},
          new[] {"kyunote", "ကြၽႏု္ပ္", "-1"},
          new[] {"nhite", "၌", "-1"},
          new[] {"hnite", "၌", "-1"},
          new[] {"shat", "ယွက္", "-1"},
          new[] {"nyin", "ညာဥ္", "-1"},
          new[] {"shin", "ယွဥ္", "-1"},
          new[] {"kyar", "က်္ာ", "-1"},
          new[] {"yway", "၍", "-1"},
          new[] {"umm", "အမ္"},
          new[] {"imm", "အင္းမ္..."},
          new[] {"yin", "ယာဥ္", "-1"},
          new[] {"yin", "ယ်ာဥ္", "-1"},
          new[] {"ywe", "၍", "-1"},
          new[] {"d", "ဒီ"},
          new[] {"u", "ယူ"},
          new[] {"own", "အံုး", "-1"},
          new[] {"it", "ဧတ္"},
          new[] {"el", "ဧည့္"},
          new[] {"ei", "ဣ"},
          new[] {"or", "ဪ", "-1"},
          new[] {"ei", "၏"},
          new[] {"ei", "ဤ"},
          new[] {"oo", "ဥ"},
          new[] {"ah", "အ"},
          new[] {"aw", "ဪ"},
          new[] {"ay", "ဧ"},
          new[] {"ag", "ေအာင္"},
          new[] {"oo", "ဦး"},
          new[] {"oh", "အိုး"},
          new[] {"r", "အာ"},
          new[] {"ae", "အဲ"},
          new[] {"ei", "အိ"},
          new[] {"ei", "အီ"},
          new[] {"um", "အမ္"},
          new[] {".", "။"},
          new[] {",", "၊"},
          new[] {",", "ျပီး"},
          new[] {".", "ျပီ"},
          new[] {"4", "၎", "-1"},
          new[] {".", "ဤ"},
          new[] {".", "သည္"},
          new[] {".", "၏"},
          new[] {",", "၌"},
          new[] {",", "၍"},
          new[] {",", "ႏွင့္"},
          new[] {"f", "--္", "-1"},
          new[] {"b", "ျပီ"},
          new[] {"o", "အို"},
          new[] {"p", "ျပီ"},
          new[] {"e", "ဤ"},
          new[] {"a", "အ"},
          new[] {"a", "ေအ", "-1"},
          new[] {"u", "ဥ"},
          new[] {"u", "ဦး"},
          new[] {"h", "--့", "-1"},
          new[] {";", "း"},
          new[] {"eu", "အူ"},
          new[] {"u", "အူ"},
          new[] {"u", "အု"},
          new[] {"a", "အစ္", "-1"},
          new[] {"tun", "ထြန္း"}
        };

        // Equivalent to Python's known_words_map
        knownWordsMap = new Dictionary<string, string>();
        foreach (var item in wI)
        {
            if (item.Length >= 2 && !knownWordsMap.ContainsKey(item[0]))
            {
                knownWordsMap.Add(item[0], item[1]);
            }
        }

        // Using the dictionary from the original Python file as it's not in the JS data
        burmeseDictionary = new HashSet<string>
        {
            "ကြၽႏု္ပ္", "သူ", "ငါ", "ပညာ", "ေက်ာင်း", "စာအုပ္", "ေန", "ည",
            "စား", "သြား", "လာ", "ရွိ", "ျဖစ္", "ေျပာ", "ေနထိုင္", "ေကာင္း",
            "မွန္", "ျမန္ျမန္", "လား", "တယ္", "၏", "ႏွင့္", "ကို",
            "မဂၤလာပါ", "ေန႔ေကာင္းလား"
        };

        
        wB = new Dictionary<string, string[]>
        {
            {"ynn", new[] {"$1င္း"}},
            {"yn", new[] {"$1င္", "$1င္း"}},
            {"ye", new[] {"$1ိုင္း", "$1ိုင္", "$1ိုင့္"}},
            {"y", new[] {"$1ိုင္", "$1ိုင္း", "$1ိုင့္"}},
            {"uz", new[] {"$1ြဇ္", "ေ$1ာဇ္"}},
            {"uu", new[] {"$1ူး"}},
            {"ut", new[] {"$1ြတ္", "$1ြပ္", "ေ$1ာတ္", "$1ြဋ္"}},
            {"urt", new[] {"$1ာတ္", "$1ာက္", "$1ာဟ္"}},
            {"urd", new[] {"$1ာဒ္", "$1ာ႒္"}}, // Duplicate resolved
            {"urr", new[] {"$1ား"}},
            {"urk", new[] {"$1ာတ္", "$1ာက္", "$1ာဟ္"}},
            {"urh", new[] {"$1ာ့"}},
            {"urb", new[] {"$1ာဘ္"}},
            {"ur", new[] {"$1ာ", "$1ား", "$1ာ့"}},
            {"unt", new[] {"$1ြန္႕", "$1ြံ႕", "$1ြမ့္"}},
            {"unn", new[] {"$1ြန္း", "$1ြမ္း", "$1န္း", "$1ြဏ္း"}},
            {"un", new[] {"$1ြန္", "$1ြန္း", "$1ြမ္", "$1ြံ", "$1ြဏ္", "$1ြဏ္း"}},
            {"umt", new[] {"$1ြမ့္"}},
            {"umm", new[] {"$1ြမ္း", "$1ြမ္"}},
            {"um", new[] {"$1ြမ္", "$1ြမ္း"}},
            {"uh", new[] {"$1ူ႕"}},
            {"u", new[] {"$1ူ", "$1ု", "$1ူ႕"}},
            {"t", new[] {"ေ$1ာက္"}},
            {"rr", new[] {"$1ား"}},
            {"rh", new[] {"$1ာ့"}},
            {"r", new[] {"$1ာ", "$1ား", "$1ာ့"}},
            {"oy", new[] {"$1ိြဳင္"}},
            {"ove", new[] {"$1ုဗ္"}},
            {"ov", new[] {"$1ုဗ္"}},
            {"out", new[] {"ေ$1ာက္", "ေ$1ာတ္", "ေ$1ာဂ္"}},
            {"ount", new[] {"ေ$1ာင့္"}},
            {"ounh", new[] {"ေ$1ာင့္"}},
            {"oung", new[] {"ေ$1ာင္", "ေ$1ာင္း"}},
            {"oun", new[] {"ေ$1ာင္', 'ေ$1ာင္း"}}, // Duplicate resolved
            {"ou", new[] {"$1ိုး", "$1ို"}},
            {"oth", new[] {"$1ို႕"}},
            {"ote", new[] {"$1ုတ္", "$1ုပ္", "$1ုက္", "$1ုစ္", "$1ုဇ္", "$1ုဂ္", "$1ုဋ္"}},
            {"ot", new[] {"$1ို႕", "ေ$1ာ့", "$1ိုယ့္"}}, // Duplicate resolved
            {"ort", new[] {"ေ$1ာ့"}},
            {"orh", new[] {"ေ$1ာ့"}},
            {"or", new[] {"ေ$1ာ္", "ေ$1ာ"}},
            {"ope", new[] {"$1ုပ္", "$1ုတ္"}},
            {"op", new[] {"$1ို႕", "ေ$1ာ့", "$1ိုယ့္"}}, // Duplicate resolved
            {"ooz", new[] {"$1ြဇ္", "ေ$1ာဇ္"}},
            {"oot", new[] {"$1ြတ္", "$1ြပ္"}},
            {"oont", new[] {"$1ြန့္", "$1ြမ့္"}},
            {"oonh", new[] {"$1ြန့္", "$1ြမ့္"}},
            {"oon", new[] {"$1ြန္း", "$1ြန္", "$1ြမ္း", "$1ြမ္"}},
            {"oomt", new[] {"$1ြမ့္", "$1ြန့္"}},
            {"oomh", new[] {"$1ြမ့္", "$1ြန့္"}},
            {"oom", new[] {"$1ြမ္း", "$1ြမ္"}},
            {"ood", new[] {"$1ြဒ္", "$1ြတ္"}},
            {"oo", new[] {"$1ိုး", "$1ူး"}},
            {"ont", new[] {"$1ြန္႔", "$1ြံ႕", "$1ြမ့္", "$1ုန္႕", "$1ံု႕"}},
            {"one", new[] {"$1ုန္း", "$1ုမ္း", "$1ုံး", "$1ုဥ္း", "$1ုန္", "$1ုမ္", "$1ုံ"}},
            {"on", new[] {"$1ြန္", "$1ြံ", "$1ံု", "$1ြဏ္"}},
            {"ol", new[] {"$1ိုလ္", "ေ$1ာ", "$1ိုဠ္"}},
            {"oke", new[] {"$1ုက္"}},
            {"ok", new[] {"$1ုက္"}},
            {"oi", new[] {"$1ိြဳင္"}},
            {"ohnh", new[] {"$1ုန္႕", "$1ုမ့္"}},
            {"ohn", new[] {"$1ုန္း", "$1ုမ္း"}},
            {"ohmh", new[] {"$1ုန္႕", "$1ုမ့္"}},
            {"ohm", new[] {"$1ုန္း", "$1ုမ္း"}},
            {"oh", new[] {"$1ို႕", "ေ$1ာ့", "$1ိုယ့္"}},
            {"oet", new[] {"$1ို႕"}},
            {"oeh", new[] {"$1ို႕"}},
            {"oe", new[] {"$1ိုး"}},
            {"ode", new[] {"$1ုဒ္", "$1ုဎ္"}},
            {"od", new[] {"$1ုဒ္", "$1ုဎ္"}},
            {"oav", new[] {"$1ုဗ္"}},
            {"oat", new[] {"$1ုတ္", "$1ုပ္", "$1ုက္", "$1ုစ္", "$1ုဇ္", "$1ုဂ္", "$1ုဋ္"}},
            {"oap", new[] {"$1ုပ္", "$1ုတ္"}},
            {"oant", new[] {"$1ုန္႕", "$1ုမ့္", "$1ံု႔", "$1ုဥ့္"}},
            {"oann", new[] {"$1ုန္း", "$1ုမ္း", "$1ံုး", "$1ုဥ္း"}},
            {"oanh", new[] {"$1ုန္႕", "$1ုမ့္", "$1ံု႔", "$1ုဥ့္"}},
            {"oan", new[] {"$1ုန္", "$1ုမ္", "$1ံု", "$1ုဏ္", "$1ုဥ္", "$1ုလ္"}},
            {"oak", new[] {"$1ုက္"}},
            {"oad", new[] {"$1ုဒ္", "$1ုဎ္", "$1ုသ္"}},
            {"oa", new[] {"$1ြာ"}},
            {"o", new[] {"$1ို", "$1ိုး", "$1ိုရ္", "$1ိုယ္", "$1ိုဠ္", "$1ိုဟ္"}},
            {"iz", new[] {"$1ဇ္", "$1ာဇ္"}},
            {"ite", new[] {"$1ိုက္"}},
            {"it", new[] {"$1စ္", "$1တ္", "ေ$1တ္", "ေ$1က္", "$1ဋ္", "ေ$1စ္"}},
            {"is", new[] {"$1စ္", "$1တ္", "ေ$1တ္", "ေ$1က္", "$1ဋ္", "ေ$1စ္"}}, // Duplicate resolved
            {"int", new[] {"$1င့္", "$1ဥ့္"}},
            {"inn", new[] {"$1င္း", "$1ဥ္း"}},
            {"ing", new[] {"$1င္း", "$1ဥ္း"}},
            {"ine", new[] {"$1ိုင္", "$1ိုင္း", "$1ိုဏ္း"}},
            {"in", new[] {"$1င္", "$1င္း", "$1ဥ္", "ေ$1န္", "$1ဥ္း"}},
            {"ike", new[] {"$1ိုက္"}},
            {"ik", new[] {"$1စ္", "$1တ္", "ေ$1တ္", "ေ$1က္", "$1ဋ္", "ေ$1စ္"}},
            {"ii", new[] {"$1ီး", "$1ည္း", "$1ည့္"}},
            {"ih", new[] {"$1ည့္"}},
            {"ide", new[] {"$1ိုဒ္"}},
            {"id", new[] {"$1စ္"}},
            {"i", new[] {"$1ိ", "$1ီ", "$1ည္", "$1ည္း", "$1ည့္"}},
            {"g", new[] {"ေ$1ာင္", "ေ$1ာင္း"}},
            {"f", new[] {"$1္"}},
            {"eyy", new[] {"ေ$1း", "$1ည္း"}},
            {"eyt", new[] {"ေ$1့", "$1ည့္"}},
            {"eyh", new[] {"ေ$1့", "$1ည့္"}},
            {"ey", new[] {"ေ$1', '$1ည္', 'ေ$1း', 'ေ$1့', '$1ည္း"}}, // Duplicate resolved
            {"et", new[] {"$1က္", "$1တ္", "$1ပ္"}},
            {"ert", new[] {"$1ာတ္", "$1ာက္", "$1ာဟ္"}}, // Duplicate resolved
            {"err", new[] {"$1ား"}},
            {"erk", new[] {"$1ာတ္", "$1ာက္", "$1ာဟ္"}},
            {"erh", new[] {"$1ာ့"}},
            {"erd", new[] {"$1ာဒ္", "$1ာ႒္"}},
            {"erb", new[] {"$1ာဘ္"}},
            {"er", new[] {"$1ာ", "$1ား", "$1ာ့"}},
            {"en", new[] {"$1ဲန္း", "$1ဲန္", "$1န္"}},
            {"elh", new[] {"$1ဲ့", "$1ယ့္", "$1ည့္"}},
            {"el", new[] {"$1ဲ", "$1ယ္", "$1ည္", "$1ည္း", "$1ဲ့", "$1ည့္"}},
            {"ek", new[] {"$1က္"}},
            {"eit", new[] {"$1ိတ္", "$1ိပ္", "$1ိက္", "$1ိဋ္", "$1ိသ္"}},
            {"eint", new[] {"$1ိန္႕", "$1ိမ့္"}},
            {"einn", new[] {"$1ိန္း", "$1ိမ္း", "$1ိဏ္း"}},
            {"einh", new[] {"$1ိန္႕", "$1ိမ့္"}},
            {"ein", new[] {"$1ိန္", "$1ိမ္", "$1ႎ", "$1ိင္", "$1ိဥ္", "$1ိဏ္", "$1ိလ္"}},
            {"eih", new[] {"ေ$1့", "$1ဲ့", "$1ယ့္", "$1ည့္"}},
            {"ei", new[] {"ေ$1း", "$1ဲ", "$1ယ္", "ေ$1", "$1ည္း", "$1ည္"}},
            {"eh", new[] {"$1ဲ့", "$1ည့္", "ေ$1့", "$1ဲ", "$1ည္"}},
            {"ee", new[] {"$1ီး", "$1ည္း"}},
            {"eck", new[] {"$1က္"}},
            {"ec", new[] {"$1က္"}},
            {"e`", new[] {"$1ဲ့", "$1ဲ"}},
            {"e", new[] {"$1ီ", "$1ဲ", "$1ည္", "$1ယ္", "ေ$1", "ေ$1း", "$1ည့္", "$1ဲ့"}},
            {"ayy", new[] {"ေ$1း", "$1ည္း"}},
            {"ayt", new[] {"ေ$1့", "$1ည့္"}},
            {"ayh", new[] {"ေ$1့", "$1ည့္"}},
            {"aye", new[] {"ေ$1း", "$1ည္း"}},
            {"ay", new[] {"ေ$1", "$1ည္", "ေ$1း", "ေ$1့", "$1ည္း"}},
            {"aww", new[] {"ေ$1ာ"}},
            {"awt", new[] {"ေ$1ာ့"}},
            {"awn", new[] {"ေ$1ာန္"}},
            {"awh", new[] {"ေ$1ာ့"}},
            {"aw", new[] {"ေ$1ာ", "ေ$1ာ္", "ေ$1ာ့", "ေ$1ာဝ္"}},
            {"ave", new[] {"$1ိဗ္"}},
            {"av", new[] {"$1ဗ္"}},
            {"aut", new[] {"ေ$1ာက္", "ေ$1ာတ္"}},
            {"aunt", new[] {"ေ$1ာင့္"}},
            {"aunh", new[] {"ေ$1ာင့္"}},
            {"aung", new[] {"ေ$1ာင္", "ေ$1ာင္း"}},
            {"aun", new[] {"ေ$1ာင္', 'ေ$1ာင္း"}}, // Duplicate resolved
            {"auk", new[] {"ေ$1ာက္", "ေ$1ာတ္"}},
            {"au", new[] {"ေ$1ာ"}},
            {"ath", new[] {"$1သ္"}},
            {"ate", new[] {"$1ိတ္", "$1ိပ္", "$1ိဇ္", "$1ိစ္", "$1ိက္", "$1ိဋ္", "$1ိသ္"}},
            {"at", new[] {"$1တ္", "$1က္", "$1ပ္", "$1ဟ္", "$1ဋ္"}},
            {"art", new[] {"$1ာတ္", "$1ာက္", "$1ာဟ္"}}, // Duplicate resolved
            {"arr", new[] {"$1ား"}},
            {"arnn", new[] {"$1ာန္း", "$1ာဏ္း"}},
            {"arn", new[] {"$1ာန္", "$1ာဏ္", "$1ာဟ္"}},
            {"arl", new[] {"$1ာယ္", "$1ာည္"}},
            {"ark", new[] {"$1ာတ္", "$1ာက္", "$1ာဟ္"}},
            {"arh", new[] {"$1ာ့"}},
            {"ard", new[] {"$1ာဒ္", "$1ာ႒္"}},
            {"arb", new[] {"$1ာဘ္"}},
            {"ar", new[] {"$1ာ", "$1ား", "$1ာ့"}},
            {"ape", new[] {"$1ိပ္"}},
            {"ap", new[] {"$1ပ္"}},
            {"ant", new[] {"$1န္႕", "$1ံ့", "$1မ့္"}},
            {"ann", new[] {"$1န္း", "$1မ္း", "$1ဏ္း"}},
            {"an", new[] {"$1န္", "$1ံ", "$1မ္", "$1ဏ္", "$1လ္"}},
            {"amm", new[] {"$1မ္း"}},
            {"am", new[] {"$1မ္", "$1မ္း", "$1န္", "$1ံ"}},
            {"alh", new[] {"$1ယ့္", "$1ဲ့", "$1ည့္", "$1ဲ", "$1ည္"}},
            {"al", new[] {"$1ယ္", "$1ဲ", "$1ည္", "$1ည္း", "$1ဲ့", "$1ည့္", "$1လ္"}},
            {"ake", new[] {"$1ိက္", "$1ိတ္"}},
            {"ak", new[] {"$1က္"}},
            {"aiv", new[] {"$1ိဗ္"}},
            {"ait", new[] {"$1ိတ္", "$1ိပ္", "$1ိဇ္", "$1ိစ္", "$1ိက္"}},
            {"aid", new[] {"$1ိဒ္"}}, // Duplicate resolved
            {"aip", new[] {"$1ိပ္"}},
            {"aint", new[] {"$1ိန္႕", "$1ိမ့္"}},
            {"ainn", new[] {"$1ိန္း", "$1ိမ္း", "$1ိဏ္း"}},
            {"ainh", new[] {"$1ိန္႕", "$1ိမ့္"}},
            {"aing", new[] {"$1ိုင္", "$1ိုင္း"}},
            {"ain", new[] {"$1ိန္", "$1ိမ္", "$1ႎ", "$1ိင္", "$1ိဥ္", "$1ိဏ္", "$1ိလ္"}},
            {"aik", new[] {"$1ိက္", "$1ိတ္"}},
            {"ai", new[] {"$1ိုင္း", "$1ိုင္", "$1ိုဏ္း", "$1ိုင့္", "ေ$1"}},
            {"ag", new[] {"$1ဂ္"}},
            {"aeh", new[] {"ေ$1့", "$1ည့္", "$1ဲ့"}},
            {"ae", new[] {"$1ယ္", "$1ဲ", "ေ$1", "$1ည္", "ေ$1း"}},
            {"ade", new[] {"$1ိဒ္"}},
            {"ad", new[] {"$1ဒ္", "$1ဎ္"}},
            {"ack", new[] {"$1က္"}},
            {"ac", new[] {"$1က္"}},
            {"ab", new[] {"$1ဘ္"}},
            {"a`", new[] {"$1ဲ့", "$1ဲ"}},
            {"a", new[] {"$1", "$1ာ့"}},
            {"`", new[] {"$1ဲ့", "$1ဲ"}}
        };

        wU = new Dictionary<string, string[]>
        {
            {"zz", new[] {"စ်"}},
            {"zw", new[] {"ဇြ"}},
            {"zh", new[] {"စ်"}},
            {"z", new[] {"ဇ", "စ်"}},
            {"yy", new[] {"ယ", "ယ်"}},
            {"yw", new[] {"ရြ", "ယြ"}},
            {"yh", new[] {"ယ", "ယ်"}}, // Duplicate resolved
            {"y", new[] {"ရ", "ယ", "လ်", "ယ်"}},
            {"x", new[] {"ဆ", "စ"}},
            {"wh", new[] {"ဝွ"}},
            {"w", new[] {"ဝ"}},
            {"v", new[] {"ဗ", "ဘ"}},
            {"u", new[] {"အ"}},
            {"ty", new[] {"တ်", "ၾတ"}},
            {"tw", new[] {"တြ"}},
            {"tt", new[] {"ဋ"}},
            {"tr", new[] {"တ်", "ၾတ"}},
            {"thw", new[] {"သြ"}},
            {"th", new[] {"သ"}},
            {"t", new[] {"တ", "ဋ", "ထ"}},
            {"sy", new[] {"ၾဆ"}},
            {"sw", new[] {"စြ", "ဆြ"}},
            {"ss", new[] {"ဆ"}},
            {"shw", new[] {"ရႊ"}},
            {"sh", new[] {"ရွ", "လွ်", "သွ်"}},
            {"s", new[] {"စ", "ဆ"}},
            {"r", new[] {"ရ", "ယ", "လ်"}},
            {"q", new[] {"က"}},
            {"py", new[] {"ျပ", "ပ်"}},
            {"pw", new[] {"ပြ"}},
            {"phy", new[] {"ျဖ", "ဖ်"}},
            {"phw", new[] {"ဖြ"}},
            {"ph", new[] {"ဖ"}},
            {"p", new[] {"ပ"}},
            {"o", new[] {"အ"}},
            {"ny", new[] {"ည", "ျင", "ဉ"}},
            {"nw", new[] {"ႏြ"}},
            {"nn", new[] {"ဏ"}},
            {"nhy", new[] {"ညွ", "ျငွ", "ဥွ"}},
            {"nhw", new[] {"ႏႊ"}},
            {"nhg", new[] {"ငွ"}},
            {"nh", new[] {"ငွ", "ႏွ", "ဏွ"}},
            {"ngw", new[] {"ငြ"}},
            {"ngh", new[] {"ငွ"}},
            {"ng", new[] {"င"}},
            {"n", new[] {"န", "ဏ"}},
            {"my", new[] {"ျမ", "မ်"}},
            {"mw", new[] {"မြ", "ျမြ"}},
            {"mhy", new[] {"မွ်", "ျမွ"}},
            {"mhw", new[] {"မႊ", "ျမႊ"}},
            {"mh", new[] {"မွ"}},
            {"m", new[] {"မ"}},
            {"ly", new[] {"လ်", "လွ်"}},
            {"lw", new[] {"လြ", "လႊ"}},
            {"ll", new[] {"ဠ"}},
            {"lhy", new[] {"လွ်", "လ်"}},
            {"lhw", new[] {"လႊ"}},
            {"lh", new[] {"လွ", "ဠွ"}},
            {"l", new[] {"လ", "ဠ"}},
            {"kyw", new[] {"ၾကြ", "ကြၽ"}},
            {"ky", new[] {"က်", "ၾက"}},
            {"kw", new[] {"ကြ"}},
            {"khw", new[] {"ခြ"}},
            {"kh", new[] {"ခ"}},
            {"k", new[] {"က", "ခ"}},
            {"j", new[] {"ဂ်", "ျဂ"}},
            {"i", new[] {"အ"}},
            {"hw", new[] {"ဟြ"}},
            {"htw", new[] {"ထြ"}},
            {"htt", new[] {"ဌ"}},
            {"ht", new[] {"ထ", "ဌ", "႒"}},
            {"hs", new[] {"ဆ"}},
            {"hnw", new[] {"ႏႊ"}},
            {"hn", new[] {"ႏွ", "ဏွ"}},
            {"hmy", new[] {"မွ်", "ျမွ"}},
            {"hmw", new[] {"မႊ", "ျမႊ"}},
            {"hm", new[] {"မွ"}},
            {"hly", new[] {"လွ်", "လ်"}},
            {"hlw", new[] {"လႊ"}},
            {"hl", new[] {"လွ", "ဠွ"}},
            {"hdd", new[] {"ဎ"}},
            {"hd", new[] {"ဍ", "ဎ"}},
            {"h", new[] {"ဟ"}},
            {"gy", new[] {"ဂ်", "ျဂ", "ၾက"}},
            {"gw", new[] {"ဂြ"}},
            {"gh", new[] {"ဃ"}},
            {"gg", new[] {"ဃ"}},
            {"g", new[] {"ဂ", "က", "ဃ"}},
            {"fy", new[] {"ျဖ", "ဖ်"}},
            {"fw", new[] {"ဖြ", "ဘြ"}},
            {"f", new[] {"ဖ"}},
            {"e", new[] {"အ"}},
            {"dw", new[] {"ဒြ", "ျဒ"}},
            {"dr", new[] {"ဒြ", "ျဒ"}},
            {"dd", new[] {"ဓ"}},
            {"d", new[] {"ဒ", "ဓ", "တ", "ဍ", "ဎ"}},
            {"chw", new[] {"ခြၽ", "ျခြ"}},
            {"ch", new[] {"ခ်", "ျခ"}},
            {"c", new[] {"က"}},
            {"by", new[] {"ဗ်", "ျဗ", "ဘ်"}},
            {"bw", new[] {"ဘြ", "ဗြ", "ပြ"}},
            {"b", new[] {"ဘ", "ဗ", "ပ"}},
            {"a", new[] {"အ"}}
        };

        wj = new[]
        {
          new[] {@"([^ျၾ][ခဂငဒပဝ၀][ၠ-ၼႇႇွုူႈႉြႊ]?)ာ", @"$1ါ"},
          new[] {@"^([ခဂငဒပဝ၀][ၠ-ၼႇႇွုူႈႉြႊ]?)ာ", @"$1ါ"},
          new[] {@"ါ္", @"ၚ"},
          new[] {@"န([ံိဲ]?[ၠ-ၼႇွုူႈႉြႊ])", @"ႏ$1"},
          new[] {@"([ၠ-ၼႇွုူႈႉြႊန်ဳဴ].?)[့႔]", @"$1႕"},
          new[] {@"([ျၾ-ႄ][က-အႏ႐][ိြွႊ]?)ု", @"$1ဳ"},
          new[] {@"([ျၾ-ႄ][က-အႏ႐][ိြွႊ]?)ူ", @"$1ဴ"},
          new[] {@"([ဈ-ဍဠဥ်ၠ-ၽႅ႑-႓][ိြွႊ]?)ု", @"$1ဳ"},
          new[] {@"([ဈ-ဍဠဥ်ၠ-ၽႅ႑-႓][ိြွႊ]?)ူ", @"$1ဴ"},
          new[] {@"ွ([ံိႎဲ]?)ု", @"$1ႈ"},
          new[] {@"ွ([ံိႎဲ]?)ူ", @"$1ႉ"},
          new[] {@"ရ([ံိႎဲ]?[ုူႈႉ])", @"႐$1"},
          new[] {@"ွ([ိႎဲၽ်]?)ွ", @"ွ$1"},
          new[] {@"ြွ|ွြ|ႊွ", @"ႊ"},
          new[] {@"်ြ", @"ြၽ"},
          new[] {@"[ွႇ]([ိီ်ၽ]?)ြ", @"ႊ$1"},
          new[] {@"်ွ", @"ွ်"},
          new[] {@"([ျၾ].{0,2})ွ", @"$1ႇ"},
          new[] {@"([ဝြႊ].{0,2})ြ", @"$1"},
          new[] {@"([ဥ])[ုဳ]", @"$1"},
          new[] {@"ၤ(.{0,2})ိ", @"ႋ$1"},
          new[] {@"ၤ(.{0,2})ီ", @"ႌ$1"},
          new[] {@"ာ႕", @"ာ့"},
          new[] {@"ၾ([ကဃဆဏတထဘယလသဟ][ံိႎဲ]?ြ)", @"ႂ$1"},
          new[] {@"([^ျၾ][ခဂငဒပဝ၀][ၠ-ၼႇႇွုူႈႉြႊ]?)ာ္", @"$1ၚ"}
        };

        w9 = new[]
        {
            new[] {@"အု", @"ဥ"},
            new[] {@"အိ(?![ု])", @"ဣ"},
            new[] {@"ေအာ(?![့္])", @"ဩ"}
        };

        wS_Units = new[] { "တစ္", "ႏွစ္", "သံုး", "ေလး", "ငါး", "ေျခာက္", "ခုႏွစ္", "ရွစ္", "ကိုး" };
        wS_Tens = new[]
        {
          new[] {"ဆယ္", "ဆယ့္"},
          new[] {"ရာ", "ရာ့"},
          new[] {"ေထာင္", "ေထာင့္"},
          new[] {"ေသာင္း", "ေသာင္း"},
          new[] {"သိန္း", "သိန္း"},
          new[] {"သန္း", "သန္း"},
          new[] {"ကုေဋ", "ကုေဋ"}
        };
        wS_Digits = "၀၁၂၃၄၅၆၇၈၉";
    }

    /// <summary>
    /// Calculates the Levenshtein distance between two strings.
    /// </summary>
    /// <param name="s1">The first string.</param>
    /// <param name="s2">The second string.</param>
    /// <returns>The minimum number of edits required to change s1 into s2.</returns>
    public static int LevenshteinDistance(string s1, string s2)
    {
        s1 = s1.ToLower();
        s2 = s2.ToLower();

        if (s1.Length < s2.Length)
        {
            return LevenshteinDistance(s2, s1);
        }

        if (s2.Length == 0)
        {
            return s1.Length;
        }

        int[] previousRow = new int[s2.Length + 1];
        for (int i = 0; i < previousRow.Length; i++)
        {
            previousRow[i] = i;
        }

        for (int i = 0; i < s1.Length; i++)
        {
            char c1 = s1[i];
            int[] currentRow = new int[s2.Length + 1];
            currentRow[0] = i + 1;
            for (int j = 0; j < s2.Length; j++)
            {
                char c2 = s2[j];
                int insertions = previousRow[j + 1] + 1;
                int deletions = currentRow[j] + 1;
                int substitutions = previousRow[j] + (c1 == c2 ? 0 : 1);
                currentRow[j + 1] = Math.Min(insertions, Math.Min(deletions, substitutions));
            }
            previousRow = currentRow;
        }

        return previousRow[s2.Length];
    }

    /// <summary>
    /// Gets a list of all raw transliteration suggestions for a given Romanized word.
    /// This is ideal for populating a suggestion drop-down list.
    /// </summary>
    /// <param name="word">The Romanized word.</param>
    /// <returns>A List of possible Burmese transliterations.</returns>
    public List<string> GetRawSuggestions(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return new List<string>();
        }

        var suggestions = new List<Tuple<string, string>>();

        // Check wI for prefixes
        foreach (var item in wI)
        {
            if (item[0].StartsWith(word))
            {
                suggestions.Add(Tuple.Create(item[0], item[1]));
            }
        }

        // Check for digits 
        if (long.TryParse(word, out _)) // Check if it's purely numeric
        {
            // Direct digit-to-digit translation
            var burmeseNumerals = new StringBuilder();
            foreach (char d in word)
            {
                burmeseNumerals.Append(wS_Digits[int.Parse(d.ToString())]);
            }
            suggestions.Add(Tuple.Create(word, burmeseNumerals.ToString()));

            // Number-to-word translation
            string reversedDigits = new string(word.Reverse().ToArray());
            if (reversedDigits.Length <= wS_Tens.Length + 1)
            {
                var resultWord = new StringBuilder();
                bool isFirstDigit = true;
                for (int i = 0; i < reversedDigits.Length; i++)
                {
                    int digit = int.Parse(reversedDigits[i].ToString());
                    if (digit == 0)
                    {
                        isFirstDigit = false;
                        continue;
                    }

                    string unitText;
                    if (i == 0)
                    {
                        unitText = wS_Units[digit - 1];
                    }
                    else
                    {
                        string unit = wS_Tens[i - 1][isFirstDigit ? 0 : 1];
                        unitText = wS_Units[digit - 1] + unit;
                    }
                    resultWord.Insert(0, unitText);
                    isFirstDigit = false;
                }
                if (resultWord.Length > 0)
                {
                    suggestions.Add(Tuple.Create(word, resultWord.ToString()));
                }
            }
        }

        // Consonant and Vowel rule processing 
        string inputLower = word.ToLower();
        string consonantPart = "";

        // Find longest consonant prefix
        int i_consonant = 1;
        while (i_consonant <= inputLower.Length)
        {
            string prefix = inputLower.Substring(0, i_consonant);
            if (wU.ContainsKey(prefix))
            {
                consonantPart = prefix;
            }
            i_consonant++;
        }

        string vowelPart = inputLower.Substring(consonantPart.Length);

        if (string.IsNullOrEmpty(consonantPart) && wU.ContainsKey(inputLower))
        {
            consonantPart = inputLower;
            vowelPart = "a";
        }

        if (string.IsNullOrEmpty(vowelPart))
        {
            vowelPart = "a"; // Default to 'a' if no vowel part, like in the python script
        }

        if (wB.TryGetValue(vowelPart, out string[] burmeseVowelRules))
        {
            if (wU.TryGetValue(consonantPart, out string[] burmeseConsonants))
            {
                foreach (string burmeseConsonant in burmeseConsonants)
                {
                    foreach (string rule in burmeseVowelRules)
                    {
                        // Apply the vowel rule (e.g., "$1ာ") to the consonant (e.g., "က")
                        string result = Regex.Replace(burmeseConsonant, "^(.*)$", rule);
                        if (!Regex.IsMatch(result, "[a-zA-Z]"))
                        {
                            suggestions.Add(Tuple.Create(word, result));
                        }
                    }
                }
            }
        }

        // Post-processing rules (wj)
        var generatedStrings = suggestions.Select(s => s.Item2).ToList();
        var processedStrings = new List<string>();
        foreach (string s in generatedStrings)
        {
            string temp_s = s;
            foreach (var rule in wj)
            {
                try
                {
                    temp_s = Regex.Replace(temp_s, rule[0], rule[1]);
                }
                catch (ArgumentException)
                {
                    // Ignore invalid regex patterns
                }
            }
            processedStrings.Add(temp_s);
        }

        // Final processing rules (w9)
        var finalStrings = new List<string>();
        foreach (string s in processedStrings)
        {
            finalStrings.Add(s);
            foreach (var rule in w9)
            {
                if (Regex.IsMatch(s, rule[0]))
                {
                    string new_s = Regex.Replace(s, rule[0], rule[1]);
                    if (!finalStrings.Contains(new_s))
                    {
                        finalStrings.Add(new_s);
                    }
                }
            }
        }

        // Return unique suggestions
        return finalStrings.Distinct().ToList();
    }

    /// <summary>
    /// Generates suggestions and intelligently picks the best one using fuzzy matching.
    /// This is ideal for setting the final text.
    /// </summary>
    /// <param name="word">The Romanized word.</param>
    /// <returns>The best single Burmese transliteration, or a placeholder.</returns>
    public string GetBestSuggestion(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return "";
        }

        string lowerWord = word.ToLower();

        // 1. Check for an exact match
        if (knownWordsMap.TryGetValue(lowerWord, out string exactMatch))
        {
            return exactMatch;
        }

        // 2. Find closest known word using Levenshtein distance
        int minDistance = int.MaxValue;
        string closestWord = null;
        int threshold = (word.Length <= 4) ? 1 : 2;

        foreach (string knownWord in knownWordsMap.Keys)
        {
            int distance = LevenshteinDistance(word, knownWord);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestWord = knownWord;
            }
        }

        if (closestWord != null && minDistance <= threshold)
        {
            return knownWordsMap[closestWord];
        }

        // 3. Fall back to the raw transliteration engine
        List<string> suggestions = GetRawSuggestions(word);
        if (suggestions.Count == 0)
        {
            return $"<{word}>?";
        }

        // 4. Filter raw suggestions against our dictionary
        List<string> validSuggestions = suggestions
            .Where(s => burmeseDictionary.Contains(s))
            .ToList();

        // 5. Decide on the best available option
        if (validSuggestions.Count > 0)
        {
            return validSuggestions[0];
        }
        else
        {
            return suggestions[0];
        }
    }
}

