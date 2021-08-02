--[[
利用SSH将本地代理端口转发到远程服务器。
仅支持Win10系统，需要先配置好免密码登录。
]]

local servs = {
    {"Alex@192.168.1.100", 8080},
    {"Bob@192.168.1.101", 1080},
    -- 添加更多服务器信息
}

function ShowMenu()
    local m = {}
    for k, v in pairs(servs) do
        local t = v[1] .. ":" .. v[2]
        table.insert(m, t)
    end
    
    return Misc:Choice("转发代理端口给：", m, true)
end

function Forward(idx)
    local s = servs[idx]
    local auth = s[1]
    local port = s[2]
    local args = "-o ExitOnForwardFailure=yes -N -R " .. port .. ":localhost:" .. port .. " " .. auth
    local t = "端口转发" .. auth
    print(t)
    local proc = Sys:Run("ssh", args, nil, nil, false, true)
    Misc:Sleep(1000)
    
    if Sys:HasExited(proc) then
        Misc:Alert(t .. "失败")
        return
    else
        Misc:Alert(t .. "成功")
    end
    
    while not Signal:Stop() and not Sys:HasExited(proc) do
        Misc:Sleep(1000)
    end

    if not Sys:HasExited(proc) then
        Sys:SendStopSignal(proc)
    end

    Misc:Alert(t .. "结束")
end
    
function Main()
    local idx = ShowMenu()
    if idx < 1 then
        print("用户取消")
        return
    end
    Forward(idx)
end

Main()