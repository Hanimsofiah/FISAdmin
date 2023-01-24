using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.Generic;

namespace FISAdmin.Models
{
    
    public class RolesModel
    {
        /* dropdown roles list */
        public List<SelectListItem> Roles { get; set; }

        /* dbfis - CRUD for dbo.AspNetRoles */

       
        [Key]
        [DisplayName("Role ID")]
        [Column("Id")]
        [DataType(DataType.Text)]

        public string Id { get; set; }

        [DisplayName("Role Name")]
        [Column("Name")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Role Name is required")]

        public string Name { get; set; }


        /* GUID for Id and Currency Stamp */
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();


        [DisplayName("Created By")]
        [Column("CreatedBy")]
        [DataType(DataType.Text)]
        public string? CreatedBy { get; set; }


        [DisplayName("Created On")]
        [Column("CreatedDate")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDate { get; set; }


        [DisplayName("Last Modified By")]
        [Column("LastModifiedBy")]
        public string? LastModifiedBy { get; set; }


        [DisplayName("Last Modified On")]
        [Column("LastModifiedDate")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastModifiedDate { get; set; }

    }
}
