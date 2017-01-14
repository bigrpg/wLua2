#define LUA_LIB

#include "lprefix.h"

#include <stdarg.h>
#include <string.h>

#include "lua.h"
#include "lauxlib.h"
#include "lualib.h"


//Unity MT serach index function

//function(t,key)
//		local v = rawget(mt[key])
//		if v then return v end
//		v = mt[key]
//		if not v then rawset(mt[key],v) end
//		return v
//end
static int cs_indexfunc(lua_State * L) //t,key
{
	lua_pushvalue(L, 2); //args/key
	lua_rawget(L, lua_upvalueindex(1));
	if (!lua_isnil(L, -1))
		return 1;

	lua_pop(L, 1); //args
	lua_gettable(L, lua_upvalueindex(1));//args/mt[key]
	if (!lua_isnil(L, -1))
	{
		lua_pushvalue(L, 2); //args/mt[key]/key
		lua_pushvalue(L, -2);//args/mt[key]/key/mt[key]
		lua_rawset(L, lua_upvalueindex(1)); //args/mt[key]					//[-2, +0, m]
	}
	return  1;
}

//返回一个以mt为upvalue的index函数
//param: mt
LUA_API int wlua_makecsindex(lua_State * L,int index)		//[-0,+1,m]
{
	lua_pushvalue(L, index);
	lua_pushcclosure(L, cs_indexfunc, 1);
	return 1;
}


#define LUA_ERROR  -1

//when call failed, return -1 , and errormsg in stack[-1]

static int _wlua_getglobal(lua_State *L)
{
	const char * name = lua_tostring(L, 1);
	int ret = lua_getglobal(L, name);
	lua_pushinteger(L, ret);
	return 2;
}

LUA_API int wlua_getglobal(lua_State *L, const char *name) {

	lua_pushcfunction(L, _wlua_getglobal);
	lua_pushstring(L, name);
	int ret = lua_pcall(L, 1, 2, 0);
	if (ret == LUA_OK)
	{
		ret = (int)lua_tointeger(L, -1);
		lua_pop(L, 1);
		return ret;
	}
	return LUA_ERROR;

}

//
static int _wlua_setglobal(lua_State *L)
{
	const char* name = lua_tostring(L, -1);
	lua_pushvalue(L, -2);
	lua_setglobal(L, name);
	return 0;
}

LUA_API int wlua_setglobal(lua_State *L, const char *name) {

	lua_pushcfunction(L, _wlua_setglobal);
	lua_insert(L, -2);
	lua_pushstring(L, name);
	int ret = lua_pcall(L, 2, 0, 0);
	if (ret == LUA_OK)
	{
		return 0;
	}
	return LUA_ERROR;

}

static int _wluaL_openlibs(lua_State *L)
{
	luaL_openlibs(L);
	return 0;
}

LUA_API int wluaL_openlibs(lua_State * L)
{
	lua_pushcfunction(L, _wluaL_openlibs);
	int ret = lua_pcall(L, 0, 0, 0);
	if (ret == LUA_OK)
	{
		return 0;
	}
	return LUA_ERROR;

}

static int _wlua_gettable(lua_State *L)
{
	luaL_checktype(L, -2, LUA_TTABLE);  /* argument must be a table */
	int ret = lua_gettable(L, -2);
	lua_pushinteger(L, ret);
	return 2;
}

LUA_API int wlua_gettable(lua_State * L, int index)
{
	//you should push t firstly ,because index maybe negative index
	lua_pushvalue(L, index); //key,t
	lua_pushcfunction(L, _wlua_gettable); //key,t,func
	lua_pushvalue(L, -2); //key,t,func,t
	lua_pushvalue(L, -4);//key,t,func,t,key
	int ret = lua_pcall(L, 2, 2, 0);	
	if (ret == LUA_OK)//key,t,value,result
	{
		int ret = (int)lua_tointeger(L, -1);
		lua_pop(L, 1);	//key,t,value
		lua_insert(L, -3);//value,key,t
		lua_pop(L, 2); //value
		return ret;
	}
	//key,t,errmsg
	lua_insert(L, -3);//errmsg,key,t
	lua_pop(L, 2); //errmsg
	return LUA_ERROR;

}

//params: t,k,value
static int _wlua_setfield(lua_State *L)
{
	luaL_checktype(L, 1, LUA_TTABLE);  /* argument must be a table */
	const char* k = lua_tostring(L, -2);	//[-0, +0, m]
	lua_pushvalue(L, -1); //t,k,value,value
	lua_setfield(L, -4, k); //t,k,value
	return 0;
}

LUA_API int wlua_setfield(lua_State * L, int index,const char* k)
{
	lua_pushvalue(L, index); //value,t
	lua_pushcfunction(L, _wlua_setfield); //value,t,func
	lua_pushvalue(L, -2); //value,t,func,t
	lua_pushstring(L, k);//value,t,func,t,k
	lua_pushvalue(L, -5);//value,t,func,t,k,value
	int ret = lua_pcall(L, 3, 0, 0);	
	if (ret == LUA_OK)//value,t
	{
		lua_pop(L, 2);//
		return 0;
	}
	//value,t,errmsg
	lua_insert(L, -3);//errmsg,value,t
	lua_pop(L, 2); //errmsg
	return LUA_ERROR;
}

