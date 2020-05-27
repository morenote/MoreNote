﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MoreNote.Logic.Entity
{
    public class GoodOrder
    {
         [Key]
        public long GoodOrderId { get; set; }
        /// <summary>
        /// mchid
        /// </summary>
        public String mchid { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public int total_fee { get; set; }
        /// <summary>
        /// 用户端订单号 string(32)
        /// </summary>
        public String out_trade_no { get; set; }
        /// <summary>
        /// 订单标题
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// 用户自定义数据，在notify的时候会原样返回
        /// </summary>
        public string attch { get; set; }
        /// <summary>
        /// 接收微信支付异步通知的回调地址。必须为可直接访问的URL，不能带参数、session验证、csrf验证。留空则不通知
        /// </summary>
        public string notify_url { get; set; }
        /// <summary>
        /// 支付宝交易传值：alipay ，微信支付无需此字段
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// PAYJS 订单号
        /// </summary>
        public string payjs_order_id { get; set; }
        /// <summary>
        /// 微信用户手机显示订单号
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 用户OPENID标示，本参数没有实际意义，旨在方便用户端区分不同用户
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 是否回调成功
        /// </summary>
        public bool Notify { get; set; }
        /// <summary>
        /// 是否支付成功
        /// </summary>
        public bool PayStatus { get; set; }
        /// <summary>
        /// 是否被退款
        /// </summary>
        public bool Refund { get; set; }
        /// <summary>
        /// 二维码支付请求数据
        /// </summary>
        public string NativeRequestMessage { get; set; }
        /// <summary>
        /// 二维码支付返回数据
        /// </summary>
        public string NativeResponseMessage { get; set; }
        /// <summary>
        /// 支付成功异步通知接口返回信息
        /// </summary>
        public string NotifyResponseMessage { get; set; }


    }
}
