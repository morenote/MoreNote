﻿using Morenote.Models.Models.Entity;

using System.ComponentModel.DataAnnotations.Schema;

namespace MoreNote.Models.Entity.Leanote.MyOrganization
{

	[Table("organization_member_role")]
	public class OrganizationMemberRole : BaseEntity
	{



		[Column("organization_id")]
		public long? OrganizationId { get; set; }


		[Column("role_name")]
		public string? RoleName { get; set; }
	}
}