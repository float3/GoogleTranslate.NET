#region

using GoogleTranslate;

#endregion

namespace TranslateTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ExceptionTest()
    {
        try
        {
            GoogleTranslator translator = new GoogleTranslator();
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
        GoogleTranslator translator = new GoogleTranslator();
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
        GoogleTranslator translator = new GoogleTranslator();
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
}