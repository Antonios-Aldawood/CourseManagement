using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Domain.Users;
using CourseManagement.Application.Users.Common.Dto;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface IUsersRepository
    {
        Task AddUserAsync(User user);
        Task<UserCreatePreRequirementsDto?> GetUserCreatePreRequirementsAsync(string roleType, string title, string city, string region, string road); 
        Task UpdateUserRefreshTokenAsync(int userId, string updatedRefreshToken, DateTime updatedRefreshTokenExpiryTime);
        Task<User?> GetUserByRefreshTokenHashAsync(string hashedToken);
        Task<bool> ExistsAsync(int userId);
        Task<bool> AliasOrEmailExistsAsync(string alias, string email);
        Task<bool> CheckForDuplicateAliasOrEmailAsync(string alias, string email);
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByExactNameAsync(string alias);
        Task<UserFullDto?> GetFullByEmailAsync(string email);
        Task<List<UserDto>> GetByNameAsync(string alias);
        Task<List<string>> GetAllUsersPositionAsync();
        Task<List<string>> GetUpperAliasesByUserDepartmentAsync(string jobTitle);
        Task<List<UserDto>> FilterByPositionAsync(string position);
        Task<List<UserDto>> FilterByRoleAsync(string roleType);
        Task<List<UserDto>> FilterByJobAsync(string jobTitle);
        Task<UserPosJobDepartDto?> GetUserWithJobAndDepartmentByIdAsync(int userId);
        Task<List<UserPosJobDepartCoursesDto>> GetUserWithPosJobDepartmentCoursesAndCourseSessionsByIdAsync(int userId);
        Task<UserJobAndDepartmentDto?> GetUpper1WithPropertiesAsync(string upper1Alias, string departmentName);
        Task<List<UserDto>> FilterByUpper1Async(string upper1Alias);
        Task<List<UserDto>> FilterByUpper2Async(string upper2Alias);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<(List<UserDto> items, int totalCount)> GetAllUsersPaginatedAsync(int pageNumber, int pageSize);
        Task<bool> AddressExistsAsync(int addressId);
        Task<Address?> GetExactAddressAsync(string city, string region, string road);
        Task<List<Address>> GetAllAddressesAsync();
        Task<List<UserJobSkillsDto>> GetUserWithJobAndJobSkillsAsync(string alias);
        Task<bool> UsersHaveUppersInDepartmentAsync(int departmentId, int oldJobId);
        Task<bool> CheckIfTrainerExistsByIdAsync(int userId);
        Task<User?> GetTrainerByIdAsync(int userId);
        Task<List<UserShortDto>> GetAllTrainersAsync();
    }
}
