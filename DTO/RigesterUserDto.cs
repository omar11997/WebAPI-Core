using System.ComponentModel.DataAnnotations;

namespace Chrestin_Project.DTO
{
	public class RigesterUserDto
	{
		[Required]
		public string UserName { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		[Compare("Password")]
		public string ConfirmPassword { get; set; }
	}
}
