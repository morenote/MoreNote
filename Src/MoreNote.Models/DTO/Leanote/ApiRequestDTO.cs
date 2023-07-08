﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreNote.Models.DTO.Leanote
{
    public class ApiRequestDTO
    {
        /// <summary>
        /// 消息唯一标识 客户端必须保证唯一性，否则服务端会拒绝
        /// 推荐使用UUID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 消息创建unix时间戳，秒级别
        /// </summary>
        public string Timestamp { get; set; };
        /// <summary>
        /// 访问用户ID（条件可选）
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 访问凭证（条件可选）
        /// AccessID和AccessKey是对应关系
        /// </summary>
        public string AccessID { get;set; }
        /// <summary>
        /// 客户端Id（不允许重复，客户端可以生成UUID后存储）
        /// </summary>
        public string ClinetId { get;set; }
        /// <summary>
        /// 请求数据(Base64编码)
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 签名方式 条件可选，条件可选
        /// 可选签名参数：HMAC-SHA256、HMAC-SM3、SM2
        /// </summary>
        public string SignType { get; set; } = "SM2";

        /// <summary>
        /// 需要使用AccessKey进行签名，条件可选
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 获得签名参数
        /// </summary>
        /// <returns></returns>
        public byte[] GetSginValue()
        {
            StringBuilder sb = new StringBuilder(); 
            sb.Append(Id);
            sb.Append(Timestamp);
            sb.Append(UserId);
            sb.Append(AccessID);
            sb.Append(Data);
            sb.Append(SignType);
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

    }
}
