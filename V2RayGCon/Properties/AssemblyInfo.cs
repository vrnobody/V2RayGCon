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
[assembly: AssemblyVersion("2.2.4.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// xray-core v25.9.11 simplify vless outbound config
// postpone vless://... decode support
// config example https://github.com/XTLS/Xray-core/pull/5124

/*

--------------------------------------------------------------------
v2.2.3.16 Show hy2@host as summary for hysteria protoal.
v2.2.3.15 (W.I.P) Support hy2://... share link.
v2.2.3.14 Adjust UI.
v2.2.3.13 Improve tooltip of server selector user control in Composer plug-in.
v2.2.3.12 Add select-share-link-types window.
v2.2.3.11 Set buttons background of plug-in user controls to transparent.
v2.2.3.10 Add calculate TLS cert. hash feature in FormToolbox.
v2.2.3.9 Remove W.I.P mark of Composer plug-in.
v2.2.3.8 Supports tlsSettings.pinnedPeerCertSha256.
v2.2.3.7 AcmComboBox supports tab key.
v2.2.3.6 Commander plug-in supports drag&drop.
v2.2.3.5 Support auto-complete in FormServerSelector of Composer plug-in.
v2.2.3.4 Add Proxy-Chain feature to Composer plug-in.
         Add isProxyChain parameter to std.Server:ComposeServersToString().
v2.2.3.3 Add more template to Composer plug-in.
v2.2.3.2 Modify tooltips.
v2.2.3.1 Fix null reference bug in server boot list.
--------------------------------------------------------------------
v2.2.2.8 Add std.Server:ComposeServersToString().
v2.2.2.7 Add Composer plug-in.
v2.2.2.6 Remove sleep() in StopCore().
v2.2.2.5 Merge PAC setting tab into PAC tab in ProxySetter plug-in.
v2.2.2.4 Show friendly exception message in NeoLuna plug-in.
v2.2.2.3 Remove std.Sys:LusServSetIndex(name, index).
v2.2.2.2 Add tag1-3 to form-batch-modify.
v2.2.2.1 Fix FormMain title not showing tag bug.
--------------------------------------------------------------------
v2.2.1.6 Add decode-mob-sharelink menu item in FormToolbox.
v2.2.1.5 Add last boot timestamp to user settings. ;>
v2.2.1.4 Set current state to closing after detecting poweroff event.
v2.2.1.3 Add http protocol to mob://...
v2.2.1.2 Add wserv:GetMobShareLink() in NeoLuna plug-in.
v2.2.1.1 Add new share link format mob://...
--------------------------------------------------------------------
v2.2.0.4 Disable proxy port setting under Auto mode in ProxySetter plug-in.
v2.2.0.3 Fix GetAssemblyVersion() return null bug.
v2.2.0.2 Fix #31 auto-track feature not working bug.
v2.2.0.1 Support debug message logging while app init.
         Add version information to userSettings.json.
--------------------------------------------------------------------
v2.1.9.8 More aggressive string compression size limit.
v2.1.9.7 Replace GZip with ZSTD.
v2.1.9.6 Add back save-user-settings code in FormOption.
v2.1.9.5 Remove save-user-settings delay.
v2.1.9.4 Modify debugging log.
v2.1.9.3 Fix System.NullReferenceException in ServerUI.
v2.1.9.2 Call Server.SaveServersSettingsNow() before saving user settings to disk.
v2.1.9.1 Write userSettings.json to disk in sequential order.
         Add 10 seconds delay to write user settings to file at system shutdown.
--------------------------------------------------------------------
v2.1.8.4 Add move-to-custom-index menu item to ServerUI.
v2.1.8.3 Reduce the risk of data corruption during shutdown.
v2.1.8.2 Clear clipboard before watching.
v2.1.8.1 Disable STDIN and STDOUT redirect in window-mode or shell-support is enabled in Commander plug-in.
--------------------------------------------------------------------
v2.1.7.7 Support displaying simplified xray-core outbound config.
v2.1.7.6 Show hashing progress in FormToolbox.
v2.1.7.5 Add std.Misc:Sha1(str).
v2.1.7.4 Add calculate-hashes feature to FormToolbox.
v2.1.7.3 FormToolbox supports undo/redo.
v2.1.7.2 Add format json feature to Commander plug-in.
v2.1.7.1 FormMultiLineInput supports TAB key.
         Commander plug-in supports comment in args and env-vars.
--------------------------------------------------------------------
v2.1.6.6 Add vlessenc to FormToolbox.
v2.1.6.5 Support basic authentification in std.Web:Fetch().
v2.1.6.4 Auto scroll to bottom in clipboard watcher.
v2.1.6.3 Add clipboard watcher to FormToolbox.
v2.1.6.2 Add date to logs of Commander plug-in.
v2.1.6.1 Add F5 keyboard shortcut to Commander plug-in.
--------------------------------------------------------------------
v2.1.5.8 Add vless-encryption config item in FormSimpleConfiger.
v2.1.5.7 Add ML-KEM-768 key generating feature to FormToolbox.
v2.1.5.6 Refactor loading logic of plug-in setting.
v2.1.5.5 Update plug-in list while initializing plug-in option tab.
v2.1.5.4 Add commander plug-in.
v2.1.5.3 Reduce memory usage while serializing user settings to file.
v2.1.5.2 Add upper/lower case converter to FormToolbox.
v2.1.5.1 Add vmess link body decoder to FormToolbox.
--------------------------------------------------------------------
v2.1.4.8 Move config example feature to FormTextCinfigEditor.
v2.1.4.7 Refactor config example feature in FormToolbox.
v2.1.4.6 Fix crash bug in std.Misc:Choice().
v2.1.4.5 Rename FormKeyGen to FormToolbox.
         Add encode/decode tools to FormToolbox.
v2.1.4.4 Add gen-password feature to FormKeyGen.
v2.1.4.3 Add std.Misc:ShowFormToolbox().
v2.1.4.2 Add KeyGenForm.
v2.1.4.1 Fix crash when scanning QR code in win7.
--------------------------------------------------------------------
v2.1.3.6 Support tls.ech and reality.mldsa65.
v2.1.3.5 Add max length limit for config compressing.
v2.1.3.4 Change timeout to 20 seconds.
v2.1.3.3 Change check new core versions timeout to 10 seconds.
v2.1.3.2 Show server's title in confirm-deletion prompt.
v2.1.3.1 Confirm whether switch to direct download before updating subscription.
--------------------------------------------------------------------
v2.1.2.8 Add #note search-filter.
v2.1.2.7 Add #goto search-filter.
v2.1.2.6 #take filter support negative param.
v2.1.2.5 Add MoveToCustomIndex menu item.
v2.1.2.4 Add std.Server:MoveServers(uids, destTopIndex).
v2.1.2.3 Fix random order bug in MoveToTop() and MoveToBottom().
v2.1.2.2 Refactor sorting algorithm of servers.
v2.1.2.1 Decrease outbounds tag length in generated servers package config.
--------------------------------------------------------------------
v2.1.1.5 Auto-scale progress bar in FormImportLinksResult.
v2.1.1.4 Add subscription shortcut in WelcomeUI.
v2.1.1.3 Switch default PAC domain source to Loyalsoldier/v2ray-rules-dat.
v2.1.1.2 Refactor TicketPool.
v2.1.1.1 Enhance LinksTextExtractor().
--------------------------------------------------------------------
v2.1.0.10 Fix some menu items are not translated in FormMain.
v2.1.0.9 Add std.Web:UnicodeDecode(str).
v2.1.0.8 Save user settings to "%ProgramData%/V2RayGCon/" in non-portable mode.
v2.1.0.7 Refactor algorithm for extracting share links from text.
v2.1.0.6 Add "auth" property in generated server side socks config.
v2.1.0.5 Increase min len of base64 subs. string to 260 chars.
v2.1.0.4 Fix app crash when user close FormImportLinksResult while loading.
v2.1.0.3 Add std.Misc:GenServerSideConfig(meta).
         Add std.Misc:GetMetaData(config).
v2.1.0.2 Add generate server config feature in FormSimpleConfiger.
v2.1.0.1 Add restart button in ServerUI.
--------------------------------------------------------------------
v2.0.9.1 Scroll to the corresponding line when formatting json fails.
--------------------------------------------------------------------
v2.0.8.3 Show error message when formatting json fails.
v2.0.8.2 Add a duplicated "host" setting to websocket stream.
v2.0.8.1 Change auto-generated search keywords to #mark is "mark".
         NeoLuna focus to editor-control after opened.
         Neoluna initiate with navigating bar disabled.
--------------------------------------------------------------------
v2.0.7.2 Add std.Misc:GetNeoLunaLogAsString() in NeoLuna plug-in.
         Fix typo std.Sys:Lu[s -> a]ServSetIndex().
v2.0.7.1 Support xhttp and raw stream settings in vless://...
--------------------------------------------------------------------
v2.0.6.2 Fix CVE-2024-48510 in ZipExtractor.
v2.0.6.1 Remove DotNetZip NuGet package.
--------------------------------------------------------------------
v2.0.5.4 Add std.Servers:GetFilteredServers(keyword).
v2.0.5.3 Add WebView2 form for WebUI.
v2.0.5.2 Improve #orderby modify speed.
v2.0.5.1 Remove index optimization in AdvNumberFilter.
--------------------------------------------------------------------
v2.0.4.7 Remove priority from search-filters.
v2.0.4.6 Add Take filter in search.
v2.0.4.5 Pass down enter-keydown event in auto-complete menu of search.
v2.0.4.4 Disable auto-sort feature of OR operator in search.
v2.0.4.3 Add custom search keywords setting.
v2.0.4.2 Add a predefined filter keyword.
v2.0.4.1 Add std.Misc.GetMutexPoolCount() in NeoLuna plug-in.
--------------------------------------------------------------------
v2.0.3.5 Add AdvOrderBy filter.
v2.0.3.4 Optimize index matching in search.
v2.0.3.3 Impletment highlighter for boolean expression filter.
v2.0.3.2 Add priority levels in boolean expression.
v2.0.3.1 Add quotes to auto-generated mark list in search box.
--------------------------------------------------------------------
v2.0.2.4 Imporve UX of search keyword tooltip.
v2.0.2.3 Remove `#@` prefix from boolean expression search keyword.
v2.0.2.2 Add ManualResetEvent pool.
v2.0.2.1 Add VgcLibs.Libs.Tasks.Waiter().
         Repalce ManualResetEvent with waiter.
--------------------------------------------------------------------
v2.0.1.14 Add MATCH string operator.
v2.0.1.13 Add "NOT" operator and remove conflict operators.
v2.0.1.12 Add more menu itesm to current page selecting menu.
v2.0.1.11 Show more detail result after importing share links.
v2.0.1.10 Auto patch not even parentheses.
v2.0.1.9 GetAllServers() returns a read-only collection.
v2.0.1.8 Arrange ServerUIs into multi-columns when FormMain is maximized.
v2.0.1.7 Add index search tag.
v2.0.1.6 Evaluate boolean expression from left to right.
v2.0.1.5 Support boolean expression in search.
v2.0.1.4 Refactor tokenizer.
v2.0.1.3 Fix std.Misc:ShowData() in NeoLuna plug-in.
v2.0.1.2 Disable mouse enter event in FormMain.
         Disable tips if search keyword is #+nuber.
v2.0.1.1 Try to reduce CPU usage.
--------------------------------------------------------------------
v2.0.0.8 Left click to select search tooltips.
v2.0.0.7 Use AutocompleteMenu to show tooltips.
v2.0.0.6 Support quoted spaces in advance search parameters.
v2.0.0.5 Add starts and ends string-operators in search.
         Disable Tab / KeyUp / KeyDown in search box.
v2.0.0.4 Support 4 digits year.
v2.0.0.3 Patch date string before search.
v2.0.0.2 Handle search-tag-modify as number.
v2.0.0.1 Add core/selected/modify/port search tag names.
         Fix batch modify not respect index order bug.
         Fix search keyword case-sensitive bug.
--------------------------------------------------------------------
v1.9.9.12 Refactor search featrue.
v1.9.9.11 Fix save option cause UI freezing bug.
v1.9.9.10 Support for searching numbers.
v1.9.9.9 Zoom search box on focus.
v1.9.9.8 Refactor search feature.
v1.9.9.7 Fix NullReferenceException in GetIndex() of ServerUI.
v1.9.9.6 Reduce the time it takes to save user settings to files.
v1.9.9.5 Modify search rules.
v1.9.9.4 Improve search feature and highlight feature.
v1.9.9.3 Fix scroll server into view not working bug.
v1.9.9.2 Fix decode tlsType bug.
v1.9.9.1 Refactor AutoEllipsis().
--------------------------------------------------------------------
v1.9.8.8 Try another approach to optimize searching.
v1.9.8.7 Refactoring.
v1.9.8.6 More aggressive search optimization.
v1.9.8.5 Support numpad keys in FormChoice(s) of NeoLuna plug-in.
v1.9.8.4 Improve selecting servers performance.
v1.9.8.3 Show totals in form import result.
v1.9.8.2 Improve search experience in form main.
v1.9.8.1 Fix MessageBox do not show up in boot-stage bug.
--------------------------------------------------------------------
v1.9.7.5 Fix startup core version check bug.
v1.9.7.4 Add vrnobody/xraye to core download-source.
v1.9.7.3 Supports downloading xray-core win7 binary.
         Fix core updater can not be cancelled bug.
v1.9.7.2 Try to fix IndexOutOfRangeException in LuaCoreCtrl.
v1.9.7.1 Support remarks param in subscription url.
--------------------------------------------------------------------
v1.9.6.5 Verify zip file before extracting new downloaded core.
v1.9.6.4 Support SplitHTTP trasport.
v1.9.6.3 Cleanup obsolete codes.
v1.9.6.2 Fix vMESS sharelink decoder.
v1.9.6.1 Fix vmess://... codec bug.
--------------------------------------------------------------------
v1.9.5.11 Server settings panel supports tags editing.
v1.9.5.10 Remove connectivity conf-item in leastLoad servers package.
v1.9.5.9 Set latency to zero if speed test is cancelled.
         Prompt to clear record when clicking the latency label or statistic label.
v1.9.5.8 Show info of first outbound in summary of chained outbounds.
v1.9.5.7 Add custom template type 'ModifyOutbound'.
v1.9.5.6 Refactoring.
v1.9.5.5 Remove port in sni. Issue #25.
v1.9.5.4 Improve Enter key and Tab key UX of std.Misc:Input().
v1.9.5.3 Add std.Misc:EncodeToShareLinkMetaData().
         Add table.sortedkeys(), table.sortedkeysdesc().
v1.9.5.2 Supports comments in config.
v1.9.5.1 Disable buttons after clicked.
--------------------------------------------------------------------
v1.9.4.3 Supports leastLoad balancer strategy in Pacman plug-in.
v1.9.4.2 Supports HTTPUpgrade and gRPC.authority in vless://... share link.
v1.9.4.1 Fix modifications of templates not taking effect bug.
--------------------------------------------------------------------
v1.9.3.7 Disable RecycleBin().
v1.9.3.6 Add JsonTextWriterWithPadding().
v1.9.3.5 Place template outbounds at the buttom.
v1.9.3.4 Reduce memory usage of JSON merger.
v1.9.3.3 Remove config v3 format supports.
v1.9.3.2 Fix a memory leak.
v1.9.3.1 Cache final config when stat is off.
         Save inbounds-info-cache to disk.
         Set RecycleBin min config size to 4 KiB.
--------------------------------------------------------------------
v1.9.2.19 Refresh summary after inject-template-settings changed.
v1.9.2.18 Add timeout to StringLruCache().
v1.9.2.17 Adjust save user settings interval from 5 minutes to 20 minutes.
v1.9.2.16 Move global logger to VgcApis.
v1.9.2.15 Add std.Sys:GetPluginSettingKeys().
v1.9.2.14 Add Misc.Caches.ZipStrLru().
v1.9.2.13 Add finalConfigCache.
          Add JsonRecycleBin().
          Using file stream to save user settings.
v1.9.2.12 Merge Ascii/UnicodeStringStream into ReadonlyStringStream.
v1.9.2.11 Add LRU cache.
v1.9.2.10 Add ArrayPoolMemoryStream, AsciiStringStream, UnicodeStringStream.
v1.9.2.9 Re-enable config compression.
v1.9.2.8 Refactor QueueLogger().
         Refactor Routine().
v1.9.2.7 Remove decode cache.
         Refactor serialize user settings.
v1.9.2.6 Refactor Bar().
v1.9.2.5 Using stream in TimedDownloadTest().
v1.9.2.4 Refactor Services.Settings.
v1.9.2.3 Add coreConfiger:ClearInboundsInfoCache().
v1.9.2.2 Refactor coreConfiger:GatherInfoForNotifyIcon().
v1.9.2.1 Disable config compression.
--------------------------------------------------------------------
v1.9.1.4 Change server tag back to "agentout+index".
v1.9.1.3 Fix stats port.
v1.9.1.2 Format server tag as "node-index" in servers package.
         Text editor supports for formatting JArray.
         Supports roundRobin balancing strategy.
v1.9.1.1 Add std.Misc:SetNotifyIconTag().
--------------------------------------------------------------------
v1.9.0.5 Add std.Misc:ToJson().
v1.9.0.4 Add std.Sys:RunAndGetResult().
v1.9.0.3 Add std.Server:PackServersToString().
         Add coreConfiger:SetConfigQuiet().
v1.9.0.2 Dispose EventWaitHandle.
v1.9.0.1 Dispose CancellationTokenSource after cancelled.
--------------------------------------------------------------------
v1.8.9.7 GUI fully supports socks5 proxy.
         Remove Services.Cache.Html().
v1.8.9.6 Add std.Web:GetAllActiveProxiesInfo().
         Add std.Web:GetSocksProxyPort().
         Add std.Web:GetHttpProxyPort().
v1.8.9.5 Add coreCtrl:IsSpeedTesting().
v1.8.9.4 Refactor std.Web:Fetch().
v1.8.9.3 Enhance socks://... decoder.
v1.8.9.2 Add std.Web.UriEncode/Decode().
v1.8.9.1 Simplify write settings to file algo.
--------------------------------------------------------------------
v1.8.8.10 Add std.Web:ExtractShareLinks(text, prefix).
          Add std.Misc:DecodeShareLinkToMetadata(shareLink);
          Add std.Misc:EncodeMetadataToShareLink(meta);
v1.8.8.9 std.Web:FetchSocks5() and std.Web:TimedDownloadTestingScoks5() support authentication.
v1.8.8.8 Remove wintun.dll from V2RayGCon.zip
         Add more options in batch modifier.
         Supports sock outbound in simple editor.
         Remove custom vmess decode template feature.
         Remove 3rd/templates.
v1.8.8.7 Supports disable vmess/vless sharelink decoders.
         Add std.Web:FetchSocks5().
         Add std.Server:AddNew().
         Add std.Web:TimedDownloadTestingSocks5().
v1.8.8.6 Supports socks://... sharelink.
v1.8.8.5 Add ignore sendThrough option.
v1.8.8.4 Fix file conflict happens while multiple processes are writing the same config file.
v1.8.8.3 Support multiple instances of V2RayGCon.
v1.8.8.2 Remove VgcApis.Misc.Utils.RunInBgSlim().
v1.8.8.1 Change systray icon color while using tun mode.
--------------------------------------------------------------------
v1.8.7.13 Custom config template add inject-option.
v1.8.7.12 Fix GitHub action.
v1.8.7.11 Tuna supports IPv6.
v1.8.7.10 Refactor wait for speedtesting algo.
v1.8.7.9 Show all inbounds info.
v1.8.7.8 Refactor speedtest pool.
v1.8.7.7 Fix default inbound template name always reset to http bug.
v1.8.7.6 Fix ServerUI buttons not auto hided bug.
v1.8.7.5 Add Tuna in ProxySetter plug-in.
v1.8.7.4 Fix FormSearch not close before FormTextConfigEditor exit.
v1.8.7.3 Remove resume menu item from FormLog of NeoLuna plug-in.
v1.8.7.2 Speedtest template supports SOCKS5 inbound.
v1.8.7.1 Fix std.Sys:WriteAllText().
--------------------------------------------------------------------
v1.8.6.8 wserv:GetFinalConfig() return a string.
v1.8.6.7 Add timeout in stream.
v1.8.6.6 Refactoring.
v1.8.6.5 Expose DetectConfigType() to NeoLuna.
v1.8.6.4 Fix {...} in NeoLuna plug-in.
v1.8.6.3 Replace HttpClient with WebCliet in latency test.
v1.8.6.2 Refactoring.
v1.8.6.1 Fix UI text.
--------------------------------------------------------------------
v1.8.5.8 Replace custom inbound settings with custom config template settings.
v1.8.5.7 Remove obsolete user settings.
v1.8.5.6 Add inbound option in custom core settings.
v1.8.5.5 Add back simple configer.
v1.8.5.4 Refactor ShareLinkMgr.
v1.8.5.3 Postpone round label updating in ServerUI.
v1.8.5.2 Reduce threads creation in speedtesting.
v1.8.5.1 Refactoring.
--------------------------------------------------------------------
v1.8.4.15 Remove im(ex)port from(to) file in FormMain.
          Fix internal plugins not loaded bug.
v1.8.4.14 Fix format code bug in text editor.
v1.8.4.13 Refactoring.
v1.8.4.12 Fix default core name bug.
v1.8.4.11 Move WaitUntilCoreReady() to Libs.V2Ray.Core.cs.
v1.8.4.10 Fix extract keys from config bug.
v1.8.4.9 Add basic YAML support.
v1.8.4.8 Disable YAML config format.
v1.8.4.7 Add lock mechanism for writing config file to disk.
v1.8.4.6 Minor UI update.
v1.8.4.5 (WIP) custom cores: add custom inbounds setting tab FormOption.
v1.8.4.4 Fix FormMain page switchers not working if filter is set to #index.
         Add Ctrl+F hotkey for FormMain.
v1.8.4.3 Fix FormMain not scroll smoothly bug.
v1.8.4.2 (WIP) custom cores:
         Remove json editor.
         Remove global import.
         Remove multi config file.
         Upgrade v2cfg://... to ver 2.
         Add navigation box in text editor.
         Remove "v2raygcon" section in config.json.

v1.8.4.1 Fix UI controls misplaced.
         BatchModify supports change custom core setting.
--------------------------------------------------------------------
v1.8.3.16 (WIP) custom cores.
v1.8.3.15 Add Luna plug-in into GitHub release assets.
v1.8.3.14 (WIP) Remove v://... decoder.
                Set custom core name while importing sharelink.
v1.8.3.13 (WIP) Add custom core settings.
v1.8.3.12 Modify title of FormJsonConfigEditor.
v1.8.3.11 Add text editor.
v1.8.3.10 Do not trim config.
v1.8.3.9 Locate server if search server by #index.
v1.8.3.8 Reduce ServerUI height.
v1.8.3.7 Fix GitHub action.
v1.8.3.6 Update GitHub actions.
v1.8.3.5 Fix server index bugs.
v1.8.3.4 Refactor QueueLogger().
v1.8.3.3 Add RunInBgSlim().
v1.8.3.2 Refactor LazyGuy().
v1.8.3.1 Remove Luna plug-in.
--------------------------------------------------------------------
v1.8.2.9 Redesign external plug-ins loading logic.
v1.8.2.8 Add wserv:GetRawConfig(), std.Misc:CompressToBase64(), std.Misc:DecompressFromBase64().
v1.8.2.7 Fix std. not prepend to api properties.
v1.8.2.6 Fix delete server not working bug.
v1.8.2.5 Format code.
v1.8.2.4 Add comments in IPlugin.
v1.8.2.3 Modify Tasks.LazyGuy.
v1.8.2.2 Add isLoad3rdPartyPlugins option.
v1.8.2.1 Move Luna plug-in to 3rd/plugins.
--------------------------------------------------------------------
v1.8.1.16 Add std.Misc:Notify().
v1.8.1.15 Replace std.Sys:PipedProcCreate() with std.Sys:PipedProcRun().
v1.8.1.14 Replace std.Sys:RunWithPipe() with std.Sys:PipedProc***().
v1.8.1.13 Fix misused anonymous pipe.
v1.8.1.12 Add std.Sys:RunWithPipe().
v1.8.1.11 Fix call GetFilteredList() twice in one search.
v1.8.1.10 Improve search algorithm.
v1.8.1.9 Adjust FormOption.
v1.8.1.8 Remove scripts.zip.
v1.8.1.7 Add std.Misc:ShowFormNeoLunaLog().
v1.8.1.6 Filter of FormMain supports #index.
v1.8.1.5 Remove std.Sys:SetWarnOnExit().
v1.8.1.4 Add copy-log-menu-item in FormLog of NeoLuna plug-in.
v1.8.1.3 Add FormLog in NeoLuna plug-in.
v1.8.1.2 Add sortedCoreCtrlServListCache in Services.Servers.
v1.8.1.1 Add Html-Agility-Pack back, again.
--------------------------------------------------------------------
v1.8.1.0  Fix current dir not set problem.
--------------------------------------------------------------------
v1.7.1.32 Add header and attachment properties in LuaMailBox.
          Filter non-pritable chars before writing logs.
          Fix table.unpack() returns nil bug.
v1.7.1.31 Fix UTC ticks bug in utils.lua of NeoLuna plug-in.
v1.7.1.30 Move snap cache to post office.
v1.7.1.29 Add capacity to MailBox.
v1.7.1.28 Intorduce NeoLua.
v1.7.1.27 Move core folder into 3rd.
v1.7.1.26 Remove Server:GetAllWrappedServers().
v1.7.1.25 Add Sys:SetWarnOnExit().
          Add SnapCache().
          Fix null ref bug.
v1.7.1.24 Add thread.lua.
v1.7.1.23 Move ICoreServCtrlWrapper from Luna plug-in to V2RayGCon project and rename to IWrappedCoreServCtrl.
          Add "wserv" snippet keyword.
v1.7.1.22 Refactoring.
v1.7.1.21 Disable CoreServCtrlWrapper.
v1.7.1.20 Refactoring.
v1.7.1.19 Add Servers:DeleteServerByUid().
v1.7.1.18 Fix Wrap(null) bug.
v1.7.1.17 Intorduce cserv:***(), which is a simpler but slower version of coreServ:***().
v1.7.1.16 Refactor QRCode and import-result-form.
v1.7.1.15 Filter control chars in server's name.
v1.7.1.14 Replace List<T> coreServList with Dict<T> coreServCache in Servers service.
v1.7.1.13 Refactoring.
v1.7.1.12 Modify write file logic of user settings file.
v1.7.1.11 Move local storage out of plug-in settings.
v1.7.1.10 Add HTML cache size limit.
v1.7.1.9 Refactor QueueLogger.
v1.7.1.8 Surround IPv6 with square brackets.
v1.7.1.7 Restart app with user privileges after app updated.
v1.7.1.6 WIP: Remove AutoUpdater.NET
v1.7.1.5 Unescape unicode in vless://...
v1.7.1.4 Add connections limit in HTTP server of Luna plug-in.
v1.7.1.3 Fix &amp;amp;...
v1.7.1.2 Add coreCtrl:RestartCoreIgnoreError().
-------------------------------------------------------------
v1.7.1.1 Fix a bug in table.dump().
-------------------------------------------------------------
v1.7.0.6 Add Services.Server.SortServersBy*(uids).
v1.7.0.5 Add Sha256Hash() and Sha512Hash().
v1.7.0.4 Update lua.libs.utils.lua.
         Fix typo CORS.
v1.7.0.3 Add Misc:CopyToClipboard() in Luna plug-in.
v1.7.0.2 Add Sys:LuaVmRun(string luavm, string name, string script, bool isLoadClr).
v1.7.0.1 Parse NON-STANDARD IPv6 host in vless://...
         Add Server:IsRunningSpeedTest() in Luna plug-in.
         Cache Server:CountSelected().
-------------------------------------------------------------
v1.6.9.4 Fix custom user agent not working bug.
         Modify Misc:GetUserSettings().
         Add Sys:LuaServAbort() in Luna plug-in.
         Add Sys:Ls() Sys:LuaServRestart().
         Add Sys:LuaVmRemoveStopped(), Sys:LuaVmGetScript(), Sys:LuaVmGetAllVmsInfo().
         Fix a bug in writer.lua.
v1.6.9.3 Add left click settings for systray icon.
         Add user agent setting.
v1.6.9.2 Add Sys:Lua*() functions in Luna plug-in.
         Set index for new added Luna script.
         Add Serverices.AstServer in Luna plug-in.
         Add Sys:LuaGenModuleSnippets() in Luna plug-in.
v1.6.9.1 Add space for print() in Luna plug-in.
         Add Server:Add(config, mark) in Luna plug-in.
         Filter in FormMain supports tag1 to tag3.
         Fix a bug in utils.lua.
         Format config before adding new server.
-------------------------------------------------------------
v1.6.8.7 Add PseudoRandom.cs.
v1.6.8.6 Luna add string.split().
         Fix lua.modules.reader.lua.
         Update timestamp every time in coreState:SetSpeedTestResult().
         Luna add Sys:CreateHttpServer(url, inbox, outbox, source, enableCORS);
         Add Web:Ping(), Web:GetFreeTcpPort(), Sys:Start(), Sys:GetAppVersion(), Web:FetchWithCustomConfig(), coreCtrl:Fetch().
         Add a web UI.
v1.6.8.5 Fix xray-core stat.
         Add shortId or spiderX ondemand.
v1.6.8.4 Refactoring.
v1.6.8.3 Fix share links decoding bugs.
v1.6.8.2 Fix core downloader not working bug.
         vless://... supports reality.
         Modiyf simple editor.
v1.6.8.1 Add Server:[Pack/Chain]ServersWithUids() in Luna plug-in.
-------------------------------------------------------------
v1.6.7.10 Show custom tags in ServerUI.
v1.6.7.9 Add Server:Delete() in Luna plug-in.
v1.6.7.8 Try to fix index not update bug.
v1.6.7.7 Choice(s) accept <Enter> key event.
v1.6.7.6 Add coreState:SetDescripton() in Luna plugin.
v1.6.7.5 Add coreState:SetName() in Luna plugin.
v1.6.7.4 Update to Newtonsoft.Json v13.0.2
v1.6.7.3 Try to fix InvalidOperationException in BestMatchSnippets of Luna-plugin.
v1.6.7.2 Refactoring.
v1.6.7.1 Revert 09755e5137934e8511a04d26b6bafd8395130e96.
-------------------------------------------------------------
v1.6.6.3 Keep v4 cores only in core downloader.
v1.6.6.2 Upgrade NuGet packages.
v1.6.6.1 Add Sys:SetTimeout() and task.lua in Luna plug-in.
-------------------------------------------------------------
v1.6.5.6 Remove coreConfiger:GetHash().
v1.6.5.5 Refresh the totals after subscriptions are updated in form option.
v1.6.5.4 Reduce the time it takes to remove server.
v1.6.5.3 Refactor duplicate checking method.
v1.6.5.2 Try to fix dispose exception in form option.
v1.6.5.1 Disable unrelated controls when custom PAC is checked in ProxySetter plug-in.
-------------------------------------------------------------
v1.6.4.3 Fix serde unicode string throw exception bug.
v1.6.4.2 Update utils.lua in Luna plug-in.
v1.6.4.1 Add lua module httpServ.
-------------------------------------------------------------
v1.6.3.8 Dispose ACMs.
v1.6.3.7 Dispose removed controls in FlyServer.
v1.6.3.6 Release handle after winform is closed.
v1.6.3.5 Set AuxSiWinForm to null after form closed.
v1.6.3.4 Add RestartOneServerByUid() in servers service.
v1.6.3.3 Dispose lua core after script finished.
v1.6.3.2 Use stream in clumsy writer.
v1.6.3.1 Adjust save user settings interval.
-------------------------------------------------------------
v1.6.2.8 Refactoring.
v1.6.2.7 Compress plug-ins setting.
v1.6.2.6 Refactoring.
v1.6.2.5 Add SaveUserSettingsLater() in Luna plug-in.
         Change servers setting interval to 60 seconds.
         Refactoring.
v1.6.2.4 Call PerformLayout() after dropdown-menu-item cleared.
v1.6.2.3 Fix a bug.
v1.6.2.2 Remove json related functions in Luna plug-in.
v1.6.2.1 Use stream in compression.
-------------------------------------------------------------
v1.6.1.8 Synchronize logging in Libs.V2Ray.Core.
v1.6.1.7 Compress servers config.
v1.6.1.6 Replace user settings cache with md5 hash.
v1.6.1.5 Remove stat history.
v1.6.1.4 Show server's title in notify-icon-menu.
v1.6.1.3 Supports name field in Trojan URI.
v1.6.1.2 Refactoring.
v1.6.1.1 Improve adding new servers performance.
-------------------------------------------------------------
v1.6.0.3 Merge elements with same tag in in(out)bound(Detour) of global import.
v1.6.0.2 Add failover.lua to scripts package.
v1.6.0.1 Prepare for TLS13.
         Add htmlparser.lua to Luna plug-in.
         Update reader.lua and writer.lua in Luna plug-in.
-------------------------------------------------------------
v1.5.9.10 Add 3 invisible tags for coreState in Luna plug-in.
          Server:get_balancerStrategyRandom() => Server:get_BalancerStrategyRandom().
          Server:get_balancerStrategyLeastPing() => Server:get_BalancerStrategyLeastPing().
v1.5.9.9 Refactoring.
v1.5.9.8 Switch to dynamic paging menus when the number of pages is greater than 5000.
         Add servers:Count() in Luna plug-in.
v1.5.9.7 Replace lock with ReaderWriterLockSlim in servers service.
v1.5.9.6 Update uid and title after summary is updated.
v1.5.9.5 Refactoring.
v1.5.9.4 Refactoring.
v1.5.9.3 Show real index range in paging memu.
v1.5.9.2 Refactoring.
v1.5.9.1 Fix divided by zero exception.
------------------------------------------------------------
v1.5.8.20 Increase servers menu index length to 5 digits.
v1.5.8.19 Refactoring.
v1.5.8.18 Show progress bar while loading import results.
v1.5.8.17 Refactoring.
v1.5.8.16 Update summary while adding new server.
v1.5.8.15 Reset index ondemand.
v1.5.8.14 Fix a bug in all pages selector.
v1.5.8.13 Fix a bug in current page selectors.
v1.5.8.12 Try to improve search performance again.
v1.5.8.11 Revert v1.5.8.9
v1.5.8.10 Supports shadowsocks SIP002 url sharing.
v1.5.8.9 Refactor Servers.UpdateAllServersSummary().
v1.5.8.8 Supports shadowsocks SIP002.
v1.5.8.7 Show index range in paging menu.
v1.5.8.6 Try to improve search performance.
v1.5.8.5 Supports web safe base64.
v1.5.8.4 Refactoring.
v1.5.8.3 More docs.
v1.5.8.2 More docs.
v1.5.8.1 Add docs.
----------------------------------------------------------
v1.5.7.10 Try writethrough mode.
v1.5.7.9 Reverse to v1.5.7.6
v1.5.7.8 Retry when write user settings to file failed.
v1.5.7.7 Try to fix file cache bug.(but failed)
v1.5.7.6 Refactoring.
v1.5.7.5 Add coreCtrl:RunSpeedTestThen() in Luna plug-in.
v1.5.7.4 Tweak Pacman plug-in.
v1.5.7.3 Add script SSH-port-forwarding.lua.
v1.5.7.2 Refactoring.
v1.5.7.1 Add probeURL and ProbeInterval in Pacman plug-in.
----------------------------------------------------------
v1.5.6.2 Fix win7 emoji bug.
v1.5.6.1 gRPC partial supports in vless://...
----------------------------------------------------------
v1.5.5.5 Show proxy chain length in summary.
v1.5.5.4 Fix: form option prompt options were not saved.
v1.5.5.3 Refactoring.
v1.5.5.2 Adjust UI.
v1.5.5.1 Add server chaining feature for Pacman plug-in.
---------------------------------------------------------
v1.5.4.5 Add 32bit dll for Luna plug-in.
v1.5.4.4 Adjust UI.
v1.5.4.3 Fix empty package bug in pacman plug-in.
v1.5.4.2 Supports least ping balancer strategy. (v2ray-core v4.38.0+)
         Fix pacman plug-in crashing bug.
         Fix speedtesting bug. (introduced by v2ray-core v4.35.1+)
v1.5.4.1 Modify WelcomeUI.
--------------------------------------------------------
v1.5.3.7 Add some enviroment variables.
v1.5.3.6 Modify UI.
v1.5.3.5 Add enable-uTLS checkbox.
v1.5.3.4 Adjust UI.
v1.5.3.3 Add uTLS option.
v1.5.3.2 Supports sni and gRPC in vmess://...
v1.5.3.1 Add stream type grpc in simple editor.
--------------------------------------------------------
v1.5.2.4 Supports updating core from XTLS/Xray-core.
v1.5.2.3 Add some json snippets.
v1.5.2.2 Try to fix buttons flashing bug in form main.
v1.5.2.1 Auto select link type in form server setting.
------------------------------------------------------
v1.5.1.6 Add vless://... encoder.
v1.5.1.5 Add vless://... decoder.
v1.5.1.4 Disable ProxySetter plug-in by default.
v1.5.1.3 Add Misc:GetSpeedtestQueueLength() in Luna.
v1.5.1.2 Refactoring.
v1.5.1.1 Disable individual logger.
------------------------------------------------------
v1.5.0.4 Supports xray, PARTIALLY!
v1.5.0.3 Refactoring.
v1.5.0.2 Optimize memory usage.
v1.5.0.1 Disable mux for socks protocol.
------------------------------------------------------
v1.4.9.2 Try to fix some memory leaks.
v1.4.9.1 Add Sys:GarbgeCollect().
------------------------------------------------------
v1.4.8.4 Add Web:ExtractBase64String(text, minLen) in Luna plug-in.
v1.4.8.3 Changes ServerUI refresh logic.
v1.4.8.2 Try to fix UI freezing bug.
v1.4.8.1 Add option flow in Trojan outbound. (v2fly PR #334)
------------------------------------------------------
v1.4.7.12 Fix Web:UpdateSubscriptions(port) ignore isUse setting problem.
v1.4.7.11 Refactor vee encoder/decoder.
v1.4.7.10 Add streamSettings.serverName in v://... share link.
v1.4.7.9 Fix a bug in form option.
v1.4.7.8 Refactor form simple editor.
v1.4.7.7 Refactor form configer.
v1.4.7.6 Use red color to highlight invalid UUID in simple editor.
v1.4.7.5 Fix UI bugs. (form logger, page menu)
v1.4.7.4 Show balancer tag in summary for servers package.
v1.4.7.3 Fix a bug.
v1.4.7.2 Add Sys:EmptyRecycle().
v1.4.7.1 Improve shutdown logic.
------------------------------------------------------
v1.4.6.6 (testing) Supports trojan-url.
v1.4.6.5 Show simple editor when user click modify-time-label.
v1.4.6.4 Refactoring.
v1.4.6.3 Refactoring.
v1.4.6.2 Supports trojan protocol from v2fly pull request #181.
v1.4.6.1 Notify icon switch to dynamic context menu when server count is greater than 2K.
-----------------------------------------------------
v1.4.5.7 Do not restart plug-ins while v2ray-core updating.
v1.4.5.6 Refactor Pacman plug-in.
v1.4.5.5 Refactor OnCoreExit().
v1.4.5.4 Refactor log forms.
v1.4.5.3 Fix core counter may less then zero bug.
v1.4.5.2 Add Misc:SetSubscriptionConfig(cfgStr) in Luna plug-in.
v1.4.5.1 Ignore all errors in speed testing.
-----------------------------------------------------
v1.4.4.15 Show title in single server log form.
v1.4.4.14 Add sort by stat. total.
v1.4.4.13 Fix Luna script manager flickering.
v1.4.4.12 Add feature batch reset server's stat. record.
v1.4.4.11 Refactor RestartCore() and StopCore().
v1.4.4.10 Lua editor supports file drag drop.
v1.4.4.9 Fix a dead lock in RestartCore().
v1.4.4.8 Show popup submenu in the same screen.
v1.4.4.7 Refactor sharelink codes.
v1.4.4.6 Update sharelink codecs.
v1.4.4.5 Try to fix misleading error message 'v2ray-core fail to start'.
v1.4.4.4 Support changing index in FormModifyServerSettings.
v1.4.4.3 Kill core directly.
v1.4.4.2 Fix some potential deadlocks.
v1.4.4.1 Try to fix a deadlock.
-------------------------------------------------
v1.4.3.5 Fix serverUI deletion bug.
v1.4.3.4 Tweak UI.
v1.4.3.3 Add simple editor.
v1.4.3.2 Remove form QR code.
v1.4.3.1 Support plain vmess://... in subscription url.
--------------------------------------------------
v1.4.2.11 Refactor lua.modules.coreEvent.
v1.4.2.10 Refactor lua.modules.coreEvent.
v1.4.2.9 Anti flicker in searching.
v1.4.2.8 Refactoring.
v1.4.2.7 Anti flicker in page switching.
v1.4.2.6 Add lua.modules.coreEvent.
v1.4.2.5 Adjust ServerUI.
v1.4.2.4 Auto show/hide control buttons in servers panel.
v1.4.2.3 Tweak Misc:Choice() and Misc:Input().
v1.4.2.2 Fix a bug which was introduced by v1.4.2.1.
v1.4.2.1 Add encoding param in Sys:Run().
         v2ray-core win32 v4.26.0
-----------------------------------------------------
v1.4.1.12 Add option check for v2ray-core update when app start.
v1.4.1.11 Disable prompt of loading default config in configer.
v1.4.1.10 Reverse v1.4.1.9
v1.4.1.9 Add download speed in tooltip of total-down-label.
v1.4.1.8 Clear paging menu before adding new item.
v1.4.1.7 Adjust UI.
v1.4.1.6 Fix bugs.
v1.4.1.5 Remove statistics plug-in.
v1.4.1.4 Add stream settings support in vmess://... decode template.
v1.4.1.3 Fix a bug.
v1.4.1.2 Fix a bug in pager updating.
v1.4.1.1 Add vmess://... decode template support.
------------------------------------------------
v1.4.0.11 18080 -> 8080
v1.4.0.10 Print server index in logs.
v1.4.0.9 Remove HtmlAgilityPack.
v1.4.0.8 Keep current selections in searching.
v1.4.0.7 Refactoring.
v1.4.0.6 Try to fix a dead lock.
v1.4.0.5 Create menu items on demand.
v1.4.0.4 Fix a bug in Misc:RandHex().
v1.4.0.3 Add v2fly as core update source.
v1.4.0.2 Custom inbounds support.
v1.4.0.1 Fix a bug in v2ray-core updating.
----------------------------------------
v1.3.9.8 Fix a bug which would block core updating.
v1.3.9.7 Add luasocket and luasec.
v1.3.9.6 Add Sys:CreateHttpServer().
v1.3.9.5 Adjust LuaUI.
v1.3.9.4 Selection hotkeys support in Misc:Choice().
         Add feature, stop script from systray menu.
         Add Sys:Volume*().
         Fix index out of range again.
v1.3.9.3 Add edit button in LuaUI.
v1.3.9.2 Fix index out of range bug.
         Choice support double click.
v1.3.9.1 Fix bugs in analyzer.
-----------------------------------------------------
v1.3.8.5 Tweak serverUI labels' color.
v1.3.8.4 Fix a bug in modules.set.lua.
v1.3.8.3 Remove global CLR setting in Luna.
v1.3.8.2 Add Misc:Invoke(luaFunction) in Luna.
v1.3.8.1 Fix searching in form configer has no highlight bug.
------------------------------------------------
v1.3.7.15 Refactoring.
v1.3.7.14 Rewrite lua-complete.
v1.3.7.13 Add table.dump() remove table.tostring().
v1.3.7.12 Add go to line feature.
v1.3.7.11 Modify searchbox.
v1.3.7.10 Modify core ready detection.
v1.3.7.9 Refactoring.
v1.3.7.8 Redesign Luna editor.
v1.3.7.7 Fix total of subs. not update bug.
v1.3.7.6 Auto-complete in Luna editor.
v1.3.7.5 Non-blocking input/output controls in Luna.
v1.3.7.4 Luna editor supports history.
v1.3.7.3 Add navigation bar in Luna editor.
------------------------------------------------
v1.3.7.1 Fix bugs.
------------------------------------------------
v1.3.6.16 Stop all servers if app close by user.
v1.3.6.15 Modify notify icon corner mark.
v1.3.6.14 Modify chained task in lazy guy.
v1.3.6.13 Stop cores after services disposed.
v1.3.6.12 Add Signal:ScreenLocked().
v1.3.6.11 Use native window, again.
v1.3.6.10 Run in background.
v1.3.6.9 Redesign initialization procedure.
v1.3.6.8 Refactor lazy guy.
v1.3.6.7 Bump version.
v1.3.6.6 Disable notifyicon debug logging.
v1.3.6.5 Try to fix a UI freezing problem.
v1.3.6.4 Fix FormOption I18N.zh-CN.
v1.3.6.3 Try to fix AutoUpdate bug.
v1.3.6.2 Add Sys:RunAndForgot().
         Log more detail in debug logs.
v1.3.6.1 Fix bugs.
-----------------------------------------------
v1.3.5.11 Misc:ShowData() return a json string.
v1.3.5.10 Add debug log file option.
v1.3.5.9 Fix bugs.
v1.3.5.8 Refactoring.
v1.3.5.7 Try to fix a UI freezing bug.(failed)
v1.3.5.6 Refactoring.
v1.3.5.5 Refactoring.
v1.3.5.4 Fix a bug.
v1.3.5.3 Add hotkey supports in Luna plug-in.
v1.3.5.2 Move ILuaJson into ILuaMisc.
v1.3.5.1 Optional CLR loading in Luna plug-in.
----------------------------------------------
v1.3.4.14 Refactoring.
v1.3.4.13 Luna enable clr support.
v1.3.4.12 Update lua editor.
v1.3.4.11 Redesign lua controller UI.
v1.3.4.10 Add allow-insecure option.
v1.3.4.9 Add GetState() in ILuaMail.
v1.3.4.8 Update ILuaSys.
v1.3.4.7 (Luna) Switch to edit tab after load script form file.
v1.3.4.6 Refactoring.
v1.3.4.5 Hide output panel in Luna plug-in by default.
v1.3.4.4 Refactor mailbox of Luna plug-in.
         Fix "function" keyword indentation problem in lua editor.
v1.3.4.3 Add mailbox feature in Luna plug-in.
v1.3.4.2 Add clear speed test results.
         Add interface ILuaSys in Luna plug-in.
v1.3.4.1 Selfadapting quick switch menu.
         Optional latency limitation in quick switch menu.
-----------------------------------------------------------------
v1.3.3.13 Modify servers menu in notify icon.
v1.3.3.12 Add total in FormDataGrid of Luna plug-in.
v1.3.3.11 Fix a bug that notify icon menu does not update after sorting.
v1.3.3.10 Change menu group size from 18 to 12.
          Add reverse selected server by index menu item in form main.
v1.3.3.9 Add quick switch menu to systray icon.
v1.3.3.8 Copy on click in subs UI.
v1.3.3.7 Fix a bug in lua/libs/utils.lua.
v1.3.3.6 Luna output box supports unicode.
v1.3.3.5 Refactoring.
v1.3.3.4 Add remark label on server panel.
         Preserve speed-test-results.
         Add Servers:StopAllServers() in Luna plug-in.
         Adjust UI.
v1.3.3.3 Pause notify icon updating when menu shows up.
v1.3.3.2 Try to fix port is taken up problem.
v1.3.3.1 Validate port range on vmess share link.
------------------------------------------------------
v1.3.2.14 Data grid supports select by cell.
v1.3.2.13 Add new features in data gird dialog.
v1.3.2.12 Show unicode in rich text box.
v1.3.2.11 Change background color of data grid view.
v1.3.2.10 Add BrowseFolder(), BrowseFile(), ShowData() in Luna plug-in.
v1.3.2.9 Run batch speed testing in random order.
v1.3.2.8 Show result amid speed testing.
v1.3.2.7 Auto append new subs. item in form option.
v1.3.2.6 Add context menustrip to Input of Luna plug-in.
v1.3.2.5 Fix a bug in Web:Fetch() of Luna plug-in.
v1.3.2.4 Fix a bug.
v1.3.2.3 MultiConf supports relative path.
v1.3.2.2 Add GetOsVersion(), GetOsReleaseInfo() in Luna plug-in.
v1.3.2.1 Disable url detecting in all log form.
--------------------------------------------------------
v1.3.1.5 Fix a bug.
v1.3.1.4 Refine speed testing algorithm.
v1.3.1.3 Support multiple-config-files feature of v2ray-core v4.23.1.
v1.3.1.2 Fix notify icon menu of ProxySetter not update bug.
         Add env var V2RAY_LOCATION_CONFDIR.
         Upgrade to v2ray-core v4.23.1
v1.3.1.1 Retry with no restart after update failed.
         Max concurrent v2ray core setting take effect in next speed testing.
--------------------------------------------------------
v1.3.0.6 Adjust UI.
         Use semaphorse to throttle speed testing.
v1.3.0.5 Refactoring.
v1.3.0.4 Rollback to v1.3.0.2 due to form focus problem.
v1.3.0.3 Modify UI update logic of form main.
         Choice of Luna plug-in supports defult choice.
v1.3.0.2 Throttle UI update frequency.
v1.3.0.1 AutoGroupMenuItem supports multiple level grouping. 服务器太多了ヾ(≧▽≦*)o
         Fix a bug in form-main status-bar updating.
         Fix a bug in streamSettings decoding.
----------------------------------------------------
v1.2.9.11 Remove code for debugging.
v1.2.9.10 Fix a dead-lock.
v1.2.9.9 Fix systray-icon-text update bug.
v1.2.9.8 Forgot to comment out debugging codes. XD
v1.2.9.7 Try to fix issue #4 SubscriptionUI bug.
v1.2.9.6 Add version information in bug report.
v1.2.9.5 Input of Luna plug-in can initialize with content.
v1.2.9.4 fix "锟斤拷"
v1.2.9.3 add click event handler in tags of server panel
v1.2.9.2 refactor
v1.2.9.1 add Alert, Choice ... in Luna plug-in

*/
