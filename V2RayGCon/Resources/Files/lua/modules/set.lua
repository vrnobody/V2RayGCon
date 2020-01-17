-- helper function
local function ToLower(str)
	return string.lower(str)
end

-- class function
local function Init(self, data)
	self.datas = {}
	if data ~= nil and type(data) == "table" then
		for _, item in pairs(data) do
			self.datas[item] = true
		end
	end	
end

local function Count(self)
	local count = 0
	for key, item in pairs(self.datas) do	
		-- print("data ", key, " = ", item)
		if item == true then
			count = count + 1
		end
	end
	return count
end

local function Add(self, element)
	self.datas[element] = true
end

local function Remove(self, element)
	self.datas[element] = nil
end

local function MatchesPartially(self, text)
	for key, _ in pairs(self.datas) do
		if string.find(ToLower(text), ToLower(key)) then
			return true
		end
	end
	return false
end

local function ContainsPartially(self, element)
	for key, _ in pairs(self.datas) do
		if string.find(ToLower(key), ToLower(element)) then
			return true
		end
	end
	return false
end

local function Contains(self, element)
	return self.datas[element] ~= nil
end

local function ContainsCi(self, element)
	for key, _ in pairs(self.datas) do
		if ToLower(key) == ToLower(element) then
			return true
		end
	end
	return false
end

local function Reset(self, data)
	self.datas = {}
	if data == nil then
		return
	end
	for key, item in pairs(data) do
		-- print("set ", key, " = ", item)
		self.datas[item] = true
	end
end

local function Create(initData)
	local Set = {}
	Init(Set, initData)

	Set.Add = Add
	Set.Contains = Contains
	Set.ContainsCi = ContainsCi
	Set.ContainsPartially = ContainsPartially
	Set.Count = Count
	Set.MatchesPartially = MatchesPartially
	Set.Remove = Remove
	Set.Reset = Reset
	
	return Set
end

return Create