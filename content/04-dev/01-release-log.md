---
title: "发布记录"
date: 2020-02-01T22:16:17+08:00
draft: false
weight: 10
---

##### `v2.1.9.0` 20251014
减小关机时数据丢失的概率  
服务器面板添加“移动-至指定序号”菜单  
工具箱窗口启用监视剪切板功能时，自动清空当前已复制的内容  
Commander 插件不选“隐藏窗口”时禁用 STDIN/STDOUT 功能  

##### `v2.1.8.0` 20250913
工具箱窗口添加计算 Hash 功能  
Commander 插件的参数和环境变量支持注释  
Commander 插件环境变量改为必需每个一行  
支持显示 xray-core 的简化版 outbound 配置  
暂时不支持生成简化版配置  

##### `v2.1.7.0` 20250905
工具箱添加 vlessenc 生成密钥功能  
工具箱窗口添加监视切剪板功能  
更新几个配置示例  
Commander 插件添加快捷键  
Commander 插件日志添加日期时间  
std.Web:Fetch() 及订阅，支持带账号密码的 URL  

##### `v2.1.6.0` 20250830
添加 Commander 插件，用于调用命令行 API  
添加 vless 协议的 encryption 配置项支持  
工具箱添加解码 vmess://... 功能  
工具箱添加字符串大小写转换功能  
降低写盘配置文件时的内存占用  

##### `v2.1.5.0` 20250819
修复 win7 扫描二维码会崩溃的问题  
修复 std.Misc:Choice() 传入空列表会崩溃的问题  
添加工具箱窗口，用于生成密钥对、密码等  
添加 std.Misc:ShowFormToolbox()  
Text 编辑器添加示例菜单  

##### `v2.1.4.0`
支持 tls.ech 和 reality.mldsa65  
更新订阅无代理可用时，询问是否切换到直接下载  
检查 core 更新超时从 60 秒调整为 20 秒  

##### `v2.1.3.0`
解除服务器数量上限封印  
修复置顶、置底时服务器顺序错乱的问题  
缩短生成的服务器包配置的 tag 序号长度  
添加 #goto #note 搜索过滤器，原 #take 过滤器支持负数参数  
添加 std.Server:MoveServers(uids, destTopIndex) 函数  

##### `v2.1.2.0`
PAC 白名单改用 Loyalsoldier/v2ray-rules-dat  
界面少量调整  

##### `v2.1.1.0`
非便携模式下，用户数据改为保存到 %ProgramData% 目录内  
主窗口的服务器面板添加一个重启按钮  
降低导入大量（10万+）节点的内存占用量  
简易编辑器添加生成服务端配置功能  
修复导入结果窗口加载完成前关闭，会崩溃的问题  

NeoLuna 插件添加以下函数：  
std.Misc:GenServerSideConfig(meta)  
std.Misc:GetMetaData(config)  
std.Web:UnicodeEncode(str)  
std.Web:UnicodeDecode(str)  

##### `v2.1.0.0`
配置编辑器格式化JSON失败时，滚动到相应行  

##### `v2.0.9.0`
格式化JSON失败时，显示详细错误信息  
给 websocket 协议添加一个重复的 host 配置项  
自动生成的搜索关键词改为 #mark is (mark)  
NeoLuna 编辑器打开时聚焦到编辑区域  
NeoLuna 的导航栏初始状态修改为禁用  

##### `v2.0.8.0`
支持raw（原tcp）传输协议  
支持xhttp的mode参数，不-支-持 extra 参数  
添加std.Misc:GetNeoLunaLogAsString()函数  
修复拼写错误std.Sys:Lu[s]ServSetIndex()  

##### `v2.0.7.0`
修复`CVE-2024-48510` ZIP文件路径穿越漏洞  

##### `v2.0.6.0`
优化`#orderby modify`速度  
更新WebUI至v1.0.2  
添加WebUI专用窗口，详见手册NeoLuna/WebUI最底说明  
添加std.Misc:ShowFormWebUI()  
添加std.Servers:GetFilteredServers(keyword)  

##### `v2.0.5.0`
选项窗口添加自定义搜索关键词  
搜索功能添加#take过滤器  
修复搜索时回车键被自动补全拦截问题  
取消`|`(OR操作符)的自动排序功能  
NeoLuna插件添加std.Misc.GetMutexPoolCount()函数  

##### `v2.0.4.0`
搜索功能添加排序过滤器  

