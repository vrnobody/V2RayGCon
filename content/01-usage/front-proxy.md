---
title: "前置代理"
date: 2020-02-06T11:05:55+08:00
draft: false
weight: 50
---

实现前置代理最简单的办法是使用 [Pacman 插件]({{< relref "03-plugins/02-pacman.md" >}})，把多个服务器串连起来变成一条代理链。这时会先连接第一个服务器，然后连接第二、第三……最后连接目标网站。  

使用插件的缺点是，每次变更服务器都要重新生成代理链。另一个办法是使用 http 或者 socks 协议作为中介，串接两个服务器。  
首先在选项窗口里面添加两个模板：  
```json
{
  // 名称：front-proxy
  // 合并方式: ModifyOutbound
  // 参数: agentout

  "proxySettings": {
    "tag": "http-out",
    "transportLayer": true
  }
}
```
注：“合并方式” 为 `ModifyOutbound` 的模板只和 outbound 合并。“参数” 用于匹配 outbound 的 `tag` 前缀。  

```json
{
  // 名称：http-out
  // 合并方式: Concat

  "outbounds": [
    {
      "tag": "http-out",
      "protocol": "http",
      "settings": {
        "servers": [
          {
            "address": "127.0.0.1",
            "port": 8888
          }
        ]
      }
    }
  ]
}
```

然后钩选这两个模板并保存。这时查看任意一个服务器的最终配置会看到上面两个模板的内容。接着把用落地代理的服务器的 inbound 设置成 `http` 协议，使用 `8888` 端口，注意不要钩选 “*注入” 选项。最后利用 “多开” 功能，开启任意一个前置代理的同时开启一个落地代理。  

但是这样把前置代理和落地代理混在一起，并不好管理。折中的办法是使用 [多实例]({{< relref "01-usage/multiple-inst-lnk.md" >}}) 功能再开启一个 `V2RayGCon` 实例，分别存放前置代理和落地代理。  

小技巧：  
把 http 出站替换为 freedom 出站，就实现了分片功能。具体参考文本编辑器里面的示例。  
前置代理不一定是单个服务器，还可以是多个 outbounds 组成的负载均衡器。落地代理也可以是负载均衡器。  
