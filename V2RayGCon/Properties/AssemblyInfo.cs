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
[assembly: AssemblyVersion("1.2.7.15")]
[assembly: AssemblyFileVersion("1.0.0.0")]

/*
 * v1.2.7.15 fix form main focus problem
 * v1.2.7.14 fix lua and template folders are empty
 * v1.2.7.13 enable ProxySetter by default
 * v1.2.7.12 fix bug in subscription UI
 * v1.2.7.11 fix bug in form disposing
 * v1.2.7.10 update Nuget packages
 * v1.2.7.9 enable mux in vee.socks
 * v1.2.7.8 fix a bug in lua predefined function
 * v1.2.7.7 update user manual url
 * v1.2.7.6 socks vee share link supports user auth.
 * v1.2.7.5 support socks outbound in v://...
 *          (https://github.com/v2ray/discussion/issues/513)
 *          
 * v1.2.7.4 enable udp support by default for socks inbound
 * v1.2.7.3 add drag drop support on subs total label
 * v1.2.7.2 fix bug report not show up problem
 * v1.2.7.1 remove blocking code in disposing
 */
