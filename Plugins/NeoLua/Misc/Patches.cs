using Neo.IronLua;
using System;

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

            // 没这需求.
            // g["math"] = mathLibCache;
        }
        #endregion

        #region private methods
        #region --- Math Lib ----------------------------------------------------------

        readonly static LuaTable mathLibCache = CreateMathLibrary();

        private static LuaTable CreateMathLibrary()
        {
            LuaTable math = new LuaTable();

            math["huge"] = LuaLibraryMath.huge;
            math["pi"] = LuaLibraryMath.pi;
            math["e"] = LuaLibraryMath.e;
            math["mininteger"] = LuaLibraryMath.mininteger;
            math["maxinteger"] = LuaLibraryMath.maxinteger;

            math["abs"] = new Func<double, double>(LuaLibraryMath.abs);
            math["acos"] = new Func<double, double>(LuaLibraryMath.acos);
            math["asin"] = new Func<double, double>(LuaLibraryMath.asin);
            math["atan"] = new Func<double, double>(LuaLibraryMath.atan);
            math["atan2"] = new Func<double, double, double>(LuaLibraryMath.atan2);
            math["ceil"] = new Func<double, double>(LuaLibraryMath.ceil);
            math["cos"] = new Func<double, double>(LuaLibraryMath.cos);
            math["cosh"] = new Func<double, double>(LuaLibraryMath.cosh);
            math["deg"] = new Func<double, double>(LuaLibraryMath.deg);
            math["exp"] = new Func<double, double>(LuaLibraryMath.exp);
            math["floor"] = new Func<double, double>(LuaLibraryMath.floor);
            math["fmod"] = new Func<double, double, double>(LuaLibraryMath.fmod);
            math["frexp"] = new Func<double, Neo.IronLua.LuaResult>(LuaLibraryMath.frexp);
            math["ldexp"] = new Func<double, int, double>(LuaLibraryMath.ldexp);
            math["log"] = new Func<double, double, double>(LuaLibraryMath.log);
            math["max"] = new Func<double[], double>(LuaLibraryMath.max);
            math["min"] = new Func<double[], double>(LuaLibraryMath.min);
            math["modf"] = new Func<double, LuaResult>(LuaLibraryMath.modf);
            math["pow"] = new Func<double, double, double>(LuaLibraryMath.pow);
            math["rad"] = new Func<double, double>(LuaLibraryMath.rad);
            math["random"] = new Func<object, object, object>(LuaLibraryMath.random);
            math["randomseed"] = new Action<object>(LuaLibraryMath.randomseed);
            math["sin"] = new Func<double, double>(LuaLibraryMath.sin);
            math["sinh"] = new Func<double, double>(LuaLibraryMath.sinh);
            math["sqrt"] = new Func<double, double>(LuaLibraryMath.sqrt);
            math["tan"] = new Func<double, double>(LuaLibraryMath.tan);
            math["tanh"] = new Func<double, double>(LuaLibraryMath.tanh);
            math["type"] = new Func<object, string>(LuaLibraryMath.type);
            math["tointeger"] = new Func<object, object>(LuaLibraryMath.tointeger);
            math["ult"] = new Func<long, long, bool>(LuaLibraryMath.ult);

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
            LuaTable str = new LuaTable();

            str["byte"] = new byteDelg(Neo.IronLua.LuaLibraryString.@byte);
            str["char"] = new charDelg(Neo.IronLua.LuaLibraryString.@char);
            str["dump"] = new Func<Delegate, string>(Neo.IronLua.LuaLibraryString.dump);
            str["find"] = new Func<string, string, int, bool, LuaResult>(
                Neo.IronLua.LuaLibraryString.find
            );
            str["format"] = new formatDelg(Neo.IronLua.LuaLibraryString.format);
            str["gmatch"] = new Func<string, string, LuaResult>(
                Neo.IronLua.LuaLibraryString.gmatch
            );
            str["gsub"] = new Func<string, string, object, int, LuaResult>(
                Neo.IronLua.LuaLibraryString.gsub
            );
            str["len"] = new Func<string, int>(Neo.IronLua.LuaLibraryString.len);
            str["lower"] = new Func<string, string>(Neo.IronLua.LuaLibraryString.lower);
            str["match"] = new Func<string, string, int, LuaResult>(
                Neo.IronLua.LuaLibraryString.match
            );
            str["rep"] = new Func<string, int, string, string>(Neo.IronLua.LuaLibraryString.rep);
            str["reverse"] = new Func<string, string>(Neo.IronLua.LuaLibraryString.reverse);
            str["sub"] = new Func<string, int, int, string>(Neo.IronLua.LuaLibraryString.sub);
            str["upper"] = new Func<string, string>(Neo.IronLua.LuaLibraryString.upper);
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
            LuaTable tbl = new LuaTable();
            tbl["concat"] = new concatDelg(LuaTable.concat);
            tbl["insert"] = new Action<LuaTable, object, object>(LuaTable.insert);
            tbl["move"] = new Action<LuaTable, int, int, int, LuaTable>(LuaTable.move);
            tbl["pack"] = new Func<object[], LuaTable>(LuaTable.pack);
            tbl["remove"] = new Func<LuaTable, int, object>(LuaTable.remove);
            tbl["sort"] = new Action<LuaTable, object>(LuaTable.sort);
            tbl["unpack"] = new Func<LuaTable, LuaResult>(LuaTable.unpack);
            return tbl;
        }
        #endregion

        #endregion

        #region protected methods

        #endregion
    }
}
