﻿using System;

namespace MoreNote.Common.Utils
{
	public class Util
	{
		public static string Md5(string str)
		{
			return SHAEncryptHelper.MD5Encrypt(str);
		}
		public static bool IsObjectId(string id)
		{
			throw new Exception();
		}


	}
}
