---
title: "Commander"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 5
---

这个插件设计用于调用 core 的 API 命令。  
{{< figure src="../../images/plugins/plugin_commander.png" >}}

###### 注意事项

- 关闭窗口后程序会继续执行，直到禁用插件或者退出 V2RayGCon
- 多行“参数”会自动合并成一行
- “环境变量”必需每个一行，格式：`A= "hello`，等号后面所有内容，包括空格和双引号都是 A 的值
- “参数”和“环境变量”编辑器中，可以在开头添加 `//` 来注释掉整行
