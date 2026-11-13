using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace ModuleStatLoc
{
    /// <summary>
    /// 中文语言下覆盖模组三项属性名、对应物品名和机器说明书。属性短名才加括号后缀。
    /// </summary>
    [HarmonyPatch]
    internal static class LocOverridePatch
    {
        private const string MechanicTable = "Mechanic";
        private const string ItemTable = "Item";
        private const string SuffixFormatKey = "suffix_format";
        private const string DefaultSuffixFormat = "{0}（{1}）";

        private static readonly HashSet<string> StatKeys = new HashSet<string>
        {
            "mech_module_stat_performance",
            "mech_module_stat_efficiency",
            "mech_module_stat_quality"
        };

        private static readonly HashSet<string> ItemKeys = new HashSet<string>
        {
            "item_system_module_performance_name",
            "item_system_module_efficiancy_name",
            "item_system_module_quality_name",
            "item_alarm_instruction_flavor",
            "item_desequencer_instruction_flavor",
            "item_furnace_instruction_flavor",
            "item_projector_instruction_flavor",
            "item_moisture_farm_instruction_flavor",
            "item_water_purifier_instruction_flavor",
            "item_age_well_instruction_flavor",
            "item_evaporator_instruction_flavor",
            "item_3d_printer_instruction_flavor",
            "item_hydroponic_instruction_flavor",
            "item_dna_manipulator_instruction_flavor"
        };

        private static readonly Dictionary<string, string> TextCache = new Dictionary<string, string>();
        private static string cachedLocale;
        private static bool isChinese;

        #region MARK: 补丁目标
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(LocHelper), "Get", new[]
            {
                typeof(string),
                typeof(string),
                typeof(object[])
            });
        }
        #endregion

        #region MARK: Postfix
        private static void Postfix(string table, string key, ref string __result)
        {
            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(__result))
            {
                return;
            }

            if (!IsChineseLocale())
            {
                return;
            }

            if (table == MechanicTable && StatKeys.Contains(key))
            {
                ApplyStatOverride(key, ref __result);
                return;
            }

            if (table == ItemTable && ItemKeys.Contains(key))
            {
                string replacement = Loc(key);
                if (replacement != key)
                {
                    __result = replacement;
                }
            }
        }

        private static void ApplyStatOverride(string key, ref string __result)
        {
            string name = Loc(key);
            if (name == key)
            {
                return;
            }

            switch (StatLocConfig.Suffix)
            {
                case SuffixMode.Original:
                    __result = FormatSuffix(name, __result);
                    break;
                case SuffixMode.English:
                    __result = FormatSuffix(name, Loc(key + "_en"));
                    break;
                default:
                    __result = name;
                    break;
            }
        }
        #endregion

        #region MARK: 语言与缓存
        private static bool IsChineseLocale()
        {
            string code = LocHelper.GetCurrentLocaleCode();
            if (code != cachedLocale)
            {
                cachedLocale = code;
                isChinese = !string.IsNullOrEmpty(code) && code.StartsWith("zh", StringComparison.Ordinal);
                TextCache.Clear();
            }

            return isChinese;
        }

        private static string Loc(string csvKey)
        {
            if (TextCache.TryGetValue(csvKey, out string cached))
            {
                return cached;
            }

            string value = ModHelper.GetLocalized(ModEntry.MOD_ID, csvKey);
            if (value != csvKey)
            {
                TextCache[csvKey] = value;
            }

            return value;
        }

        private static string FormatSuffix(string name, string suffix)
        {
            string format = Loc(SuffixFormatKey);
            if (format == SuffixFormatKey)
            {
                format = DefaultSuffixFormat;
            }

            try
            {
                return string.Format(format, name, suffix);
            }
            catch (FormatException)
            {
                return name + "（" + suffix + "）";
            }
        }
        #endregion
    }
}
