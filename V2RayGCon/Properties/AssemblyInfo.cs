using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("V2RayGCon")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("V2RayGCon")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: InternalsVisibleTo("V2RayGCon.Test")]

// 将 ComVisible 设置为 false 会使此程序集中的类型
//对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型
//请将此类型的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("7b799000-e68f-450f-84af-5ec9a5eff384")]

// 程序集的版本信息由下列四个值组成: 
//
//      主版本
//      次版本
//      生成号
//      修订号
//
// 可以指定所有值，也可以使用以下所示的 "*" 预置版本号和修订号
// 方法是按如下所示使用“*”: :
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.4.4.3")]
[assembly: AssemblyFileVersion("1.0.0.0")]

/*
 * v1.4.4.3 Kill core directly.
 * v1.4.4.2 Fix some potential deadlocks.
 * v1.4.4.1 Try to fix a deadlock.
 * -------------------------------------------------
 * v1.4.3.5 Fix serverUI deletion bug.
 * v1.4.3.4 Tweak UI.
 * v1.4.3.3 Add simple editor.
 * v1.4.3.2 Remove form QR code.
 * v1.4.3.1 Support plain vmess://... in subscription url.
 * --------------------------------------------------
 * v1.4.2.11 Refactor lua.modules.coreEvent.
 * v1.4.2.10 Refactor lua.modules.coreEvent.
 * v1.4.2.9 Anti flicker in searching.
 * v1.4.2.8 Refactoring.
 * v1.4.2.7 Anti flicker in page switching.
 * v1.4.2.6 Add lua.modules.coreEvent.
 * v1.4.2.5 Adjust ServerUI.
 * v1.4.2.4 Auto show/hide control buttons in servers panel.
 * v1.4.2.3 Tweak Misc:Choice() and Misc:Input().
 * v1.4.2.2 Fix a bug which was introduced by v1.4.2.1.
 * v1.4.2.1 Add encoding param in Sys:Run().
 *          v2ray-core win32 v4.26.0
 * -----------------------------------------------------
 * v1.4.1.12 Add option check for v2ray-core update when app start.
 * v1.4.1.11 Disable prompt of loading default config in configer.
 * v1.4.1.10 Reverse v1.4.1.9
 * v1.4.1.9 Add download speed in tooltip of total-down-label.
 * v1.4.1.8 Clear paging menu before adding new item.
 * v1.4.1.7 Adjust UI.
 * v1.4.1.6 Fix bugs.
 * v1.4.1.5 Remove statistics plug-in.
 * v1.4.1.4 Add stream settings support in vmess://... decode template.
 * v1.4.1.3 Fix a bug.
 * v1.4.1.2 Fix a bug in pager updating.
 * v1.4.1.1 Add vmess://... decode template support.
 * ------------------------------------------------
 * v1.4.0.11 18080 -> 8080
 * v1.4.0.10 Print server index in logs.
 * v1.4.0.9 Remove HtmlAgilityPack.
 * v1.4.0.8 Keep current selections in searching.
 * v1.4.0.7 Refactoring.
 * v1.4.0.6 Try to fix a dead lock.
 * v1.4.0.5 Create menu items on demand.
 * v1.4.0.4 Fix a bug in Misc:RandHex().
 * v1.4.0.3 Add v2fly as core update source.
 * v1.4.0.2 Custom inbounds support.
 * v1.4.0.1 Fix a bug in v2ray-core updating.
 * ----------------------------------------
 * v1.3.9.8 Fix a bug which would block core updating.
 * v1.3.9.7 Add luasocket and luasec.
 * v1.3.9.6 Add Sys:CreateHttpServer().
 * v1.3.9.5 Adjust LuaUI.
 * v1.3.9.4 Selection hotkeys support in Misc:Choice().
 *          Add feature, stop script from systray menu.
 *          Add Sys:Volume*().
 *          Fix index out of range again.
 * v1.3.9.3 Add edit button in LuaUI.
 * v1.3.9.2 Fix index out of range bug.
 *          Choice support double click.
 * v1.3.9.1 Fix bugs in analyzer.
 * -----------------------------------------------------
 * v1.3.8.5 Tweak serverUI labels' color.
 * v1.3.8.4 Fix a bug in modules.set.lua.
 * v1.3.8.3 Remove global CLR setting in Luna.
 * v1.3.8.2 Add Misc:Invoke(luaFunction) in Luna.
 * v1.3.8.1 Fix searching in form configer has no highlight bug.
 * ------------------------------------------------
 * v1.3.7.15 Refactoring.
 * v1.3.7.14 Rewrite lua-complete.
 * v1.3.7.13 Add table.dump() remove table.tostring().
 * v1.3.7.12 Add go to line feature.
 * v1.3.7.11 Modify searchbox.
 * v1.3.7.10 Modify core ready detection.
 * v1.3.7.9 Refactoring.
 * v1.3.7.8 Redesign Luna editor.
 * v1.3.7.7 Fix total of subs. not update bug.
 * v1.3.7.6 Auto-complete in Luna editor.
 * v1.3.7.5 Non-blocking input/output controls in Luna.
 * v1.3.7.4 Luna editor supports history.
 * v1.3.7.3 Add navigation bar in Luna editor.
 * ------------------------------------------------
 * v1.3.7.1 Fix bugs.
 * ------------------------------------------------
 * v1.3.6.16 Stop all servers if app close by user.
 * v1.3.6.15 Modify notify icon corner mark.
 * v1.3.6.14 Modify chained task in lazy guy.
 * v1.3.6.13 Stop cores after services disposed.
 * v1.3.6.12 Add Signal:ScreenLocked().
 * v1.3.6.11 Use native window, again.
 * v1.3.6.10 Run in background.
 * v1.3.6.9 Redesign initialization procedure.
 * v1.3.6.8 Refactor lazy guy.
 * v1.3.6.7 Bump version.
 * v1.3.6.6 Disable notifyicon debug logging.
 * v1.3.6.5 Try to fix a UI freezing problem.
 * v1.3.6.4 Fix FormOption I18N.zh-CN.
 * v1.3.6.3 Try to fix AutoUpdate bug.
 * v1.3.6.2 Add Sys:RunAndForgot().
 *          Log more detail in debug logs.
 * v1.3.6.1 Fix bugs.
 * -----------------------------------------------
 * v1.3.5.11 Misc:ShowData() return a json string.
 * v1.3.5.10 Add debug log file option.
 * v1.3.5.9 Fix bugs.
 * v1.3.5.8 Refactoring.
 * v1.3.5.7 Try to fix a UI freezing bug.(failed)
 * v1.3.5.6 Refactoring.
 * v1.3.5.5 Refactoring.
 * v1.3.5.4 Fix a bug.
 * v1.3.5.3 Add hotkey supports in Luna plug-in.
 * v1.3.5.2 Move ILuaJson into ILuaMisc.
 * v1.3.5.1 Optional CLR loading in Luna plug-in.
 * ----------------------------------------------
 * v1.3.4.14 Refactoring.
 * v1.3.4.13 Luna enable clr support.
 * v1.3.4.12 Update lua editor.
 * v1.3.4.11 Redesign lua controller UI.
 * v1.3.4.10 Add allow-insecure option.
 * v1.3.4.9 Add GetState() in ILuaMail.
 * v1.3.4.8 Update ILuaSys.
 * v1.3.4.7 (Luna) Switch to edit tab after load script form file.
 * v1.3.4.6 Refactoring.
 * v1.3.4.5 Hide output panel in Luna plug-in by default.
 * v1.3.4.4 Refactor mailbox of Luna plug-in.
 *          Fix "function" keyword indentation problem in lua editor.
 * v1.3.4.3 Add mailbox feature in Luna plug-in.
 * v1.3.4.2 Add clear speed test results.
 *          Add interface ILuaSys in Luna plug-in.
 * v1.3.4.1 Selfadapting quick switch menu.
 *          Optional latency limitation in quick switch menu.
 * -----------------------------------------------------------------
 * v1.3.3.13 Modify servers menu in notify icon.
 * v1.3.3.12 Add total in FormDataGrid of Luna plug-in.
 * v1.3.3.11 Fix a bug that notify icon menu does not update after sorting.
 * v1.3.3.10 Change menu group size from 18 to 12.
 *           Add reverse selected server by index menu item in form main. 
 * v1.3.3.9 Add quick switch menu to systray icon.
 * v1.3.3.8 Copy on click in subs UI.
 * v1.3.3.7 Fix a bug in lua/libs/utils.lua.
 * v1.3.3.6 Luna output box supports unicode.
 * v1.3.3.5 Refactoring.
 * v1.3.3.4 Add remark label on server panel.
 *          Preserve speed-test-results.
 *          Add Servers:StopAllServers() in Luna plug-in.
 *          Adjust UI.
 * v1.3.3.3 Pause notify icon updating when menu shows up.
 * v1.3.3.2 Try to fix port is taken up problem.
 * v1.3.3.1 Validate port range on vmess share link.
 * ------------------------------------------------------
 * v1.3.2.14 Data grid supports select by cell.
 * v1.3.2.13 Add new features in data gird dialog.
 * v1.3.2.12 Show unicode in rich text box.
 * v1.3.2.11 Change background color of data grid view.
 * v1.3.2.10 Add BrowseFolder(), BrowseFile(), ShowData() in Luna plug-in.
 * v1.3.2.9 Run batch speed testing in random order.
 * v1.3.2.8 Show result amid speed testing.
 * v1.3.2.7 Auto append new subs. item in form option.
 * v1.3.2.6 Add context menustrip to Input of Luna plug-in.
 * v1.3.2.5 Fix a bug in Web:Fetch() of Luna plug-in.
 * v1.3.2.4 Fix a bug.
 * v1.3.2.3 MultiConf supports relative path.
 * v1.3.2.2 Add GetOsVersion(), GetOsReleaseInfo() in Luna plug-in.
 * v1.3.2.1 Disable url detecting in all log form.
 * --------------------------------------------------------
 * v1.3.1.5 Fix a bug.
 * v1.3.1.4 Refine speed testing algorithm.
 * v1.3.1.3 Support multiple-config-files feature of v2ray-core v4.23.1.
 * v1.3.1.2 Fix notify icon menu of ProxySetter not update bug.
 *          Add env var V2RAY_LOCATION_CONFDIR.
 *          Upgrade to v2ray-core v4.23.1
 * v1.3.1.1 Retry with no restart after update failed.
 *          Max concurrent v2ray core setting take effect in next speed testing.
 * --------------------------------------------------------
 * v1.3.0.6 Adjust UI.
 *          Use semaphorse to throttle speed testing.
 * v1.3.0.5 Refactoring.
 * v1.3.0.4 Rollback to v1.3.0.2 due to form focus problem.
 * v1.3.0.3 Modify UI update logic of form main.
 *          Choice of Luna plug-in supports defult choice.
 * v1.3.0.2 Throttle UI update frequency.
 * v1.3.0.1 AutoGroupMenuItem supports multiple level grouping. 服务器太多了ヾ(≧▽≦*)o
 *          Fix a bug in form-main status-bar updating.
 *          Fix a bug in streamSettings decoding.
 * ----------------------------------------------------
 * v1.2.9.11 Remove code for debugging.
 * v1.2.9.10 Fix a dead-lock.
 * v1.2.9.9 Fix systray-icon-text update bug.
 * v1.2.9.8 Forgot to comment out debugging codes. XD
 * v1.2.9.7 Try to fix issue #4 SubscriptionUI bug.
 * v1.2.9.6 Add version information in bug report.
 * v1.2.9.5 Input of Luna plug-in can initialize with content.
 * v1.2.9.4 fix "锟斤拷"
 * v1.2.9.3 add click event handler in tags of server panel
 * v1.2.9.2 refactor
 * v1.2.9.1 add Alert, Choice ... in Luna plug-in
 */
