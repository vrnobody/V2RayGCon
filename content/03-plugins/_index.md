---
title: "插件"
date: 2020-01-31T21:28:12+08:00
draft: false
weight: 3
---

这个软件通过插件的方式，提供一些辅助功能。可以在 “选项” 窗口的 “插件” 分页中启用或关闭。  

{{< figure src="../images/forms/form_option_plugins.png" >}}

然后从主窗口（或者托盘图标）的 “插件” 菜单进入相应插件。  
{{< figure src="../images/forms/form_main_plugins.png" >}}

##### 第三方插件
从 `v1.8.3` 开始又重新支持第三方插件。把发布文件解压到 V2RayGCon 目录内，然后钩上 “第三方” 选项，再点 “刷新” 就会出现在列表里面。第三方插件制作方法详见 [IPlugin.cs](https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Interfaces/IPlugin.cs) 里面的注释。简单来说就是，创建一个 .net framework 4.5 的 dll 项目并引用 VgcApis 项目，然后实现 IPlugin 接口。如果你想分享自己写的插件，直接发 issue 即可。  
  
安全提示：  
点选 “刷新” 按钮会执行 3rd/plugins 目录中插件 dll 的代码，有一定风险！  
由于我水平不高加上本项目的特殊性，我不会对第三方插件做任何检查。  
  
第三方插件列表：  
<等你来添加>  
