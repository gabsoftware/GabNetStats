using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace GabNetStats
{
    internal static class LanguageManager
    {
        //
        //  Constants
        //
        internal const string DEFAULT_LANGUAGE = "en";

        internal static CultureInfo[] GetAvailableLanguages()
        {
            var cultures = new List<CultureInfo> { CultureInfo.GetCultureInfo(DEFAULT_LANGUAGE) };

            foreach (CultureInfo candidate in CultureInfo.GetCultures(CultureTypes.SpecificCultures | CultureTypes.NeutralCultures))
            {
                if (string.IsNullOrEmpty(candidate.Name)
                    || string.Equals(candidate.Name, DEFAULT_LANGUAGE, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string resourcesPath = Path.Combine(Application.StartupPath, candidate.Name, "GabNetStats.resources.dll");
                if (File.Exists(resourcesPath))
                {
                    cultures.Add(candidate);
                }
            }

            return cultures.ToArray();
        }

        //
        //  Returns the language to use when no explicit choice has been saved yet: the OS UI
        //  culture if a matching localization is available, otherwise the default (English).
        //
        internal static string GetDefaultLanguage()
        {
            CultureInfo osCulture = CultureInfo.InstalledUICulture;

            foreach (CultureInfo available in GetAvailableLanguages())
            {
                if (string.Equals(available.TwoLetterISOLanguageName, osCulture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
                {
                    return available.Name;
                }
            }

            return DEFAULT_LANGUAGE;
        }

        internal static void ApplyLanguage(string cultureName)
        {
            string effectiveCultureName = string.IsNullOrEmpty(cultureName) ? GetDefaultLanguage() : cultureName;
            CultureInfo culture = CultureInfo.GetCultureInfo(effectiveCultureName);

            CultureInfo.DefaultThreadCurrentUICulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
