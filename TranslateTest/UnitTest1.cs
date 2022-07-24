#region

using GoogleTranslate;

#endregion

namespace TranslateTest;

public class Tests
{
    private GoogleTranslator translator;

    [SetUp]
    public void Setup()
    {
        translator = new();
    }

    [Test]
    public void ExceptionTest()
    {
        try
        {
            translator.TranslateSingle("Test");
            Assert.Pass("Exception Test Passed");
        }
        catch (SuccessException)
        {
            //   
        }
        catch (Exception e)
        {
            Assert.Fail("Exception Test Failed" + e.Message);
        }
    }

    [Test]
    public void ParseTest()
    {
        List<string> test = translator.Translate("Hi", "eng", "de");
        if (test.Count > 0 && !string.IsNullOrEmpty(test[0]))
        {
            Assert.Pass("Parse Test Passed");
        }
        else
        {
            Assert.Fail("Parse Test Failed");
        }
    }

    [Test]
    public void TranslateTest()
    {
        List<string> test = translator.Translate("Hi", "eng", "de");
        if (test[0] == "Hi")
        {
            Assert.Pass("Translate Test Passed");
        }
        else
        {
            Assert.Fail("Translate Test Failed");
        }
    }
    
    [Test]
    public void CacheTest()
    {
        List<string> test = translator.TranslateFromCacheOrNew("Hi", "eng", "de");
        if (test[0] == "Hi")
        {
            Assert.Pass("Translate Test Passed");
        }
        else
        {
            Assert.Fail("Translate Test Failed");
        }
    }
}