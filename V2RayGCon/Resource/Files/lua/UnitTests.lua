local Utils = require "lua.libs.utils"
local Set = require "lua.modules.set"

local failCounter = 0
local successCounter = 0

local function Assert(testName, expect, result)
	if expect == result then
		print("pass ", testName)
        successCounter = successCounter + 1
	else
		print("fail ", testName)
        failCounter = failCounter + 1
	end
end

local function ShowTestResult()
    local total = failCounter + successCounter
    print("Total: ", total)
    print("success: ", successCounter)
    print("fail: ", failCounter)
end

local function UtilsTest()
    -- Test 1. how-to use libs.utils.lua
    Utils.Echo("Hello")
	local data = {"AbC123", "123BcD"}
	Assert("Utils.IsInTable.True", true, Utils.IsInTable(data, "abc123"))
	Assert("Utils.IsInTable.False", false, Utils.IsInTable(data, "ac"))
	Assert("Utils.IsInTablePartially.True", true, Utils.IsInTablePartially(data, "bc123"))
	Assert("Utils.IsInTablePartially.False", false, Utils.IsInTablePartially(data, "b2"))
end

local function SetTest()
    -- Test 3. how-to use set
    local cache = Set()
    Assert("Set.Init.Count", 0, cache:Count())

    cache:Reset({1,2,3})
    Assert("Set.ResetNumber.Count", 3, cache:Count())

    cache:Reset({"aBc123","A1b2C3","123AbC"})
    Assert("Set.ResetString.Count", 3, cache:Count())
    cache:Add("aBc123")
    Assert("Set.AddDupVal.Count", 3, cache:Count())
    cache:Add("deF456")
    Assert("Set.AddNewVal.Count", 4, cache:Count())
    cache:Remove("deF456")
    Assert("Set.RmNewVal.Count", 3, cache:Count())

    Assert("Set.Contains.True", true, cache:Contains("aBc123"))
    Assert("Set.Contains.False", false, cache:Contains("abc123"))

    Assert("Set.ContainsCi.True", true, cache:ContainsCi("abc123"))
    Assert("Set.ContainsPartially.True", true, cache:ContainsPartially("1B2"))
	
	Assert("Set.MatchesPartially.True", true, cache:MatchesPartially("aabc1234"))
	Assert("Set.MatchesPartially.False", false, cache:MatchesPartially("bc123"))
end

local function Main()
	print("Run utils tests")
    UtilsTest()
    print(" ")
    SetTest()
    print(" ")
    ShowTestResult()
end

Main()