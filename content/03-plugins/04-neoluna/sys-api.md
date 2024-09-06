---
title: "Sys库示例"
date: 2020-04-14T17:35:52+08:00
draft: false
weight: 30
---

##### 存取键值对  

```lua
-- 用SnapCacheApply()申请的SnapCache在申请者脚本结束时自动清理
local token = std.Sys:SnapCacheApply()

-- key的类型是string, value的类型是object所以存什么都可以
local key = "hello"
std.Sys:SnapCacheSet(token, key, "world")

-- 知道token的其他脚本可以通过Get获取内容
local value = std.Sys:SnapCacheGet(token, key)

print("token:", token)
print("key:", key)
print("value:", value)
```

```lua
-- 也可以自定义token
local token = "任意字符串"
local ok = std.Sys:SnapCacheCreate(token)
print("success:", ok)

-- 但是用完要手工删除token，不然还会占内存
std.Sys:SnapCacheRemove(token)
```

##### 启动系统中的程序

```lua
-- 在浏览器中打开网址  
std.Sys:Start("https://www.baidu.com")

-- 还可以填.lnk路径，启动系统中的程序  
```

##### 运行可执行文件
```lua
-- 假设trojan的位置是V2RayGCon/trojan/trojan.exe
local trojan = "trojan/trojan.exe"
local args = "-c trojan/config.json"

-- 代码
local proc = std.Sys:Run(trojan, args, nil, nil, false, true)
while not std.Signal:Stop() and not std.Sys:HasExited(proc) do
    std.Misc:Sleep(1000)
end
if not std.Sys:HasExited(proc) then
    std.Sys:SendStopSignal(proc)
    std.Sys:WaitForExit(proc)
end
```

##### 多个脚本之间通信
开2个窗口，同时运行以下脚本
```lua
local from = "Alex"
local to = "Bob"

local mailbox = std.Sys:CreateMailBox(from)
if mailbox == nil then
    from, to = to, from
    mailbox = std.Sys:CreateMailBox(from)
end
assert(mailbox ~= nil)

mailbox:SendCode(to, 1)
print("等待", to, "的信息...")
repeat
    local mail = mailbox:Wait()
    if mail ~= nil then
        local code = mail:GetCode()
        print("来自:", mail:GetAddress(), "信息:", code)
        std.Misc:Sleep(800)
        mailbox:ReplyCode(mail, code + 1)
    end
until mail == nil
```
Mailbox里面还有个SendAndWait()函数，配合Sys:CreateMailBox(name, capacity)使用，相当于一个golang的channel。  

##### 全局热键

```lua
local hkMgr = require('3rd/neolua/mods/hotkey').new()
    
local hkCfgs = {
    -- ctrl + 5 打开记事本
    {"D5", function() std.Sys:Run("notepad.exe") end},

    -- ctrl + 6 打开cmd.exe
    {"D6", function() std.Sys:Run("cmd.exe") end},

    -- ctrl + 7 打开画图
    {"D7", function() std.Sys:Run("mspaint.exe") end},
}

for index, hkCfg in ipairs(hkCfgs) do
    if not hkMgr:Reg(hkCfg[1], hkCfg[2], true, true, false) then
        local msg = "注册热键[" .. hkCfg[1] .. "]失败"
        assert(false, msg)
    end
end

while not std.Signal:Stop() do
    if not hkMgr:Wait(1500) then
        print("请按 Ctrl + Alt + 5 或 6 或 7")
    end
end
hkMgr:Destroy()
```
基本步骤是先用`Reg`把快捷键和某个函数绑定，然后按需选 `Check()`轮询 或者 `Wait()`阻塞 等待按键消息。  
其实`hotkey`模块只是对`std.Sys`库中的`std.Sys:RegisterHotKey(...)`等几个函数简单包装了一下。  
如果你想硬核一点可以直接使用`std.Sys`库中的函数。具体代码参考`3rd/neolua/mods/hotkey.lua`。  
  
小技巧：不知道键码（比如上面的 "D5", "D6", "D7"）的时候，可以打开ProxySetter插件来查。  
