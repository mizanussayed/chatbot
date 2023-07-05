using System.ComponentModel.DataAnnotations;

namespace SignarlRChat.Enum
{
    public enum TransectionStatus
    {
        [Display(Name = "Group Updated")]
        GroupUpdated = 1,
        [Display(Name = "Group Deleted")]
        GroupDeleted = 2,
        [Display(Name = "Member Added In Group")]
        MemberAddedInGroup = 3,
        [Display(Name = "Member Remove From Group")]
        MemberRemoveFromGroup = 4
    }
}
