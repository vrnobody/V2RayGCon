local Writer = require "lua.modules.writer"
local Logger = require "lua.modules.logger"

local testDataFileName = "testData.txt"
local testLogFileName = "testLog.txt"

local function WriterExample()
    local dw = Writer(testDataFileName)
    print("Write hello to file.")
    dw:WriteLine("hello")
    print("Clear file.")
    dw:Clear()
    print("Write hello world to file.")
    dw:WriteLine("Hello, world!")
end

local function LoggerExample()
    print("Run logger tests")
    local logger = Logger(testLogFileName)
    logger:Info("Hello")
    logger:Warn("world")
    logger:Error("!")
    logger:Log("nothing", "here")
end

local function Main()
   WriterExample() 
   LoggerExample()
end

Main()