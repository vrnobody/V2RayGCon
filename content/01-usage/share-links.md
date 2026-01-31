---
title: "各种分享链接"
date: 2020-02-01T22:27:56+08:00
draft: false
weight: 80
---

##### hy2://...

`v2.2.4` 起支持解码 hy2://... 链接为 xray-core 的 config.json  
注：只是按文档写的，我自己并不使用 hy2，如果配置不当请发 issue

##### mob://...

mob 是 MetaOutBound 的缩写。你可以在 [mob playground](../../pages/mob.html) 在线体验编、解码效果。链接格式详见 [mob://...]({{< relref "01-usage/mob-share-link.md" >}}) 说明。

##### socks://...

`v1.8.9` 起支持 v2rayN 的 `socks://auth@host:port#remark` 链接  
auth 是把 `username:password` 进行 base64 编码得出  
host 如果是 IPv6 要加中括号  
remark 需要进行 URI encode

##### ss://...

`v1.5.8.8` 起支持导入 SIP002 链接  
链接格式：`ss://(base64)#name`

##### trojan://...

`v1.6.9` 起支持和 `vless://...` 相同的各项参数  
链接格式： [trojan-url](https://github.com/trojan-gfw/trojan-url)

##### v://...

注意！`v1.8.4` 起废弃 `v://...` 链接。  
这是本软件自创的一种分享链接。“简易编辑器” 里的各种配置组合都可以用这种链接导入、导出。  
它又叫做 vee 链接，主要特点是短，编码思想出自 v2ray-core [issue 1392][2]。具体实现可以看 [VeeDecoder.cs][1]，不过代码是用 Component 的方式写的，比较散比较乱。

##### v2cfg://...

这也是本软件自创的一种分享链接，主要用于备份、还原数据，目前有两个版本。 `v1` 直接把整个 config.json 进行 base64 编码得出。`v1.8.5` 起 `v2cfg://...` 升级为 `v2`。它内部使用 json 格式，序列化后 gzip 压缩成二进制，最后 base64 编码成文本，细节见 [V2Cfg.cs](https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Models/Datas/V2Cfg.cs)。旧版客户端导出的 `v1` 链接可以在新版客户端导入，但旧版客户端无法导入 `v2` 链接。升级到 `v2` 主要目的是支持 `yaml` 等其他配置格式。

因为 v2ray 功能过于强大，有可能被有心人利用，通过 revers 把本地端口暴露到公网，所以 `v2cfg://...` 链接除了 “主窗口” - “文件” - “从剪切板导入” 之外，其他地方都不能导入。

再补充一个例子，精心设计的 `burstObservatory.pingConfig.connectivity` 可以用来收集用户信息。`burstObservatory` 还可以用来干其他事情。不过 `reverse` 和 `burstObservatory` 并不是最危险的配置项，还有破坏力更大的这里就不详说了。其实根本问题不是配置项是否安全，而是配置项不能完全受控于人。所以不要用提供完整配置的订阅源。

##### vless://...

`v1.5.6.1` 起支持 `gRPC` 的 `gun` 及 `multi` 模式，不支持 `guna` 模式  
`v2.0.8.0` 起支持 `xhttp` 的 `mode` 参数，不支持 `extra` 参数  
`v2.2.4.0` 起支持 `tlsSettings.pinnedPeerCertSha256`  
链接格式：[Xray-core issues #91](https://github.com/XTLS/Xray-core/issues/91)

p.s. 这个学院派的标准设想得很美好，然而经过两年多（2023-07止）实战考验后发现到处是坑

首先是提取链接的坑：  
在理想的世界里面，每条链接应该单独一行，但是很不幸我们并不生活在那样的世界里面。现实中多个链接有时用空格分隔，有时用逗号分隔。所以提取单个链接的时候用 `split("\r", "\n", " ", ",")` 就行了。。。吗？并不行！还有像 "vless://...","vless://..." 或者 &lt;li>vless://...&lt;/li>&lt;li>vless://...&lt;/li> 等等千奇百怪的分隔形式，用 `split()` 根本处理不过来。既然 `split()` 不行我们可以用正则吗？以前 `vmess://(base64)` 形式的分享链接可以，然而 `vless://...` 用的是 URI 标准，这个正则写起来不是一般的复杂。在这提醒一句网上搜到的正则是有坑的，有些链接会匹配不到。V2RayGCon 里面的正则也有坑，会把链接以外的字符也匹配进去。

其次是分解链接的坑：

1. host:port 之间有冒号，但 IPv6 地址里面也有冒号，所以不要对整个链接 split(":")
2. query 前面有时候是 "?" 有时候是 "/?"，但不要用 split("/", "?")，因为有些链接的 alpn 没按标准转义，直接填 "h2,http/1.1"
3. HTML 会把 `&` 转义成 `&amp;` 但这还没完，在某些基于网页的应用里面多复制、粘贴几次 `&amp;` 会变成 `&amp;amp;amp;...` 最近有些分享链接还玩出新花样，把 `&` 替换成 `\u0026`，这已经不符合 URI 标准了
4. 有些分享链接会把 reality 的 serverName 填在 host 里而不是 sni 之中。当然这属于乱搞了，不能算分享链接标准的坑了

上面分解链接的坑表面上看是因为没遵守标准，其实深层的原因是标准太依赖君子协定式的转义。很多人喜欢手搓 URI 编、解码代码导致转义要求没有约束力。举个反例 JSON 就没多少人手搓编、解码代码，转义会规范很多。提取链接的坑则是因为手搓编、解码的参数值没有转义直接使用明文，难以和其他文本区分。JSON 也有难以区分的缺陷也不适合用做链接主体。目前最容易提取的是没加 # 号尾巴的 base64。

在这里发一堆牢骚是因为 vless://... 的解码代码我已经改了不下十次。然而人们群众的智慧是无限的，相信过不了多久又要改代码。希望有缘看到这里的小伙伴少踩点坑。也希望以后要是有新的协议需要定制分享链接标准时，请多从实用的角度考虑，不要老想着往一个本来就不是为分享链接设计的标准去靠。

##### vmess://...

仅支持 v2rayN 的 vmess(ver2) 分享链接，不支持其他 vmess 分享链接

##### 各种链接长度

| 链接类型          | 平均长度(bytes) |
| ----------------- | --------------- |
| hy2               | 70              |
| v                 | 120             |
| userSettings.json | 180             |
| vless             | 200             |
| trojan            | 200             |
| mob               | 300             |
| vmess             | 300             |
| v2cfg v2          | 800             |
| config.json       | 1000            |
| v2cfg v1          | 1320            |

##### 处理速度

每秒大概可以导入 1000 个链接。导入结果窗口每秒加载约 1000 个结果。

##### 资源需求（大概）

| 每 1 万个服务器   | 数值 | 单位 | 补充说明             |
| :---------------- | :--: | :--: | :------------------- |
| 内存（memory）    | 100  | MiB  |                      |
| userSettings.json |  2   | MiB  |                      |
| 存盘耗时          |  1   |  秒  |                      |
| 十年写盘量        | 881  | GiB  | 7 \* 24 高强度使用下 |

[1]: https://github.com/vrnobody/V2RayGCon/blob/1.8.3/V2RayGCon/Services/ShareLinkComponents/VeeDecoder.cs "VeeDecoder.cs"
[2]: https://github.com/v2ray/v2ray-core/issues/1392 "v2ray-core #1392"
