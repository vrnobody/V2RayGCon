local parser = require("lua.libs.ast.parser")

local analyze = {}
analyze.defaultPackagePath = package.path

-- for compatibilityy
local M = {}

-- a lua getParams function
-- adapted from https://facepunch.com/showthread.php?t=884409
if not jit and not _ENV then
    -- for vanilla lua 5.1
    function M.getParams(func)
        -- keep track of output
        local paramNameList = {}
        local paramCount = 0

        -- create a coroutine with the func
        local co = coroutine.create(func)

        -- set hook to run when the coroutine is called
        debug.sethook(co, function()

            -- this is done to constrain the args to debug.getlocal
            local i, k = 1, debug.getlocal(co, 2, 1)
            while k do
                if k ~= "(*temporary)" then
                    paramCount = paramCount + 1
                    paramNameList[paramCount] = k
                end
                i = i+1
                k = debug.getlocal(co, 2, i)
            end

            -- it errors out at the end, so the function never really runs
            error("~~end~~")
        end, "c")

        -- try to start the coroutine, it should error out immediately
        local res, err = coroutine.resume(co)

        -- nothing should ever return.
        if res then
            error("The function provided defies the laws of the universe.", 2)

        -- and if errs with anything other than "~~end~~", something went wrong
        elseif string.sub(tostring(err), -7) ~= "~~end~~" then
            error("The function failed with the error: " .. tostring(err), 2)
        end

        -- ... shows up as "arg" at the end. This will replace it.
        -- it should cover most uses, unless your last arg is named arg...
        if paramNameList[paramCount] == "arg" then
            paramNameList[paramCount] = "..."
        end

        return paramNameList
    end
else

    -- for everything else
    function M.getParams(func, paramCount, isVarArg)
        -- set it all up
        local paramNameList = {}

        -- get the parameters
        for i = 1, paramCount do
            local param_name = debug.getlocal(func, i)
            paramNameList[i] = param_name
        end

        -- add the ...'s for varargs
        if isVarArg then
            paramNameList[paramCount+1] = "..."
        end
        return paramNameList
    end
end

local function TableEqFunction(t, i, output, line)
   -- parse through a table
    -- I'm not dealing with tables in tables yet.
    local currentTable = t[1][i][1]
    if currentTable.tag ~= "Index" then
        if output.variables[currentTable[1]] then
            -- add value to currentTable in outputvariables
            local varName = t[1][i][2][1]
            output.variables[currentTable[1]].table[varName] = {
                ["type"] = string.lower(t[2][i].tag)
            }
        -- else
            if t[2][i].tag == "Function" then
                output.variables[currentTable[1]].table[varName]["function"] = {
                    ["paramList"] = {},
                    ["location"] = line,
                    ["what"] = "Lua"
                }
                for j, param in ipairs(t[2][i][1]) do
                    output.variables[currentTable[1]].table[varName]["function"].paramList[j] = param[1]
                end
            end
        end
    end
    -- print()
    -- pretty.dump(t[1])
    -- for _, entry in ipairs(t[1][i]) do
    --     print(entry.tag, entry[1])
    -- end
    -- print("EQUALS")
    -- print(t[2][i][1], t[2][i].tag)
    -- pretty.dump(t[2][i])
    -- print()
    -- for _, entry in ipairs(t[2][i]) do
    --     print(entry.tag, entry[1])
    -- end
    -- pretty.dump(t[1][i])
    -- pretty.dump(t[2][i])
end

local function VarEqRequire(t, i, output, line)
    -- table.insert(output.modules, {
    -- set the variable name
    -- local tag = t[1][i][1]
    output.modules[t[1][i][1]] = {
        ["module"] = t[2][i][2][1],
        ["location"] = line
        -- ["line"]
        -- could add scoping info here later
        -- ["location"] = t[1][i].location
    }
    -- ["location"] = t[1][i].location
    -- })
    -- print(t[1][i][1])    -- name used
    -- print(t[2][i][1][1]) -- require
    -- print(t[2][i][2][1]) -- hexafont

    -- elseif t[2][i].tag == "Call" then
    -- we might be able to do some type things here
    -- print("Call")
    -- pretty.dump(t[2][i])
end

local function VarEqFunction(t, i, output, line)
    local functionName = t[1][i][1]
    output.variables[functionName] = {
        ["type"] = "function",
        ["function"] = {["paramList"] = {}},
        ["location"] = line
    }
    for j, param in ipairs(t[2][i][1]) do
        output.variables[functionName]["function"].paramList[j] = param[1]
    end
end

