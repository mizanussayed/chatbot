using SignalRChat.Interface;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.DTOs;

namespace SignalRChat.Service
{
    public class ToastNotification : IToastNotification
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public ToastNotification(IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
        }
        public async Task Success(string message, int? duration = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);

            await Task.Delay(0);
            tempData["Message"] = JsonConvert.SerializeObject(new { ToastStatus = "Success", Message = $"{message}" });
            //httpContext.Session.SetString("ToastNotify", JsonConvert.SerializeObject(new ToastAppNotification(message, "Success")));
        }
        public async Task Error(string message, int? duration = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);
            await Task.Delay(0);
            tempData["Message"] = JsonConvert.SerializeObject(new { ToastStatus = "Error", Message = $"{message}" });
            //httpContext.Session.SetString("ToastNotify", JsonConvert.SerializeObject(new ToastAppNotification(message, "Error")));
        }
        public async Task Warning(string message, int? duration = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);
            await Task.Delay(0);
            tempData["Message"] = JsonConvert.SerializeObject(new { ToastStatus = "Warning", Message = $"{message}" });
            //httpContext.Session.SetString("ToastNotify", JsonConvert.SerializeObject(new ToastAppNotification(message, "Warning")));
        }
        public async Task Info(string message, int? duration = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);
            await Task.Delay(0);
            tempData["Message"] = JsonConvert.SerializeObject(new { ToastStatus = "Info", Message = $"{message}" });
            //httpContext.Session.SetString("ToastNotify", JsonConvert.SerializeObject(new ToastAppNotification(message, "Info")));
        }

        public void Custom(string message, int? durationInSeconds = null, string backgroundColor = "black", string iconClassName = "home")
        {
            throw new NotImplementedException();
        }

    }
}
