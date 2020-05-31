

--[[
credits:
    luacheck 0.23.0 https://github.com/mpeterv/luacheck.git 
    lua-complete https://github.com/FourierTransformer/lua-complete.git
--]]

local json = require('lua.libs.json')
local parser = require('lua.libs.luacheck.parser')

local anz = {}
 
local function DumpTable(t)
    print( table.dump(t, "    ", " >(((@> ") )
end
    
local function ParseVarName(name)
    
    if type(name) == "string" 
        and string.match(name, "[^%w^_]") == nil 
    then
        if string.match(name, "[^%d^.]") == nil then
            -- pure numbers 
            return "[" .. name .. "]"
        else
            return name
        end
    end
    
    local s = tostring(name)
    if string.match(s, '"') ~= nil then
        return "['" .. s .. "']"
    end
    return '["' .. s .. '"]'
end

local function InsertOnce(t, v)
    if not table.contains(t, v) then
        table.insert(t, v)
    end
end

local function analyzeFuncParams(f)
    
    local ft = "funcs"
    local ps = {}
    
    if type(f) ~= "table" then
        return ft, ps
    end
   
    for k,v in pairs(f) do
        local pn = v[1]
        if pn == "self" then
            ft = "methods"
        else
            table.insert(ps, pn)
        end
    end
    
    return ft, ps
end

function anz.parse(s)
    if s == nil or type(s) ~= "string" then
        return nil
    end
    
    local ast
    local index
    repeat
        local ok
        ok, ast = pcall(function () return parser.parse(s) end)
        index = string.find(s, "[\n\r%s]*\n[^\n]*$")
        if index == nil then
            return nil
        end
        s = string.sub(s, 1, index - 1)
    until ok
    return ast
end 

-- anz module start
local function GetModuleObjName(src)
    for i = #src, 1, -1 do
        local v = src[i]
        if v.tag == "Return" and v[1] then
            return v[1][1]
        end
    end
    return nil
end

local function GetModuleProps(src, r)
    local on = GetModuleObjName(src)
    if on == nil or string.isempty(on) then
        return
    end
    -- table.dump(t, indent, header)
    -- print( table.dump(src, "    ", " --> ") )
    for k,v in pairs(src) do
        if v[1] and v[1][1] and v[1][1][1] == on and v[1][1].tag == "Id"
            and v[2] and v[2][1] and v[2][1].tag == "Table" 
        then
            -- table.dump(t, indent, header)
            -- print( table.dump(v[2][1], "    ", " --> ") )
            for sk, sv in pairs(v[2][1]) do
                if type(sv) == "table" and sv[1] then
                    InsertOnce(r["props"], sv[1][1])
                end
            end
        elseif v[1] and v[1][1] and v[1][1][1] and v[1][1][1][1] == on
            and v.tag == "Set" and v[1][1].tag == "Index" and v[1][1][2]
        then
            InsertOnce(r["props"], v[1][1][2][1])
        end
    end
end

local function GetModuleInfo(ast, mName)
    
    local r = {
        ["funcs"] = {},
        ["methods"] = {},
        ["props"] = {},
    }
    
    for k, v in pairs(ast) do
        if v.tag == "Local"
            and v[1] and v[1][1] and v[1][1][1] == mName
            and v[2] and type(v[2][1]) == "table"
        then
            -- print( DumpTable(v) )
            for sk, sv in pairs(v[2][1]) do
                if type(sv) == "table" and sv.tag == "Pair"
                    and type(sv[1]) == "table" and type(sv[1][1]) == "string"
                then
                    -- print( DumpTable(sv) )
                    InsertOnce(r["props"], sv[1][1])
                end
            end
        elseif v.tag == "Set"
            and v[1] and v[1][1] and v[1][1][1] and v[1][1][1][1] == mName
            and v[1][1][2] and type(v[1][1][2][1]) == "string"
            and v[2] and v[2][1] and v[2][1].tag
        then
            if v[2][1].tag ~= "Function" then
                -- props
                InsertOnce(r["props"], v[1][1][2][1])
            elseif v[2][1][2] -- function has source code
                and type(v[1][1][2][1]) == "string"
            then
                local fn = v[1][1][2][1]
                local lfn = string.lower(fn or "")
                if lfn == "new" or lfn == "create" then
                    GetModuleProps(v[2][1][2], r) -- src of new()
                end 
                local ft, ps = analyzeFuncParams(v[2][1][1])
                r[ft][fn] = ps
            end
        end
    end
    
    return r 
