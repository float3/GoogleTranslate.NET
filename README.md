# GoogleTranslate.NET

### Version 1.1.0

A free and unlimited .NET Library for google translate without an API Key.

**This Library is only for Private, Open-Source, or Academic use.**

**Do not use this Library for commercial use!**

**For commercial use, please get an API key and use official Libraries. https://cloud.google.com/translate/docs/setup.**

# Usage
```cs
using GoogleTranslate;

GoogleTranslator translator = new ();
string targetLang = "eng";
string sourceLang = "de";
List<string> result = translator.Translate("Hi", targetLang, sourceLang);
// or
string result2 = translator.TranslateSingle("Hi", targetLang, sourceLang);
// you can also use automatic language detection
string result3 = translator.TranslateSingle("Hi", targetLang, "auto");
// you can can also cache Translations for less API calls
string result4 = translator.TranslateFromCacheOrNewSingle("Hi", targetLang, "auto");
```

# License
Copyright (C) 2022  3

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.