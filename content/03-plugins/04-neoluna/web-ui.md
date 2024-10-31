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
在WinForm界面的“选项窗口-设置-托盘单击”中填入`webui://localhost:4000`，可以实现点下托盘图标，调出WebUI专用窗口。  
上面的设置项还可以填`http://localhost:4000`，调出浏览器。或者填快捷方式(.lnk)的路径启动外部程序。  

更多信息请看[WebUI项目](https://github.com/vrnobody/WebUI)。  