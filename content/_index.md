---
title: "Home"
date: 2020-01-31T21:02:39+08:00
draft: false
---

V2RayGCon 是 windows 下 [xray-core](http://github.com/xtls/xray-core) 及 [v2ray-core v4.x](https://www.v2fly.org) 的图形(graphic)配置器(configer)。  

##### 安装
先安装 .net framework 4.5+（win10已自带），然后下载解压 [V2RayGCon-box.zip](https://github.com/vrnobody/V2RayGCon/releases/latest) 到任意目录。  
*win7 用户需要到 [xray-core](https://github.com/xtls/xray-core/releases) 项目下载专用内核，解压到 `./3rd/core/` 目录内。*  

##### 后备（v2.1.2)
{{< figure src="images/releases/vgc-v2.1.2.png" >}}
*把上面的图片另存为 vgc.png，然后执行 linux 命令：*  
`exiftool -s3 -Description vgc.png | base64 -d > vgc.zip`  
MD5:  
26c7ae18b530fa5b5cd647d335ab0ff8 (vgc.png)  
acd4ff70a801c215aa603f4795c3ad9b (vgc.zip)  

##### 简要用法演示（v2.1.4.1）
{{< figure src="images/forms/demo_basics_v2.1.4.1.gif" >}}  

##### WebUI演示
详见 [插件 - NeoLuna - WebUI]({{< relref "03-plugins/04-neoluna/web-ui.md" >}})  
{{< figure src="images/webui/light_v0.0.2.0.png" >}}