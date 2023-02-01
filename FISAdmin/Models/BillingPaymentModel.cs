using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISAdmin.Models
{
    /* dbfis - dbo.billing_category_bill, dbo.billing_customer_category, dbo.billing_doc_type, dbo.billing_fis_destination, dbo.billing_priority, dbo.billing_source_bill & dbo.billing_s tatus_bill */
    /* dbfis -  dbo.payment_doc_type, dbo.payment_source, dbo.payment_status & dbo.payment_type */
    public class BillingPaymentModel
    {
        [Key]
        [DisplayName("Id")]
        [Column("id")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Id is required")]
        [StringLength(3, MinimumLength = 1, ErrorMessage = "Id cannot be more than 3 characters.")]
        public string? Id { get; set; }

        [DisplayName("Description")]
        [Column("description")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Description is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Description cannot be longer than 30 characters")]
        public string? Description { get; set; }
    }
}
