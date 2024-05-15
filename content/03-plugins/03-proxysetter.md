---
title: "ProxySetter"
date: 2020-02-01T17:58:23+08:00
draft: false
weight: 40
---

这个插件有两个相互独立的功能，一个是在 `Tuna` 启用 “tun模式”，另一个是在 `PAC` 页面设置windows系统 “Internet选项” 里的代理设定。这两个功能使用其中一个就好，同时使用会互相干扰。  

#### Tuna
{{< figure src="../../images/plugins/proxysetter_tuna.png" >}}
  
Tuna通过tun2socks开启tun模式，使用前需要先下载下面两个软件：  
[https://www.wintun.net/](https://www.wintun.net/)  
[https://github.com/xjasonlyu/tun2socks](https://github.com/xjasonlyu/tun2socks)  
把wintun.dll和tun2socks-*.exe放到同一个文件夹内就可以。  
如果你下载的是v1.8.8及以后版本的V2RayGCon-box.zip则已经自带这两软件，无需另外下载。  

第一次进入Tuna设置界面时，他会自动填充各项参数。如果左侧的参数不合适，可以自行修改然后点 “生成参数” 按钮，右边的 “启动参数” 会更新。tun2socks需要admin权限运行，所以每次点启动时都会弹出UAC确认窗口。如果以管理员权限启动V2RayGCon可以避免弹UAC窗口。但是非常不建议这么做，因为很不安全。tun会转发UDP和TCP两种协议的数据，所以服务器的inbound最好是socks协议并开启UDP支持。如果DNS留空那么DNS请求将从原来的网卡发出，俗称DNS泄露。如果不钩IPv6而你的网络又支持IPv6，那么访问IPv6网站的时候就是在裸奔。但是IPv6需要本地和远程服务器同时支持才会有完整的体验。开启tun之后PAC分流失效，需要配置v2ray-core的routing来分流。可以在选项窗口编写routing模板然后钩选。重启一下服务器，选中的模板就会注入到配置里面。  

tun的原理是新建一张虚拟网卡，然后网络流量优先从这张网卡发送。但是有些不讲武德的软件可以无视这个优先级，直接从物理网卡发送数据。v2ray就是其中之一。所以开启tun模式并不等于所有流量都走代理。  

tun对复杂的网络不太友好，比如需要同时访问专用网、内网、外网。这种场景建议使用按进程代理的软件：  
[https://github.com/PragmaTwice/proxinject](https://github.com/PragmaTwice/proxinject)  

因启用tun模式导致网络崩了，用netsh命令修复一下路由表就行。不想打命令的话就禁用ProxySetter插件然后重启电脑。如果重启后网络还是有问题，那大概率不是tun引起的。  

#### PAC及系统代理
{{< figure src="../../images/plugins/plugin_proxysetter.png" >}}
最右侧的 “在记事本中...” 和 “在浏览器中...” 对调试问题会有点帮助。  

通常选下 “PAC” 或者 “系统”，然后钩上后面的自动（默认已钩）就不用管其他选项了。“自动” 的意思是当你切换不同服务器时，代理设定会跟着更新（日志窗口有相应提示）。不过 “自动” 不是万能的，假如你选了 “系统” 模式，但只开启一个socks服务器，那么就没法设定代理。因为windows的系统代理必需是http代理。  

基于以下几点原因，本项目不使用GFWList作为默认PAC源：
 1. “正版” GFWList已经很久没更新，很多网站已经不适用
 2. 其他人维护的GFWList多如牛毛，选哪个都会有人跳出来说不好
 3. GFWList使用正则来匹配域名的算法，已在其他（非v2ray相关）项目中显示出弊端

不过。。。这个插件有个 “自定义PAC” 功能，想用什么就用什么吧。  
使用自定义PAC的时候，注意主窗口中服务器的模式和端口要和你所使用的PAC相匹配。浏览器调试功能以及 “设置” 分页中的白黑名单设置对自定义PAC无效。  
 