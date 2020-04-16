local Writer = require "lua.modules.writer"
local Reader = require "lua.modules.reader"
local Logger = require "lua.modules.logger"

local testDataFileName = "testData.txt"
local testLogFileName = "testLog.txt"

local function ReaderExample(filename)
    print("[", filename, "]")
    local dr = Reader(filename)
    local lines = dr:ReadAllLines()
    for idx, line in pairs(lines) do
        print(line)
    end
end

local function WriterExample()
    local dw = Writer(testDataFileName)
    print("Write [hello] to file.")
    dw:WriteLine("hello")
    print("Clear file.")
    dw:Clear()
    print("Write [hello world] to file.")
    dw:WriteLine("Hello, world!")
end

local function LoggerExample()
    local logger = Logger(testLogFileName)
    logger:Info("Hello")
    logger:Warn("world")
    logger:Error("!")
    logger:Log("nothing", "here")
end

local function Main()
    print("\nRun write file tests")
    WriterExample() 
    print("\nRun logger tests")
    LoggerExample()
    print("\nRun read file tests")
    ReaderExample(testDataFileName)
    print("")
    ReaderExample(testLogFileName)
end

Main()