##### `v2.0.3.0`
去除组合搜索的"#@"前缀  
优化搜索关键词自动补全体验  

##### `v2.0.2.0`
添加组合搜索功能  
添加not index match搜索操作符  
搜索框不再响应鼠标进入事件  
主窗口改为多列显示  
当前页选择菜单添加几个菜单项  
显示更详细的分享链接导入结果统计信息  
修复NeoLuna插件的std.Misc:ShowData()控件  

##### `v2.0.1.0`
更友好的搜索提示  
支持用双引号搜索带空格的字符串  
支持搜索字符串的开头和结尾  
支持搜索修改日期、自定义core名、是否选中、入站端口  
修复搜索关键字必须小写的bug  
修复批量修改时顺序错乱的bug  

##### `v2.0.0.0`
重新设计搜索功能，详见手册  
修复选项窗口点保存后界面卡顿问题  
优化配置写盘算法  
版本号只是个流水号没特别意思  

##### `v1.9.9.0`
修复配置文件损坏时没弹出错误信息窗口bug  
优化搜索、全选速度  
导入结果窗口显示数量统计信息  
NeoLuna插件的单选、多选窗口支持小键盘数字键  

##### `v1.9.8.0`
添加vrnobody/xraye作为更新源  
支持更新win7版xray-core  
支持订阅链接中的remarks参数  
修复下载core窗口无法停止的bug  

##### `v1.9.7.0`
支持SplitHTTP传输方式  
没版本信息的vmess链接当作v2处理  
修复更新core功能的一个bug  
移除一些旧的兼容代码  

