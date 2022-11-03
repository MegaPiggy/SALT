using SALT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SALT.Extensions
{
    using Honyaku;
    public static class HonyakuExtensions
    {
        private static KanaTools _kanaTools = new KanaTools().UseObsoleteKana(true); 

        public static string ToHiragana(this string romaji)
        {
            if (romaji.IsNullOrWhiteSpace())
                return string.Empty;
            return _kanaTools.ToHiragana(romaji);
        }

        public static string ToKatakana(this string romaji)
        {
            if (romaji.IsNullOrWhiteSpace())
                return string.Empty;
            return _kanaTools.ToKatakana(romaji);
        }

        public static string ToRomaji(this string japanese)
        {
            if (japanese.IsNullOrWhiteSpace())
                return string.Empty;
            return _kanaTools.ToRomaji(japanese);
        }
    }
}

namespace SALT.Honyaku
{
    /* Conversion class */
    public class KanaTools
    {
        // Codes for kana symbols
        private const int UppercaseStart = 0x41;
        private const int UppercaseEnd = 0x5A;
        private const int HiraganaStart = 0x3041;
        private const int HiraganaEnd = 0x3096;
        private const int KatakanaStart = 0x30A1;
        private const int KatakanaEnd = 0x30FA;

        // Options
        private bool _imeMode;
        private bool _useObsoleteKana;

        public KanaTools() { }

        /* Options */

        public KanaTools UseIme(bool mode)
        {
            _imeMode = mode;
            return this;
        }

        public KanaTools UseObsoleteKana(bool flag)
        {
            _useObsoleteKana = flag;
            return this;
        }

        /* Public API */
        public string OnTheFlyToKana(string input, bool hiragana = true, bool katakana = false)
        {
            var result = RomajiToHiragana(input);
            return katakana ? HiraganaToKatakana(result) : result;
        }

        public string ToHiragana(string input)
        {
            if (IsRomaji(input))
            {
                return RomajiToHiragana(input);
            }

            return IsKatakana(input) ? KatakanaToHiragana(input) : input;
        }

        public string ToKatakana(string input)
        {
            if (IsHiragana(input))
            {
                return HiraganaToKatakana(input);
            }

            return IsRomaji(input) ? HiraganaToKatakana(RomajiToHiragana(input)) : input;
        }

        public string ToRomaji(string input)
        {
            return HiraganaToRomaji(input);
        }

        public string ToKana(string input)
        {
            return RomajiToKana(input, false);
        }


        public bool IsHiragana(string input)
        {
            return AllTrue(input, str => IsCharHiragana(str[0]));
        }

        public bool IsKatakana(string input)
        {
            return AllTrue(input, str => IsCharKatakana(str[0]));
        }

        public bool IsKana(string input)
        {
            return AllTrue(input, str => (IsKatakana(str) || IsHiragana(str)));
        }

        public bool IsRomaji(string input)
        {
            return AllTrue(input, str => (!IsKatakana(str) && !IsHiragana(str)));
        }



        /* Character check methods */

        private static bool IsCharInRange(char chr, int start, int end)
        {
            var code = (int)chr;
            return (start <= code && code <= end);
        }

        private static bool IsCharVowel(char chr, bool includeY)
        {
            var regexp = includeY ? new Regex(@"[aeiouy]") : new Regex(@"[aeiou]");
            return regexp.IsMatch(chr.ToString());
        }

        private static bool IsCharConsonant(char chr, bool includeY)
        {
            var regexp = includeY ? new Regex(@"[bcdfghjklmnpqrstvwxyz]") : new Regex(@"[bcdfghjklmnpqrstvwxz]");
            return regexp.IsMatch(chr.ToString());
        }

        /* KanaTools character check methods */

        private static bool IsCharHiragana(char chr)
        {
            return IsCharInRange(chr, HiraganaStart, HiraganaEnd);
        }

