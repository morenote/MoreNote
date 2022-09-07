﻿using Morenote.Models.Models.Entity;

using MoreNote.Models.Enum;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreNote.Models.Entity.Leanote
{
    /// <summary>
    /// 文件仓库
    /// </summary>
    [Table("file_repository")]
    public class FileRepository:BaseEntity
    {
      

        [Column("avatar")]
        public string? Avatar { get; set; }//仓库唯一名称
                                         //
        [Column("name")]
        public string? Name { get; set; }//仓库唯一名称

        [Column("description")]
        public string? Description { get; set; }//仓库摘要说明

        [Column("license")]//开源协议
        public string? License { get; set; }//仓库开源说明

        [Column("owner_type")]
        public RepositoryOwnerType OwnerType { get; set; }//仓库类型

        [Column("owner_id")]
        public long? OwnerId { get; set; }//拥有者

        [Column("visible")]
        public bool Visible { get; set; }//是否公开仓库
        
    }
}
