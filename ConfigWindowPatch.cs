using System;
using HarmonyLib;

namespace ModuleStatLoc
{
    /// <summary>
    /// 模组设置面板打开后，把分区名和下拉选项换成当前语言。
    /// 存档仍按枚举名读写，只改显示文本。
    /// </summary>
    [HarmonyPatch(typeof(ModConfigWindow), nameof(ModConfigWindow.Open))]
    internal static class ConfigWindowPatch
    {
        #region MARK: Postfix
        private static void Postfix(LoadedMod mod, CustomUIWindow __result)
        {
            if (mod?.Manifest == null || __result == null || !__result.IsAlive)
            {
                return;
            }

            if (mod.Manifest.ID != ModEntry.MOD_ID)
            {
                return;
            }

            __result.Get("section_General")?.SetText(ModEntry.L("cfg_section"));
            LocalizeDropdown(__result, "term_performance", StatLocConfig.PerformanceKeys);
            LocalizeDropdown(__result, "term_efficiency", StatLocConfig.EfficiencyKeys);
            LocalizeDropdown(__result, "term_quality", StatLocConfig.QualityKeys);
            LocalizeDropdown(__result, "name_suffix", StatLocConfig.SuffixKeys);
        }

        private static void LocalizeDropdown(CustomUIWindow window, string id, string[] keys)
        {
            CustomUIElement dropdown = window.Get(id);
            if (dropdown == null)
            {
                return;
            }

            int index = dropdown.GetIndex();
            string[] labels = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                labels[i] = ModEntry.L(keys[i]);
            }

            dropdown.SetOptions(labels);
            dropdown.SetIndex(Math.Clamp(index, 0, keys.Length - 1));
        }
        #endregion
    }
}
