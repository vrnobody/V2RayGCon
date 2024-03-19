---
title: "用户交互控件"
date: 2020-02-26T13:22:12+08:00
draft: false
weight: 20
---

##### Alert, Choice, Input

{{< figure src="../../../images/neoluna/user_io_ctrl.png" >}}  

```lua
function Main()
    local choices = {
        "显示当前时间(Alert演示)",
        "简单计算(Input演示)",
        "你喜欢什么水果(Choices演示)",
    }
        
    local idx = std.Misc:Choice("请选择演示内容(点'取消'退出)", choices, true)
    
    if idx == 1 then
        std.Misc:Alert(os.date('%Y-%m-%d %H:%M:%S'))
    end
    
    if idx == 2 then
        local expr = std.Misc:Input("请输入简单算式, 例如: 1+2*3", 1)
        local f = load('return ' .. expr)
        std.Misc:Alert("结果是：" .. tostring(f()))
    end
    
    if idx == 3 then
        ChooseFruit()
    end
    
    if idx < 1 then
        return false
    end
    return true
end

function ChooseFruit()
    local fruit = {
        "香蕉",
        "橙子",
        "鸭梨",
    }
    
    local choices = std.Misc:Choices("你喜欢吃什么水果?", fruit)
    local r = ""
    foreach index in choices do
        r = r .. fruit[index] .. ","
    end
    if string.isempty(r) then
        std.Misc:Alert("没一个喜欢的")
    else
        std.Misc:Alert(string.sub(r, 0, #r - 1))
    end
end

repeat
    local again = Main()
until not again
```

##### ShowData输出控件
 
```lua
local utils = require('3rd/neolua/libs/utils')

function Main()
    local servs = std.Server:GetAllServers()
    local v = ShowDatas(servs)
    print(v)
end

function ShowDatas(servs)
    local rows = {}
    foreach coreServ in servs do
        local wserv = coreServ:Wrap()
        local row = {
            wserv:GetIndex(),
            wserv:GetLongName(),
            wserv:GetSummary(),
            wserv:GetMark(),
            utils.ToLuaDate(wserv:GetLastModifiedUtcTicks()),
            wserv:GetStatus(),
        }
        table.insert(rows, row)
    end
    -- print(table.dump(rows))
    local columns = {"序号", "名称", "摘要", "标记", "修改日期", "测速"}
    return std.Misc:ShowData("服务器列表:", columns, rows, 3)
end

Main()
```

