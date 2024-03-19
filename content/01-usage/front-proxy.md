---
title: "前置代理"
date: 2020-02-06T11:05:55+08:00
draft: false
weight: 50
---

V2Ray 的 outbound 里面有个 proxySettings 配置项，可以用来实现前置代理。  
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

然后钩选这两个模板并保存。这时查看任意一个服务器的最终配置会看到上面两个模板的内容。接着把用作前置代理的服务器的 inbound 设置成 `http` 协议，使用 `8888` 端口，注意不要钩选 “*注入” 选项。最后利用 “多开” 功能，开启前置代理的同时开启任意一个服务器。这时代理的连接会先经过前置代理，再经过代理服务器，最后到达目标地址。  

上面这个方法用起来比较麻烦，更推荐的用法是使用 [多实例]({{< relref "01-usage/multiple-inst-lnk.md" >}}) 功能开启两个 `V2RayGCon` ，其中一个专门用作前置代理。  

##### 落地代理
当 A 是 B 的前置代理时，B 就是 A 的落地代理。  

小技巧：  
把 http 出站替换为 freedom 出站，就可以使用分片功能。  
前置代理不一定是单个服务器，还可以是多个 outbounds 组成的负载均衡器。落地代理也可以是负载均衡器。  
