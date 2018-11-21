using Chushka.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Chushka.ViewModels
{
    public class EditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public ProductType Type { get; set; }
    }
}