##### `v1.9.6.0`
自定义模板中添加ModifyOutbound合并方式，详见手册：前置代理  
服务器设置面板支持修改标签1-3  
NeoLuna添加wserv:GetShareLinkMetaData()函数  
修复导入分享链接SNI多加了port的bug(issue #25)  
修复JSON配置不能使用注释的bug  
Pacman插件生成的配置的connectivity项改为空字符串  
点击服务器面板的延迟或者流量标签时，弹出清除数据窗口  
停止测速时，清除未完成测速的服务器的延迟数据（原来当作超时处理）  
链式代理的摘要改为显示第一个outbound的信息  

##### `v1.9.5.0`
Pacman插件支持leastLoad均衡策略  
vless/trojan分享链接支持HTTPUpgrade及gRPC的authority参数  
修复修改配置模板不生效的bug  

##### `v1.9.4.0`
移除旧配置格式v2ray v3的支持  
修复一处内存泄露  
提高大配置文件(1k+ outbounds）的处理速度  
简化合并规则，outbounds改为仅追加  

##### `v1.9.3.0`
优化一丢丢CPU、内存占用  
配置存盘间隔从5分钟延长至20分钟  
修改模板注入设置后刷新全部服务器摘要  
  
注：  
这次更新内存占用下降40%，CPU占用下降90%，内存分配回收减少99%  
这些数据说明了一件事，之前的程序写得太烂了T.T  
  
##### `v1.9.2.0`
修复流量统计失效bug，仅支持默认core默认配置  
添加std.Misc:SetNotifyIconTag()临时修改托盘标签  
Pacman插件支持roundRobin均衡策略  
打包服务器时tag改为agentout+6位数字  

##### `v1.9.1.0`
支持热切换outbounds，详见手册  

##### `v1.9.0.0`
支持通过SOCKS5协议更新订阅、升级core等  
支持解码用户名和密码是明文的socks://...链接  
优化yaml类型的hy2配置的摘要显示  
添加std.Web:GetSocksProxyPort()等多个函数  

##### `v1.8.9.0`
支持开启多个V2RayGCon图形界面功能（详见手册：多实例）  
支持导入socks://...分享链接  
开启tun模式后托盘图标变成便便色  
批量修改窗口添加更多可修改项  
选项窗口支持钩选是否导入vmess/vless链接  
选项窗口移除vmess解码模板功能  
服务器设置窗口添加忽略sendThrough注入选项  
NeoLuna添加std.Web:FetchSocks5()等多个函数  

##### `v1.8.8.0`
ProxySetter插件添加Tuna分页，支持tun模式，详见手册  
模板添加注入选项  
支持对只有socks协议的自定义core进行测速  
统一各日志窗口菜单项  
修复选项窗口无法保存默认模板设置bug  
修复服务器面板的按钮不会自动隐藏bug  
修复Text编辑器打开搜索窗口时退出会报错bug  
修复std.Sys:WriteAllText()只能追加文本bug  

##### `v1.8.7.0`
更新至WebUI v1.0  

##### `v1.8.6.0`
删除userSettings.json里面一些过时的配置项  
Text编辑器里面添加简单编辑器  
选项窗口Inbounds分页改名为模板分页  
自定义core添加 环境变量 和 测速模板 配置项  
提高主窗口响应速度  
减少并发测速时线程占用数  

##### `v1.8.5.0`
删除Json编辑器，简易编辑器，全局Import，多文件配置功能  
删除config.json里面的v2raygcon分节  
Text编辑器添加导航功能  
添加基本的YAML配置支持  
选项窗口添加Inbounds分页  
v2cfg://...链接升级为ver2格式  
修复主窗口滚动卡顿问题  
修复主窗口搜索 #序号 时不能切换页面的问题  
修复选项窗口插件分页不加载问题  

##### `v1.8.4.0`
不再支持v://...分享链接  
删除简单编辑器，添加文本编辑器  
移除Luna插件，改为单独发布  
选项窗口添加 自定义cores 分页（开发中）  
修复服务器序号设置为小数后不刷新的bug  
主窗改为只显示一列服务器  
调低服务器面板高度，改用更紧凑的显示方式  
用 #序号 搜索时，跳转到服务器所在分页  

##### `v1.8.3.0`
修复删除服务器有时失效的bug  
对较大的服务器配置进行压缩  
选项窗口可加载第三方外置插件  
Luna从内置插件移出，预计下个版本停止发布  

##### `v1.8.2.0`
停止发布scripts.zip脚本包  
删除std.Sys:SetWarnOnExit()  
主窗口可以输入 #序号 来定位服务器  
优化主窗口搜索速度  
NeoLuna插件添加日志窗口  
添加std.Misc:Notify()托盘气泡消息  
添加std.Web:ParseHtml()  
添加std.Sys:PipedProc***()几个函数  
用法见3rd/DyFetch项目源码  

##### `v1.8.1.0`
新增一个NeoLuna插件（测试中）  
处理vless://...中的&amp;amp;amp;以及\u0026等奇葩问题  
魔改了一下第三方自动升级包的代码  
修改userSettings.json的写盘方式，降低内存占用  
修复显示器缩放引起的扫描二维码红框偏移问题  
Lua插件添加wserv关键词，通过coreServ:Wrap()得到，是coreServ的简化版  
Lua插件添加Sys:SetWarnOnExit()，脚本非预期结束时弹个消息框  
Lua插件添加Sys:SnapCache***()函数，跨脚本、跨插件共享键值对用  
Luna添加thread.lua，可以把function序列化到新线程执行  
LuaMail添加一个attachment属性，用于发送object类型对象  
LuaMailBox添加capacity参数，用于SendAndWait()函数  
增加3rd文件夹，用来存放core, NeoLuna脚本, 升级辅助程序等等  
WebUI搬迁到3rd/neoluna/webui里面，需在NeoLuna插件中启动  

##### `v1.7.1.1`
更新至WebUI v0.0.4  
调整table.dump()默认递归深度成9  

##### `v1.7.1.0`
更新至WebUI v0.0.3  
scripts包添加查询服务器出入口地区的脚本  

##### `v1.7.0.0`
主窗口过滤功能支持tag1-3  
Luna print()输出多参数时用空格分隔  
Luna Sys库添加一堆Lua相关函数  
添加自定义user agent功能  
添加单击托盘图标时执行自定义命令功能  
修复lua.modules.writer中的一个bug  

##### `v1.6.9.0`
修复core下载器无法使用的问题  
支持Xray-core的reality（仅可用于vless和trojan协议）  
添加一个网页版客户瑞，详见 WebUI
修复lua.modules.reader.lua中的一个bug  
Luna插件添加string.split(), Web:Ping()等多个函数  
替换随机数生成器  

##### `v1.6.8.0` 
服务器面板显示Tag1-3（过滤对Tag无效）  
添加Server:DeleteByUids()函数  
添加coreState:SetName()函数  

##### `v1.6.7.4` 
更新Newtonsoft.Json至v13.0.2  
修复Luna插件自动补全的一个bug  
恢复记录每个服务器各自的日志  

##### `v1.6.7.0` 
添加lua/modules/task.lua模块，详见脚本内注释  
Core下载器仅显示v4.x内核  
更新多个NuGet包  

##### `v1.6.6.0`
提高导入、删除服务器速度  
调整ProxySetter界面  
删除coreConfiger:GetHash()  

##### `v1.6.5.0`
Luna插件添加httpServ模块  
修复v1.6.2起可能无法保存服务器信息的bug  

##### `v1.6.4.0`
优化一点点内存占用  

##### `v1.6.3.0`
移除Luna插件所有Json相关的函数  
优化Luna脚本内存占用  

##### `v1.6.2.1`
一丢丢优化  

##### `v1.6.2.0`
提高导入服务器速度  
支持trojan链接的remark项  
托盘图标服务器菜单改为显示标题  
配置文件部分使用压缩格式  

##### `v1.6.1.0`
修改import合并规则，合并in(out)bounds中tag相同的项  
Luna插件添加htmlparser.lua（可以写爬虫了）  
Scripts包添加“故障转移.lua”  

##### `v1.6.0.0`
主窗口分页菜单显示实际的序号范围  
修复服务器较多(4K+)时搜索卡顿及部分菜单无法弹出的问题  
Luna插件添加Server:Count()  
Luna插件的coreState添加tag1到3三个自定义标签  
Luna插件Server.balancerStrategyRandom首字母b改成大写  
Luna插件Server.balancerStrategyLeastPing首字母b改成大写  
Scripts包添加“查询服务器所在国家.lua”  

##### `v1.5.9.1`
支持使用web-safe base64编码的订阅  
支持导入、分享shadowsocks SIP002标准的链接  
分页菜单添加序号范围  
导入结果窗口添加进度条  
~~修复服务器较多(4k+)时搜索会卡顿的问题~~ 偶尔还会卡顿，下个版本将修复此问题  
修复当前页选择菜单会清除其他页面选中状态的问题  

##### `v1.5.8.0`
修改配置文件存盘方式（测试中，注意多备份）  
scripts包添加一个SSH端口转发示例脚本  
Pacman插件添加ProbeURL及ProbeInterval选项  
Luna插件添加coreCtrl:RunSpeedTestThen()函数  

##### `v1.5.7.0`
修复Win7下有些序号显示不全的问题  
`vless://...`分享链接支持gRPC部分传输模式  

##### `v1.5.6.0`
Pacman插件添加将多个服务器串成一条代理链的功能  

##### `v1.5.5.0`
修复v2ray-core v4.35.1起出现的测速失败问题  
Pacman插件添加leastPing均衡策略（需v2ray-core v4.38.0+)  
Luna插件添加32位dll文件  

##### `v1.5.4.0`
支持vmess分享链接中的`sni`及`gRPC`配置项  
`选项`窗口添加uTLS指纹伪装配置项  
添加`XRAY_BROWSER_DIALER`等几个环境变量  
`简易编辑器`添加`gRPC`传输类型  

##### `v1.5.3.0`
支持从XTLS/Xray-core更新core  
修复服务器面板按钮闪烁问题  
服务器设置界面自动切换分享链接类型  

##### `v1.5.2.0`
支持vless0分享链接提案  
“此服务器日志”在窗口打开后才开始记录日志  
Luna插件添加Misc:GetSpeedtestQueueLength()函数  
默认关闭ProxySetter插件（感谢Microsoft Defender） 

##### `v1.5.1.0`
导入socks协议时，默认关闭mux  
临时支持xray，将xray.exe解压到core目录内即可  
到xray不兼容v2ray-core或者GUI发布时将不再支持  

##### `v1.5.0.0`
修复一处内存泄漏  

##### `v1.4.9.0`
修复偶尔出现的100%~空手接白刃~CPU占用问题（观察中）  
Luna插件添加Web:ExtractBase64String(text, minLen)函数  
支持Trojan协议的flow配置项(v2fly PR #334)  

##### `v1.4.8.0`
调整简易编辑器，添加serverName配置项  
服务器大于1024个时改用动态菜单（上一版是2048个）  
修复订阅的"启用"选项对Luna脚本的Web:UpdateSubscriptions()无效的问题  
服务器包的摘要改为显示balancer的tag  
优化程序退出时的清理步骤  
Scripts包添加"托管（简单版）.lua"  

##### `v1.4.7.0`
服务器大于2048个时，托盘改用动态菜单以减少卡顿  
点击“修改时间”标签时弹出“简单编辑器”  
适配v2fly PR #181添加的trojan协议(需要v2ray-core v4.30.0+)  

##### `v1.4.6.0`
测速过程中遇到错误不再弹出提示窗口  
更新v2ray-core时不重启插件  
优化日志窗口刷新算法减少卡顿  
优化多屏体验  
Luna添加`Misc:SetSubscriptionConfig(cfgStr)`  

##### `v1.4.5.0`
修复v1.4.4出现的界面小概率冻结问题  

##### `v1.4.4.0`
修复v1.4.3出现的删除服务器后界面不刷新问题  
添加一个简单版服务器编辑器  
二维码功能合并进服务器设定窗口  
订阅支持直接给vmess://...链接的网页  
GitHub的release页面添加scripts.zip脚本包   

##### `v1.4.3.0`
调整服务器面板各元素布局  
减少搜索及换页时出现的闪烁现象  
导入分享链接时忽略vmess最后面的等号  
添加lua.modules.coreEvent详见手册-所有函数  

##### `v1.4.2.0`
移除Staticstics插件（请先备份数据）  
流量统计功能整合进主窗口，试验中还有点bug  
添加`解码模板`选项，样板参见[templates/custom/vmessDecodeTemplate.json](https://raw.githubusercontent.com/vrnobody/V2RayGCon/master/V2RayGCon/Resources/Files/templates/custom/vmessDecodeTemplate.json)  
添加检查v2ray-core更新选项  

##### `v1.4.1.0`
添加自定义inbounds选项  
添加可选v2fly更新源  
修复了一个磨人的小(死)妖(锁)精  
修复文件占用引起的v2ray-core更新失败问题  
主窗口搜索时保留当前的选中状态  
删除Luna中的`Web:FindAllHrefs()`函数  

##### `v1.4.0.0`  
Luna界面少量调整  
添加luasocket、luasec模块  
添加Sys:CreateHttpServer()、Sys:Volume*()  
Misc:Choice()支持数字键选取及双击确定  
托盘菜单可以停止脚本  
少量其他bug修复  

##### `v1.3.9.0`
界面微调  

##### `v1.3.8.0`
Luna插件少量更新  

##### `v1.3.7.2`  
Luna添加是否允许加载CLR库选项  
`Sys`库添加热键功能(详见手册Sys库最底)  
原`Json`库移进`Misc`并添加`modules.json`模块  
`Signal`添加`ScreenLocked()`查询屏幕是否处于锁定状态  
`Misc:ShowData()`改为返回`Json`字符串  

##### `v1.3.6.2`
此版本因存在界面冻结bug取消发布  

##### `v1.3.5.0`
快速切换功能的迟延要求改为可选  
添加允许自签证书选项  
Luna支持加载CLR库  
Luna添加ILuaSys接口及其他少量调整  

##### `v1.3.4.0`
托盘图标服务器菜单添加快速切换功能  
服务器面板添加备注标签  
程序退出后保留上次测速结果  
优化unicode支持  
用户界面多处微调  

##### `v1.3.3.0`
Luna插件添加Misc:ShowData等几个函数  
批量测速时改用随机顺序  
订阅界面少量调整  

##### `v1.3.2.0`
优化测速算法  
支持v2ray-core v4.23.1的多文件配置功能  
修复ProxySetter插件的菜单不更新问题  
升级失败时无需重启软件  

##### `v1.3.1.0`
修复导入链接解码失败时，其他链接丢失的问题  
服务器很多时，相关菜单自动切换为多层嵌套模式  
优化批量测速的CPU占用  
优化界面刷新逻辑  

##### `v1.3.0.0`
Luna插件的Input控件可设定初值  
修复一个导致菜单冻住的bug  
修复中文tag乱码问题  

##### `v1.2.9.3`
luna插件添加Input，Choices等几个输入控件  

##### `v1.2.9.0`
界面少量调整  
修改lua/libs/utils.lua  

##### `v1.2.8.7`
`v://...`分享链接支持http协议  

##### `v1.2.8.4`
重新设计服务器面板  

##### `v1.2.8.2`
添加停止测速功能  
修复主窗口的失焦问题  

##### `v1.2.8.0`
更新各Nuget包  
修复lua/templates文件夹为空的问题  
修复订阅选项页有时会崩溃的问题  
修复主窗口聚焦问题  
默认启用ProxySetter插件  

##### `v1.2.7.7`
vee链接支持v2ray/discussion [#513](https://github.com/v2ray/discussion/issues/513) 中提出的socks outbound（测试中）

##### `v1.2.7`  
少量更新  

