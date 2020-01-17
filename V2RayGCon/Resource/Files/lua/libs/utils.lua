local u={}

function u.Echo(message)
	print(message)
end

function u.WriteLine(filename, content)
	local file=io.open(filename, "a")
    file:write(content .. "\n")
    file:close()
end

function u.IsInTable(haystack, needle)
	for _, item in pairs(haystack)
    do
        if string.lower(needle) == string.lower(item) then
            return true
        end
    end
    return false
end

function u.IsInTablePartially(haystack, needle)
	for _, item in pairs(haystack)
    do
        if string.find(string.lower(item), string.lower(needle)) then
            return true
        end
    end
    return false
end

return u