using System.Collections.Generic;

namespace Eventures.Web.ViewModels
{
    public class CreateEventOrderViewModel
    {
        public CreateEventOrderViewModel()
        {
            this.CurrentPage = 1;
        }

        public IEnumerable<CreateEventViewModel> CreateEventViewModels { get; set; }

        public CreateOrderViewModel CreateOrderViewModel { get; set; }

        public int CurrentPage { get; set; }
    }
}
