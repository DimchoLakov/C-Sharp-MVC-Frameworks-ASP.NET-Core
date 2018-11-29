using System.Collections.Generic;

namespace Eventures.Web.ViewModels
{
    public class CreateEventOrdelViewModel
    {
        public IEnumerable<CreateEventViewModel> CreateEventViewModels { get; set; }

        public CreateOrderViewModel CreateOrderViewModel { get; set; }
    }
}
