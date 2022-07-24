#region

using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

// ReSharper disable StringLiteralTypo

#endregion

namespace GoogleTranslate;

// https://github.com/lushan88a/google_trans_new
// https://kovatch.medium.com/deciphering-google-batchexecute-74991e4e446c
public class GoogleTranslator
{
    private const string URLBase = "https://translate.google.";
    private const string URLExt = "/_/TranslateWebserverUi/data/batchexecute";

    private readonly int _timeout;
    private readonly string _url;
    private readonly string _urlSuffix;
    private readonly string _urlSuffixDefault = "de";

    public Dictionary<string, List<string>> Cache = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="GoogleTranslator" /> class.
    /// </summary>
    /// <param name="urlSuffix"></param>
    /// <param name="timeout"></param>
    public GoogleTranslator(string urlSuffix = "en", int timeout = 100000)
    {
        _urlSuffix = Constants.URLSuffixes.Contains(urlSuffix) ? urlSuffix : _urlSuffixDefault;
        _url = URLBase + _urlSuffix + URLExt;
        _timeout = timeout;
    }

    // https://kovatch.medium.com/deciphering-google-batchexecute-74991e4e446c
    /// <summary>
    ///     Formats the request content.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="langTgt"></param>
    /// <param name="langSrc"></param>
    /// <returns></returns>
    private string PackageRPC(string text, string langTgt = "auto", string langSrc = "auto")
    {
        string googleTTSRPC = "MkEWBc";
        string rpc =
            $"[[[\"{googleTTSRPC}\",\"[[\\\"{text.Trim()}\\\",\\\"{langSrc}\\\",\\\"{langTgt}\\\",true],[1]]\",null,\"generic\"]]]";

        rpc = HttpUtility.UrlEncode(rpc);

        return $"f.req={rpc}&";
    }

    /// <summary>
    ///     Translates the specified text.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="langTgt"></param>
    /// <param name="langSrc"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public List<string> Translate(string text, string langTgt = "auto", string langSrc = "auto")
    {
        List<string> translations = new();

        if (!Constants.Languages.ContainsKey(langSrc)) langSrc = "auto";

        if (!Constants.Languages.ContainsKey(langTgt)) langTgt = "auto";

        if (text.Length >= 5000) throw new Exception("Warning: Can only detect less than 5000 characters");

        if (text.Length == 0) throw new Exception("Empty Input");

        string freq = PackageRPC(text, langSrc, langTgt);
        byte[] bytes = Encoding.ASCII.GetBytes(freq);


#pragma warning disable SYSLIB0014
        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_url);
#pragma warning restore SYSLIB0014

        request.Method = "POST";
        request.Referer = $"{URLBase}{_urlSuffix}/";
        request.UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
        request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
        request.ContentLength = bytes.Length;
        request.Timeout = _timeout;

        Stream requestStream = request.GetRequestStream();
        requestStream.Write(bytes, 0, bytes.Length);
        requestStream.Close();

        HttpWebResponse response = (HttpWebResponse) request.GetResponse();

        Stream responseStream = response.GetResponseStream();

        StreamReader reader = new(responseStream, Encoding.Default);
        string result = reader.ReadToEnd();
        reader.Close();
        responseStream.Close();
        response.Close();


        List<string> arr = result.Split('\n').ToList();

        result = arr[2].Trim();

        JArray outerJson = JArray.Parse(result);

        JArray innerJson = JArray.Parse(outerJson[0][2].ToString());

        string first = innerJson[1][0][0][5][0][0].ToString();

        translations.Add(first);

        try
        {
            foreach (JToken item in innerJson[1][0][0][5][0][4])
            {
                if (item[0] != null && !translations.Contains(item[0].ToString()))
                {
                    translations.Add(item[0].ToString());
                }
            }
        }
        catch
        {
            // ignored
        }

        if (!Cache.ContainsKey(text + langSrc + langTgt))
        {
            Cache.Add(text + langSrc + langTgt, translations);
        }
        else
        {
            Cache[text + langSrc + langTgt] = translations;
        }


        return translations;
    }

    /// <summary>
    ///     Translates the specified text or retrieves it from Cache.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="langTgt"></param>
    /// <param name="langSrc"></param>
    /// <returns></returns>
    public List<string> TranslateFromCacheOrNew(string text, string langTgt = "auto", string langSrc = "auto")
    {
        if (Cache.ContainsKey(text + langSrc + langTgt))
        {
            return Cache[text + langSrc + langTgt];
        }

        return Translate(text, langSrc, langTgt);
    }

    /// <summary>
    ///     Translates the specified text.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="langTgt"></param>
    /// <param name="langSrc"></param>
    /// <returns></returns>
    public string TranslateSingle(string text, string langTgt = "auto", string langSrc = "auto")
    {
        return Translate(text, langSrc, langTgt).First();
    }

    /// <summary>
    ///     Translates the specified text or retrieves it from Cache.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="langTgt"></param>
    /// <param name="langSrc"></param>
    /// <returns></returns>
    public string TranslateFromCacheOrNewSingle(string text, string langTgt = "auto", string langSrc = "auto")
    {
        return TranslateFromCacheOrNew(text, langSrc, langTgt).First();
    }
}

