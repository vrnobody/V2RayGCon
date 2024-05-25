﻿using System;
using System.Collections.Generic;

namespace NeoLuna.Models.Consts
{
    public static class Lua
    {
        public const string LuaModules = "require module import";

        public const string NeoLuaKeyWords =
            "std std.Signal std.Misc std.Server std.Web std.Sys" + LuaKeywords;

        const string LuaKeywords =
            " and break do else elseif end for foreach function if in local nil not or repeat return then until while false true goto";

        public static string NeoLuaApiFuncNames = string.Join(@" ", GetterApiFuncNames());

        public static string LuaFunctions =
            "assert collectgarbage dofile error _G getmetatable ipairs loadfile next pairs pcall print rawequal rawget rawset setmetatable tonumber tostring type _VERSION xpcall string table math coroutine io os debug"
            + " getfenv gcinfo load loadlib loadstring select setfenv unpack _LOADED LUA_PATH _REQUIREDNAME package rawlen package bit32 utf8 _ENV";

        public const string LuaSubFunctions =
            " string.byte string.char string.dump string.find string.format string.gsub string.len string.lower string.rep string.sub string.upper"
            + " table.concat table.insert table.remove table.sort"
            + " math.abs math.acos math.asin math.atan math.atan2 math.ceil math.cos math.deg math.exp math.floor math.frexp math.ldexp math.log math.max math.min math.pi math.pow math.rad math.random math.randomseed math.sin math.sqrt math.tan"
            + " string.gfind string.gmatch string.match string.reverse string.pack string.packsize string.unpack"
            + " table.foreach table.foreachi table.getn table.setn table.maxn table.pack table.unpack table.move"
            + " math.cosh math.fmod math.huge math.log10 math.modf math.mod math.sinh math.tanh math.maxinteger math.mininteger math.tointeger math.type math.ult"
            + " bit32.arshift bit32.band bit32.bnot bit32.bor bit32.btest bit32.bxor bit32.extract bit32.replace bit32.lrotate bit32.lshift bit32.rrotate bit32.rshift"
            + " utf8.char utf8.charpattern utf8.codes utf8.codepoint utf8.len utf8.offset"
            + " coroutine.create coroutine.resume coroutine.status coroutine.wrap coroutine.yield coroutine.isyieldable coroutine.running"
            + " io.close io.flush io.input io.lines io.open io.output io.read io.tmpfile io.type io.write io.stdin io.stdout io.stderr io.popen"
            + " os.clock os.date os.difftime os.execute os.exit os.getenv os.remove os.rename os.setlocale os.time os.tmpname"
            + " package.loaders package.seeall package.config package.searchers package.searchpath package.cpath package.loaded package.loadlib package.path package.preload";

        public static List<string> NeoLuaPredefinedFunctions = new List<string>() { "Each", };

        public static List<string> LuaPredefinedSubFunctions = new List<string>()
        {
            "string.split(text, sep)",
            "string.endswith(text, keyword)",
            "string.isempty(text)",
            "string.startswith(text, keyword)",
            "table.contains(haystack, needle)",
            "table.sortedkeys(t)",
            "table.sortedkeysdesc(t)",
            "table.keys(t)",
            "table.length(t)",
            "table.load(str)",
            "table.dump(t)",
            "table.dump(t, indent)",
            "table.dump(t, indent, header)",
            "table.dump(t, indent, header, level)",
            "table.dump(t, indent, header, level, maxLevel)",
        };

        static List<string> GetterApiFuncNames()
        {
            var luaKeywords = new List<string>();

            var types = new List<Type>
            {
                typeof(Interfaces.ILuaSignal),
                typeof(VgcApis.Interfaces.PostOfficeComponents.ILuaMailBox),
                typeof(VgcApis.Interfaces.PostOfficeComponents.ILuaMail),
                typeof(VgcApis.Interfaces.ICoreServCtrl),
                typeof(VgcApis.Interfaces.CoreCtrlComponents.IConfiger),
                typeof(VgcApis.Interfaces.CoreCtrlComponents.ICoreCtrl),
                typeof(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates),
                typeof(VgcApis.Interfaces.CoreCtrlComponents.ILogger),
            };

            types.AddRange(
                new List<Type>
                {
                    typeof(Interfaces.ILuaSys),
                    typeof(Interfaces.ILuaMisc),
                    typeof(Interfaces.ILuaServer),
                    typeof(Interfaces.ILuaWeb),
                }
            );

            foreach (var t in types)
            {
                foreach (var methodName in VgcApis.Misc.Utils.GetPublicMethodNames(t))
                {
                    if (!luaKeywords.Contains(methodName))
                    {
                        luaKeywords.Add(methodName);
                    }
                }
            }

            return luaKeywords;
        }
    }
}
