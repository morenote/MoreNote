﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Konscious.Security.Cryptography;
using MoreNote.Common.Utils;

namespace MoreNote.Logic.Service.PasswordSecurity
{
    /// <summary>
    /// 实现BCrypt算法验证
    /// </summary>
    public class Argon2PasswordStore : IPasswordStore
    {
       
        /// <summary>
        /// 线程限制=物理核心x2
        /// </summary>
        public int DegreeOfParallelism{ get;set;} =8;
        /// <summary>
        /// 内存限制 kByte
        /// </summary>
        public int MemorySize{ get;set;} =1024*2;//kByte
        public byte[] Encryption(byte[] pass, byte[] salt, int iterations)
        {
            if (salt.Length!=16)
            {
                throw new ArgumentException("Salt must be equal to 16 byte");
            }
            if (iterations < 1 || iterations > 4096)
                throw new ArgumentException("Bad number of iterations", "iterations");

            var argon2 = new Argon2id(pass)
            {
                Salt=salt,
                DegreeOfParallelism= DegreeOfParallelism,//CPU
                Iterations=iterations,//轮数
                MemorySize= MemorySize //内存
            };
            return argon2.GetBytes(32);
        }

        public bool VerifyPassword(byte[] encryData,byte[] pass , byte[] salt, int iterations)
        {
            var newhash=Encryption(pass,salt,iterations);

            return SecurityUtil.SafeCompareByteArray(encryData,newhash);
            
        }
    }
}
