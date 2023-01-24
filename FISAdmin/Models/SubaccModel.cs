using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISAdmin.Models
{
    /* dbfis - dbo.subaccount_aktiviti, dbo.subaccount_kump_wang, dbo.subaccount_penyumbang & dbo.subaccount_sumber */

    public class SubaccModel
    {

        public List<SelectListItem>? aktiviti { get; set; }

        public List<SelectListItem>? kump_wang { get; set; }

        public List<SelectListItem>? penyumbang { get; set; }

        public List<SelectListItem>? sumber { get; set; }

        public string? subacc_type { get; set; }


        

        [Key]
        [DisplayName("Kod")]
        [Column("kod")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage="Kod is required")]
        /*[StringLength(6, MinimumLength = 2, ErrorMessage = "Kod cannot more 6 characters and less than 2 characters.")]*/

        public string? Kod { get; set; }




        [DisplayName("Tajuk")]
        [Column("tajuk")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Tajuk is required")]
        /*[StringLength(200, MinimumLength = 1, ErrorMessage = "Tajuk cannot be longer than 200 characters")]*/
        public string? Tajuk { get; set; }



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
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}",ApplyFormatInEditMode = true)]
        public DateTime? LastModifiedDate { get; set; }


    }
}