end

local function GetModuleName(ast)
    for i = #ast, 1, -1 do
        local v = ast[i]
        if v and v.tag == "Return" and v[1] then
            return v[1][1]
        end
    end
    return nil
end

function anz.analyzeModule(src)
    local ast = anz.parse(src)
    if ast == nil then
        return nil
    end
    local mn = GetModuleName(ast)
    if mn == nil then
        return nil
    end
    -- print( table.dump(ast) )
    local r = GetModuleInfo(ast, mn)
    return json.encode(r)
end

-- anz module end

-- anz module ex start
local function analyzeModuleFunction(n, f, r)
    local info = debug.getinfo(f, "uS")
    
    if info.what == "C" then
        return
    end
    
    local ps = {}
    local isMemberFunc = false
    for i = 1, info.nparams do
        local p = debug.getlocal(f, i)
        if p == "self" then
            isMemberFunc = true
        else
            table.insert(ps, p)
        end
    end
    
    local target = isMemberFunc and "methods" or "funcs"
    r[target][n] = info.isvararg and {"..."} or ps
end

function anz.analyzeModuleEx(src)

    local ok, m = pcall(load(src))
    if not ok or type(m) ~= "table" then
        return nil
    end
    
    local r = {
        ["funcs"] = {},
        ["methods"] = {},
        ["props"] = {},
    }
    
    for k,v in pairs(m) do
        if type(v) ~= "function" then
            table.insert(r["props"], k)
        else
            analyzeModuleFunction(k, v, r)
        end
    end
    
    return json.encode(r)
end
-- anz module ex end


-- anz codes start

local function analyzeName(n, sep)
    
    if type(n) ~= "table" then
        return false, ParseVarName(n)
    end
    
    if n.tag == "Index" and n[1] and n[2] then
        local itl, left = analyzeName(n[1], ".")
        local itr, right = analyzeName(n[2], ".")
        local c = "."
        if right ~= nil and string.sub(right, 1, 1) == '[' then
            c = ""
        end
        return true, left .. c .. right
    end

    return false, ParseVarName(n[1])
end

local function analyzeRequirePath(v, r)
    if type(v) ~= "table" then
        return "", ""
    end
    
    if v[1] and v[1][1] == "require" and v[2][1] then
        return "modules", v[2][1]
    elseif v.tag == "Call" and v[1] and v[1][1] and v[1][1][1] 
        and v[1][1][1][1] == "require" 
        and v[1][1][2] 
    then
        return "modules", v[1][1][2][1]
    end
    
    if v[1] and v[1][1] and type(v[1][1][1]) == "string" 
        and v[1][2] and type(v[1][2][1]) == "string"
    then
        local mm = string.lower(v[1][2][1])
        local mn = v[1][1][1]
        local mp = r["modules"][mn]
        if mp ~= nil and (mm == "new" or mm == "create") then
            return "modules", mp
        end
    end
    return "", ""
end
    
local function analyzeValue(v, r)
    if type(v) ~= "table" then
        return "vars", nil
    end
    
    if v.tag == "Call" then 
        return "vars", nil
    elseif v.tag == "Function" then
        return analyzeFuncParams(v[1])
    end   
    return "vars", nil
end

local function analyzeOneRequire(n, v, r)
    local vt, vv = analyzeRequirePath(v, r)
    if vt == "modules" then
        local isTable, nv = analyzeName(n, ".")
        if nv ~= nil and nv ~= "" then
            r[vt][nv] = vv
        end
    end
end

local function analyzeRequire(ast, r)
    for n, line in pairs(ast) do
        if type(line) == "table" and line["line"] 
            and line[1] and line[2] 
            and type(line[1]) == "table"
            and type(line[2]) == "table"
        then
            local ln = line["line"]
            local len = table.length(line[1]) 
            for n = 1, len do
                analyzeOneRequire(line[1][n], line[2][n], r)
            end
        end
        if type(line) == "table" then
            analyzeRequire(line, r)
        end
    end
end

