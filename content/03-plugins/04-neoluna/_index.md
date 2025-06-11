---
title: "NeoLuna"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 10
---

NeoLuna插件是采用NeoLua作为后端的Lua脚本管理器。多插个字母n是为了避免c#命名空间重复问题。NeoLua是直接用c#写的lua5.3。而Luna插件则是通过P/Invoke调用c写的lua53.dll。  

使用NeoLua的时候注意这些坑：  
 1. 正则是通过简单的查找、替换把lua的pattern转成c#的regex，所以像string.gsub(), string.match()之类的函数，执行结果会和NLua不完全一致。要特别注意参数带table或者function的情况。  
 2. select()是从0开始算，而NLua是从1开始算。  
 3. 原版NeoLua的string, table不支持添加函数。NeoLuna插件给他们加了点魔法，让他们支持扩展。副作用是部分函数行为会和NLua不一致。  
 4. require()里面的路径用'/'分隔，所以NeoLua不能共用NLua写的模块。  
 5. 不支持string.dump()，所以用不了thread.lua模块。  
 6. os.time()是带时区的秒数，但os.date()输出结果又和NLua相同。  
 7. io库里面的读、写文件函数有点问题，建议使用std.Sys:Read/Write***()函数。  
