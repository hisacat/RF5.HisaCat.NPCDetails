using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

namespace RF5.HisaCat.NPCDetails.Localization
{
    internal static class LocalizationManager
    {
        private static Dictionary<string, LocalizedString> dic = null;

        public const string MainPath = "Localized";
        public static string Load(string key) => Load(key, BootSystem.OptionData.SystemLanguage);
        public static string Load(string key, BootOption.SystemLanguage lang) => Load(MainPath, key, lang);
        public static string Load(string path, string key) => Load(path, key, BootSystem.OptionData.SystemLanguage);
        public static string Load(string path, string key, BootOption.SystemLanguage lang)
        {
            if (dic == null)
                dic = new Dictionary<string, LocalizedString>(StringComparer.OrdinalIgnoreCase);

            if (dic.ContainsKey(path) == false)
                dic.Add(path, new LocalizedString(path));

            return dic[path].Load(key, lang);
        }
    }

    internal class LocalizedString
    {
        public static string GetLangCode(BootOption.SystemLanguage lang)
        {
            switch (lang)
            {
                case BootOption.SystemLanguage.English:
                    return "en";
                case BootOption.SystemLanguage.Japanese:
                    return "ja";
                case BootOption.SystemLanguage.ChineseSimplified:
                    return "chs";
                case BootOption.SystemLanguage.ChineseTraditional:
                    return "cht";
                case BootOption.SystemLanguage.Korean:
                    return "ko";
                case BootOption.SystemLanguage.French:
                    return "fr";
                case BootOption.SystemLanguage.Germen:
                    return "de";
                default:
                    return string.Empty;
            }
        }
        public static void CreateTemplate()
        {
            var data = new Dictionary<string, string>();
            for (int i = 0; i < 3; i++)
                data.Add($"key{i}", "text");

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            for (var lang = BootOption.SystemLanguage.English; lang <= BootOption.SystemLanguage.Germen; lang++)
            {
                var path = System.IO.Path.Combine(Paths.PluginPath, BepInExLoader.GUID, $"{GetLangCode(lang)}.json");
                System.IO.File.WriteAllText(path, json);
            }
        }

        public const BootOption.SystemLanguage FallbackLang = BootOption.SystemLanguage.English;
        private Dictionary<BootOption.SystemLanguage, Dictionary<string, string>> dic = null;
        public readonly string Path = string.Empty;
        public LocalizedString(string path)
        {
            this.Path = path;
            this.dic = new Dictionary<BootOption.SystemLanguage, Dictionary<string, string>>();
        }

        public string Load(string key) => Load(key, BootSystem.OptionData.SystemLanguage);
        public string Load(string key, BootOption.SystemLanguage lang)
        {
            if (this.dic.ContainsKey(lang) == false)
            {
                //Load.
                var datas = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var curPath = System.IO.Path.Combine(Paths.PluginPath, BepInExLoader.GUID, this.Path, $"{GetLangCode(lang)}.json");
                if (System.IO.File.Exists(curPath))
                {
                    var json = System.IO.File.ReadAllText(curPath);
                    datas = JsonConvert.DeserializeObject<Dictionary<string,string>>(json);
                }
                this.dic.Add(lang, datas);
            }

            if (this.dic[lang].ContainsKey(key))
                return this.dic[lang][key];
            else
            {
                //Fallback
                if (lang != FallbackLang)
                    return Load(key, FallbackLang);
                else
                    return key;
            }
        }
    }
}