local function analyzeEqual(n, v, r, ln)
    
    local vt, vv = analyzeValue(v, r)
    
    -- print( table.dump(v) )
    -- print("vt: ", vt)
    -- print("vv: ", table.dump(vv))
    
    if vt ~= "vars" and (vv == nil or vv == "") then
        return
    end
        
    local sep = vt == "methods" and ":" or "."
    local isTable, name = analyzeName(n, sep)
    
    -- print( table.dump(n) )
    -- print("isTable: ", isTable)
    -- print("name: ", name)
    
    if name == nil or name == "" then
        return
    end
    
    if isTable and vt == "funcs" then
        vt = "subs"
    end
    
    local el = nil
    if vt == "vars" then
        el = ln
    elseif vt == "modules" then
        el = vv
    else
        el = {
            ["line"] = ln,
            ["params"] = vv,
        }
    end
    
    if not r[vt][name] then
        r[vt][name] = el
    end
end

local function analyzeLines(ast, r)
    for n, line in ipairs(ast) do
        local tag = line.tag
        if tag == "Local" or tag == "Set" or tag == "Localrec" then
            local ln = line["line"]            
            local len = table.length(line[1]) 
            for n = 1, len do
                if line[2] and line[2][n] then
                    -- print( "analyzing: ", table.dump(line))
                    analyzeEqual(line[1][n], line[2][n], r, ln)
                else
                    local isTable, name = analyzeName(line[1][n], ".")
                    if name ~= nil and name ~= "" and r["vars"][name] == nil then
                        r["vars"][name] = ln
                    end
                end
            end
        end
    end
end    

function anz.analyzeCode(src)
     local r = {
        ["modules"] = {},
        ["vars"] = {},
        ["funcs"] = {},
        ["subs"] = {},
        ["methods"] = {},
    }
    
    local ast = anz.parse(src)
    if ast ~= nil then
        analyzeRequire(ast, r)
        analyzeLines(ast, r)
    end
    
    -- print("results:")
    -- print( table.dump(r) )
    return json.encode(r)
end

-- anz codes end


-- debug codes start
local testCode = [[
local json = require('lua.libs.json')
local hkmgr = require('lua.modules.hotkey').new()
local utils = require('lua.libs.utils')
local ut = utils.new()

local v
v = 10
gv = true

local BYTE_0, BYTE_9, BYTE_f, BYTE_F = sbyte("0"), sbyte("9"), sbyte("f"), sbyte("F")

local t = { 1, 2, 3}
t["a"] = true
t[1], v = false, 11
local worker, err = function(a) return a end, nil

while true do
    print(1)
end

repeat
    print(2)
until trrue  -- oopsy!

local function Add(a, b)
    local c = a + b
    return c
end

function t.abc(self, a, b)
    print(a, b)
end

t['{'] = function(a, b)
    return a
end

function Main(ps)
    print("hello!")
    return true
end

function ThisFunctionWillNotShowUpInAst()
    because this line is not parsable!
    return nothing
end

]]


local testModule = [[
local m = {
    ["v1"] = "hello",
    ["v2"] = true,
}

function m:GetName()
    local k = 1
    return self.name
end

function m.Say(content)
    print(content)
end

function m.new(name)
    o = {
        name = name,
        age = -1,
    }
    
    o.height = 0
    
    setmetatable(o, {__index = m})
    return o
end

return m
]]



local function testCodeToAst(src)
    local ast = anz.parse(src)
    local s = table.dump(ast, "    ", " --> ")
    print(s)
end

local function testAnalyzeCode()
    local r = anz.analyzeCode(testCode)
    local o = json.decode(r)
    print( DumpTable(o) )
    print(r)
end

local function testAnalyzeModule()
    local r = anz.analyzeModule(testModule)
    local o = json.decode(r)
    print( DumpTable(o) )
    print(r)
end

local function testAnalyzeModuleEx()
    local r = anz.analyzeModuleEx(testModule)
    local o = json.decode(r)
    print( DumpTable(o) )
    print(r)
end

-- testCodeToAst(testCode)
-- testAnalyzeCode()
-- testAnalyzeModule()
-- testAnalyzeModuleEx()

-- debug codes end

function anz.new()
    local o = {}
    setmetatable(o, {__index = anz})
    return o
end

return anz
