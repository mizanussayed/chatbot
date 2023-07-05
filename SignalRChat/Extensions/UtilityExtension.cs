using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SignarlRChat.Extensions
{
    public static class UtilityExtension
    {
        public static string GetModelStateError(this ModelStateDictionary ModelState)
        {
            return string.Join(System.Environment.NewLine, ModelState.Values
                                          .SelectMany(x => x.Errors)
                                          .Select(x => x.ErrorMessage));
        }
    }
}
