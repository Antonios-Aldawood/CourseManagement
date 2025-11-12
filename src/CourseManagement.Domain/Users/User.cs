using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Users
{
    public class User
    {
        public int Id { get; init; }
        public string Alias { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int AddressId { get; set; }
        public Address? Address { get; set; }
        public int RoleId { get; set; }
        public int JobId { get; set; }
        public double AgreedSalary { get; init; }
        public int Upper1Id { get; set; }
        public User? Upper1 { get; set; }
        public int Upper2Id { get; set; }
        public User? Upper2 { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsVerified { get; set; } = false;
        public string VerificationCode { get; set; } = string.Empty;

        private User(
            string alias,
            string email,
            string passwordHash,
            string phoneNumber,
            string position,
            double agreedSalary,
            string refreshToken,
            DateTime refreshTokenExpiryTime,
            bool isVerified,
            string verificationCode,
            int roleId,
            int jobId)
        {
            Alias = alias;
            Email = email;
            PasswordHash = passwordHash;
            PhoneNumber = phoneNumber;
            Position = position;
            AgreedSalary = agreedSalary;
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = refreshTokenExpiryTime;
            IsVerified = isVerified;
            VerificationCode = verificationCode;
            RoleId = roleId;
            JobId = jobId;
        }

// Safe for most usages, including insertion or deletion.
// But only safe if when any of the following is updated, then all the old positions of the same name are also updated.
        internal static string[] Positions()
        {
            return [
                "CEO",
                "CTO",
                "Director",
                "Manager",
                "Supervisor",
                "Specialist",
                "Intern"];
        }

        private bool ValidPosition()
        {
            string[] positions = Positions();

            for (int i = 0; i < positions.Length; i++)
            {
                if (Position == positions[i])
                {
                    return true;
                }
            }

            return false;
        }

        private ErrorOr<Success> CheckIfUserIsValid()
        {
            if (!ValidPosition())
            {
                return UserErrors.UnknownPosition;
            }

            return Result.Success;
        }

        private ErrorOr<Success> AddUserAddress(
            Address? existingAddress,
            string city,
            string region,
            string road)
        {
            if (existingAddress is Address userAddress)
            {
                AddressId = userAddress.Id;
                Address = userAddress;

                return Result.Success;
            }

            var address = new Address(
                city: city,
                region: region,
                road: road);

            AddressId = address.Id;
            Address = address;

            return Result.Success;
        }

        private ErrorOr<Success> AddUserUpper1(User upper1)
        {
            if (upper1 is null)
            {
                return UserErrors.UserUpper1NotFound;
            }

            if (upper1.CheckIfUserIsValid() != Result.Success)
            {
                return UserErrors.UserUpper1Invalid;
            }

            Upper1Id = upper1.Id;
            Upper1 = upper1;

            return Result.Success;
        }

        private ErrorOr<Success> AddUserUpper2(User upper2)
        {
            if (upper2 is null)
            {
                return UserErrors.UserUpper2NotFound;
            }

            if (upper2.CheckIfUserIsValid() != Result.Success)
            {
                return UserErrors.UserUpper2Invalid;
            }

            Upper2Id = upper2.Id;
            Upper2 = upper2;

            return Result.Success;
        }

        public static ErrorOr<User> AddUser(
            string alias,
            string email,
            string passwordHash,
            string phoneNumber,
            string position,
            string refreshToken,
            DateTime refreshTokenExpiryTime,
            bool isVerified,
            string verificationCode,
            Address? address,
            string city,
            string region,
            string road,
            int roleId,
            int jobId,
            double agreedSalary,
            User upper1,
            User upper2)
        {
            var user = new User(
                alias,
                email,
                passwordHash,
                phoneNumber,
                position,
                agreedSalary,
                refreshToken,
                refreshTokenExpiryTime,
                isVerified,
                verificationCode,
                roleId,
                jobId);

            user.AddUserAddress(
                address,
                city,
                region,
                road);

            var userUpper1 = user.AddUserUpper1(upper1);

            var UserUpper2 = user.AddUserUpper2(upper2);

            if (userUpper1 != Result.Success)
            {
                return userUpper1.Errors;
            }

            if (UserUpper2 != Result.Success)
            {
                return UserUpper2.Errors;
            }

            return user;
        }
    }
}

/*
        internal static string[] Positions()
        {
            const string CEO = "CEO";
            const string CTO = "CTO";
            const string Director = "Director";
            const string Manager = "Manager";
            const string Supervisor = "Supervisor";
            const string Specialist = "Specialist";
            const string Intern = "Intern";

            string[] positions =
            [
                CEO,
                CTO,
                Director,
                Manager,
                Supervisor,
                Specialist,
                Intern,
            ];

            return positions;
        }
*/