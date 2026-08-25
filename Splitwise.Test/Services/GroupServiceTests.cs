using Microsoft.EntityFrameworkCore;
using Splitwise.Contracts.DTOs.Groups;
using Splitwise.Contracts.DTOs.GroupMembers;
using Splitwise.DataAccess;
using Splitwise.Models;
using Splitwise.Services;

namespace Splitwise.Test.Services
{
    public class GroupServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly GroupService _groupService;

        public GroupServiceTests()
        {
            _dbContext = TestHelpers.CreateInMemoryDbContext(Guid.NewGuid().ToString());
            _groupService = new GroupService(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetAllGroupsAsync_ReturnsAllGroups()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser("user-1");
            _dbContext.Users.Add(user);
            _dbContext.Groups.Add(new Group { Name = "Group 1", CreatedBy = "user-1", CreatedAt = DateTime.UtcNow });
            _dbContext.Groups.Add(new Group { Name = "Group 2", CreatedBy = "user-1", CreatedAt = DateTime.UtcNow });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _groupService.GetAllGroupsAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetGroupsForUserAsync_ReturnsUserGroups()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser("user-1");
            var group1 = new Group { Name = "Group 1", CreatedBy = "user-1", CreatedAt = DateTime.UtcNow };
            var group2 = new Group { Name = "Group 2", CreatedBy = "user-2", CreatedAt = DateTime.UtcNow };
            _dbContext.Users.Add(user);
            _dbContext.Groups.AddRange(group1, group2);
            _dbContext.GroupMembers.Add(new GroupMember { GroupId = 1, UserId = "user-1" });
            _dbContext.GroupMembers.Add(new GroupMember { GroupId = 2, UserId = "user-1" });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _groupService.GetGroupsForUserAsync("user-1");

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetGroupByIdAsync_WhenValid_ReturnsGroupDto()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser("user-1");
            var group = new Group { Name = "Test Group", CreatedBy = "user-1", CreatedAt = DateTime.UtcNow };
            _dbContext.Users.Add(user);
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _groupService.GetGroupByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Group", result.Name);
        }

        [Fact]
        public async Task GetGroupByIdAsync_WhenInvalid_ReturnsNull()
        {
            // Act
            var result = await _groupService.GetGroupByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateGroupAsync_CreatesGroupWithCreatorAsMember()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser("user-1");
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var dto = new CreateGroupDto
            {
                Name = "New Group",
                Description = "Test Description",
                CreatedBy = "user-1"
            };

            // Act
            var result = await _groupService.CreateGroupAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Group", result.Name);
            Assert.Equal(1, result.MemberCount);
        }

        [Fact]
        public async Task CreateGroupAsync_WithAdditionalMembers_CreatesAllMembers()
        {
            // Arrange
            var user1 = TestHelpers.CreateTestUser("user-1");
            var user2 = TestHelpers.CreateTestUser("user-2", "user2@test.com");
            _dbContext.Users.AddRange(user1, user2);
            await _dbContext.SaveChangesAsync();

            var dto = new CreateGroupDto
            {
                Name = "New Group",
                CreatedBy = "user-1",
                MemberUserIds = new List<string> { "user-2" }
            };

            // Act
            var result = await _groupService.CreateGroupAsync(dto);

            // Assert
            Assert.Equal(2, result.MemberCount);
        }

        [Fact]
        public async Task UpdateGroupAsync_WhenValid_ReturnsTrue()
        {
            // Arrange
            var group = new Group { Name = "Old Name", CreatedBy = "user-1", CreatedAt = DateTime.UtcNow };
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            var dto = new UpdateGroupDto { Name = "New Name", Description = "Updated" };

            // Act
            var result = await _groupService.UpdateGroupAsync(1, dto);

            // Assert
            Assert.True(result);
            var updated = await _dbContext.Groups.FindAsync(1);
            Assert.Equal("New Name", updated!.Name);
        }

        [Fact]
        public async Task UpdateGroupAsync_WhenInvalid_ReturnsFalse()
        {
            // Arrange
            var dto = new UpdateGroupDto { Name = "New Name" };

            // Act
            var result = await _groupService.UpdateGroupAsync(999, dto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteGroupAsync_WhenValid_ReturnsTrue()
        {
            // Arrange
            var group = new Group { Name = "Test Group", CreatedBy = "user-1", CreatedAt = DateTime.UtcNow };
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _groupService.DeleteGroupAsync(1);

            // Assert
            Assert.True(result);
            Assert.Equal(0, await _dbContext.Groups.CountAsync());
        }

        [Fact]
        public async Task DeleteGroupAsync_WhenInvalid_ReturnsFalse()
        {
            // Act
            var result = await _groupService.DeleteGroupAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetMembersAsync_ReturnsMembers()
        {
            // Arrange
            var user1 = TestHelpers.CreateTestUser("user-1");
            var user2 = TestHelpers.CreateTestUser("user-2", "user2@test.com");
            var group = new Group { Name = "Test Group", CreatedBy = "user-1", CreatedAt = DateTime.UtcNow };
            _dbContext.Users.AddRange(user1, user2);
            _dbContext.Groups.Add(group);
            _dbContext.GroupMembers.Add(new GroupMember { GroupId = 1, UserId = "user-1" });
            _dbContext.GroupMembers.Add(new GroupMember { GroupId = 1, UserId = "user-2" });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _groupService.GetMembersAsync(1);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task AddMemberAsync_WhenValid_ReturnsSuccess()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser("user-1");
            var group = new Group { Name = "Test Group", CreatedBy = "user-1", CreatedAt = DateTime.UtcNow };
            _dbContext.Users.Add(user);
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _groupService.AddMemberAsync(1, "user-1");

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task AddMemberAsync_WhenGroupNotFound_ReturnsFailure()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser("user-1");
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _groupService.AddMemberAsync(999, "user-1");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Group not found.", result.Error);
        }

        [Fact]
        public async Task AddMemberAsync_WhenUserNotFound_ReturnsFailure()
        {
            // Arrange
            var group = new Group { Name = "Test Group", CreatedBy = "user-1", CreatedAt = DateTime.UtcNow };
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _groupService.AddMemberAsync(1, "invalid-user");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("User not found.", result.Error);
        }

        [Fact]
        public async Task AddMemberAsync_WhenAlreadyMember_ReturnsFailure()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser("user-1");
            var group = new Group { Name = "Test Group", CreatedBy = "user-1", CreatedAt = DateTime.UtcNow };
            _dbContext.Users.Add(user);
            _dbContext.Groups.Add(group);
            _dbContext.GroupMembers.Add(new GroupMember { GroupId = 1, UserId = "user-1" });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _groupService.AddMemberAsync(1, "user-1");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("User is already a member of this group.", result.Error);
        }

        [Fact]
        public async Task RemoveMemberAsync_WhenValid_ReturnsTrue()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser("user-1");
            var group = new Group { Name = "Test Group", CreatedBy = "user-1", CreatedAt = DateTime.UtcNow };
            _dbContext.Users.Add(user);
            _dbContext.Groups.Add(group);
            _dbContext.GroupMembers.Add(new GroupMember { GroupId = 1, UserId = "user-1" });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _groupService.RemoveMemberAsync(1, "user-1");

            // Assert
            Assert.True(result);
            Assert.Equal(0, await _dbContext.GroupMembers.CountAsync());
        }

        [Fact]
        public async Task RemoveMemberAsync_WhenInvalid_ReturnsFalse()
        {
            // Act
            var result = await _groupService.RemoveMemberAsync(1, "invalid-user");

            // Assert
            Assert.False(result);
        }
    }
}
