local json = require('lua.libs.json')

--[[
credits:
    luacheck 0.23.0 https://github.com/mpeterv/luacheck.git 
    lua-complete https://github.com/FourierTransformer/lua-complete.git
--]]

local parser = require('lua.libs.luacheck.parser')

local analyzer = {}
 
local function DebugTable(t)
    print( table.dump(t, "    ", " >(((@> ") )
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

local function GetModuleObjName(src)
    for i = #src, 1, -1 do
        local v = src[i]
        if v.tag == "Return" and v[1] then
            return v[1][1]
        end
    end
    return nil
end

local function InsertOnce(t, v)
    if not table.contains(t, v) then
        table.insert(t, v)
    end
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

local function GetFunctionParams(t)
    local isMember = false
    local ps = {}
    for k,v in pairs(t) do
        if v[1] == "self" then
            isMember = true
        else
            table.insert(ps, v[1])
        end
    end
    return isMember, ps
end

local function GetModuleInfo(ast, mName)
    
    local r = {
        ["funcs"] = {},
        ["methods"] = {},
        ["props"] = {},
    }
    
    for k, v in pairs(ast) do
        local ps = {}
        local isMemberFuncs = false
        if v.tag == "Set" 
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
                isMemberFuncs, ps = GetFunctionParams(v[2][1][1])
                if isMemberFuncs then
                    r["methods"][fn] = ps
                else
                    r["funcs"][fn] = ps
                end 
            end
        end
    end
    
    return r 
end

local function AnalyzeRequire(ast, r)
    for k,v in pairs(ast) do
        if type(v) ~= "table" then
            -- continute
        elseif v[2] and v[2][1] and v[2][1][1] and v[2][1][1][1] == "require"
            and v[1] and v[1][1] and v[2][1][2]
        then
            local mn = v[1][1][1]
            if not r["modules"][mn] then
                r["modules"][mn] = v[2][1][2][1]
            end
        elseif v[2] and v[2][1] and v[2][1][1] and v[2][1][1][1] and v[2][1][1][1][1]
            and v[2][1][1][1][1][1] == "require" 
            and v[1] and v[1][1] and v[2][1][1][1][2]
        then
            local mn = v[1][1][1]
            if not r["modules"][mn] then
                r["modules"][mn] = v[2][1][1][1][2][1]
            end
        end
        if type(v) == "table" then
            AnalyzeRequire(v, r)
        end
    end
end

local function GetFunctionInfo(v, r)
    
    if type(v[1][1][1]) == "string" and type(v[2][1][1]) == "table" then
        -- add(a, b)
        local fn = v[1][1][1]
        local isMember, params = GetFunctionParams(v[2][1][1])
        r["funcs"][fn] = {
            ["line"] = v[1][1]["line"],
            ["params"] = params,
        }
    elseif type(v[1][1][1]) == "table" and type(v[2][1][1]) == "table"
        and type(v[1][1][1][1]) == "string" and type(v[1][1][2][1]) == "string"
    then
        -- utils.add(a, b) or utils.add(self, a, b)
        local isMember, params = GetFunctionParams(v[2][1][1])
        local fn = v[1][1][1][1] .. (isMember and ":" or ".") .. v[1][1][2][1]
        local key = isMember and "methods" or "subs"
        r[key][fn] = {
            ["line"] = v[1][1]["line"],
            ["params"] = params,
        }
    end        
end

local function AnalyzeAst(ast, r)
    for k,v in pairs(ast) do
        -- print( table.dump(v, "    ", " >(((@> ") )
        -- require (work but not robust)
        if v[2] and v[2][1] and v[2][1][1] and v[2][1][1][1] == "require"        
        then
            -- continue
        elseif v[2] and v[2][1] and v[2][1][1] and v[2][1][1][1] and v[2][1][1][1][1]
            and v[2][1][1][1][1][1] == "require" 
        then
            -- continue
        elseif v[2] and v[2][1] and v[2][1].tag == "Function"
            and v[1] and v[1][1] and v[1][1][1] -- fn
            and v[2][1][1] and type(v[2][1][1]) == "table"  -- params
        then
            -- functions
            GetFunctionInfo(v, r)
        elseif v[1] and v[1][1] and type(v[1][1][1]) == "string"
            and (v.tag == "Set" or v.tag == "Local")  
        then
            -- variables
            local vn = v[1][1][1]
            if v[2] and v[2][1] and v[2][1][1] and v[2][1][1][1] 
                and r["modules"][ v[2][1][1][1][1] ]
            then
                r["modules"][vn] = r["modules"][ v[2][1][1][1][1] ]
            elseif not r["vars"][vn] then
                r["vars"][vn] =  v[1][1]["line"]
            end
        end
    end
end 

function analyzer.parse(s)
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

function analyzer.analyzeModule(src)
    local ast = analyzer.parse(src)
    if ast == nil then
        return nil
    end
    local mn = GetModuleName(ast)
    if mn == nil then
        return nil
    end
    local r = GetModuleInfo(ast, mn)
    return json.encode(r)
end

local function analyzeFunction(n, f, r)
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

function analyzer.analyzeModuleEx(src)

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
            analyzeFunction(k, v, r)
        end
    end
    
    return json.encode(r)
end

function analyzer.analyzeCode(src)
       
    local r = {
        ["modules"] = {},
        ["vars"] = {},
        ["funcs"] = {},
        ["subs"] = {},
        ["methods"] = {},
    }
    
    local ast = analyzer.parse(src)
    -- print( table.dump(ast, "    ", " >(((@> ") )
    if ast ~= nil then
        AnalyzeRequire(ast, r)
        AnalyzeAst(ast, r)
    end
    
    -- print( table.dump(r) )
    return json.encode(r)
end 

function analyzer.new()
    local o = {}
    setmetatable(o, {__index = analyzer})
    return o
end

local code = [[
local json = require('lua.libs.json')
local hkmgr = require('lua.modules.hotkey').new()
local utils = require('lua.libs.utils')
local ut = utils.new()

local v

v = 10

local t = { 1, 2, 3}
gv = true

while true do
    print(1)
end

repeat
    print(2)
until trrue

local function Add(a, b)
    local c = a + b
    return c
end

function Main(ps)
    print("hello!")
    retrrrurn true
end

]]

local modSrc = [[
local m = {
    ["smem"] = "hello"
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

local function DumpCodeAst(src)
    local ast = analyzer.parse(src)
    local s = table.dump(ast, "    ", " --> ")
    print(s)
end

local function TestModuleAnalyzer()
    local r = analyzer.analyzeModule(modSrc)
    print(r)
end

local function TestCodeAnalyzer()
    local r = analyzer.analyzeCode(code)
    print(r)
end

local function TestModuleExAnalyzer()
    local r = analyzer.analyzeModuleEx(modSrc)
    print(r)
end

-- DumpCodeAst(code)
-- TestModuleAnalyzer()
-- TestCodeAnalyzer()
-- TestModuleExAnalyzer()

return analyzer