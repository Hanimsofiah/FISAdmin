using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FISAdmin.Models
{

    public class UsersModel
    {
        /* dropdown user list & dropdown roles list */
        public List<SelectListItem> Roles { get; set; }
        public List<SelectListItem> Users { get; set; }

        /* dbfis - dbo.AspNetUsers - list all column data to use for edit/display */
        [Key]
        [DisplayName("User ID")]
        [Column("Id")]
        [DataType(DataType.Text)]
        public string Id { get; set; }

        [Column("Nopeng")]
        [DisplayName("IC")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "IC is required")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "IC must be 12 characters")]
        public string Nopeng { get; set; } = "";

        [Column("Nama")]
        [DisplayName("Name")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "User is required")]
        public string Nama { get; set; } = "";

        [Column("ActiveStatus")]
        [DisplayName("Active Status")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Active Status is required")]
        public int ActiveStatus { get; set; } = 1;

        [Required]
        [Column("Email")]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Column("CreatedBy")]
        [DisplayName("Created By")]
        [DataType(DataType.Text)]
        public string CreatedBy { get; set; } = "";

        [Column("CreatedDate")]
        [DisplayName("Created Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("LastModifiedBy")]
        [DisplayName("Last Modified By")]
        [DataType(DataType.Text)]
        public string? LastModifiedBy { get; set; }

        [Column("LastModifiedDate")]
        [DisplayName("Last Modified Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedDate { get; set; } = DateTime.Now;

        /* unique id for cshtml */
        public string user_no { get; set; } = "user";

        /* dbfis - dbo.AspNetUsersRoles */
        [DisplayName("Role Id")]
        [Column("Id")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Role is required")]
        public string RoleId { get; set; }

        [DisplayName("Role Name")]
        [Column("Name")]
        [DataType(DataType.Text)]
        public string RoleName { get; set; }
    }
}