//params:t,k
static int _wlua_getfield(lua_State *L)
{
	luaL_checktype(L, 1, LUA_TTABLE);  /* argument must be a table */
	const char* k = lua_tostring(L, -1);	//[-0, +0, m]
	int ret = lua_getfield(L, -2, k); //t,k,value
	lua_pushinteger(L, ret); //t,k,value,ret
	return 2;
}

LUA_API int wlua_getfield(lua_State * L, int index, const char* k)
{
	lua_pushvalue(L, index); //t
	lua_pushcfunction(L, _wlua_getfield); //t,func
	lua_pushvalue(L, -2); //t,func,t
	lua_pushstring(L, k);//t,func,t,k
	int ret = lua_pcall(L, 2, 2, 0);	
	if (ret == LUA_OK)//t,value,ret
	{
		ret = (int)lua_tointeger(L, -1);
		lua_pop(L, 1); //t,value
		lua_insert(L, -2);//value,t
		lua_pop(L, 1); //value
		return ret;
	}
	//t,errmsg
	lua_insert(L, -2);//errmsg,t
	lua_pop(L, 1);//errmsg
	return LUA_ERROR;
}


//params:t,key
static int _wlua_next(lua_State * L)
{
	luaL_checktype(L, 1, LUA_TTABLE);  /* argument must be a table */
	int ret = lua_next(L, 1);
	if (ret)
	{
		lua_pushinteger(L, ret); //k,v,ret
		return 3;
	}

	return 0;

}

LUA_API int wlua_next(lua_State * L, int index, int * exception)
{
	*exception = 0;
	lua_pushvalue(L, index);	//key,t
	lua_pushvalue(L, -2);		//key,t,key
	lua_pushcfunction(L, _wlua_next); //key,t,key,func
	lua_insert(L, -3);//key,func,t,key
	int top = lua_gettop(L) - 3;
	int ret = lua_pcall(L, 2, LUA_MULTRET, 0);
	if (ret == LUA_OK) //
	{
		int nresult = lua_gettop(L) - top;
		if (nresult == 0) //key
		{
			lua_pop(L, 1); //
			return 0;
		}
		else //key,keynext,valuenext,ret
		{
			ret = (int)lua_tointeger(L, -1);
			lua_pop(L, 1); ////key,keynext,valuenext
			lua_insert(L, -3);//valuenext,key,keynext
			lua_insert(L, -3);//keynext,valuenext,key
			lua_pop(L, 1);
			return ret;
		}
	}

	//key,errmsg
	lua_insert(L, -2);//errmsg,key
	lua_pop(L, 1);//errmsg
	*exception = 1;
	return LUA_ERROR;

}

//param: t,k,v
static int _wlua_settable(lua_State * L)
{
	luaL_checktype(L, 1, LUA_TTABLE);  /* argument must be a table */
	lua_settable(L, 1);
	return 0;
}
LUA_API int wlua_settable(lua_State * L, int index)
{
	lua_pushvalue(L, index); //k,v,t
	lua_pushcfunction(L, _wlua_settable); //k,v,t,f
	lua_insert(L, -4); //f,k,v,t
	lua_insert(L, -3); //f,t,k,v
	int ret = lua_pcall(L, 3, 0, 0);
	if (ret == LUA_OK) //
	{
		return 0;
	}
	//errmsg
	return LUA_ERROR;

}

//param: v
static int _wlua_len(lua_State * L)
{
	lua_len(L, 1);
	return 1;
}

LUA_API int wlua_len(lua_State * L,int index)
{
	lua_pushvalue(L, index); //v
	lua_pushcfunction(L, _wlua_len); //v,f
	lua_insert(L, -2); //f,v
	int ret = lua_pcall(L, 1, 1, 0);
	if (ret == LUA_OK) //len
	{
		return 0;
	}
	//errmsg
	return LUA_ERROR;
}


//////////////////////////////////////////////////////

//Register function
static char * s = ""
"return function(f,isoctr) "
"	local function stripParam(...) "
"       local a,errMsg = ..."
"		if not a and errMsg then "
"			error(errMsg,2) "
"		end "
"		return ...  "
"	end  "
""
"   if not isoctr then "
"		return function(...) "
"			return stripParam(f(...)) "
"		end "
"   else"
"       return function(...) "
"           local n = select('#',...)   "
"			local t = {...}  "
"			t[n+1] = namespace  "
"			return stripParam(f(table.unpack(t,1,n+1))) "
"       end "
"   end "
"end "
"";

LUA_API int wlua_init(lua_State * L,const char* wluaNameSpace)
{
	char text[1024];
	snprintf(text, sizeof(text), " %s = {}; local namespace = %s; %s", wluaNameSpace, wluaNameSpace, s);
	int ret = luaL_loadbuffer(L, text, strlen(text), "wrapfunc");
	if (ret == LUA_OK)
	{
		ret = lua_pcall(L, 0, 1, 0);
		if (ret == LUA_OK)
		{
			wlua_setglobal(L, "cswrapfunc");
			return 0;
		}
	}
	return LUA_ERROR;
}


//