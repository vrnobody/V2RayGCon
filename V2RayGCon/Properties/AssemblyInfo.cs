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
[assembly: AssemblyVersion("1.3.4.10")]
[assembly: AssemblyFileVersion("1.0.0.0")]

/*
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
