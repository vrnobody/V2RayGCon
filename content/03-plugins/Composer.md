---
title: "Composer"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 7
---

这个插件用于把多个服务器合并成一个服务器，只支持 json 配置格式。
{{< figure src="../../images/plugins/plugin_composer_form_main.png" >}}
“骨架”是基础配置，选中的“节点”配置会插入到“骨架”的 outbounds 里面。

点击右边“选择”按钮会出现“节点选择器”窗口：
{{< figure src="../../images/plugins/plugin_composer_form_selector.png" >}}
“Tag”插入“骨架”时的 outbound 的 tag 前缀。  
“过滤器”用于动态筛选节点。里面填 [搜索功能]({{< relref "../01-usage/search.md" >}}) 的高级搜索表达式。  
“自定义节点”用于添加不太好用表达式过滤的节点。将改叫“其他节点”。可以从主窗口拖进来，也可以先在主窗口勾选然后点“拉取”。

设置完成后在主界面点“打包”，生成服务器包。
