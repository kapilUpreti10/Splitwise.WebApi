using Splitwise.Contracts.DTOs.Groups;
using Splitwise.Contracts.DTOs.GroupMembers;

namespace Splitwise.Services.Interfaces
{
    public interface IGroupService
    {
        Task<List<GroupDto>> GetAllGroupsAsync();
        Task<List<GroupDto>> GetGroupsForUserAsync(string userId);
        Task<GroupDto?> GetGroupByIdAsync(int id);
        Task<GroupDto> CreateGroupAsync(CreateGroupDto dto);
        Task<bool> UpdateGroupAsync(int id, UpdateGroupDto dto);
        Task<bool> DeleteGroupAsync(int id);

        Task<List<GroupMemberDto>> GetMembersAsync(int groupId);
        Task<(bool Succeeded, string? Error)> AddMemberAsync(int groupId, string userId);
        Task<bool> RemoveMemberAsync(int groupId, string userId);
    }
}
