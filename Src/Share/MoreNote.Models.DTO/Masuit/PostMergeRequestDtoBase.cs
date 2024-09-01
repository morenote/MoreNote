using Masuit.MyBlogs.Core.Models.Enum;

namespace Masuit.MyBlogs.Core.Models.DTO
{
	/// <summary>
	/// �����޸�����
	/// </summary>
	public class PostMergeRequestDtoBase : BaseDto
	{
		/// <summary>
		/// ԭ��id
		/// </summary>
		public int PostId { get; set; }

		/// <summary>
		/// ԭ����
		/// </summary>
		public string PostTitle { get; set; }

		/// <summary>
		/// ����
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// �޸���
		/// </summary>
		public string Modifier { get; set; }

		/// <summary>
		/// �޸�������
		/// </summary>
		public string ModifierEmail { get; set; }

		/// <summary>
		/// �ϲ�״̬
		/// </summary>
		public MergeStatus MergeState { get; set; }

		/// <summary>
		/// �ύʱ��
		/// </summary>
		public DateTime SubmitTime { get; set; }

		/// <summary>
		/// �ύ��IP
		/// </summary>
		public string IP { get; set; }
	}
}