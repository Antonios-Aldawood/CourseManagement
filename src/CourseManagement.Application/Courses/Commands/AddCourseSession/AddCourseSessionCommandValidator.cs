using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.Courses.Commands.Validator;

namespace CourseManagement.Application.Courses.Commands.AddCourseSession
{
    public class AddCourseSessionCommandValidator : AbstractValidator<AddCourseSessionCommand>
    {
        public AddCourseSessionCommandValidator()
        {
            SessionValidatorRules.ApplySessionRules(
                validator: this,
                getName: x => x.sessionName,
                getStartDate: x => x.startDate,
                getEndDate: x => x.endDate,
                getSeats: x => x.seats,
                getApp: x => x.app);
        }
    }
}
