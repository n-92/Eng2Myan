## Eng2Myan: Phonetic Zawgyi & Unicode Utility

A C# Windows Forms utility that transliterates English phonetic (Burglish) input into both Zawgyi and Unicode Burmese text simultaneously.

## Requirements

To run this application, you will need:

1. **Windows OS:** This is a Windows Forms application and runs on Windows 10 or Windows 11.

2. **.NET Desktop Runtime:** The application requires the .NET Desktop Runtime. If it's not on your system, you will be prompted to install it.

   * *Based on our previous errors, this project requires **.NET 8.0 Desktop Runtime**.*

3. **Fonts:** The application is designed to show two different Burmese encodings. For the text to display correctly, you need:

   * **Zawgyi-One Font:** Required for the "Zawgyi Output" box. This font is **not** standard on Windows and must be installed manually.

   * **Myanmar Text Font:** Required for the "Unicode Output" box. This font is included by default on modern Windows systems.

## How It Works

This application functions by chaining two powerful conversion engines:

1. **`BurglishIME.cs`:** A C# port of the classic `burglish.js` transliteration engine. It converts English phonetic syllables (like `laip`) into Zawgyi-encoded text (like `လိပ္`).

2. **`ZawgyiConverter.cs`:** A C# port of the Python-based Rabbit Converter rules. It takes the Zawgyi text from the first engine and transforms it into the correct, modern Unicode standard (e.g., `အင်္ဂလိပ်`).

## Acknowledgements

The core logic is a direct port of two separate projects:

* **Burglish IME:** The original JavaScript logic for the English-to-Zawgyi phonetic transliteration.

* **Rabbit Converter:** The powerful conversion ruleset used in `zg-uni-file-converter.py` for the Zawgyi-to-Unicode logic.
