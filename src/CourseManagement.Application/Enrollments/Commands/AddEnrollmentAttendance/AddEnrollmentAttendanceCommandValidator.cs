using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.Enrollments.Commands.Validator;

namespace CourseManagement.Application.Enrollments.Commands.AddEnrollmentAttendance
{
    public class AddEnrollmentAttendanceCommandValidator : AbstractValidator<AddEnrollmentAttendanceCommand>
    {
        public AddEnrollmentAttendanceCommandValidator()
        {
            AttendanceValidator.ApplyAttendanceRules(
                validator: this,
                getDateAttended: x => x.dateAttended);
        }
    }
}
