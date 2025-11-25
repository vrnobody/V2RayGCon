---
title: "mob://..."
date: 2020-02-01T22:27:56+08:00
draft: false
weight: 90
---
mob://... 是 V2RayGCon v2.2.1.2 新增的分享链接类型。意思是 (M)eta(O)ut(B)ound。这个链接支持多种 v2ray/xray 协议，省去为每个协议定义一个分享链接的麻烦。  

#### 链接格式
`mob://(base64 encoded json body)`

#### json body 格式
```json
{
    "ver": "1",      // 链接版本
    "server": [],    // 服务器配置
    "protocol": [],  // 协议配置
    "stream": [],    // streamSettings 配置
    "enc": []        // tls, xtls, reality 等加密配置
}
```
注1：所有数据都是字符串类型。  
注2：数组末尾的空字符串可以省略，但是夹在中间的不可以省略。  

#### server 配置数组
|参数 #1|参数 #2|参数 #3|
|-|-|-|
|name|address|port|

注：name 存放分享链接的名字  

#### protocol 配置数组
|参数 #1|参数 #2|参数 #3|参数 #4|
|-|-|-|-|
|vmess|UUID|||
|vless|UUID|flow|encryption|
|trojan|password|flow||
|ss / shadowsocks|password|method||
|socks / http|user name|password||

注: V2RayGCon v2.2.1.3+ 才支持 http 协议  

#### stream 配置数组
|参数 #1|参数 #2|参数 #3|参数 #4|
|-|-|-|-|
|tcp / raw||||
|ws / h2 / httpupgrade|path|host||
|xhttp|mode|path|host|
|grpc|multi mode: "true" / "false"|service name|authority|

注：stream: [] 表示 streamSettings 留空。

#### enc 配置数组
|参数 #1|参数 #2|参数 #3|参数 #4|参数 #5|参数 #6|参数 #7|参数 #8|
|-|-|-|-|-|-|-|-|
|tls / xtls|server name|fingerprint|alpn|ech|||||
|reality|server name|fingerprint|alpn|public key|short ID|spider X|ML-DSA-65|

注：enc: [] 表示 tls/xtls/... 配置留空。

#### 示例
分享链接：
{{< rawhtml >}}
<p style="padding: 1rem; background-color: #eed; word-break: break-all;  word-wrap: break-word;">mob://eyJ2ZXIiOiIxIiwic2VydmVyIjpbInNlcnYxIiwiMS4yLjMuNCIsIjU2NzgiXSwicHJvdG9jb2wiOlsidmxlc3MiLCIzY2Y0MTJjMC1kZjJhLTQ4ZGYtOWNjOC02MTQ4MTQwZTRlNDgiLCIiLCJub25lIl0sInN0cmVhbSI6WyJ3cyIsIi9wYXRoIl0sImVuYyI6WyJ0bHMiLCJiaW5nLmNvbSJdfQ==</p>
{{< /rawhtml >}}

解码链接主体的 base64 得到 json body:
```json
{
  "ver": "1",
  "server": [
    "serv1",
    "1.2.3.4",
    "5678"
  ],
  "protocol": [
    "vless",
    "3cf412c0-df2a-48df-9cc8-6148140e4e48",
    "",
    "none"
  ],
  "stream": [
    "ws",
    "/path"
  ],
  "enc": [
    "tls",
    "bing.com"
  ]
}
```

解码成 xray-core v25.10.15+ 简化配置：  
```json
{
  "log": {
    "loglevel": "warning"
  },
  "inbounds": [
    {
      "tag": "agentin",
      "port": 1080,
      "listen": "127.0.0.1",
      "protocol": "socks",
      "settings": {}
    }
  ],
  "outbounds": [
    {
      "protocol": "vless",
      "settings": {
        "address": "1.2.3.4",
        "port": 5678,
        "id": "3cf412c0-df2a-48df-9cc8-6148140e4e48",
        "encryption": "none"
      },
      "tag": "agentout",
      "streamSettings": {
        "network": "ws",
        "security": "tls",
        "wsSettings": {
          "path": "/path",
          "headers": {}
        },
        "tlsSettings": {
          "serverName": "bing.com"
        }
      }
    }
  ]
}
```

`mob://...` 只定义了分享数据格式，开发者可以自行决定怎么解码。  
