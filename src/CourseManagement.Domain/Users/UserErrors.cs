using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Users
{
    public class UserErrors
    {
        public static readonly Error UnknownPosition = Error.Validation(
            "User.UnknownPosition",
            "This user's positions isn't one of ours, please enter either 'CEO', 'Director', 'Manager', 'Supervisor', or one of the three levels 'Specialists'.");

        public static readonly Error AddressNotFound = Error.Validation(
            "Address.AddressNotFound",
            "User address not found.");

        public static readonly Error UserAlreadyHasAddress = Error.Validation(
            "User.UserAlreadyHasAddress",
            "User already has been given this address.");

        public static readonly Error UserUpper1NotFound = Error.Validation(
            "User.UserUpper1NotFound",
            "User first upper not found.");

        public static readonly Error UserUpper1Invalid = Error.Failure(
            "User.UserUpper1Invalid",
            "User first upper invalid.");

        public static readonly Error UserAlreadyHasUpper1 = Error.Validation(
            "User.UserAlreadyHasUpper1",
            "User already is under this first upper.");

        public static readonly Error UserUpper2NotFound = Error.Validation(
            "User.UserUpper2NotFound",
            "User second upper not found.");

        public static readonly Error UserUpper2Invalid = Error.Failure(
            "User.UserUpper2Invalid",
            "User second upper invalid.");

        public static readonly Error UserAlreadyHasUpper2 = Error.Validation(
            "User.UserAlreadyHasUpper2",
            "User already is under this second upper.");
    }
}
