---
title: "NeoLuna"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 10
---

NeoLuna 插件是采用 NeoLua 作为后端的 lua 脚本管理器。多插个字母 n 是为了避免 c# 命名空间重复问题。NeoLua 是直接用 c# 写的 lua5.3。而Luna 插件则是通过 P/Invoke 调用 c 写的 lua53.dll。  

使用 NeoLua 的时候注意这些坑：  
 1. 正则是通过简单的查找、替换把 lua 的 pattern 转成 c# 的 regex，所以像 string.gsub(), string.match() 之类的函数，执行结果会和 NLua 不完全一致。要特别注意参数带 table 或者 function 的情况。  
 2. select() 是从 0 开始算，而 NLua 是从 1 开始算。  
 3. 原版 NeoLua 的 string, table 不支持添加函数。NeoLuna 插件给他们加了点魔法，让他们支持扩展。副作用是部分函数行为会和 NLua 不一致。  
 4. require() 里面的路径用 '/' 分隔，所以 NeoLua 不能共用 NLua 写的模块。  
 5. 不支持 string.dump()，所以用不了 thread.lua 模块。  
 6. os.time() 是带时区的秒数，但 os.date() 输出结果又和 NLua 相同。  
 7. io 库里面的读、写文件函数有点问题，建议使用 std.Sys:Read/Write***() 函数。  
