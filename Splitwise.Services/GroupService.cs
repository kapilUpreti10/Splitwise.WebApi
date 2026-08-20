using Microsoft.EntityFrameworkCore;
using Splitwise.Contracts.DTOs.Groups;
using Splitwise.Contracts.DTOs.GroupMembers;
using Splitwise.DataAccess;
using Splitwise.Models;
using Splitwise.Services.Interfaces;

namespace Splitwise.Services
{
    public class GroupService : IGroupService
    {
        private readonly ApplicationDbContext _db;

        public GroupService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<GroupDto>> GetAllGroupsAsync()
        {
            return await _db.Groups
                .Include(g => g.CreatedByUser)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    CreatedBy = g.CreatedBy,
                    CreatedByName = g.CreatedByUser != null ? g.CreatedByUser.Name : null,
                    CreatedAt = g.CreatedAt,
                    MemberCount = g.GroupMembers.Count
                })
                .ToListAsync();
        }

        public async Task<List<GroupDto>> GetGroupsForUserAsync(string userId)
        {
            return await _db.GroupMembers
                .Where(gm => gm.UserId == userId)
                .Select(gm => gm.Group!)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    CreatedBy = g.CreatedBy,
                    CreatedByName = g.CreatedByUser != null ? g.CreatedByUser.Name : null,
                    CreatedAt = g.CreatedAt,
                    MemberCount = g.GroupMembers.Count
                })
                .ToListAsync();
        }

        public async Task<GroupDto?> GetGroupByIdAsync(int id)
        {
            var g = await _db.Groups
                .Include(x => x.CreatedByUser)
                .Include(x => x.GroupMembers)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (g == null) return null;

            return new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                CreatedBy = g.CreatedBy,
                CreatedByName = g.CreatedByUser?.Name,
                CreatedAt = g.CreatedAt,
                MemberCount = g.GroupMembers.Count
            };
        }

        public async Task<GroupDto> CreateGroupAsync(CreateGroupDto dto)
        {
            var group = new Group
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            _db.Groups.Add(group);
            await _db.SaveChangesAsync();

            // Creator is always a member, plus anyone else passed in.
            var memberIds = new HashSet<string>(dto.MemberUserIds ?? new List<string>());
            memberIds.Add(dto.CreatedBy);

            foreach (var userId in memberIds)
            {
                _db.GroupMembers.Add(new GroupMember
                {
                    GroupId = group.Id,
                    UserId = userId
                });
            }
            await _db.SaveChangesAsync();

            return (await GetGroupByIdAsync(group.Id))!;
        }

        public async Task<bool> UpdateGroupAsync(int id, UpdateGroupDto dto)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return false;

            group.Name = dto.Name;
            group.Description = dto.Description;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteGroupAsync(int id)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return false;

            // NOTE: this cascades to GroupMembers/Expenses/ExpenseSplits by default EF Core
            // convention. That's fine for a v1 vibecode project, but worth knowing —
            // deleting a group nukes its whole expense history.
            _db.Groups.Remove(group);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<GroupMemberDto>> GetMembersAsync(int groupId)
        {
            return await _db.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Include(gm => gm.User)
                .Select(gm => new GroupMemberDto
                {
                    Id = gm.Id,
                    GroupId = gm.GroupId,
                    UserId = gm.UserId,
                    UserName = gm.User != null ? gm.User.UserName : null,
                    Name = gm.User != null ? gm.User.Name : null
                })
                .ToListAsync();
        }

        public async Task<(bool Succeeded, string? Error)> AddMemberAsync(int groupId, string userId)
        {
            var groupExists = await _db.Groups.AnyAsync(g => g.Id == groupId);
            if (!groupExists) return (false, "Group not found.");

            var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
            if (!userExists) return (false, "User not found.");

            var alreadyMember = await _db.GroupMembers.AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
            if (alreadyMember) return (false, "User is already a member of this group.");

            _db.GroupMembers.Add(new GroupMember { GroupId = groupId, UserId = userId });
            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<bool> RemoveMemberAsync(int groupId, string userId)
        {
            var member = await _db.GroupMembers.FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
            if (member == null) return false;

            _db.GroupMembers.Remove(member);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