        private static bool IsCharKatakana(char chr)
        {
            return IsCharInRange(chr, KatakanaStart, KatakanaEnd);
        }

        private bool IsCharKana(char chr)
        {
            return IsCharHiragana(chr) || IsCharKatakana(chr);
        }

        /* Utility methods */

        private static bool AllTrue(string stringToCheck, Func<string, bool> method)
        {
            return stringToCheck.All(t => method(Convert.ToString(t)));
        }


        /* Conversions */

        private string RomajiToHiragana(string romaji)
        {
            return RomajiToKana(romaji);
        }

        // TODO: move ignore case to options?
        private string RomajiToKana(string romaji, bool ignoreCase = true)
        {
            var chunk = "";
            var chunkLc = "";
            var position = 0;
            var len = romaji.Length;
            const int maxChunk = 3;
            var kana = "";
            var kanaChar = "";

            while (position < len)
            {
                var chunkSize = Math.Min(maxChunk, len - position);

                while (chunkSize > 0)
                {
                    // NB: this is (start, end - start) equivalent to Java's (start, end)
                    chunk = romaji.Substring(position, chunkSize);
                    chunkLc = chunk.ToLower();

                    if ((chunkLc.Equals("lts") || chunkLc.Equals("xts")) && (len - position) >= 4)
                    {
                        chunkSize++;
                        // The second parameter in substring() is an end point, not a length!
                        chunk = romaji.Substring(position, chunkSize);
                        chunkLc = chunk.ToLower();
                    }

                    if (Convert.ToString(chunkLc[0]).Equals("n"))
                    {
                        // Convert n' to ん
                        if (_imeMode && chunk.Length == 2 && Convert.ToString(chunkLc[1]).Equals("'"))
                        {
                            chunkSize = 2;
                            chunk = "nn";
                            chunkLc = chunk.ToLower();
                        }
                        // If the user types "nto", automatically convert "n" to "ん" first
                        // "y" is excluded from the list of consonants so we can still get にゃ, にゅ, and にょ
                        if (chunk.Length > 2 && IsCharConsonant(chunkLc[1], false) && IsCharVowel(chunkLc[2], true))
                        {
                            chunkSize = 1;
                            // I removed the "n"->"ん" mapping because the IME wouldn't let me type "na" for "な" without returning "んあ",
                            // so the chunk needs to be manually set to a value that will map to "ん"
                            chunk = "nn";
                            chunkLc = chunk.ToLower();
                        }
                    }

                    // Prepare to return a small-つ because we're looking at double-consonants.
                    if (chunk.Length > 1 && !Convert.ToString(chunkLc[0]).Equals("n")
                        && IsCharConsonant(chunkLc[0], true)
                        && chunk[0] == chunk[1])
                    {
                        chunkSize = 1;
                        // Return a small katakana ツ when typing in uppercase
                        if (IsCharInRange(chunk[0], UppercaseStart, UppercaseEnd))
                        {
                            chunkLc = chunk = "ッ";
                        }
                        else
                        {
                            chunkLc = chunk = "っ";
                        }
                    }

                    // Try to parse the chunk
                    kanaChar = null;
                    Honyaku.RomajiToKana.Table.TryGetValue(chunkLc, out kanaChar);

                    // Continue, if found this item in table
                    if (kanaChar != null)
                    {
                        break;
                    }

                    // If could not find key, then try again with the smaller chunk
                    chunkSize--;
                }

                if (kanaChar == null)
                {
                    chunk = ConvertPunctuation(Convert.ToString(chunk[0]));
                    kanaChar = chunk;
                }

                if (_useObsoleteKana)
                {
                    if (chunkLc.Equals("wi"))
                    {
                        kanaChar = "ゐ";
                    }
                    if (chunkLc.Equals("we"))
                    {
                        kanaChar = "ゑ";
                    }
                }

                if (romaji.Length > (position + 1) && _imeMode && Convert.ToString(chunkLc[0]).Equals("n"))
                {
                    if ((Convert.ToString(romaji[position + 1]).ToLower().Equals("y") && position == (len - 2)) || position == (len - 1))
                    {
                        kanaChar = Convert.ToString(chunk[0]);
                    }
                }

                if (!ignoreCase)
                {
                    if (IsCharInRange(chunk[0], UppercaseStart, UppercaseEnd))
                    {
                        kanaChar = HiraganaToKatakana(kanaChar);
                    }
                }

                kana += kanaChar;

                position += chunkSize > 0 ? chunkSize : 1;
            }

            return kana;
        }

