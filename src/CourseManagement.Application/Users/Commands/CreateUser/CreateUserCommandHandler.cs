using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Domain.Users;
using CourseManagement.Domain.Roles;
using Microsoft.AspNetCore.Identity;
using CourseManagement.Application.Users.Common.Token;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler(
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        IJobsRepository jobsRepository,
        IEmailService emailService,
        IRefreshTokensRepository refreshTokensRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<CreateUserCommand, ErrorOr<UserShortDto>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IRolesRepository _rolesRepository = rolesRepository;
        private readonly IJobsRepository _jobsRepository = jobsRepository;
        private readonly IEmailService _emailService = emailService;
        private readonly IRefreshTokensRepository _refreshTokensRepository = refreshTokensRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ErrorOr<UserShortDto>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                //Checking if alias or email is already taken.
                if (await _usersRepository.AliasOrEmailExistsAsync(command.alias, command.email) == true)
                {
                    return Error.Validation(description: "Alias or email already taken.");
                }

                //Checking if address exists, or a new one needs to be created in User's domain.
                Address? existingAddress = null;
                if (await _usersRepository.GetExactAddressAsync(command.city, command.region, command.road) is Address address)
                {
                    existingAddress = address;
                }

                //Checking if role exists.
                if (await _rolesRepository.GetByExactNameAsync(command.roleType) is not Role role)
                {
                    return Error.Validation(description: "Role not found.");
                }

                //Checking if job exists. And assigning its ID to user.
                JobDto? foundJob = await _jobsRepository.GetJobByExactNameDtoAsync(command.title);
                if (foundJob is not JobDto jobDto)
                {
                    return Error.Validation(description: "Job not found.");
                }
                int jobId = foundJob.JobId;

                //Checking if agreed salary is in bounds of user's job's salary bounds.
                //Done here because it's a cross aggregate concern.
                if (command.agreedSalary < foundJob.MinSalary || command.agreedSalary > foundJob.MaxSalary)
                {
                    return Error.Validation(description: "User salary out of job's bounds.");
                }

                //Checking if upper exists, and if they are in the same department as the user.
                var upper1 = await _usersRepository.GetUpper1WithPropertiesAsync(command.upper, foundJob.Department);
                if (upper1 == null || foundJob.Department != upper1.Department)
                {
                    return Error.Validation(description: "First upper not found or is from another department.");
                }

                upper1.AffectedId = upper1.User.Id;

                //Hashing password, generating verification code, and refresh token with its expiry date.
                var passwordHash = new PasswordHasher<CreateUserCommand>()
                    .HashPassword(command, command.password);

                var verificationCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

                Token token = new();
                /*
                var refreshToken = token.UserRefreshToken();

                if (refreshToken.IsError)
                {
                    return refreshToken.Errors;
                }
                */
                var (rawRefreshToken, hashRefreshToken) = token.GenerateRefreshTokenAndHash();

                //Actually creating the damn user.
                var user = User.AddUser(
                    alias: command.alias,
                    email: command.email,
                    phoneNumber: command.phoneNumber,
                    passwordHash: passwordHash,
                    position: command.position,
                    refreshToken: hashRefreshToken,
                    refreshTokenExpiryTime: DateTime.UtcNow.AddDays(10),
                    isVerified: false,
                    verificationCode: verificationCode,
                    address: existingAddress ?? null,
                    city: command.city,
                    region: command.region,
                    road: command.road,
                    roleId: role.Id,
                    jobId: jobId,
                    agreedSalary: command.agreedSalary,
                    upper1: upper1.User,
                    upper2: upper1.Upper1);

                if (user.IsError)
                {
                    return user.Errors;
                }

                rawRefreshToken = "";

                await _usersRepository.AddUserAsync(user.Value);

                var email = await _emailService.SendVerificationEmailAsync(user.Value.Email, user.Value.Alias, command.password, verificationCode);

                if (email != Result.Success)
                {
                    return email.Errors;
                }

                await _unitOfWork.CommitChangesAsync();

                RefreshToken refreshToken = new RefreshToken(
                    userId: user.Value.Id,
                    hashedRefreshToken: hashRefreshToken,
                    expiresAt: DateTimeOffset.UtcNow.AddDays(10));

                await _refreshTokensRepository.AddAsync(refreshToken);

                await _unitOfWork.CommitChangesAsync();

                UserShortDto createdUser = new UserShortDto
                {
                    Id = user.Value.Id,
                    Alias = user.Value.Alias
                };

                createdUser.AffectedId = user.Value.Id;

                return createdUser;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

/*
    public async Task<ErrorOr<UserShortDto>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                //Checking if Job, Role, and Address exist, in one single DB roundtrip.
                //Checking for address, to process if it needs to be created or assigned to the user.
                var preRequirements =
                await _usersRepository.GetUserCreatePreRequirementsAsync(
                    roleType: command.roleType,
                    title: command.title,
                    city: command.city,
                    region: command.region,
                    road: command.road);

                if (preRequirements is null)
                {
                    return Error.Validation(description: "No role or job found.");
                }

                Address? existingAddress = null;
                if (preRequirements.AddressId != 0)
                {
                    existingAddress = preRequirements.Address;
                }

                if (preRequirements.Role is null)
                {
                    return Error.Validation(description: "Role not found.");
                }
            
                if (preRequirements.Job is null)
                {
                    return Error.Validation(description: "Job not found.");
                }

                //Checking if agreed salary is in bounds of user's job's salary bounds.
                //Done here because it's a cross aggregate concern.
                if (command.agreedSalary < preRequirements.Job.MinSalary ||
                    command.agreedSalary > preRequirements.Job.MaxSalary)
                {
                    return Error.Validation(description: "User salary out of job's bounds.");
                }

                //Checking if upper exists, and if they are in the same department as the user.
                var upper1 = await _usersRepository.GetUpper1WithProperties(command.upper, preRequirements.DepartmentName!);
                if (upper1 == null || preRequirements.DepartmentName != upper1.Department)
                {
                    return Error.Validation(description: "First upper not found or is from another department.");
                }

                upper1.AffectedId = upper1.User.Id;

                //Hashing password, generating verification code, and refresh token with its expiry date.
                var passwordHash = new PasswordHasher<CreateUserCommand>()
                    .HashPassword(command, command.password);

                var verificationCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

                Token token = new();
                var refreshToken = token.UserRefreshToken();

                if (refreshToken.IsError)
                {
                    return refreshToken.Errors;
                }

                //Actually creating the damn user.
                var user = User.AddUser(
                    alias: command.alias,
                    email: command.email,
                    phoneNumber: command.phoneNumber,
                    passwordHash: passwordHash,
                    position: command.position,
                    refreshToken: refreshToken.Value,
                    refreshTokenExpiryTime: DateTime.UtcNow.AddDays(2),
                    isVerified: false,
                    verificationCode: verificationCode,
                    address: existingAddress ?? null,
                    city: command.city,
                    region: command.region,
                    road: command.road,
                    roleId: preRequirements.RoleId,
                    jobId: preRequirements.JobId,
                    agreedSalary: command.agreedSalary,
                    upper1: upper1.User,
                    upper2: upper1.Upper1);

                if (user.IsError)
                {
                    return user.Errors;
                }

                if (await _usersRepository.AddUserAsync(user.Value) == false)
                {
                    return Error.Validation(description: "Alias or email already taken.");
                }

                var email = await _emailService.SendVerificationEmailAsync(user.Value.Email, user.Value.Alias, command.password, verificationCode);

                if (email != Result.Success)
                {
                    return email.Errors;
                }

                await _unitOfWork.CommitChangesAsync();

                UserShortDto createdUser = new UserShortDto
                {
                    Id = user.Value.Id,
                    Alias = user.Value.Alias
                };

                createdUser.AffectedId = user.Value.Id;

                return createdUser;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        } 
*/