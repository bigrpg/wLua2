using System;
using System.Runtime.Serialization;

namespace LuaInterface
{
    /// <summary>
    /// Exceptions thrown by the Lua runtime
    /// </summary>
    [Serializable]
    public class LuaException : Exception
    {
		void DisposeException(IntPtr L)
		{
			LuaStateCache cache = LuaStateCacheMan.GetLuaStateCache(L);
			cache.setException();

		}

        public LuaException()
        {}

		public LuaException(IntPtr L, string message) : base(message)
		{
			DisposeException(L);
		}

		public LuaException(IntPtr L,string message, Exception innerException) : base(message, innerException)
        {
			DisposeException(L);
		}

    }
}
