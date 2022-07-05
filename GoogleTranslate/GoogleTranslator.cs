#region

using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

#endregion

namespace GoogleTranslate;

// https://github.com/lushan88a/google_trans_new
public class GoogleTranslator
{
    private const string URLBase = "https://translate.google.";
    private const string URLExt = "/_/TranslateWebserverUi/data/batchexecute";
    private readonly int _timeout;

    private readonly string _url;
    private readonly string _urlSuffix;

    private readonly string _urlSuffixDefault = "de";

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

        Dictionary<string, string> headers = new()
        {
            {
                "Referer",
                $"https://translate.google.{_urlSuffix}/"
            },
            {
                "User-Agent",
                // ReSharper disable once StringLiteralTypo
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36"
            },
            {
                "Content-Type",
                "application/x-www-form-urlencoded;charset=utf-8"
            }
        };

        string freq = PackageRPC(text, langSrc, langTgt);
        byte[] bytes = Encoding.ASCII.GetBytes(freq);


        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_url);

        request.Method = "POST";
        request.Referer = headers["Referer"];
        request.UserAgent = headers["User-Agent"];
        request.ContentType = headers["Content-Type"];
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

        dynamic outerJson = JArray.Parse(result);

        dynamic innerJson = JArray.Parse(outerJson[0][2].ToString());

        string first = innerJson[1][0][0][5][0][0];

        translations.Add(first);

        try
        {
            foreach (dynamic item in innerJson[1][0][0][5][0][4])
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

        return translations;
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
        return Translate(text, langSrc, langTgt)[0];
    }
}