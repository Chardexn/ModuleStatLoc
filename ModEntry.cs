namespace ModuleStatLoc
{
    /// <summary>
    /// Mod 入口。Init / OnEnable 在主菜单运行；补丁由加载器自动应用。
    /// </summary>
    public class ModEntry : IMod
    {
        public const string MOD_ID = "module_stat_loc";

        public static ModEntry Instance { get; private set; }
        public ModManifest Manifest { get; private set; }
        public ModLog Log { get; private set; }

        #region MARK: 生命周期
        public void Init(ModManifest manifest)
        {
            Instance = this;
            Manifest = manifest;
            Log = new ModLog(manifest);
            StatLocConfig.Define();
            Log.Log($"{manifest.Name} v{manifest.ModVersion} loaded.");
        }

        public void OnEnable()
        {
            ModHook.OnModAssetsLoaded += OnAssetsLoaded;
        }

        public void OnDisable()
        {
            ModHook.OnModAssetsLoaded -= OnAssetsLoaded;
            ModHelper.RemoveDebugFunctions(MOD_ID);
        }
        #endregion

        #region MARK: 钩子处理
        private void OnAssetsLoaded(LoadedMod mod)
        {
            if (mod.Manifest.ID != MOD_ID)
            {
                return;
            }

            ModHelper.AddDebugFunction(
                MOD_ID,
                ModHelper.DEBUG_CATEGORY_COMMAND,
                L("dump_name"),
                LocDump.Run);
        }
        #endregion

        #region MARK: 本地化
        public static string L(string key, params object[] args)
        {
            return ModHelper.GetLocalized(MOD_ID, key, args);
        }
        #endregion
    }
}
