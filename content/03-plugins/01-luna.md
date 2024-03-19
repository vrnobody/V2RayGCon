---
title: "Luna"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 10
---

`v1.8.4` 起 [NeoLuna]({{< relref "03-plugins/04-neoluna/_index.md" >}}) 插件取代了 Luna 插件。  

现在 Luna 插件以第三方外置插件形式发布。需要到 [Releases页面](https://github.com/vrnobody/V2RayGCon/releases) 下载 Luna-plugin.zip，解压到 V2RayGCon 目录内才能使用。Luna 插件的优点是采用了原生的 lua53，缺点是有内存泄露问题。Luna 插件只能在 64 位系统中使用，如果想在 32 位系统中使用，需要把 `libs/x86/lua53.dll` 复制出来，替换掉 `libs/lua53.dll`。  
