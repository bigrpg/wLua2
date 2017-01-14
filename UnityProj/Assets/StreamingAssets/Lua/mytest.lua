

local UnityEngine = wlua
print("my test lua")

local t = { a = "aaa", b = "ccc" }



--Game.test(t)


local go = UnityEngine.GameObject.GameObject('hhh')  
go:set_tag('mytest')   
print(go:get_name());  
UnityEngine.GameObject.Destroy(go);  
go = nil
go = UnityEngine.GameObject.GameObject('xxx');  
go:set_tag('Player');  
local com = go.AddComponent(go,"BoxCollider")
--com:set_tag("mytest")
local com1 = go:AddComponent("MeshFilter")
local com2 = go:AddComponent("MeshRenderer")
print("com:",com)
print("com1",com1)
print("com2",com2)
com2:set_receiveShadows(true)
com2:set_name("haha")


com:set_enabled(false)

local com3 = go:GetComponent("Component")
print("com3:",com3)

box = wlua.GameObject.CreatePrimitive(1)
box:set_name("box")

print("com:",com)
local com11 = go:GetComponent("Collider")
print("com11:",com11)

--UnityEngine.GameObject.Destroy(go)  

--go = nil

collectgarbage("collect")

print("aaa")
print(wlua.hello)


mytable = { "11","4433" ,sex = "male", "aaddf", "ssss",name = "wang"}


function func()
	--Game.testerror()
end

function func1()
	func()
end

function func2()
	func1()
end
