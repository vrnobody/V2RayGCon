---
title: "开发相关"
date: 2020-02-01T18:16:55+08:00
draft: false
weight: 4
---

##### 开发环境
这个项目使用 Visual Studio 2019 编写。如果你用的是 Visual Studio 2022 请看下面这链接：  
[https://stackoverflow.com/questions/70022194/open-net-framework-4-5-project-in-vs-2022-is-there-any-workaround](https://stackoverflow.com/questions/70022194/open-net-framework-4-5-project-in-vs-2022-is-there-any-workaround)  

##### 备用
[.net framework SDK 4.5](../images/releases/dotnetf45.zip)

##### 内存占用
这个软件的运行数据都在内存里，每 1 万个服务器约占用 100M 内存。服务器数量设计上限是 100 万个。  

##### 在GitHub上发送加密信息：
把下面的内容保存为 `nobody.pub`
```txt
-----BEGIN PUBLIC KEY-----
MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEArbiEOyfnFB0rrIWW6P//
wYYBx8t3OmUei/qfbtQ+V+udti/XS/nHAqkxib291qrtO5W0QwlccNmrSvPYCfBO
DbzXv3ZYr+KyV69gYru0BJ2dLkMQv+6S1Y4NewNypzVqQ+rT173s560JDjSPGbsb
adFd1e2KoFP7IpxJA/Dq1WXnOnaVlnd7DHq5TSqMvU4vUyEHlu/BOL7lV42+LN1Z
hwv/qd4fRmb7JoR0crCl72lO+9r8snb089gIwmkRiqPcW2fSiJR4+fwD9Sta7OPy
Og5IAYYZe/vHPkyGMf4FLzVTtH0Xs8NNDXBnYfT6mArAv2zfcPl1tz3uzAhRPJWg
xy2gDaih6h+kAr1qf8VxcXw3iHdQ3bR0QFgWMhKZGnEGQ9gugtLJUtemim9NTTwm
IPjSNiTdRdB6173JuMImD2PTkFPYXBWz6gOIUgMnzg0P3JFeQJSm1GWS2gvuRiyi
cGKdZOhFug7oyt1wkEhWTgKTmJygZGmxrHk0ZKmX4hVI3U6pZXCB23Asi1qDNEiu
ydgsIpFmMfn0XiMAhnhxiovDbkgHO/rtQgHJCAFTdVgJgXmj7xL5VbM8rh0QoEkq
3JgNdSsGF5t7jh0dDmv2HN2ijAFcWehvliy+cqNvI4+DRD3JJAxZSwyBtuvzrGSs
zviSOYV9eF1wKxSNCP6KnasCAwEAAQ==
-----END PUBLIC KEY-----
```

加密发送信息：
```bash
echo "你好😀" | openssl pkeyutl -encrypt -inkey nobody.pub -pubin | base64 -w 0
```
*注意，这个方法只能发送少量文本*

解密接收信息：
```bash
# 生成私钥，注意保密：
openssl genrsa -out private.pem 4096

# 生成 public.pem 发给对方：
openssl rsa -in private.pem -pubout -out public.pem

# 解密对方发来的 base64 信息：
echo "aGVsbG8K..." | base64 -d | openssl pkeyutl -decrypt -inkey private.pem
```

图片嵌入文件：  
```bash
# 写入文件
cat V2RayGCon.zip | base64 -w0 | exiftool -z '-Description<=-' vgc.png

# 读取文件
exiftool -s3 -Description vgc.png | base64 -d > vgc.zip
```
