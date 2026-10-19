using UnityAssetLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5XLib
{
    public static class GameMgr
    {
        static public bool IsInit { get; private set; }
        static public bool IsGlobal { get; private set; }
        static public bool IsSea { get; private set; }
        static public bool IsCbt { get; private set; }
        static private bool isScriptInit = false;
        static private bool ExitRegister = false;
        static public string? ClientPath { get; private set; }
        static public bool TryInit(string gamePath, Action<string>? progressCb = null)
        {
            if (IsInit) return false;
            if (!isScriptInit)
            {
                UnityAssetLib.ObjectFactory.RegisterMono<UIMuitipleGradientParam>("MUEngine.UI.UIMuitipleGradientParam");
                UnityAssetLib.ObjectFactory.RegisterMono<MUActorMeshExportInfo>("MUActorMeshExportInfo");
                isScriptInit = true;
            }
            if (!ExitRegister)
            {
                AppDomain.CurrentDomain.ProcessExit += OnExit;
				ExitRegister = true;
			}
            string subClient = Path.Join(gamePath, "client");
            if (Path.Exists(subClient))
            {
                gamePath = subClient;
            }
            ClientPath = gamePath;
            progressCb?.Invoke("VFileSystem");
            if (!VFileSystem.TryInit(gamePath)) return false;
            IsGlobal = VFileSystem.Exists("Config/Windows/ConfTranslationJP.bytes");
            IsSea = !IsGlobal && VFileSystem.Exists("Config/Windows/ConfTranslationCN.bytes");
            IsCbt = VFileSystem.Exists("Bundles/Windows/assetMapName.fb");
            string unityPath = Path.Join(gamePath, "pc");
            progressCb?.Invoke("AssetBundle");
            UnityAssetManager.Init(unityPath, "P5X");
            progressCb?.Invoke("AssetBundleUtils");
            AssetBundleUtils.Init();
            progressCb?.Invoke("AssetManager");
            AssetManager.Init();
            progressCb?.Invoke("LuaFileUtils");
            LuaFileUtils.Init();
            progressCb?.Invoke("Lua");
            Lua.Init(gamePath);
            progressCb?.Invoke("DB");
            DB.TryInit();
            progressCb?.Invoke("I18NAssetMap");
            I18NAssetMap.Init();
            progressCb?.Invoke("Done");
            IsInit = true;
            return true;
        }
        static private void OnExit(object? sender, EventArgs eventArgs)
        {
            Reset();
        }
        static public void Reset()
        {
            if (!IsInit) return;
            IsInit = false;
            IsGlobal = false;
            DB.Reset();
            AssetBundleUtils.Reset();
            AssetManager.Reset();
            UnityAssetManager.Reset();
            LuaFileUtils.Reset();
            Lua.Reset();
            VFileSystem.Reset();
            I18NAssetMap.Reset();
        }
    }
}
