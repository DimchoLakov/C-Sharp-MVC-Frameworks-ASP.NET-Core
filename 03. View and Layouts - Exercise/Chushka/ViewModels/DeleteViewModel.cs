using System.ComponentModel.DataAnnotations;
using Chushka.Data.Models.Enums;

namespace Chushka.ViewModels
{
    public class DeleteViewModel
    {
        [Required]
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public decimal Price { get; set; }
        
        public string Description { get; set; }
        
        public ProductType Type { get; set; }
    }
}