        public string HiraganaToRomaji(string hira)
        {
            if (IsRomaji(hira))
            {
                return hira;
            }

            var chunk = "";
            var cursor = 0;
            var len = hira.Length;
            const int maxChunk = 2;
            var nextCharIsDoubleConsonant = false;
            var roma = "";
            string romaChar = null;

            while (cursor < len)
            {
                var chunkSize = Math.Min(maxChunk, len - cursor);
                while (chunkSize > 0)
                {
                    chunk = hira.Substring(cursor, chunkSize);

                    if (IsKatakana(chunk))
                    {
                        chunk = KatakanaToHiragana(chunk);
                    }

                    if (Convert.ToString(chunk[0]).Equals("っ") && chunkSize == 1 && cursor < (len - 1))
                    {
                        nextCharIsDoubleConsonant = true;
                        romaChar = "";
                        break;
                    }

                    // Try to parse
                    romaChar = null;
                    KanaToRomaji.Table.TryGetValue(chunk, out romaChar);

                    if ((romaChar != null) && nextCharIsDoubleConsonant)
                    {
                        romaChar = romaChar[0] + romaChar;
                        nextCharIsDoubleConsonant = false;
                    }

                    if (romaChar != null)
                    {
                        break;
                    }

                    chunkSize--;
                }
                if (romaChar == null)
                {
                    romaChar = chunk;
                }

                roma += romaChar;
                cursor += chunkSize > 0 ? chunkSize : 1;
            }
            return roma;
        }

        private static string HiraganaToKatakana(string hira)
        {
            var kata = "";

            foreach (var hiraChar in hira)
            {
                if (IsCharHiragana(hiraChar))
                {
                    var code = (int)hiraChar;
                    code += KatakanaStart - HiraganaStart;
                    kata += Convert.ToString(Convert.ToChar(code));
                }
                else
                {
                    kata += hiraChar;
                }
            }

            return kata;
        }

        private static string KatakanaToHiragana(string kata)
        {
            string hira = "";

            foreach (var kataChar in kata)
            {
                if (IsCharKatakana(kataChar))
                {
                    var code = (int)kataChar;
                    code += HiraganaStart - KatakanaStart;
                    hira += Convert.ToString(Convert.ToChar(code));
                }
                else
                {
                    hira += kataChar;
                }
            }

            return hira;
        }


        // Convert punctuations: long space and dash
        private static string ConvertPunctuation(string input)
        {
            if (input.Equals(Convert.ToString(('　'))))
            {
                return Convert.ToString(' ');
            }

            return input.Equals(Convert.ToString('-')) ? Convert.ToString('ー') : input;
        }

    }

