
using System.ComponentModel.DataAnnotations;

namespace BillApplication.Dto
{
    public class CreateRoleDto
    {
        [Required(ErrorMessage = "Role name is required")]
        public string RoleName { get; set; } = null;
    }
}
