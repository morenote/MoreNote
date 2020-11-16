﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreNote.Logic.Entity.ConfigFile
{
    /// <summary>
    /// 公共API配置
    /// </summary>
   public class PublicAPIConfig
    {
        /// <summary>
        /// 是否启用随机图片API服务
        /// </summary>
        public bool RandomImageAPI { get; set; }
        /// <summary>
        /// 随机图片访问保险丝
        /// 当访问数量超过这个保险丝时，对接口进行熔断处理
        /// 避免对整个系统的运行产生破坏
        /// </summary>
        public int RandomImageFuseSize { get; set; }
        //随机图片API 随机程度
        public int RandomImageSize { get; set; }
        /// <summary>
        /// 是否启动token防盗链（基于又拍云的token防盗链）
        /// </summary>
        public bool Token_anti_theft_chain { get; set; }

    }
}