    internal class KanaToRomaji
    {
        /// <summary>
        /// Hiragana to romaji table
        /// </summary>
        public static readonly IDictionary<string, string> Table =
            new Dictionary<string, string>
            {
                {"あ", "a"},
                {"い", "i"},
                {"う", "u"},
                {"え", "e"},
                {"お", "o"},
                {"ゔぁ", "va"},
                {"ゔぃ", "vi"},
                {"ゔ", "vu"},
                {"ゔぇ", "ve"},
                {"ゔぉ", "vo"},
                {"るか", "lka"},
                {"るけ", "lke"},
                {"るき", "lki"},
                {"るく", "lku"},
                {"るこ", "lko"},
                {"すか", "ska"},
                {"すけ", "ske"},
                {"すき", "ski"},
                {"すかん", "skun"},
                {"すく", "sku"},
                {"すこ", "sko"},
                {"か", "ka"},
                {"き", "ki"},
                {"きゃ", "kya"},
                {"きぃ", "kyi"},
                {"きゅ", "kyu"},
                {"く", "ku"},
                {"け", "ke"},
                {"こ", "ko"},
                {"が", "ga"},
                {"ぎ", "gi"},
                {"ぐ", "gu"},
                {"げ", "ge"},
                {"ご", "go"},
                {"ぎゃ", "gya"},
                {"ぎぃ", "gyi"},
                {"ぎゅ", "gyu"},
                {"ぎぇ", "gye"},
                {"ぎょ", "gyo"},
                {"さ", "sa"},
                {"す", "su"},
                {"せ", "se"},
                {"そ", "so"},
                {"ざ", "za"},
                {"ず", "zu"},
                {"ぜ", "ze"},
                {"ぞ", "zo"},
                {"し", "shi"},
                {"しゃ", "sha"},
                {"しゅ", "shu"},
                {"しょ", "sho"},
                {"じ", "ji"},
                {"じゃ", "ja"},
                {"じゅ", "ju"},
                {"じょ", "jo"},
                {"た", "ta"},
                {"ち", "chi"},
                {"ちゃ", "cha"},
                {"ちゅ", "chu"},
                {"ちょ", "cho"},
                {"つ", "tsu"},
                {"て", "te"},
                {"と", "to"},
                {"だ", "da"},
                {"ぢ", "di"},
                {"づ", "du"},
                {"で", "de"},
                {"ど", "do"},
                {"な", "na"},
                {"に", "ni"},
                {"にゃ", "nya"},
                {"にゅ", "nyu"},
                {"にょ", "nyo"},
                {"ぬ", "nu"},
                {"ね", "ne"},
                {"の", "no"},
                {"は", "ha"},
                {"ひ", "hi"},
                {"ふ", "fu"},
                {"へ", "he"},
                {"ほ", "ho"},
                {"ひゃ", "hya"},
                {"ひゅ", "hyu"},
                {"ひょ", "hyo"},
                {"ふぁ", "fa"},
                {"ふぃ", "fi"},
                {"ふぇ", "fe"},
                {"ふぉ", "fo"},
                {"ば", "ba"},
                {"び", "bi"},
                {"ぶ", "bu"},
                {"べ", "be"},
                {"ぼ", "bo"},
                {"びゃ", "bya"},
                {"びゅ", "byu"},
                {"びょ", "byo"},
                {"ぱ", "pa"},
                {"ぴ", "pi"},
                {"ぷ", "pu"},
                {"ぺ", "pe"},
                {"ぽ", "po"},
                {"ぴゃ", "pya"},
                {"ぴゅ", "pyu"},
                {"ぴょ", "pyo"},
                {"ま", "ma"},
                {"み", "mi"},
                {"む", "mu"},
                {"め", "me"},
                {"も", "mo"},
                {"みゃ", "mya"},
                {"みゅ", "myu"},
                {"みょ", "myo"},
                {"や", "ya"},
                {"ゆ", "yu"},
                {"よ", "yo"},
                {"ら", "ra"},
                {"り", "ri"},
                {"る", "ru"},
                {"れ", "re"},
                {"ろ", "ro"},
                {"りゃ", "rya"},
                {"りゅ", "ryu"},
                {"りょ", "ryo"},
                {"わ", "wa"},
                {"を", "wo"},
                {"ん", "n"},
                {"ゐ", "wi"},
                {"ゑ", "we"},
                {"きぇ", "kye"},
                {"きょ", "kyo"},
                {"じぃ", "jyi"},
                {"じぇ", "jye"},
                {"ちぃ", "cyi"},
                {"ちぇ", "che"},
                {"ひぃ", "hyi"},
                {"ひぇ", "hye"},
                {"びぃ", "byi"},
                {"びぇ", "bye"},
                {"ぴぃ", "pyi"},
                {"ぴぇ", "pye"},
                {"みぇ", "mye"},
                {"みぃ", "myi"},
                {"りぃ", "ryi"},
                {"りぇ", "rye"},
                {"にぃ", "nyi"},
                {"にぇ", "nye"},
                {"しぃ", "syi"},
                {"しぇ", "she"},
                {"いぇ", "ye"},
                {"うぁ", "wha"},
                {"うぉ", "who"},
                {"うぃ", "wi"},
                {"うぇ", "we"},
                {"ゔゃ", "vya"},
                {"ゔゅ", "vyu"},
                {"ゔょ", "vyo"},
                {"すぁ", "swa"},
                {"すぃ", "swi"},
                {"すぅ", "swu"},
                {"すぇ", "swe"},
                {"すぉ", "swo"},
                {"くゃ", "qya"},
                {"くゅ", "qyu"},
                {"くょ", "qyo"},
                {"くぁ", "qwa"},
                {"くぃ", "qwi"},
                {"くぅ", "qwu"},
                {"くぇ", "qwe"},
                {"くぉ", "qwo"},
                {"ぐぁ", "gwa"},
                {"ぐぃ", "gwi"},
                {"ぐぅ", "gwu"},
                {"ぐぇ", "gwe"},
                {"ぐぉ", "gwo"},
                {"つぁ", "tsa"},
                {"つぃ", "tsi"},
                {"つぇ", "tse"},
                {"つぉ", "tso"},
                {"てゃ", "tha"},
                {"てぃ", "thi"},
                {"てゅ", "thu"},
                {"てぇ", "the"},
                {"てょ", "tho"},
                {"とぁ", "twa"},
                {"とぃ", "twi"},
                {"とぅ", "twu"},
                {"とぇ", "twe"},
                {"とぉ", "two"},
                {"ぢゃ", "dya"},
                {"ぢぃ", "dyi"},
                {"ぢゅ", "dyu"},
                {"ぢぇ", "dye"},
                {"ぢょ", "dyo"},
                {"でゃ", "dha"},
                {"でぃ", "dhi"},
                {"でゅ", "dhu"},
                {"でぇ", "dhe"},
                {"でょ", "dho"},
                {"どぁ", "dwa"},
                {"どぃ", "dwi"},
                {"どぅ", "dwu"},
                {"どぇ", "dwe"},
                {"どぉ", "dwo"},
                {"ふぅ", "fwu"},
                {"ふゃ", "fya"},
                {"ふゅ", "fyu"},
                {"ふょ", "fyo"},
                {"ぁ", "a"},
                {"ぃ", "i"},
                {"ぇ", "e"},
                {"ぅ", "u"},
                {"ぉ", "o"},
                {"ゃ", "ya"},
                {"ゅ", "yu"},
                {"ょ", "yo"},
                {"っ", ""},
                {"ゕ", "ka"},
                {"ゖ", "ka"},
                {"ゎ", "wa"},
                {"'　'", " "},
                {"んあ", "n'a"},
                {"んい", "n'i"},
                {"んう", "n'u"},
                {"んえ", "n'e"},
                {"んお", "n'o"},
                {"んや", "n'ya"},
                {"んゆ", "n'yu"},
                {"んよ", "n'yo"}
            };
    }

