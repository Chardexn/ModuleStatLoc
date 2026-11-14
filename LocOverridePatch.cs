using System;
using HarmonyLib;
using System.Reflection;

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

            if (!StatWords.IsChineseLocale())
            {
                return;
            }

            if (table == MechanicTable)
            {
                ApplyStatOverride(key, ref __result);
                return;
            }

            if (table != ItemTable)
            {
                return;
            }

            if (IsModuleNameKey(key) || IsInstructionTextKey(key))
            {
                __result = StatWords.Rewrite(__result);
            }
        }

        private static void ApplyStatOverride(string key, ref string __result)
        {
            string name = StatWords.ForMechanicKey(key);
            if (name == key)
            {
                return;
            }

            if (name == __result)
            {
                return;
            }

            switch (StatLocConfig.Suffix)
            {
                case SuffixMode.Original:
                    __result = FormatSuffix(name, __result);
                    break;
                case SuffixMode.English:
                    __result = FormatSuffix(name, ModEntry.L(key + "_en"));
                    break;
                default:
                    __result = name;
                    break;
            }
        }
        #endregion

        #region MARK: 键判断
        private static bool IsModuleNameKey(string key)
        {
            return key == "item_system_module_performance_name"
                || key == "item_system_module_efficiancy_name"
                || key == "item_system_module_quality_name";
        }

        private static bool IsInstructionTextKey(string key)
        {
            return key.EndsWith("_instruction_flavor", StringComparison.Ordinal)
                || key.EndsWith("_instruction_short", StringComparison.Ordinal);
        }
        #endregion

        #region MARK: 后缀
        private static string FormatSuffix(string name, string suffix)
        {
            string format = ModEntry.L(SuffixFormatKey);
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
