#region

using System;
using System.Collections.Generic;
using GoogleTranslate;
using NUnit.Framework;

#endregion

namespace TranslateTest;

public class Tests
{
    private GoogleTranslator _translator;

    [SetUp]
    public void Setup()
    {
        _translator = new();
    }

    [Test]
    public void ExceptionTest()
    {
        try
        {
            _translator.Translate("Test");
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
        List<string> test = _translator.Translate("Hi", "eng", "de");
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
        List<string> test = _translator.Translate("Hi", "eng", "de");
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
        _translator.Translate("Hi", "eng", "de");

        GoogleTranslator.TranslationParams translationParams = new()
        {
            Text = "Hi",
            TgtLang = "eng",
            SrcLang = "de",
        };

        if (_translator.Cache[translationParams][0] == "Hi")
        {
            Assert.Pass("Translate Test Passed");
        }
        else
        {
            Assert.Fail("Translate Test Failed");
        }
    }
}