﻿using System.Drawing;
using System.Windows.Forms;

// https://code.msdn.microsoft.com/windowsdesktop/Creating-a-simple-plugin-b6174b62
namespace VgcApis.Interfaces
{
    // 选项窗口插件面板点刷新时，有~可~能~调用到Dispose方法。
    public interface IPlugin : System.IDisposable
    {
        // 插件以名字区分，不允许重名。
        // 这个属性会在选项窗口的插件面板、主窗口的插件菜单以及托盘右键菜单中显示。
        string Name { get; }

        // 插件的版本信息，在选项窗口的插件面板中显示。
        string Version { get; }

        // 对插件的简单介绍，在选项窗口的插件面板中显示。
        string Description { get; }

        // 插件图标，在主窗口插件菜单以及托盘右键菜单中显示。
        Image Icon { get; }

        // 选项窗口插件面板中启用并保存后执行这个函数。
        // 注意有可能被多次调用。
        // 特别注意处理Dispose后被调用到的情况。
        // 可以通过传入的参数api使用V2RayGCon提供的功能。
        // 比如api.GetUtilsService().GetAppVersion()获取软件版本号。
        void Run(Services.IApiService api);

        // 点主窗口插件菜单时执行这个函数。
        // 注意有可能被多次调用。
        void ShowMainForm();

        // 选项窗口插件面板中取消启用并保存后会执行这个函数。
        // 注意有可能被多次调用。
        // 建议实现IDisposable时也调用一下这个函数。
        void Stop();

        // 自定义托盘右键插件菜单。托盘菜单刷新时调用。
        // 托盘菜单经常刷新，建议添加缓存机制。
        ToolStripMenuItem GetToolStripMenu();
    }
}

/*
创建自己的插件时，添加一个对VgcApis项目的引用，然后实现IPlugin接口就行。
不用像3rd/Luna那样搞得那么复杂。

编译生成文件目录结构如下：

3rd/plugins/里面放插件主体文件。
例如：Luna.dll, Luna.dll.config, Luna.pdb
上面的文件也可以放文件夹里。

libs/里面放插件引用到的其他dll。
例如：Newtonsoft.Json.dll, NLua.dll
如果用到和主项目相同的dll时，注意NuGet包的版本要相同。

其他目录随意。
*/
