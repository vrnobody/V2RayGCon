---
title: "Pacman"
date: 2020-02-01T17:46:32+08:00
draft: false
weight: 30
---

2026-01-14 这个插件已经被 [Composer]({{< relref "03-plugins/Composer.md" >}}) 插件取代。

这个插件用于将多个服务器合并成一个服务器包。仅支持使用 JSON 格式的 xray/v2ray 服务器。

首先在主窗口中钩选你想合并的服务器，然后点 “拉取” 小按钮。  
也可以直接从主窗口拖服务器到 “内容” 中的空白区。  
{{< figure src="../../images/plugins/plugin_pacman.png" >}}

然后点 “打包” 或 “串连”，此时主窗口会多出一个和设置同名的服务器
{{< figure src="../../images/plugins/form_main_pkgv4.png" >}}

想知道原理可以点开 Text 编辑器，查看具体配置。

###### 打包

利用 v2ray 的 balancer，将多个服务器合成带负载均衡功能的服务器包。

###### 串连(v1.5.6+)

将多个服务器串成一条代理链。  
友情提示：不要串太长，不然内存受不了。
