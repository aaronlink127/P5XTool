using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static P5XLib.LuaDLL;

namespace P5XLib
{
    public enum LuaType
    {
        Nil = 0,
        Boolean = 1,
        LightUserData = 2,
        Number = 3,
        String = 4,
        Table = 5,
        Function = 6,
        UserData = 7,
        Thread = 8
    }

    public static partial class LuaDLL
    {
        private static IntPtr s_libraryHandle = IntPtr.Zero;
        private static string? s_libraryPath;

        public static bool IsLoaded => s_libraryHandle != IntPtr.Zero;
        public static string? LoadedPath => s_libraryPath;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr luaL_newstate_fn();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void lua_close_fn(IntPtr L);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int luaL_loadstring_fn(IntPtr L, [In] byte[] chunk, [MarshalAs(UnmanagedType.LPStr)] string? chunkName);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int luaL_ref_fn(IntPtr L, int t);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int luaL_unref_fn(IntPtr L, int rf);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void lua_call_fn(IntPtr L, int nArgs, int nResults);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_pcall_fn(IntPtr L, int nArgs, int nResults, int errfunc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void lua_openlibs_fn(IntPtr L);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void luaopen_lsqlcipher_fn(IntPtr L);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void lua_createtable_fn(IntPtr L, int narray, int nargs);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_gettable_fn(IntPtr L, int idx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_settable_fn(IntPtr L, int idx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_getfield_fn(IntPtr L, int idx, [MarshalAs(UnmanagedType.LPStr)] string fieldName);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_setfield_fn(IntPtr L, int idx, [MarshalAs(UnmanagedType.LPStr)] string fieldName);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int luaL_loadbuffer_fn(IntPtr L, byte[] str, int size, [MarshalAs(UnmanagedType.LPStr)] string? name);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr lua_tolstring_fn(IntPtr L, int idx, out ulong len);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void lua_settop_fn(IntPtr L, int top);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_gettop_fn(IntPtr L);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_type_fn(IntPtr L, int index);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_ccallback(IntPtr L);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_pushnil_fn(IntPtr L);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_pushvalue_fn(IntPtr L, int idx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_pushcclosure_fn(IntPtr L, lua_ccallback fn, int n);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_pushstring_fn(IntPtr L, [MarshalAs(UnmanagedType.LPStr)] string str);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_pushnumber_fn(IntPtr L, double num);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_pushboolean_fn(IntPtr L, bool b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_pushlstring_fn(IntPtr L, byte[] data, int size);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_objlen_fn(IntPtr L, int idx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_rawseti_fn(IntPtr L, int idx, int n);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_rawgeti_fn(IntPtr L, int idx, int n);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_setmetatable_fn(IntPtr L, int objidx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr lua_newuserdata_fn(IntPtr L, ulong size);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr lua_touserdata_fn(IntPtr L, int idx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr lua_topointer_fn(IntPtr L, int idx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool lua_toboolean_fn(IntPtr L, int idx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate double lua_tonumber_fn(IntPtr L, int idx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int lua_tointeger_fn(IntPtr L, int idx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void lua_insert_fn(IntPtr L, int idx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void lua_remove_fn(IntPtr L, int idx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void luaL_traceback_fn(IntPtr L, IntPtr L1, [MarshalAs(UnmanagedType.LPStr)] string msg, int level);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void lua_error_fn(IntPtr L);

        public static luaL_newstate_fn? luaL_newstate;
        public static lua_close_fn? lua_close;
        public static luaL_loadstring_fn? luaL_loadstring;
        public static luaL_ref_fn? luaL_ref;
        public static luaL_unref_fn? luaL_unref;
        public static lua_call_fn? lua_call;
        public static lua_pcall_fn? lua_pcall;
        public static lua_openlibs_fn? lua_openlibs;
        public static luaopen_lsqlcipher_fn? luaopen_lsqlcipher;
        public static lua_createtable_fn? lua_createtable;
        public static lua_gettable_fn? lua_gettable;
        public static lua_settable_fn? lua_settable;
        public static lua_getfield_fn? lua_getfield;
        public static lua_setfield_fn? lua_setfield;
        public static luaL_loadbuffer_fn? luaL_loadbuffer;
        public static lua_tolstring_fn? lua_tolstring;
        public static lua_settop_fn? lua_settop;
        public static lua_gettop_fn? lua_gettop;
        public static lua_type_fn? lua_type;
        public static lua_pushnil_fn? lua_pushnil;
        public static lua_pushvalue_fn? lua_pushvalue;
        public static lua_pushcclosure_fn? lua_pushcclosure;
        public static lua_pushboolean_fn? lua_pushboolean;
        public static lua_pushnumber_fn? lua_pushnumber;
        public static lua_pushstring_fn? lua_pushstring;
        public static lua_pushlstring_fn? lua_pushlstring;
        public static lua_objlen_fn? lua_objlen;
        public static lua_rawseti_fn? lua_rawseti;
        public static lua_rawgeti_fn? lua_rawgeti;
        public static lua_setmetatable_fn? lua_setmetatable;
        public static lua_newuserdata_fn? lua_newuserdata;
        public static lua_touserdata_fn? lua_touserdata;
        public static lua_topointer_fn? lua_topointer;
        public static lua_toboolean_fn? lua_toboolean;
        public static lua_tonumber_fn? lua_tonumber;
        public static lua_tointeger_fn? lua_tointeger;
        public static lua_insert_fn? lua_insert;
        public static lua_remove_fn? lua_remove;
        public static luaL_traceback_fn? luaL_traceback;
        public static lua_error_fn? lua_error;

        private static T? GetExportDelegate<T>(IntPtr libHandle, string exportName) where T : Delegate
        {
            if (!NativeLibrary.TryGetExport(libHandle, exportName, out var symbol))
                return null;
            return Marshal.GetDelegateForFunctionPointer<T>(symbol);
        }

        public static void Load(string dllPath)
        {
            s_libraryHandle = NativeLibrary.Load(dllPath);
            s_libraryPath = dllPath;

            luaL_newstate = GetExportDelegate<luaL_newstate_fn>(s_libraryHandle, "luaL_newstate");
            lua_close = GetExportDelegate<lua_close_fn>(s_libraryHandle, "lua_close");
            luaL_ref = GetExportDelegate<luaL_ref_fn>(s_libraryHandle, "luaL_ref");
            luaL_unref = GetExportDelegate<luaL_unref_fn>(s_libraryHandle, "luaL_unref");
            lua_call = GetExportDelegate<lua_call_fn>(s_libraryHandle, "lua_call");
            lua_pcall = GetExportDelegate<lua_pcall_fn>(s_libraryHandle, "lua_pcall");
            lua_openlibs = GetExportDelegate<lua_openlibs_fn>(s_libraryHandle, "tolua_openlibs");
            luaopen_lsqlcipher = GetExportDelegate<luaopen_lsqlcipher_fn>(s_libraryHandle, "luaopen_lsqlcipher");
            lua_createtable = GetExportDelegate<lua_createtable_fn>(s_libraryHandle, "lua_createtable");
            lua_gettable = GetExportDelegate<lua_gettable_fn>(s_libraryHandle, "lua_gettable");
            lua_settable = GetExportDelegate<lua_settable_fn>(s_libraryHandle, "lua_settable");
            lua_getfield = GetExportDelegate<lua_getfield_fn>(s_libraryHandle, "lua_getfield");
            lua_setfield = GetExportDelegate<lua_setfield_fn>(s_libraryHandle, "lua_setfield");
            luaL_loadbuffer = GetExportDelegate<luaL_loadbuffer_fn>(s_libraryHandle, "luaL_loadbuffer");
            lua_tolstring = GetExportDelegate<lua_tolstring_fn>(s_libraryHandle, "lua_tolstring");
            lua_settop = GetExportDelegate<lua_settop_fn>(s_libraryHandle, "lua_settop");
            lua_gettop = GetExportDelegate<lua_gettop_fn>(s_libraryHandle, "lua_gettop");
            lua_type = GetExportDelegate<lua_type_fn>(s_libraryHandle, "lua_type");
            lua_pushnil = GetExportDelegate<lua_pushnil_fn>(s_libraryHandle, "lua_pushnil");
            lua_pushvalue = GetExportDelegate<lua_pushvalue_fn>(s_libraryHandle, "lua_pushvalue");
            lua_pushcclosure = GetExportDelegate<lua_pushcclosure_fn>(s_libraryHandle, "lua_pushcclosure");
            lua_pushboolean = GetExportDelegate<lua_pushboolean_fn>(s_libraryHandle, "lua_pushboolean");
            lua_pushnumber = GetExportDelegate<lua_pushnumber_fn>(s_libraryHandle, "lua_pushnumber");
            lua_pushstring = GetExportDelegate<lua_pushstring_fn>(s_libraryHandle, "lua_pushstring");
            lua_pushlstring = GetExportDelegate<lua_pushlstring_fn>(s_libraryHandle, "tolua_pushlstring");
            lua_objlen = GetExportDelegate<lua_objlen_fn>(s_libraryHandle, "lua_objlen");
            lua_rawseti = GetExportDelegate<lua_rawseti_fn>(s_libraryHandle, "lua_rawseti");
            lua_rawgeti = GetExportDelegate<lua_rawgeti_fn>(s_libraryHandle, "lua_rawgeti");
            lua_setmetatable = GetExportDelegate<lua_setmetatable_fn>(s_libraryHandle, "lua_setmetatable");
            lua_newuserdata = GetExportDelegate<lua_newuserdata_fn>(s_libraryHandle, "lua_newuserdata");
            lua_touserdata = GetExportDelegate<lua_touserdata_fn>(s_libraryHandle, "lua_touserdata");
            lua_topointer = GetExportDelegate<lua_topointer_fn>(s_libraryHandle, "lua_topointer");
            lua_toboolean = GetExportDelegate<lua_toboolean_fn>(s_libraryHandle, "lua_toboolean");
            lua_tonumber = GetExportDelegate<lua_tonumber_fn>(s_libraryHandle, "lua_tonumber");
            lua_tointeger = GetExportDelegate<lua_tointeger_fn>(s_libraryHandle, "lua_tointeger");
            lua_insert = GetExportDelegate<lua_insert_fn>(s_libraryHandle, "lua_insert");
            lua_remove = GetExportDelegate<lua_remove_fn>(s_libraryHandle, "lua_remove");
            luaL_traceback = GetExportDelegate<luaL_traceback_fn>(s_libraryHandle, "luaL_traceback");
            lua_error = GetExportDelegate<lua_error_fn>(s_libraryHandle, "lua_error");
        }

        public static void Unload()
        {
            if (!IsLoaded) return;

            NativeLibrary.Free(s_libraryHandle);
            s_libraryHandle = IntPtr.Zero;
            s_libraryPath = null;
        }

        public static void lua_pop(IntPtr luaState, int n)
        {
            if (lua_settop is null || lua_gettop is null) throw new InvalidOperationException("Lua DLL not loaded.");
            lua_settop(luaState, lua_gettop(luaState) - n);
        }
    }
    public class LuaWrap
    {
        private LuaState luaState;
        private int refId;
        public LuaWrap(LuaState luaState, int index)
        {
            this.luaState = luaState;
            refId = luaState.Ref(index);
        }
        private void PushSelf()
        {
            luaState.RawGetI(LuaState.LUA_REGISTRYINDEX, refId);
        }
        public object? this[object? index]
        {
            get
            {
                lock (luaState)
                {
                    PushSelf();
                    luaState.PushAuto(index);
                    luaState.GetTable(-2);
                    var obj = luaState.ToAuto(-1);
                    luaState.Pop(1);
                    return obj;
                }
            }
            set {
                lock (luaState)
                {
                    PushSelf();
                    luaState.PushAuto(index);
                    luaState.PushAuto(value);
                    luaState.SetTable(-3);
                    luaState.Pop(1);
                }
            }
        }
        ~LuaWrap()
        {
            lock (luaState)
            {
                luaState.UnRef(refId);
            }
        }
    }
    public class LuaException(string traceback) : Exception(traceback)
    {
    }
    public class LuaState : IDisposable
    {
        static Dictionary<IntPtr, LuaState> _stateMap = [];
        public IntPtr luaState;
        private readonly object lockObj = new();
        public object? userData;
        private readonly Dictionary<IntPtr, Func<LuaState, int>> lua_callbacks = [];
        public const int LUA_REGISTRYINDEX = -10000;
        public const int LUA_ENVIRONINDEX = -10001;
        public const int LUA_GLOBALSINDEX = -10002;
        public const int LUA_MULTRET = -1;
        public LuaState()
        {
            Open();
        }
        public static LuaState? GetLuaState(IntPtr luaState)
        {
            if (_stateMap.TryGetValue(luaState, out var luaState1))
            {
                return luaState1;
            }
            return null;
        }
        private static int GCFunction(IntPtr L)
        {
            var luaState = GetLuaState(L)!;
            IntPtr udata = luaState.ToUserData(1);
            luaState.lua_callbacks.Remove(udata);
            return 0;
        }
        public void Open()
        {
            luaState = LuaDLL.luaL_newstate!();
            _stateMap.Add(luaState, this);
            CreateTable(0,0);
            LuaDLL.lua_pushcclosure!(luaState, GCFunction, 0);
            lua_setfield!(luaState, -2, "__gc");
            SetField(LUA_REGISTRYINDEX, "CSFunction");
        }
        public void Close()
        {
            LuaDLL.lua_close!(luaState);
            _stateMap.Remove(luaState);
            luaState = IntPtr.Zero;
        }
        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }
        ~LuaState()
        {
            Close();
        }
        public int Ref(int index)
        {
            return LuaDLL.luaL_ref!(luaState, index);
        }
        public void UnRef(int index)
        {
            LuaDLL.luaL_unref!(luaState, index);
        }
        public LuaType GetType(int index)
        {
            return (LuaType)LuaDLL.lua_type!(luaState, index);
        }
        public IntPtr NewUserData(ulong size = 0)
        {
            return lua_newuserdata!(luaState, size);
        }
        public IntPtr ToUserData(int idx)
        {
            return lua_touserdata!(luaState, idx);
        }
        public IntPtr ToPointer(int idx)
        {
            return lua_topointer!(luaState, idx);
        }
        public double ToNumber(int idx)
        {
            return lua_tonumber!(luaState, idx);
        }
        public int ToInteger(int idx)
        {
            return lua_tointeger!(luaState, idx);
        }
        public int PCall(int nArgs, int nResults)
        {
            return LuaDLL.lua_pcall!(luaState, nArgs, nResults, 0);
        }
        private static int OnError(LuaState luaState)
        {
            string err = luaState.ToString(1);
            //Console.WriteLine("Err" + err);
            throw new LuaException(luaState.Traceback(err));
        }
        public void Call(int nArgs, int nResults)
        {
            int errFnIdx = GetTop() - nArgs;
            PushFunction(OnError);
            Insert(errFnIdx);
            if (LuaDLL.lua_pcall!(luaState, nArgs, nResults, errFnIdx) != 0)
            {
                string err = ToString(-1);
                Pop(1);
            }
            Remove(errFnIdx);
        }
        public string Traceback(string err)
        {
            LuaDLL.luaL_traceback!(luaState, luaState, err, 2);
            var str = ToString(-1);
            return str;
            
        }
        public void OpenLibs()
        {
            LuaDLL.lua_openlibs!(luaState);
        }
        public void OpenOtherLibs()
        {
            LuaDLL.luaopen_lsqlcipher!(luaState);
        }
        public void CreateTable(int narray = 0, int nargs = 0)
        {
            LuaDLL.lua_createtable!(luaState, narray, nargs);
        }
        public int LoadString(string chunk, string? name = null)
        {
            var bytes = Encoding.UTF8.GetBytes(chunk);
            return LuaDLL.luaL_loadbuffer!(luaState, bytes, bytes.Length, name ?? chunk);
        }
        public int LoadBuffer(byte[] str, string name = "")
        {
            return LuaDLL.luaL_loadbuffer!(luaState, str, str.Length, name);
        }
        public void PushAuto(object? obj)
        {
            switch (obj)
            {
                case string str:
                    PushString(str);
                    break;
                case double db:
                    PushNumber(db);
                    break;
                case int num:
                    PushNumber(num);
                    break;
                case bool bl:
                    PushBoolean(bl);
                    break;
                default:
                    PushNil();
                    break;
            }
        }
        public LuaWrap ToWrapper(int idx)
        {
            return new LuaWrap(this, idx);
        }
        public object? ToAuto(int index)
        {
            switch (GetType(index))
            {
                case LuaType.Nil:
                    return null;
                case LuaType.Number:
                    return ToNumber(index);
                case LuaType.String:
                    return ToString(index);
                case LuaType.Boolean:
                    return ToBoolean(index);
                case LuaType.LightUserData:
                    return ToUserData(index);
                case LuaType.UserData:
                case LuaType.Table:
                case LuaType.Function:
                    return ToWrapper(index);
                default:
                    return null;
            }
        }
        //public LuaWrap ToWrapper(int index)
        //{

        //}
        public void Error(string errMsg)
        {
            PushString(errMsg);
            LuaDLL.lua_error!(luaState);
        }
        public bool ToBoolean(int index)
        {
            return LuaDLL.lua_toboolean!(luaState, index);
        }
        public string ToString(int index)
        {
            IntPtr strPtr = LuaDLL.lua_tolstring!(luaState, index, out var len);
            if (strPtr == IntPtr.Zero) return "";
            return Marshal.PtrToStringUTF8(strPtr, (int)len) ?? "";
        }
        public int GetTable(int idx)
        {
            return LuaDLL.lua_gettable!(luaState, idx);
        }
        public int SetTable(int idx)
        {
            return LuaDLL.lua_settable!(luaState, idx);
        }
        public int GetField(int idx, string fieldName)
        {
            return LuaDLL.lua_getfield!(luaState, idx, fieldName);
        }
        public int SetField(int idx, string key)
        {
            return LuaDLL.lua_setfield!(luaState, idx, key);
        }
        public int GetGlobal(string fieldName)
        {
            return LuaDLL.lua_getfield!(luaState, LUA_GLOBALSINDEX, fieldName);
        }
        public int SetGlobal(string key)
        {
            return LuaDLL.lua_setfield!(luaState, LUA_GLOBALSINDEX, key);
        }
        public void SetTop(int top)
        {
            LuaDLL.lua_settop!(luaState, top);
        }
        public void Pop(int n)
        {
            LuaDLL.lua_pop(luaState, n);
        }
        public void PushNil()
        {
            LuaDLL.lua_pushnil!(luaState);
        }
        public void PushValue(int idx)
        {
            LuaDLL.lua_pushvalue!(luaState, idx);
        }
        public void PushBoolean(bool b)
        {
            LuaDLL.lua_pushboolean!(luaState, b);
        }
        public void PushNumber(double num)
        {
            LuaDLL.lua_pushnumber!(luaState, num);
        }
        public void PushInteger(int num)
        {
            LuaDLL.lua_pushnumber!(luaState, num);
        }
        public void Require(string name)
        {
            GetGlobal("require");
            PushString(name);
            Call(1, 1);
        }
        private static int UpValueIndex(int idx)
        {
            return LUA_GLOBALSINDEX - idx;
        }
        private static int DispatchImpl(IntPtr L)
        {
            var luaState = GetLuaState(L)!;
            try
            {
                IntPtr id = luaState.ToUserData(UpValueIndex(1));
                return luaState.lua_callbacks[id](luaState);
            }
            catch (Exception e)
            {
                luaState.Error(e.Message);
                return 0;
            }
        }
        public void PushFunction(Func<LuaState, int> action)
        {
            var ptr = NewUserData();
            GetField(LUA_REGISTRYINDEX, "CSFunction");
            SetMetaTable(-2);
            LuaDLL.lua_pushcclosure!(luaState, DispatchImpl, 1);
            lua_callbacks[ptr] = action;
        }
        public void PushString(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            LuaDLL.lua_pushlstring!(luaState, bytes, bytes.Length);
        }
        public void PushBytes(byte[] bytes)
        {
            LuaDLL.lua_pushlstring!(luaState, bytes, bytes.Length);
        }
        public void Insert(int idx)
        {
            LuaDLL.lua_insert!(luaState, idx);
        }
        public void Remove(int idx)
        {
            LuaDLL.lua_remove!(luaState, idx);
        }
        public int GetTop()
        {
            return LuaDLL.lua_gettop!(luaState);
        }
        public int ObjLen(int idx)
        {
            return LuaDLL.lua_objlen!(luaState, idx);
        }
        public int RawSetI(int idx, int n)
        {
            return LuaDLL.lua_rawseti!(luaState, idx, n);
        }
        public int RawGetI(int idx, int n)
        {
            return LuaDLL.lua_rawgeti!(luaState, idx, n);
        }
        public int SetMetaTable(int objidx)
        {
            return LuaDLL.lua_setmetatable!(luaState, objidx);
        }
    }
    public static class Lua
    {
        public static void Init(string gameDir)
        {
            var luaDll = Path.Join(gameDir, "pc/P5X_Data/Plugins/x86_64/tolua.dll");
            LuaDLL.Load(luaDll);
        }
        public static void Reset()
        {
            LuaDLL.Unload();
        }

        // convert to bytes to base64...but the game developers implemented it wrong so i can't just use C# built-in stuff
        private static string Base64Encode(byte[] data)
        {
            const string table = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

            int len = data.Length;
            int outLen = 4 * ((len + 2) / 3);
            char[] result = new char[outLen];

            int si = 0;
            int di = 0;

            while (si < len)
            {
                byte a = data[si++];
                byte b = (si < len) ? data[si++] : (byte)0;
                byte c = (si < len) ? data[si++] : (byte)0;

                result[di++] = table[a >> 2];
                result[di++] = table[((a & 0x03) << 4) | (b >> 4)];

                if (si - 1 <= len)
                {
                    if (si - 2 < len)
                        result[di++] = table[((b & 0x0F) << 2) | (c >> 6)];
                    else
                        result[di++] = '=';
                }

                if (si <= len)
                {
                    if (si - 1 < len)
                        result[di++] = table[c & 0x3F];
                    else
                        result[di++] = '=';
                }
            }
            return new string(result);
        }
        private class UserData
        {
            public Dictionary<string, string> keys = [];
            public string obKey = "sgniWkcalB";
        }
        private static int Key(LuaState luaState)
        {
            var udata = (UserData)luaState.userData!;
            var conf = luaState.ToString(1);
            if (udata.keys.ContainsKey(conf)) return 0;
            var key = luaState.ToString(2);
            udata.keys[conf] = key;
            //Console.WriteLine($"Key {conf} = {key}");
            return 0;
        }
        private static int SecureKey(LuaState luaState)
        {
            var conf = luaState.ToString(1);
            var udata = (UserData)luaState.userData!;
            if (udata.keys.ContainsKey(conf)) return 0;
            var key = luaState.ToString(2);
            byte[] obBytes = Encoding.ASCII.GetBytes(udata.obKey);
            byte[] input = Encoding.ASCII.GetBytes(key);
            byte[] tmp = new byte[input.Length];

            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = (byte)((i ^ input[i] ^ obBytes[i % obBytes.Length]) + 13);
            }
            string keyStr = Base64Encode(tmp);
            udata.keys[conf] = keyStr;
            //Console.WriteLine($"Secure Key {conf} = {keyStr}");
            return 0;
        }
        private static int SetSalt(LuaState luaState)
        {
            var desiredSalt = luaState.ToString(2);
            var udata = (UserData)luaState.userData!;
            udata.obKey = desiredSalt;
            return 0;
        }
        public static Dictionary<string, string> GetKeys()
        {
            UserData udata = new();
            byte[] dbTools = LuaFileUtils.ReadFile("DBTools");
            var dbNames = DB.GetAllDBNames();
            using var luaState = new LuaState();
            luaState.userData = udata;
            luaState.OpenLibs();
            luaState.PushFunction(Key);
            luaState.SetGlobal("_key");
            luaState.PushFunction(SecureKey);
            luaState.SetGlobal("_skey");
            luaState.PushFunction(SetSalt);
            luaState.SetGlobal("_setsalt");
            string tbl = string.Join(",", dbNames.Select(k => $"\"{k}\",\"{k}\""));
            string payload = $$"""
            local dummy
            function dummy()
                return setmetatable({}, {
                    __index = function(t, k)
                        t[k] = dummy()
                        return t[k]
                    end,
                    __call = function(t, ...)
                        return dummy()
                    end
                })
            end
            SQLiteLoader = { 
                GetDBPath = function(dbName)
                    return dbName
                end
            }
            package.preload["sqlite3"] = function()
                return {
                    set_password_salt = _setsalt,
                    open_flags = function(a, b)
                        a = a:sub(1, -7)
                        return {
                            key = function(self, key)
                                _key(a, key)
                            end,
                            key_secure = function(self, key)
                                _skey(a, key)
                            end,
                            rows = function(self)
                                return function()
                                    return nil
                                end
                            end
                        }
                    end
                }
            end
            ConfigDBMap = dummy()
            ConfigDBMap.GetConfigDBMapForLua = function()
                return {
                    ToTable = function(self)
                        return {{{tbl}}}
                    end
                }
            end
            """;
            luaState.LoadString(payload);
            luaState.Call(0, 0);
            luaState.LoadBuffer(dbTools, "DBTools");
            luaState.Call(0, 1);
            luaState.GetField(-1, "ConfConnect");
            luaState.Call(0, 0);
            if (udata.keys.Count == 0)
            {
                foreach (var dbName in dbNames)
                {
                    luaState.GetField(-1, "TableExist");
                    luaState.PushString(dbName);
                    luaState.PushString(dbName);
                    luaState.Call(2, 0);
                }
            }
            return udata.keys;
        }
    }
}
