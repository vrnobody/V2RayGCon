using System;
using Neo.IronLua;

namespace NeoLuna.Misc
{
    internal static class Patches
    {
        static Patches() { }

        #region properties

        #endregion

        #region public methods
        static public void FixTableStringMath(LuaGlobal g)
        {
            // https://github.com/neolithos/neolua/pull/148/files
            g["string"] = strLibCache;
            g["table"] = tableLibCache;

            // û������.
            // g["math"] = mathLibCache;
        }
        #endregion

        #region private methods
        #region --- Math Lib ----------------------------------------------------------

        readonly static LuaTable mathLibCache = CreateMathLibrary();

        private static LuaTable CreateMathLibrary()
        {
            LuaTable math = new LuaTable
            {
                ["huge"] = LuaLibraryMath.huge,
                ["pi"] = LuaLibraryMath.pi,
                ["e"] = LuaLibraryMath.e,
                ["mininteger"] = LuaLibraryMath.mininteger,
                ["maxinteger"] = LuaLibraryMath.maxinteger,
                ["abs"] = new Func<double, double>(LuaLibraryMath.abs),
                ["acos"] = new Func<double, double>(LuaLibraryMath.acos),
                ["asin"] = new Func<double, double>(LuaLibraryMath.asin),
                ["atan"] = new Func<double, double>(LuaLibraryMath.atan),
                ["atan2"] = new Func<double, double, double>(LuaLibraryMath.atan2),
                ["ceil"] = new Func<double, double>(LuaLibraryMath.ceil),
                ["cos"] = new Func<double, double>(LuaLibraryMath.cos),
                ["cosh"] = new Func<double, double>(LuaLibraryMath.cosh),
                ["deg"] = new Func<double, double>(LuaLibraryMath.deg),
                ["exp"] = new Func<double, double>(LuaLibraryMath.exp),
                ["floor"] = new Func<double, double>(LuaLibraryMath.floor),
                ["fmod"] = new Func<double, double, double>(LuaLibraryMath.fmod),
                ["frexp"] = new Func<double, LuaResult>(LuaLibraryMath.frexp),
                ["ldexp"] = new Func<double, int, double>(LuaLibraryMath.ldexp),
                ["log"] = new Func<double, double, double>(LuaLibraryMath.log),
                ["max"] = new Func<double[], double>(LuaLibraryMath.max),
                ["min"] = new Func<double[], double>(LuaLibraryMath.min),
                ["modf"] = new Func<double, LuaResult>(LuaLibraryMath.modf),
                ["pow"] = new Func<double, double, double>(LuaLibraryMath.pow),
                ["rad"] = new Func<double, double>(LuaLibraryMath.rad),
                ["random"] = new Func<object, object, object>(LuaLibraryMath.random),
                ["randomseed"] = new Action<object>(LuaLibraryMath.randomseed),
                ["sin"] = new Func<double, double>(LuaLibraryMath.sin),
                ["sinh"] = new Func<double, double>(LuaLibraryMath.sinh),
                ["sqrt"] = new Func<double, double>(LuaLibraryMath.sqrt),
                ["tan"] = new Func<double, double>(LuaLibraryMath.tan),
                ["tanh"] = new Func<double, double>(LuaLibraryMath.tanh),
                ["type"] = new Func<object, string>(LuaLibraryMath.type),
                ["tointeger"] = new Func<object, object>(LuaLibraryMath.tointeger),
                ["ult"] = new Func<long, long, bool>(LuaLibraryMath.ult)
            };

            return math;
        }
        #endregion

        #region --- String Lib ------------------------------------------------

        //Delegates for types that can't be stuffed into generics
        private delegate LuaResult byteDelg(string s, int? i = null, int? j = null);
        private delegate string charDelg(params int[] chars);
        private delegate string formatDelg(string format, params object[] prms);
        static readonly LuaTable strLibCache = CreateStringLibrary();

        private static LuaTable CreateStringLibrary()
        {
            LuaTable str = new LuaTable
            {
                ["byte"] = new byteDelg(LuaLibraryString.@byte),
                ["char"] = new charDelg(LuaLibraryString.@char),
                ["dump"] = new Func<Delegate, string>(LuaLibraryString.dump),
                ["find"] = new Func<string, string, int, bool, LuaResult>(LuaLibraryString.find),
                ["format"] = new formatDelg(LuaLibraryString.format),
                ["gmatch"] = new Func<string, string, LuaResult>(LuaLibraryString.gmatch),
                ["gsub"] = new Func<string, string, object, int, LuaResult>(LuaLibraryString.gsub),
                ["len"] = new Func<string, int>(LuaLibraryString.len),
                ["lower"] = new Func<string, string>(LuaLibraryString.lower),
                ["match"] = new Func<string, string, int, LuaResult>(LuaLibraryString.match),
                ["rep"] = new Func<string, int, string, string>(LuaLibraryString.rep),
                ["reverse"] = new Func<string, string>(LuaLibraryString.reverse),
                ["sub"] = new Func<string, int, int, string>(LuaLibraryString.sub),
                ["upper"] = new Func<string, string>(LuaLibraryString.upper)
            };
            return str;
        }
        #endregion

        #region --- Table Lib ------------------------------------------------

        private delegate string concatDelg(
            LuaTable t,
            string sep = null,
            int? i = null,
            int? j = null
        );

        static readonly LuaTable tableLibCache = CreateTableLibrary();

        private static LuaTable CreateTableLibrary()
        {
            LuaTable tbl = new LuaTable
            {
                ["concat"] = new concatDelg(LuaTable.concat),
                ["insert"] = new Action<LuaTable, object, object>(LuaTable.insert),
                ["move"] = new Action<LuaTable, int, int, int, LuaTable>(LuaTable.move),
                ["pack"] = new Func<object[], LuaTable>(LuaTable.pack),
                ["remove"] = new Func<LuaTable, int, object>(LuaTable.remove),
                ["sort"] = new Action<LuaTable, object>(LuaTable.sort),
                ["unpack"] = new Func<LuaTable, LuaResult>(LuaTable.unpack)
            };
            return tbl;
        }
        #endregion

        #endregion

        #region protected methods

        #endregion
    }
}
