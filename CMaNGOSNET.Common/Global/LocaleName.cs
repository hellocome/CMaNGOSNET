using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.Common.Global
{

    public class LocaleName
    {
        public static string[] localeNames = {
              "enUS",
              "koKR",
              "frFR",
              "deDE",
              "zhCN",
              "zhTW",
              "esES",
              "esMX",
              "ruRU"
            };

        public static LocaleConstant GetLocaleByName(string name)
        {
            for (int i = 0; i < localeNames.Length; i++)
                if (name == localeNames[i])
                    return (LocaleConstant)i;

            return LocaleConstant.LOCALE_enUS;                                     // including enGB case
        }

        public static LocaleConstant GetLocaleByMessageByteName(byte[] byteName)
        {
            string name = GetLocaleNameFromByMessageByteName(byteName);

            return GetLocaleByName(name);
        }

        public static string GetLocaleNameFromByMessageByteName(byte[] byteName)
        {
            return System.Text.Encoding.Default.GetString(byteName.Reverse().ToArray());
        }
    }
}
