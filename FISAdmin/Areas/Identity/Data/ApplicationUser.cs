using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace FISAdmin.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{


    /* dropdown user list & dropdown roles list */

    [NotMapped]
    public List<SelectListItem> Roles { get; set; }


    [NotMapped]
    public List<SelectListItem> Users { get; set; }



    /* dbfis - dbo.AspNetUsers - Customise extra field apart from (email, pw, pw confirm)*/



    [Column("Nopeng")]
    [DisplayName("IC")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "IC is required")]
    [StringLength(12, MinimumLength = 12, ErrorMessage = "IC must be 12 characters")]
    public string Nopeng { get; set; } = "";

    [Column("Nama")]
    [DisplayName("Nama")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "User is required")]
    public string Nama { get; set; } = "";

    [Column("ActiveStatus")]
    [DisplayName("Active Status")]
    [DataType(DataType.Text)]
    public int ActiveStatus { get; set; } = 1;

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

    /*dbfis - dbo.AspNetUsersRoles*/

    [NotMapped]
    [DisplayName("Role")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Role is required")]

    public string RoleId { get; set; }


}