public static class Constants
{
    public static readonly Dictionary<string, string> Languages = new()
    {
        {"af", "afrikaans"},
        {"sq", "albanian"},
        {"am", "amharic"},
        {"ar", "arabic"},
        {"hy", "armenian"},
        {"az", "azerbaijani"},
        {"eu", "basque"},
        {"be", "belarusian"},
        {"bn", "bengali"},
        {"bs", "bosnian"},
        {"bg", "bulgarian"},
        {"ca", "catalan"},
        {"ceb", "cebuano"},
        {"ny", "chichewa"},
        {"zh-cn", "chinese (simplified)"},
        {"zh-tw", "chinese (traditional)"},
        {"co", "corsican"},
        {"hr", "croatian"},
        {"cs", "czech"},
        {"da", "danish"},
        {"nl", "dutch"},
        {"en", "english"},
        {"eo", "esperanto"},
        {"et", "estonian"},
        {"tl", "filipino"},
        {"fi", "finnish"},
        {"fr", "french"},
        {"fy", "frisian"},
        {"gl", "galician"},
        {"ka", "georgian"},
        {"de", "german"},
        {"el", "greek"},
        {"gu", "gujarati"},
        {"ht", "haitian creole"},
        {"ha", "hausa"},
        {"haw", "hawaiian"},
        {"iw", "hebrew"},
        {"he", "hebrew"},
        {"hi", "hindi"},
        {"hmn", "hmong"},
        {"hu", "hungarian"},
        {"is", "icelandic"},
        {"ig", "igbo"},
        {"id", "indonesian"},
        {"ga", "irish"},
        {"it", "italian"},
        {"ja", "japanese"},
        {"jw", "javanese"},
        {"kn", "kannada"},
        {"kk", "kazakh"},
        {"km", "khmer"},
        {"ko", "korean"},
        {"ku", "kurdish (kurmanji)"},
        {"ky", "kyrgyz"},
        {"lo", "lao"},
        {"la", "latin"},
        {"lv", "latvian"},
        {"lt", "lithuanian"},
        {"lb", "luxembourgish"},
        {"mk", "macedonian"},
        {"mg", "malagasy"},
        {"ms", "malay"},
        {"ml", "malayalam"},
        {"mt", "maltese"},
        {"mi", "maori"},
        {"mr", "marathi"},
        {"mn", "mongolian"},
        {"my", "myanmar (burmese)"},
        {"ne", "nepali"},
        {"no", "norwegian"},
        {"or", "odia"},
        {"ps", "pashto"},
        {"fa", "persian"},
        {"pl", "polish"},
        {"pt", "portuguese"},
        {"pa", "punjabi"},
        {"ro", "romanian"},
        {"ru", "russian"},
        {"sm", "samoan"},
        {"gd", "scots gaelic"},
        {"sr", "serbian"},
        {"st", "sesotho"},
        {"sn", "shona"},
        {"sd", "sindhi"},
        {"si", "sinhala"},
        {"sk", "slovak"},
        {"sl", "slovenian"},
        {"so", "somali"},
        {"es", "spanish"},
        {"su", "sundanese"},
        {"sw", "swahili"},
        {"sv", "swedish"},
        {"tg", "tajik"},
        {"ta", "tamil"},
        {"tt", "tatar"},
        {"te", "telugu"},
        {"th", "thai"},
        {"tr", "turkish"},
        {"tk", "turkmen"},
        {"uk", "ukrainian"},
        {"ur", "urdu"},
        {"ug", "uyghur"},
        {"uz", "uzbek"},
        {"vi", "vietnamese"},
        {"cy", "welsh"},
        {"xh", "xhosa"},
        {"yi", "yiddish"},
        {"yo", "yoruba"},
        {"zu", "zulu"}
    };

    public static readonly HashSet<string> URLSuffixes = new(new[]
    {
        "ac", "ad", "ae", "al", "am", "as", "at", "az", "ba", "be",
        "bf", "bg", "bi", "bj", "bs", "bt", "by", "ca", "cat", "cc", "cd", "cf", "cg", "ch", "ci", "cl", "cm", "cn",
        "co.ao", "co.bw", "co.ck", "co.cr", "co.id", "co.il", "co.in", "co.jp", "co.ke", "co.kr", "co.ls", "co.ma",
        "co.mz",
        "co.nz", "co.th", "co.tz", "co.ug", "co.uk", "co.uz", "co.ve", "co.vi", "co.za", "co.zm", "co.zw", "co",
        "com.af",
        "com.ag", "com.ai", "com.ar", "com.au", "com.bd", "com.bh", "com.bn", "com.bo", "com.br", "com.bz", "com.co",
        "com.cu", "com.cy", "com.do", "com.ec", "com.eg", "com.et", "com.fj", "com.gh", "com.gi", "com.gt", "com.hk",
        "com.jm", "com.kh", "com.kw", "com.lb", "com.lc", "com.ly", "com.mm", "com.mt", "com.mx", "com.my", "com.na",
        "com.ng", "com.ni", "com.np", "com.om", "com.pa", "com.pe", "com.pg", "com.ph", "com.pk", "com.pr", "com.py",
        "com.qa", "com.sa", "com.sb", "com.sg", "com.sl", "com.sv", "com.tj", "com.tr", "com.tw", "com.ua", "com.uy",
        "com.vc", "com.vn", "com", "cv", "cx", "cz", "de", "dj", "dk", "dm", "dz", "ee", "es", "eu", "fi", "fm", "fr",
        "ga",
        "ge", "gf", "gg", "gl", "gm", "gp", "gr", "gy", "hn", "hr", "ht", "hu", "ie", "im", "io", "iq", "is", "it",
        "je",
        "jo", "kg", "ki", "kz", "la", "li", "lk", "lt", "lu", "lv", "md", "me", "mg", "mk", "ml", "mn", "ms", "mu",
        "mv",
        "mw", "ne", "nf", "nl", "no", "nr", "nu", "pl", "pn", "ps", "pt", "ro", "rs", "ru", "rw", "sc", "se", "sh",
        "si",
        "sk", "sm", "sn", "so", "sr", "st", "td", "tg", "tk", "tl", "tm", "tn", "to", "tt", "us", "vg", "vu", "ws"
    });
}