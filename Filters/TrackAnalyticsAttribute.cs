using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using GoogleAnalyticsTracker.WebApi2;
using Orchard;
using Microsoft.Azure;

namespace CSM.ParkingData.Filters
{
    public class TrackAnalyticsAttribute : ActionFilterAttribute
    {
        static string analyticsId = CloudConfigurationManager.GetSetting("GoogleAnalyticsId");

        private readonly string _title;

        public TrackAnalyticsAttribute(string title)
        {
            _title = title;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!String.IsNullOrEmpty(analyticsId))
            {
                string baseUrl = actionContext.ControllerContext.GetWorkContext().CurrentSite.BaseUrl;

                using (var tracker = new Tracker(analyticsId, baseUrl))
                {
                    tracker.TrackPageViewAsync(actionContext.Request, _title);
                }
            }
        }
    }
}
