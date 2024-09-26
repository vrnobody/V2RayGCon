---
title: "各种分享链接"
date: 2020-02-01T22:27:56+08:00
draft: false
weight: 80
---

##### socks://...
`v1.8.9`起支持v2rayN的`socks://auth@host:port#remark`链接  
auth是把`username:password`进行base64编码得出  
host如果是IPv6要加中括号  
remark需要进行URI encode  

##### ss://...
仅支持`ss://(base64)#name`形式的分享链接  
`v1.5.8.8`起支持导入SIP002链接  

##### trojan://...
仅支持[trojan-url](https://github.com/trojan-gfw/trojan-url)定义的分享链接标准  
`v1.6.9` 起支持和`vless://...`相同的各项参数  

##### v://...
这是本软件自创的一种分享链接。`简易编辑器`里的各种配置组合都可以用这种链接导入、导出。  
它又叫做vee链接，主要特点是短，编码思想出自v2ray-core [issue 1392][2]。具体实现可以看[VeeDecoder.cs][1]，不过代码是用Component的方式写的，比较散比较乱。  

注意！`v1.8.4`起不再支持`v://...`链接。  

##### v2cfg://...
这也是本软件自创的一种分享链接，主要用于备份、还原数据，目前有两个版本。v1直接把整个config.json进行base64编码得出。`v1.8.5`起v2cfg://...升级为v2。它内部使用json格式，序列化后gzip压缩成二进制，最后base64编码成文本，细节见[V2Cfg.cs](https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Models/Datas/V2Cfg.cs)。旧版客户端导出的v1链接可以在新版客户端导入，但旧版客户端无法导入v2链接。升级到v2主要目的是支持yaml等其他配置格式。  

因为v2ray功能过于强大，有可能被有心人利用，通过revers把本地端口暴露到公网，所以v2cfg://...链接除了`主窗口`-`文件`-`从剪切板导入`外，其他地方都不能导入。  

再补充一个例子，精心设计的"burstObservatory.pingConfig.connectivity"可以用来收集用户信息。"burstObservatory"还可以用来干其他事情。不过"reverse"和"burstObservatory"并不是最危险的配置项，还有破坏力更大的这里就不详说了。其实根本问题不是配置项是否安全，而是配置项不能完全受控于人。所以不要用提供完整配置的订阅源。  

##### vless://...
`v1.5.2`起支持Xray-core [issues 91](https://github.com/XTLS/Xray-core/issues/91)提出的vless分享链接标准  
`v1.5.4`支持到`3月7日`的修订，即暂不支持`gRPC`传输类型  
`v1.5.6.1`起支持`gRPC`的`gun`及`multi`模式，不支持`guna`模式  
`v1.6.9`起支持Xray-core的reality  
`v1.9.5`起支持HTTPUpgrade以及gRPC的authority参数  

p.s. 这个学院派的标准设想得很美好，然而经过两年多（2023-07）实战考验后发现到处是坑  

首先是提取链接的坑：  
在理想的世界里面，每条链接应该单独一行，但是很不幸我们并不生活在那样的世界里面。现实中多个链接有时用空格分隔，有时用逗号分隔。所以提取单个链接的时候用split("\r", "\n", " ", ",")就行了。。。吗？并不行！还有像"vless://...","vless://..."或者&lt;li>vless://...&lt;/li>&lt;li>vless://...&lt;/li>等等千奇百怪的分隔形式，用split()根本处理不过来。既然split()不行我们可以用正则吗？以前vmess://(base64)形式的分享链接可以，然而vless://...用的是URI标准，这个正则写起来不是一般的复杂。在这提醒一句网上搜到的正则是有坑的，有些链接会匹配不到。V2RayGCon里面的正则也有坑，会把链接以外的字符也匹配进去。

其次是分解链接的坑：  
1. host:port之间有冒号，但IPv6地址里面也有冒号，所以不要对整个链接split(":")  
2. query前面有时候是"?"有时候是"/?"，但不要用split("/", "?")，因为有些链接的alpn没按标准转义，直接填"h2,http/1.1"  
3. HTML会把&转义成&amp;amp;但这还没完，在某些基于网页的应用里复制、粘贴&amp;amp;会变成&amp;amp;amp;依此类推。最近有些分享链接还玩出新花样，把&替换成\u0026，这已经不符合URI标准了  
4. 有些分享链接会把reality的serverName填在host里而不是sni之中。当然这属于乱搞了，不能算分享链接标准的坑了  

上面分解链接的坑表面上看是因为没遵守标准，其实深层的原因是标准太依赖君子协定式的转义。很多人喜欢手搓URI编、解码代码导致转义要求没有约束力。举个反例JSON就没多少人手搓编、解码代码，转义会规范很多。提取链接的坑则是因为手搓编、解码的参数值没有转义直接使用明文，难以和其他文本区分。JSON也有难以区分的缺陷也不适合用做链接主体。目前最容易提取的是没加#号尾巴的base64。  

在这里发一堆牢骚是因为vless://...的解码代码我已经改了不下十次。然而人们群众的智慧是无限的，相信过不了多久又要改代码。希望有缘看到这里的小伙伴少踩点坑。也希望以后要是有新的协议需要定制分享链接标准时，请多从实用的角度考虑，不要老想着往一个本来就不是为分享链接设计的标准去靠。  

##### vmess://...
仅支持v2rayN的vmess(ver2)分享链接，不支持其他vmess分享链接  

##### 各种链接长度
| 链接类型 | 平均长度(bytes) |
| ------ | --- |
| vless | 200 |
| trojan | 200 |
| vmess | 300 |
| config.json | 1000 |
| v2cfg v2 | 800 |
| userSettings.json | 240 |

[1]: https://github.com/vrnobody/V2RayGCon/blob/1.8.3/V2RayGCon/Services/ShareLinkComponents/VeeDecoder.cs "VeeDecoder.cs"
[2]: https://github.com/v2ray/v2ray-core/issues/1392 "v2ray-core #1392"