local function VarEqTable(t, i, output, line)
    -- we'll get table info later...
    -- print("Table", t[1][i][1])
    local newTable = t[1][i][1]
    output.variables[newTable] = {
        ["type"] = "table",
        ["table"] = {},
        ["location"] = line
    }
    -- for j = 1, #t[2][i] do
    for _, entry in ipairs(t[2][i]) do

        -- should add the key and type into the output table
        output.variables[newTable].table[entry[1][1]] = {
            ["type"] = string.lower(entry[2].tag)
        }
        -- print(entry[1][1])
        -- print(entry[2].tag)
        -- print(entry.tag)
    end

    -- print(#t[2][i])
    -- print(t[2][i][1].tag)
end

local function VarEqInstance(t, i, output, line)
    local tag = t[1][i][1]
    -- print("suspect instance: ", tag)
    local n = t[2][i][1][1][1]
    -- print("module name: ", n)
    local m = output["modules"][n] 
    if m then
        output["instances"][tag] = {
            ["module"] = m["module"],
            ["location"] = line,
        }
    end
end

-- a basic findEquals. Could one day do a lot more, but starting small for now.
local function findEquals(fullTable, output, line)
    -- for analyzing the current line
    local t = fullTable[line]
    -- something is equal to something else!
    if t.equals_location then
        for i = 1, #t[1] do
            if t[1][i].tag == "Index" and t[2][i] and t[2][i].tag then
                TableEqFunction(t, i, output, line)
            -- if there's a module assignment
            elseif t[1][i].tag == "Id" and t[2][i] and t[2][i].tag then
                if t[2][i][1] and t[2][i][2] and t[2][i][1].tag == "Id" 
                    and t[2][i][1][1] == "require" 
                    and t[2][i][2].tag == "String" 
                then
                    -- print("hello")
                    VarEqRequire(t, i, output, line)
                elseif t[2][i][1][1] and t[2][i][1][1].tag == "Id"
                      and t[2][i][1][2][1] == "new"
                then
                    -- print("world")
                    VarEqInstance(t, i, output, line)
                elseif t[2][i].tag == "Function" then
                    VarEqFunction(t, i, output, line)
                elseif t[2][i].tag == "Table" then
                    VarEqTable(t, i, output, line)
                end
            end
            -- print("\n")
        end
    end
    
    -- use c# to handle function
    --[[
    elseif t.tag == "Function" then
        if fullTable[line-1] ~= nil then
            local functionName = fullTable[line-1][1]
            output.variables[functionName] = {
                ["type"] = "function",
                ["function"] = {
                    ["paramList"] = {},
                    ["location"] = line,
                    ["what"] = "Lua"
                }
            }
            for j, param in ipairs(t[1]) do
                output.variables[functionName]["function"].paramList[j] = param[1]
            end
        end
    end
    --]]
    -- return output
end

local function analyzeAST(t, output)
    -- yay recursion! create output if it doesn't exist
    if not output then
        output = {
            ["instances"] = {},
            ["modules"] = {},
            ["variables"] = {},
        }
    end

    -- iterate through ALL the tables!
    if type(t) == "table" then
        for line = 1, #t do
            -- mergeTable(output, findEquals(t[line]))
            findEquals(t, output, line)
            -- print(inspect(output))
            analyzeAST(t[line], output)        
        end
    end
    return output
end

-- error handler that *will* get the AST even if the parse fails.
-- NOTE: This gets kidna dangerous. I'm hoping to avoid doing this
-- in the future.
local function getAST()
    local name, value
    -- the first few levels are c/lua/syntax errors
    -- also, this could potentially be a moving target.
    for level = 5, 15 do
        local i = 1
        while true do
            name, value = debug.getlocal(level, i)
            -- when nil, there are no more things to check.
            if name == nil then break end

            -- the variable is called "block"
            if name == "block" then
                -- print("found it")
                return value
            end

            i = i + 1
        end
    end
end

-- returns nil if it can't parse or the analyzed ast
function analyze.analyzeSource(src)
    -- print("generating ast...")
    -- print(src)
    -- print(type(src))

--[[
    - lua 5.1 compat...
    local function parserParse()
        -- local text = decoder.decode(src)
        local ast = parser.parse(src)
        -- print("ast: ", table.tostring(ast) )
        return ast
    end
--]] 
    local function parserParse()
        local s = src
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

    -- local status, ast = pcall(parser.parse, src)
    local _, ast = xpcall(parserParse, getAST)

    -- print("xpcalled: ", inspect(ast))
    if not ast then
        -- print(ast)
        return nil
    end

    -- print("analyzing ast...")
    local assignments = analyzeAST(ast)
    -- pretty.dump(assignments)
    return assignments
end

local function analyzeLuaFunc(func)
    local info = debug.getinfo(func, "uS")
    local out = {what = info.what}
    -- can't do anything with a c function
    if info.what == "C" then
        return out
    end

    -- going into the code!
    local paramCount = info.nparams
    local isVarArg = info.isvararg
    local paramNameList = M.getParams(func, paramCount, isVarArg)

    -- build up the output
    out["paramList"] = paramNameList
    -- out["paramCount"] = paramCount
    -- out["isVarArg"] = isVarArg
    return out
end

local function analyzeTable(t)
    local out = {}
    local valueType
    for k, v in pairs(t) do
        valueType = type(v)
        out[k] = {["type"] = valueType}

        -- get information about sub-tables
        -- avoid metatables. recursion for days.
        if k ~= "__index" and valueType == "table" then
            out[k]["table"] = analyzeTable(v)
        elseif valueType == "function" then
            out[k]["function"] = analyzeLuaFunc(v)
        end
    end
    return out
end

function analyze.analyzeModule(module, packagePath)
    print("analyzeModule", module)
    if packagePath then
        print("special package")
        package.path = package.path .. ";" .. packagePath .. "/?.lua"
    end
    local status, analyzedModule = pcall(require, module)

    -- failed to load
    if not status then
        print("Failed to analyze", module)
        if packagePath then
            package.path = analyze.defaultPackagePath
        end
        return nil
    end

    -- go in for more info
    local moduleType = type(analyzedModule)
    local output = {["type"] = moduleType}
    if type(analyzedModule) == "table" then
        output["table"] = analyzeTable(analyzedModule)
    elseif type(analyzedModule) == "function" then
        output["function"] = analyzeLuaFunc(analyzedModule)
    end
    -- clear the module out of the cache.
    package.loaded[module] = nil
    -- reset the package path if needed
    if packagePath then
        -- pretty.dump(output)
        package.path = analyze.defaultPackagePath
    end
    return output
end

return analyze

