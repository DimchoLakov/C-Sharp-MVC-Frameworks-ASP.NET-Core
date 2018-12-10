using System.Collections.Generic;

namespace Eventures.Web.ViewModels
{
    public class CreateEventOrderViewModel
    {
        public CreateEventOrderViewModel()
        {
            this.CurrentPage = 1;
            this.FirstPage = 1;
            this.LastPage = 1;
        }

        public IEnumerable<CreateEventViewModel> CreateEventViewModels { get; set; }

        public CreateOrderViewModel CreateOrderViewModel { get; set; }

        public int CurrentPage { get; set; }

        public int FirstPage { get; set; }

        public int LastPage { get; set; }
    }
}
