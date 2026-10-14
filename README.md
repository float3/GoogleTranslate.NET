# GoogleTranslate.NET

### version 1.1.0

a free and unlimited .NET library for google translate without an API key.

**this library is only for private, open-source, or academic use.**

**do not use this library for commercial use!**

**for commercial use, please get an API key and use official libraries. https://cloud.google.com/translate/docs/setup.**

# usage
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

# license
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