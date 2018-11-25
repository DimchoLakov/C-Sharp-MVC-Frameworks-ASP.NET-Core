using System;
using Eventures.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Eventures.Web.Filters
{
    public class EventsLogActionFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private CreateEventViewModel _model;

        public EventsLogActionFilter(ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<EventsLogActionFilter>();

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this._model = context.ActionArguments.Values.OfType<CreateEventViewModel>().Single();

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (this._model != null)
            {
                var username = context.HttpContext.User.Identity.Name;
                this._logger.LogInformation(
                    $"[{DateTime.UtcNow}] Administrator {username} created event {this._model.Name} ({this._model.Start} / {this._model.End}).");
            }

            base.OnActionExecuted(context);
        }
    }
}
