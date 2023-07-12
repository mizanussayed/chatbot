using System.ComponentModel.DataAnnotations;

namespace SignalRChat.Enum;

public enum TransectionType
{
    [Display(Name = "Chat Group")]
    GroupMessage = 1,
    [Display(Name = "Auth")]
    Auth = 2
}
