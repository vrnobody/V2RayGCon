---
title: "Web UI"
date: 2020-02-25T13:40:01+08:00
draft: false
weight: 40
---

winforms太土？来试一下Web界面吧！  

#### light主题：  
{{< figure src="../../../images/webui/light_v0.0.2.0.png" >}}

#### dark主题：  
{{< figure src="../../../images/webui/dark_v0.0.2.0.png" >}}

#### 运行lua脚本：  
{{< figure src="../../../images/webui/luna_print_w.png" >}}

#### 使用方法

NeoLuna插件中运行以下脚本：  
```lua
loadfile('3rd/neolua/webui/server.lua')()
```
然后在浏览器中访问[http://localhost:4000/](http://localhost:4000/)  

小技巧：  
WinForm界面的选项窗口-设置-托盘单击中输入'http://localhost:4000'可以实现点下图标就打开WebUI。  
上面这个设置项也可以填快捷方式(.lnk)的路径，这个玩法就更多了。  
更多信息请看[WebUI项目](https://github.com/vrnobody/WebUI)。  