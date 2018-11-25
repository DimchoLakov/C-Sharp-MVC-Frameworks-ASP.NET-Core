using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Eventures.Web.ViewModels
{
    public class CreateEventViewModel
    {
        [Required]
        [Display(Name = "Name")]
        [MinLength(length: 10, ErrorMessage = "Name should be at least 10 symbols long.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Place")]
        public string Place { get; set; }

        [Required]
        [Display(Name = "Start")]
        [DataType(DataType.DateTime)]
        public DateTime Start { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "End")]
        [DataType(DataType.DateTime)]
        public DateTime End { get; set; } = DateTime.UtcNow.AddDays(1);

        [Required]
        [Display(Name = "Total Tickets")]
        [Range(0, int.MaxValue)]
        public int TotalTickets { get; set; }

        [Required]
        [Display(Name = "Price per Ticker")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal PricePerTicket { get; set; }
    }
}