    internal class RomajiToKana
    {
        /// <summary>
        /// Romaji-to-hiragana table
        /// </summary>
        public static readonly IDictionary<string, string> Table =
            new Dictionary<string, string>
            {
                {"a", "あ"},
                {"i", "い"},
                {"u", "う"},
                {"e", "え"},
                {"o", "お"},
                {"yi", "い"},
                {"wu", "う"},
                {"whu", "う"},
                {"xa", "ぁ"},
                {"xi", "ぃ"},
                {"xu", "ぅ"},
                {"xe", "ぇ"},
                {"xo", "ぉ"},
                {"xyi", "ぃ"},
                {"xye", "ぇ"},
                {"ye", "いぇ"},
                {"wha", "うぁ"},
                {"whi", "うぃ"},
                {"whe", "うぇ"},
                {"who", "うぉ"},
                {"wi", "うぃ"},
                {"we", "うぇ"},
                {"va", "ゔぁ"},
                {"vi", "ゔぃ"},
                {"vu", "ゔ"},
                {"ve", "ゔぇ"},
                {"vo", "ゔぉ"},
                {"vya", "ゔゃ"},
                {"vyi", "ゔぃ"},
                {"vyu", "ゔゅ"},
                {"vye", "ゔぇ"},
                {"vyo", "ゔょ"},
                {"ka", "か"},
                {"ki", "き"},
                {"ku", "く"},
                {"ke", "け"},
                {"ko", "こ"},
                {"lka", "るか"},
                {"lke", "るく"},
                {"lki", "るき"},
                {"lku", "るけ"},
                {"lko", "るこ"},
                {"xka", "ヵ"},
                {"xke", "ヶ"},
                {"kya", "きゃ"},
                {"kyi", "きぃ"},
                {"kyu", "きゅ"},
                {"kye", "きぇ"},
                {"kyo", "きょ"},
                {"qya", "くゃ"},
                {"qyu", "くゅ"},
                {"qyo", "くょ"},
                {"qwa", "くぁ"},
                {"qwi", "くぃ"},
                {"qwu", "くぅ"},
                {"qwe", "くぇ"},
                {"qwo", "くぉ"},
                {"qa", "くぁ"},
                {"qi", "くぃ"},
                {"qe", "くぇ"},
                {"qo", "くぉ"},
                {"kwa", "くぁ"},
                {"qyi", "くぃ"},
                {"qye", "くぇ"},
                {"ga", "が"},
                {"gi", "ぎ"},
                {"gu", "ぐ"},
                {"ge", "げ"},
                {"go", "ご"},
                {"gya", "ぎゃ"},
                {"gyi", "ぎぃ"},
                {"gyu", "ぎゅ"},
                {"gye", "ぎぇ"},
                {"gyo", "ぎょ"},
                {"gwa", "ぐぁ"},
                {"gwi", "ぐぃ"},
                {"gwu", "ぐぅ"},
                {"gwe", "ぐぇ"},
                {"gwo", "ぐぉ"},
                {"sa", "さ"},
                {"si", "し"},
                {"shi", "し"},
                {"su", "す"},
                {"se", "せ"},
                {"so", "そ"},
                {"za", "ざ"},
                {"zi", "じ"},
                {"zu", "ず"},
                {"ze", "ぜ"},
                {"zo", "ぞ"},
                {"ji", "じ"},
                {"sya", "しゃ"},
                {"syi", "しぃ"},
                {"syu", "しゅ"},
                {"sye", "しぇ"},
                {"syo", "しょ"},
                {"sha", "しゃ"},
                {"shu", "しゅ"},
                {"she", "しぇ"},
                {"sho", "しょ"},
                {"swa", "すぁ"},
                {"swi", "すぃ"},
                {"swu", "すぅ"},
                {"swe", "すぇ"},
                {"swo", "すぉ"},
                {"ska", "すか"},
                {"ske", "すけ"},
                {"ski", "すき"},
                {"sko", "すこ"},
                {"skun", "すかん"},
                {"sku", "すく"},
                {"zya", "じゃ"},
                {"zyi", "じぃ"},
                {"zyu", "じゅ"},
                {"zye", "じぇ"},
                {"zyo", "じょ"},
                {"ja", "じゃ"},
                {"ju", "じゅ"},
                {"je", "じぇ"},
                {"jo", "じょ"},
                {"jya", "じゃ"},
                {"jyi", "じぃ"},
                {"jyu", "じゅ"},
                {"jye", "じぇ"},
                {"jyo", "じょ"},
                {"ta", "た"},
                {"ti", "ち"},
                {"tu", "つ"},
                {"te", "て"},
                {"to", "と"},
                {"chi", "ち"},
                {"tsu", "つ"},
                {"ltu", "っ"},
                {"xtu", "っ"},
                {"tya", "ちゃ"},
                {"tyi", "ちぃ"},
                {"tyu", "ちゅ"},
                {"tye", "ちぇ"},
                {"tyo", "ちょ"},
                {"cha", "ちゃ"},
                {"chu", "ちゅ"},
                {"che", "ちぇ"},
                {"cho", "ちょ"},
                {"cya", "ちゃ"},
                {"cyi", "ちぃ"},
                {"cyu", "ちゅ"},
                {"cye", "ちぇ"},
                {"cyo", "ちょ"},
                {"tsa", "つぁ"},
                {"tsi", "つぃ"},
                {"tse", "つぇ"},
                {"tso", "つぉ"},
                {"tha", "てゃ"},
                {"thi", "てぃ"},
                {"thu", "てゅ"},
                {"the", "てぇ"},
                {"tho", "てょ"},
                {"twa", "とぁ"},
                {"twi", "とぃ"},
                {"twu", "とぅ"},
                {"twe", "とぇ"},
                {"two", "とぉ"},
                {"da", "だ"},
                {"di", "ぢ"},
                {"du", "づ"},
                {"de", "で"},
                {"do", "ど"},
                {"dya", "ぢゃ"},
                {"dyi", "ぢぃ"},
                {"dyu", "ぢゅ"},
                {"dye", "ぢぇ"},
                {"dyo", "ぢょ"},
                {"dha", "でゃ"},
                {"dhi", "でぃ"},
                {"dhu", "でゅ"},
                {"dhe", "でぇ"},
                {"dho", "でょ"},
                {"dwa", "どぁ"},
                {"dwi", "どぃ"},
                {"dwu", "どぅ"},
                {"dwe", "どぇ"},
                {"dwo", "どぉ"},
                {"na", "な"},
                {"ni", "に"},
                {"nu", "ぬ"},
                {"ne", "ね"},
                {"no", "の"},
                {"nya", "にゃ"},
                {"nyi", "にぃ"},
                {"nyu", "にゅ"},
                {"nye", "にぇ"},
                {"nyo", "にょ"},
                {"ha", "は"},
                {"hi", "ひ"},
                {"hu", "ふ"},
                {"he", "へ"},
                {"ho", "ほ"},
                {"fu", "ふ"},
                {"hya", "ひゃ"},
                {"hyi", "ひぃ"},
                {"hyu", "ひゅ"},
                {"hye", "ひぇ"},
                {"hyo", "ひょ"},
                {"fya", "ふゃ"},
                {"fyu", "ふゅ"},
                {"fyo", "ふょ"},
                {"fwa", "ふぁ"},
                {"fwi", "ふぃ"},
                {"fwu", "ふぅ"},
                {"fwe", "ふぇ"},
                {"fwo", "ふぉ"},
                {"fa", "ふぁ"},
                {"fi", "ふぃ"},
                {"fe", "ふぇ"},
                {"fo", "ふぉ"},
                {"fyi", "ふぃ"},
                {"fye", "ふぇ"},
                {"ba", "ば"},
                {"bi", "び"},
                {"bu", "ぶ"},
                {"be", "べ"},
                {"bo", "ぼ"},
                {"bya", "びゃ"},
                {"byi", "びぃ"},
                {"byu", "びゅ"},
                {"bye", "びぇ"},
                {"byo", "びょ"},
                {"pa", "ぱ"},
                {"pi", "ぴ"},
                {"pu", "ぷ"},
                {"pe", "ぺ"},
                {"po", "ぽ"},
                {"pya", "ぴゃ"},
                {"pyi", "ぴぃ"},
                {"pyu", "ぴゅ"},
                {"pye", "ぴぇ"},
                {"pyo", "ぴょ"},
                {"ma", "ま"},
                {"mi", "み"},
                {"mu", "む"},
                {"me", "め"},
                {"mo", "も"},
                {"mya", "みゃ"},
                {"myi", "みぃ"},
                {"myu", "みゅ"},
                {"mye", "みぇ"},
                {"myo", "みょ"},
                {"ya", "や"},
                {"yu", "ゆ"},
                {"yo", "よ"},
                {"xya", "ゃ"},
                {"xyu", "ゅ"},
                {"xyo", "ょ"},
                {"ra", "ら"},
                {"ri", "り"},
                {"ru", "る"},
                {"re", "れ"},
                {"ro", "ろ"},
                {"rya", "りゃ"},
                {"ryi", "りぃ"},
                {"ryu", "りゅ"},
                {"rye", "りぇ"},
                {"ryo", "りょ"},
                {"r", "る"},
                {"la", "ら"},
                {"li", "り"},
                {"lu", "る"},
                {"le", "れ"},
                {"lo", "ろ"},
                {"lya", "りゃ"},
                {"lyi", "りぃ"},
                {"lyu", "りゅ"},
                {"lye", "りぇ"},
                {"lyo", "りょ"},
                {"p", "ぷ"},
                {"f", "ふ"},
                {"d", "ど"},
                {"q", "く"},
                {"k", "く"},
                {"c", "く"},
                {"m", "む"},
                {"b", "ぶ"},
                {"v", "ぶ"},
                {"y", "い"},
                {"z", "ず"},
                {"h", "ー"},
                {"g", "が"},
                {"j", "じ"},
                {"wa", "わ"},
                {"wo", "を"},
                {"lwe", "ゎ"},
                {"xwa", "ゎ"},
                {"w", "を"},
                {"nn", "ん"},
                {"'n '", "ん"},
                {"xn", "ん"},
                {"n", "ん"},
                {"ltsu", "っ"},
                {"xtsu", "っ"},
                {"l", "る"},
                {"t", "と"},
                {"s", "す"},
                {"x", "っくす"}
            };
    }

