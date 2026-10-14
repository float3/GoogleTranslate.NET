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
        List<string> test = _translator.Translate("Hi", "de", "en");
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
        // Google's exact wording is its own business and changes; assert the
        // sense of the translation rather than the whole string.
        List<string> test = _translator.Translate("guten morgen", "en", "de");
        if (test.Count > 0 && test[0].Contains("morning", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Pass("Translate Test Passed");
        }
        else
        {
            Assert.Fail("Translate Test Failed, got: " + string.Join(" | ", test));
        }
    }

    [Test]
    public void CacheTest()
    {
        List<string> fresh = _translator.Translate("guten morgen", "en", "de");

        GoogleTranslator.TranslationParams translationParams = new()
        {
            Text = "guten morgen",
            TgtLang = "en",
            SrcLang = "de",
        };

        if (_translator.Cache.TryGetValue(translationParams, out List<string> cached)
            && cached.Count == fresh.Count
            && cached[0] == fresh[0])
        {
            Assert.Pass("Cache Test Passed");
        }
        else
        {
            Assert.Fail("Cache Test Failed");
        }
    }
}