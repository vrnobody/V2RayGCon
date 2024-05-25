---
title: "NeoLuna"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 10
---
V2RayGCon v1.8+

NeoLuna是采用NeoLua作为后端的Lua脚本管理器。多插个字母n主要是避免c#命名空间重复问题。NeoLua是直接用c#写的lua5.3。原来的Luna插件后端是NLua，它是通过P/Invoke调用c写的lua53.dll。  

使用NeoLua的时候注意这些坑：  
 1. 正则是通过简单的查找、替换把lua的pattern转成c#的regex，所以像string.gsub(), string.match()之类的函数，执行结果会和NLua不完全一致。要特别注意参数带table或者function的情况。  
 2. select()是从0开始算，而NLua是从1开始算。  
 3. 原版NeoLua的string, table不支持添加函数。NeoLuna插件给他们加了点魔法，让他们支持扩展。副作用是部分函数行为会和NLua不一致。  
 4. require()里面的路径用'/'分隔，所以NeoLua不能重用NLua写的模块。  
 5. 不支持string.dump()，所以用不了thread.lua模块。  
 6. os.time()是带时区的秒数，但os.date()输出结果又和NLua相同。  
 7. io库里面的读、写文件函数有点问题，建议使用std.Sys:Read/Write***()函数。  
 
 既然NeoLua有这么多坑，为什么还要加入这个功能重复的插件呢？因为NLua有个比较严重的问题。当调用像Server:GetAllServers() coreServ:GetCoreStates()这样的函数的时候，NLua会保留一个引用。就算之后删掉服务器、停止脚本、Dispose掉Lua()虚拟机，这个引用仍然存在。这导致CLR没法回收内存。服务器比较少（一千来个）的时候，问题并不严重。但是服务器比较多（上万个）并且经常添加、删除服务器的时候问题就会暴露出来了。有时内存占用会上G。还有个有意思的现象，因为这些对象只保留了一个引用并不会被访问到，所以一段时间（几小时）之后，操作系统会把这些内存转到pagefile（Linux叫swap）里面。这时看任务管理器的内存数值会降下来，给人一种内存被回收了的错觉。而NeoLua的对象完全由CLR的GC管理，没有这个问题。  

上面也提到了，以前Luna插件里面的Misc:Sleep(), Signal:Stop()等等函数，在NeoLuna插件里面统一放到`std.`里面，也就是变成std.Misc:Sleep(), std.Signal:Stop()等。不过按以前的方式打字就行，自动补全功能会加上`std.`的。然后调CLR不需要像以前那样import()，NeoLua的CLR库都在`clr.`里面，不过估计没人用这个功能。NeoLuna可以调用的函数见[Interfaces](https://github.com/vrnobody/V2RayGCon/tree/master/Plugins/NeoLua/Interfaces)目录，`3rd/neolua`目录放了点模块。其他的和Luna插件没多大区别。看[Luna插件的文档]({{< relref "03-plugins/01-luna/_index.md" >}})就行。  

正则的坑我也没想到好的解决办法。目前一个比较绕的办法是在NeoLua中通过mailbox把要处理的文本发给NLua，处理完成后再发回来。多说一句NeoLua和NLua的LocalStorage, SnapCache还有mailbox都是相通的。但是他们的table实现方式有所不同，所以如果不序列化直接把table当object发送，对方收到的将会是userdata。同一个插件的不同脚本之间发送table就没这个问题。  
