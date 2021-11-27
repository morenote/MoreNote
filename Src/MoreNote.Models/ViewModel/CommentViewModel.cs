﻿using Masuit.MyBlogs.Core.Models.Entity;
using Masuit.Tools.Models;
using System.Collections.Generic;

namespace Masuit.MyBlogs.Core.Models.ViewModel
{
    /// <summary>
    /// 评论视图模型
    /// </summary>
    public class CommentViewModel : BaseEntity, ITreeChildren<CommentViewModel>
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 文章ID
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        public string CommentDate { get; set; }

        /// <summary>
        /// 浏览器版本
        /// </summary>
        public string Browser { get; set; }

        /// <summary>
        /// 操作系统版本
        /// </summary>
        public string OperatingSystem { get; set; }

        /// <summary>
        /// 是否是博主
        /// </summary>
        public bool IsMaster { get; set; }

        /// <summary>
        /// 支持数
        /// </summary>
        public int VoteCount { get; set; }

        /// <summary>
        /// 反对数
        /// </summary>
        public int AgainstCount { get; set; }

        /// <summary>
        /// 提交人IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 地理信息
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 子级
        /// </summary>
        public ICollection<CommentViewModel> Children { get; set; }
    }
}