    /// <summary>
    /// Romaji methods
    /// </summary>
    public class Romaji
    {
        private bool _ime;
        private readonly string _romaji;
        private readonly KanaTools _tools;

        /* Constructor and settings */

        public Romaji(string romaji)
        {
            _romaji = romaji;
            _tools = new KanaTools();
        }

        public Romaji UseImeMode(bool mode)
        {
            _tools.UseIme(mode);
            _ime = true;
            return this;
        }

        public Romaji UseObsoleteKana(bool mode)
        {
            _tools.UseObsoleteKana(mode);
            return this;
        }

        /* Conversions */

        public string ToHiragana()
        {
            return _ime ? _tools.OnTheFlyToKana(_romaji) : _tools.ToHiragana(_romaji);
        }

        public string ToKatakana()
        {
            return _ime ? _tools.OnTheFlyToKana(_romaji, katakana: true) : _tools.ToKatakana(_romaji);
        }
    }

    /// <summary>
    /// Kana methods
    /// </summary>
    public class Kana
    {
        private readonly string _kana;
        private readonly KanaTools _tools;

        /* Constructor and settings */

        public Kana(string kana)
        {
            _kana = kana;
            _tools = new KanaTools();
        }


        /* Conversions */

        public string ToRomaji()
        {
            return _tools.ToRomaji(_kana);
        }

        public string ToHiragana()
        {
            return _tools.ToHiragana(_kana);
        }

        public string ToKatakana()
        {
            return _tools.ToKatakana(_kana);
        }
    }
}