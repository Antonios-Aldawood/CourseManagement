using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Infrastructure.Common.Persistence;
using CourseManagement.Domain.Users;
using CourseManagement.Application.Users.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Infrastructure.Users.Persistence
{
    public class UsersRepository : IUsersRepository
    {
        private readonly CourseManagementDbContext _dbContext;

        public UsersRepository(CourseManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddUserAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }

        public async Task<UserCreatePreRequirementsDto?> GetUserCreatePreRequirementsAsync(
            string roleType,
            string title,
            string city,
            string region,
            string road)
        {
            return await (from user in _dbContext.Users
                          join address in _dbContext.Addresses
                          on user.AddressId equals address.Id
                          join role in _dbContext.Roles
                          on user.RoleId equals role.Id
                          join job in _dbContext.Jobs
                          on user.JobId equals job.Id
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where
                          (address.City == city && address.Region == region && address.Road == road) ||
                          role.RoleType == roleType ||
                          job.Title == title
                          select new UserCreatePreRequirementsDto
                          {
                              Id = user.Id == 0 ? 0 : user.Id,
                              Alias = user.Alias ?? null,
                              JobId = job.Id,
                              Job = job,
                              RoleId = role.Id,
                              Role = role,
                              AddressId = address.Id,
                              Address = address,
                              DepartmentName = dept.Name == null ? null : dept.Name,
                          }).FirstOrDefaultAsync();
        }

        public async Task UpdateUserRefreshTokenAsync(int userId, string updatedRefreshToken, DateTime updatedRefreshTokenExpiryTime)
        {
            var updated = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            updated!.RefreshToken = updatedRefreshToken;
            updated!.RefreshTokenExpiryTime = updatedRefreshTokenExpiryTime;
        }

        public async Task<User?> GetUserByRefreshTokenHashAsync(string hashedToken)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(u => u.RefreshToken == hashedToken);
        }

        public async Task<bool> ExistsAsync(int userId)
        {
            return await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> AliasOrEmailExistsAsync(string alias, string email)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(u => u.Alias == alias || u.Email == email);
        }

        public async Task<bool> CheckForDuplicateAliasOrEmailAsync(string alias, string email)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(u => u.Alias == alias || u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _dbContext.Users
                .Include(u => u.Address)
                .Include(u => u.Upper1)
                .Include(u => u.Upper2)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users
                .Include(u => u.Address)
                .Include(u => u.Upper1)
                .Include(u => u.Upper2)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByExactNameAsync(string alias)
        {
            return await _dbContext.Users
                .Include(u => u.Address)
                .Include(u => u.Upper1)
                .Include(u => u.Upper2)
                .FirstOrDefaultAsync(u => u.Alias == alias);
        }

        public async Task<UserFullDto?> GetFullByEmailAsync(string email)
        {
            return await (from user in _dbContext.Users
                          join upper1 in _dbContext.Users
                          on user.Upper1Id equals upper1.Id
                          join upper2 in _dbContext.Users
                          on user.Upper2Id equals upper2.Id
                          join address in _dbContext.Addresses
                          on user.AddressId equals address.Id
                          join role in _dbContext.Roles
                          on user.RoleId equals role.Id
                          join job in _dbContext.Jobs
                          on user.JobId equals job.Id
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where user.Email == email
                          select new UserFullDto
                          {
                              Id = user.Id,
                              Alias = user.Alias,
                              Email = user.Email,
                              PasswordHash = user.PasswordHash,
                              PhoneNumber = user.PhoneNumber,
                              Position = user.Position,
                              City = address.City,
                              Region = address.Region,
                              Road = address.Road,
                              RoleType = role.RoleType,
                              JobTitle = job.Title,
                              JobDescription = job.Description,
                              DepartmentName = dept.Name,
                              Upper1 = upper1.Alias,
                              Upper2 = upper2.Alias,
                              RefreshToken = user.RefreshToken,
                              IsVerified = user.IsVerified
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<UserDto>> GetByNameAsync(string alias)
        {
            return await (from user in _dbContext.Users
                          join upper1 in _dbContext.Users
                          on user.Upper1Id equals upper1.Id
                          join upper2 in _dbContext.Users
                          on user.Upper2Id equals upper2.Id
                          join address in _dbContext.Addresses
                          on user.AddressId equals address.Id
                          join role in _dbContext.Roles
                          on user.RoleId equals role.Id
                          join job in _dbContext.Jobs
                          on user.JobId equals job.Id
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where user.Alias.ToLower().Contains(alias.ToLower())
                          select new UserDto
                          {
                              UserId = user.Id,
                              Alias = user.Alias,
                              Email = user.Email,
                              PhoneNumber = user.PhoneNumber,
                              Position = user.Position,
                              City = address.City,
                              Region = address.Region,
                              Road = address.Road,
                              Role = role.RoleType,
                              JobTitle = job.Title,
                              JobDescription = job.Description,
                              DepartmentName = dept.Name,
                              Upper1Alias = upper1.Alias,
                              Upper2Alias = upper2.Alias,
                              IsVerified = user.IsVerified
                          }).ToListAsync();
        }

        public async Task<List<string>> GetAllUsersPositionAsync()
        {
            return await _dbContext.Users.Select(u => u.Position).Distinct().ToListAsync<string>();
        }

        public async Task<List<string>> GetUpperAliasesByUserDepartmentAsync(string jobTitle)
        {
            return await (
                from user in _dbContext.Users
                join job in _dbContext.Jobs
                on user.JobId equals job.Id
                where job.DepartmentId == (
                    from j in _dbContext.Jobs
                    where j.Title == jobTitle
                    select j.DepartmentId
                    ).FirstOrDefault()
                select user.Alias
                ).ToListAsync();
        }

        public async Task<List<UserDto>> FilterByPositionAsync(string position)
        {
            return await (from user in _dbContext.Users
                          join upper1 in _dbContext.Users
                          on user.Upper1Id equals upper1.Id
                          join upper2 in _dbContext.Users
                          on user.Upper2Id equals upper2.Id
                          join address in _dbContext.Addresses
                          on user.AddressId equals address.Id
                          join role in _dbContext.Roles
                          on user.RoleId equals role.Id
                          join job in _dbContext.Jobs
                          on user.JobId equals job.Id
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where user.Position.ToLower().Contains(position.ToLower())
                          select new UserDto
                          {
                              UserId = user.Id,
                              Alias = user.Alias,
                              Email = user.Email,
                              PhoneNumber = user.PhoneNumber,
                              Position = user.Position,
                              City = address.City,
                              Region = address.Region,
                              Road = address.Road,
                              Role = role.RoleType,
                              JobTitle = job.Title,
                              JobDescription = job.Description,
                              DepartmentName = dept.Name,
                              Upper1Alias = upper1.Alias,
                              Upper2Alias = upper2.Alias,
                              IsVerified = user.IsVerified
                          }).ToListAsync();
        }

        public async Task<List<UserDto>> FilterByRoleAsync(string roleType)
        {
            return await (from user in _dbContext.Users
                          join upper1 in _dbContext.Users
                          on user.Upper1Id equals upper1.Id
                          join upper2 in _dbContext.Users
                          on user.Upper2Id equals upper2.Id
                          join address in _dbContext.Addresses
                          on user.AddressId equals address.Id
                          join role in _dbContext.Roles
                          on user.RoleId equals role.Id
                          join job in _dbContext.Jobs
                          on user.JobId equals job.Id
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where role.RoleType.ToLower().Contains(roleType.ToLower())
                          select new UserDto
                          {
                              UserId = user.Id,
                              Alias = user.Alias,
                              Email = user.Email,
                              PhoneNumber = user.PhoneNumber,
                              Position = user.Position,
                              City = address.City,
                              Region = address.Region,
                              Road = address.Road,
                              Role = role.RoleType,
                              JobTitle = job.Title,
                              JobDescription = job.Description,
                              DepartmentName = dept.Name,
                              Upper1Alias = upper1.Alias,
                              Upper2Alias = upper2.Alias,
                              IsVerified = user.IsVerified
                          }).ToListAsync();
        }

        public async Task<List<UserDto>> FilterByJobAsync(string jobTitle)
        {
            return await (from user in _dbContext.Users
                          join upper1 in _dbContext.Users
                          on user.Upper1Id equals upper1.Id
                          join upper2 in _dbContext.Users
                          on user.Upper2Id equals upper2.Id
                          join address in _dbContext.Addresses
                          on user.AddressId equals address.Id
                          join role in _dbContext.Roles
                          on user.RoleId equals role.Id
                          join job in _dbContext.Jobs
                          on user.JobId equals job.Id
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where job.Title.ToLower().Contains(jobTitle.ToLower())
                          select new UserDto
                          {
                              UserId = user.Id,
                              Alias = user.Alias,
                              Email = user.Email,
                              PhoneNumber = user.PhoneNumber,
                              Position = user.Position,
                              City = address.City,
                              Region = address.Region,
                              Road = address.Road,
                              Role = role.RoleType,
                              JobTitle = job.Title,
                              JobDescription = job.Description,
                              DepartmentName = dept.Name,
                              Upper1Alias = upper1.Alias,
                              Upper2Alias = upper2.Alias,
                              IsVerified = user.IsVerified
                          }).ToListAsync();
        }

        public async Task<UserPosJobDepartDto?> GetUserWithJobAndDepartmentByIdAsync(int userId)
        {
            return await (from user in _dbContext.Users
                          join job in _dbContext.Jobs on user.JobId equals job.Id
                          join department in _dbContext.Departments on job.DepartmentId equals department.Id
                          where user.Id == userId
                          select new UserPosJobDepartDto
                          {
                              UserId = user.Id,
                              Alias = user.Alias,
                              Email = user.Email,
                              Position = user.Position,
                              JobId = job.Id,
                              JobTitle = job.Title,
                              DepartmentId = department.Id,
                              DepartmentName = department.Name,

                              AffectedId = user.Id
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<UserPosJobDepartCoursesDto>> GetUserWithPosJobDepartmentCoursesAndCourseSessionsByIdAsync(int userId)
        {
            return await (from user in _dbContext.Users
                          join job in _dbContext.Jobs on user.JobId equals job.Id
                          join department in _dbContext.Departments on job.DepartmentId equals department.Id
                          join enrollment in _dbContext.Enrollments on user.Id equals enrollment.UserId
                          join course in _dbContext.Courses .Include(c => c.Sessions) on enrollment.CourseId equals course.Id
                          join session in _dbContext.Sessions on  course.Id equals session.CourseId
                          where user.Id == userId
                          select new UserPosJobDepartCoursesDto
                          {
                              UserId = user.Id,
                              Alias = user.Alias,
                              Email = user.Email,
                              Position = user.Position,
                              JobId = job.Id,
                              JobTitle = job.Title,
                              DepartmentId = department.Id,
                              DepartmentName = department.Name,
                              EnrolledCourse = course,

                              AffectedId = user.Id
                          })
                          .Distinct()
                          .ToListAsync();
        }

        public async Task<UserJobAndDepartmentDto?> GetUpper1WithPropertiesAsync(string upper1Alias, string departmentName)
        {
            return await (from upper1 in _dbContext.Users
                          join upper2 in _dbContext.Users
                          on upper1.Upper1Id equals upper2.Id
                          join job in _dbContext.Jobs
                          on upper1.JobId equals job.Id
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where upper1.Alias == upper1Alias
                          where dept.Name == departmentName
                          select new UserJobAndDepartmentDto
                          {
                              User = upper1,
                              Upper1 = upper2,
                              JobId = job.Id,
                              Job = job.Title,
                              DepartmentId = dept.Id,
                              Department = dept.Name
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<UserDto>> FilterByUpper1Async(string upper1Alias)
        {
            return await (from user in _dbContext.Users
                          join upper1 in _dbContext.Users
                          on user.Upper1Id equals upper1.Id
                          join upper2 in _dbContext.Users
                          on user.Upper2Id equals upper2.Id
                          join address in _dbContext.Addresses
                          on user.AddressId equals address.Id
                          join role in _dbContext.Roles
                          on user.RoleId equals role.Id
                          join job in _dbContext.Jobs
                          on user.JobId equals job.Id
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where upper1.Alias.ToLower().Contains(upper1Alias.ToLower())
                          select new UserDto
                          {
                              UserId = user.Id,
                              Alias = user.Alias,
                              Email = user.Email,
                              PhoneNumber = user.PhoneNumber,
                              Position = user.Position,
                              City = address.City,
                              Region = address.Region,
                              Road = address.Road,
                              Role = role.RoleType,
                              JobTitle = job.Title,
                              JobDescription = job.Description,
                              DepartmentName = dept.Name,
                              Upper1Alias = upper1.Alias,
                              Upper2Alias = upper2.Alias,
                              IsVerified = user.IsVerified
                          }).ToListAsync();
        }

        public async Task<List<UserDto>> FilterByUpper2Async(string upper2Alias)
        {
            return await (from user in _dbContext.Users
                          join upper1 in _dbContext.Users
                          on user.Upper1Id equals upper1.Id
                          join upper2 in _dbContext.Users
                          on user.Upper2Id equals upper2.Id
                          join address in _dbContext.Addresses
                          on user.AddressId equals address.Id
                          join role in _dbContext.Roles
                          on user.RoleId equals role.Id
                          join job in _dbContext.Jobs
                          on user.JobId equals job.Id
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where upper2.Alias.ToLower().Contains(upper2Alias.ToLower())
                          select new UserDto
                          {
                              UserId = user.Id,
                              Alias = user.Alias,
                              Email = user.Email,
                              PhoneNumber = user.PhoneNumber,
                              Position = user.Position,
                              City = address.City,
                              Region = address.Region,
                              Road = address.Road,
                              Role = role.RoleType,
                              JobTitle = job.Title,
                              JobDescription = job.Description,
                              DepartmentName = dept.Name,
                              Upper1Alias = upper1.Alias,
                              Upper2Alias = upper2.Alias,
                              IsVerified = user.IsVerified
                          }).ToListAsync();
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await (from user in _dbContext.Users
                          join upper1 in _dbContext.Users
                          on user.Upper1Id equals upper1.Id
                          join upper2 in _dbContext.Users
                          on user.Upper2Id equals upper2.Id
                          join address in _dbContext.Addresses
                          on user.AddressId equals address.Id
                          join role in _dbContext.Roles
                          on user.RoleId equals role.Id
                          join job in _dbContext.Jobs
                          on user.JobId equals job.Id
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          select new UserDto
                          {
                              UserId = user.Id,
                              Alias = user.Alias,
                              Email = user.Email,
                              PhoneNumber = user.PhoneNumber,
                              Position = user.Position,
                              City = address.City,
                              Region = address.Region,
                              Road = address.Road,
                              Role = role.RoleType,
                              JobTitle = job.Title,
                              JobDescription = job.Description,
                              DepartmentName = dept.Name,
                              Upper1Alias = upper1.Alias,
                              Upper2Alias = upper2.Alias,
                              IsVerified = user.IsVerified
                          }).ToListAsync();
        }

        public async Task<(List<UserDto> items, int totalCount)> GetAllUsersPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = _dbContext.Users.AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await (from user in _dbContext.Users
                               join upper1 in _dbContext.Users
                               on user.Upper1Id equals upper1.Id
                               join upper2 in _dbContext.Users
                               on user.Upper2Id equals upper2.Id
                               join address in _dbContext.Addresses
                               on user.AddressId equals address.Id
                               join role in _dbContext.Roles
                               on user.RoleId equals role.Id
                               join job in _dbContext.Jobs
                               on user.JobId equals job.Id
                               join dept in _dbContext.Departments
                               on job.DepartmentId equals dept.Id
                               select new UserDto
                               {
                                   UserId = user.Id,
                                   Alias = user.Alias,
                                   Email = user.Email,
                                   PhoneNumber = user.PhoneNumber,
                                   Position = user.Position,
                                   City = address.City,
                                   Region = address.Region,
                                   Road = address.Road,
                                   Role = role.RoleType,
                                   JobTitle = job.Title,
                                   JobDescription = job.Description,
                                   DepartmentName = dept.Name,
                                   Upper1Alias = upper1.Alias,
                                   Upper2Alias = upper2.Alias,
                                   IsVerified = user.IsVerified
                               })
                               .OrderBy(u => u.Alias)
                               .Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> AddressExistsAsync(int addressId)
        {
            return await _dbContext.Addresses.AsNoTracking().AnyAsync(a => a.Id == addressId);
        }

        public async Task<Address?> GetExactAddressAsync(
            string city,
            string region,
            string road)
        {
            return await _dbContext.Addresses
                .FirstOrDefaultAsync(a =>
                    a.City == city &&
                    a.Region == region &&
                    a.Road == road);
        }

        public async Task<List<Address>> GetAllAddressesAsync()
        {
            return await _dbContext.Addresses
                .ToListAsync();
        }

        public async Task<List<UserJobSkillsDto>> GetUserWithJobAndJobSkillsAsync(string alias)
        {
            return await (from user in _dbContext.Users
                          join job in _dbContext.Jobs on user.JobId equals job.Id
                          join department in _dbContext.Departments on job.DepartmentId equals department.Id
                          join jobSkill in _dbContext.JobsSkills on job.Id equals jobSkill.JobId
                          join skill in _dbContext.Skills on jobSkill.SkillId equals skill.Id
                          where user.Alias == alias
                          select new UserJobSkillsDto
                          {
                              UserId = user.Id,
                              Alias = user.Alias,
                              Position = user.Position,
                              JobId = job.Id,
                              JobTitle = job.Title,
                              DepartmentId = department.Id,
                              DepartmentName = department.Name,
                              SkillName = skill.SkillName,
                              Weight = jobSkill.Weight,

                              AffectedId = user.Id
                          }).Distinct().ToListAsync();

            //if (!query.Any())
            //    return null;

            //var first = query.First();

            //return new UserJobSkillsDto
            //{
            //    UserId = first.UserId,
            //    Alias = first.Alias,
            //    Email = first.Email,
            //    JobTitle = first.JobTitle,
            //    DepartmentName = first.DepartmentName,
            //    JobSkillsAndWeights = query
            //        .Select(x => (x.SkillName, x.Weight))
            //        .ToList()
            //};
        }

        public async Task<bool> UsersHaveUppersInDepartmentAsync(int departmentId, int oldJobId)
        {
            return await (from user in _dbContext.Users
                          join upper in _dbContext.Users
                          on user.Upper1Id equals upper.Id
                          join job in _dbContext.Jobs
                          on user.JobId equals job.Id
                          join department in _dbContext.Departments
                          on job.DepartmentId equals department.Id
                          where departmentId == department.Id &&
                                oldJobId == job.Id &&
                                upper.JobId == job.Id &&
                                job.DepartmentId == department.Id
                          select user).AnyAsync();
        }

        public async Task<bool> CheckIfTrainerExistsByIdAsync(int userId)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.RoleId == 2)
                .AnyAsync(u => u.Id == userId);
        }

        public async Task<User?> GetTrainerByIdAsync(int userId)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.RoleId == 2 && u.Id == userId);
        }

        public async Task<List<UserShortDto>> GetAllTrainersAsync()
        {
            return await _dbContext.Users
                .Where(u => u.RoleId == 2)
                .Select(u => new UserShortDto
                {
                    Id = u.Id,
                    Alias = u.Alias,
                    AffectedId = u.Id
                })
                .ToListAsync();
        }
    }
}
