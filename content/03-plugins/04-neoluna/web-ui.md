---
title: "Web UI"
date: 2020-02-25T13:40:01+08:00
draft: false
weight: 40
--- 

#### 使用方法
NeoLuna 插件中运行以下脚本：  
```lua
loadfile('3rd/neolua/webui/server.lua')()
```
然后在浏览器中访问 [http://localhost:4000/](http://localhost:4000/)  

小技巧：  
在 WinForms 界面的 “选项窗口-设置-托盘单击” 中填入 `webui://localhost:4000`，可以实现点下托盘图标，调出 WebUI 专用窗口。  
上面的设置项还可以填 `http://localhost:4000`，调出浏览器。或者填快捷方式（.lnk）的路径启动外部程序。  

#### 进阶用法
```lua
local Logger = require('3rd/neolua/mods/logger')

local serv = '3rd/neolua/webui/server.lua'
local options = {
    ["url"] = "http://localhost:5000/",
    ["password"] = "123abc中文😀",
    ["adminpassword"] = "123456",
    ["salt"] = "485c5940-cccd-484c-883c-66321d577992",
    ["pageSize"] = "50",
    ["public"] = "./3rd/neolua/webui",
    ["logLevel"] = Logger.logLevels.Info,
}

loadfile(serv)(options)
```

password 存储在本地，下次打开浏览器不需要再次输入。可以点左下角的 “登出” 清除密码。  
adminpassword 在关闭浏览器后自动清除。修改设置、使用 NeoLuna 等危险操作时会检查这个密码。  
上面两个选项留空就不会弹出输密码窗口。  

安全提示：  
密码验证不能防中间人攻击，请用 Nginx 之类的反向代理并启用 TLS 以提高安全性。  
但是！这还是防不了暴力破解，所以最好还是别放到公网上。  

更多信息请看 [WebUI 项目](https://github.com/vrnobody/WebUI)。 

#### 运行 lua 脚本：  
{{< figure src="../../../images/webui/luna_print_w.png" >}}

#### dark 主题：  
{{< figure src="../../../images/webui/dark_v0.0.2.0.png" >}}

#### light 主题：  
{{< figure src="../../../images/webui/light_v0.0.2.0.png" >}}
 