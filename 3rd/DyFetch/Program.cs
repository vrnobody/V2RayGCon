using System;
using System.IO;

namespace DyFetch
{
    class Program
    {
        static void Main(string[] args)
        {
            // 解释命令行参数，详见 Models.Configs.cs
            var configs = new Models.Configs(args);

            // -h 打印用法并结束程序
            if (
                args.Length < 1
                || configs.help
                || (string.IsNullOrEmpty(configs.pipeIn) && string.IsNullOrEmpty(configs.url))
            )
            {
                configs.ShowHelp();
                Environment.Exit(0);
            }

            // 创建selenium实例

            var pin = configs.pipeIn;
            var pout = configs.pipeOut;

            if (string.IsNullOrEmpty(pin) || string.IsNullOrEmpty(pout))
            {
                // 在命令行窗口中运行，没有指定管道参数时
                // 例如：DyFetch.exe -u "https://www.bing.com"
                FetchOnce(configs);
            }
            else
            {
                // 在NeoLuna插件中通过std.Sys:PipedProcRun()调用DyFetch.exe时（下附Lua示例代码）
                using (var fetcher = new Comps.Fetcher(configs))
                using (var plumber = new Comps.Plumber(pin, pout))
                {
                    // 详见Comps.Plumber.cs
                    plumber.Work(fetcher);
                }
            }

            Console.WriteLine("Goodbye!");
            Environment.Exit(0);
        }

        private static void FetchOnce(Models.Configs configs)
        {
            using (var fetcher = new Comps.Fetcher(configs))
            {
                var html = fetcher.Fetch(configs.url, configs.csses, configs.timeout, configs.wait);
                var path = configs.file;
                if (string.IsNullOrEmpty(path))
                {
                    Console.WriteLine(html);
                }
                else
                {
                    File.WriteAllText(path, html);
                }
                Console.WriteLine($"HTML len: {html.Length / 1024} KiB");
            }
        }
    }
}

// 在NeoLuna插件中利用管道调用DyFetch.exe示例
/*

local url = "https://www.bing.com"

local json = require('3rd/neolua/libs/json')

local function CreateHandle()
    -- 子进程所在目录
    local wkDir = "C:/DyFetch"
    
    -- 子进程文件绝对路径
    local exe = wkDir .. "/" .. "DyFetch.exe"
    
    -- 命令行参数中的 {0},{1} 将自动替换成输入、输出管道句柄
    local args = "-pipein={0} -pipeout={1}"
    
    -- 创建子进程
    return std.Sys:PipedProcRun(true, wkDir, exe, args)
end

local function DyFetch(url)
    local handle = CreateHandle()
    if handle == nil then
        print("创建子进程失败")
        return
    end
    
    print("创建命令消息")
    -- 命令格式见Models.Message.cs
    local msg = {
        url = url,
        timeout = 20000,
    }
    local m = json.encode(msg)
    
    print("向子进程发送命令")
    std.Sys:PipedProcWrite(handle, m)
    
    print("读取子进程返回的结果")
    local html = std.Sys:PipedProcRead(handle)

    -- 这里可以继续发送命令和读取结果
    
    print("关闭子进程的管道")
    std.Sys:PipedProcRemove(handle)
    return html
end

local function Main()
    local html = DyFetch(url)
    if not string.isempty(html) then
        print(string.sub(html, 1, 200))
        print("字节数:", string.len(html))
    else
        print("下载失败！")
    end
end

Main()

*/
