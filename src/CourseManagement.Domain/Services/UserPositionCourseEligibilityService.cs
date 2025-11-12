using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Domain.Users;

namespace CourseManagement.Domain.Services
{
    public static class UserPositionCourseEligibilityService
    {
        public static int ValidEligibilityPosition(string position)
        {
            string[] positions = User.Positions();

            for (int i = 0; i < positions.Length; i++)
            {
                if (position == positions[i])
                {
                    return i + 1;
                }
            }

            return 0;
        }
    }
}

/*
        public static int ValidEligibilityPosition(string position)
        {
            return position switch
            {
                "CEO" => 1,
                "CTO" => 2,
                "Director" => 3,
                "Manager" => 4,
                "Supervisor" => 5,
                "Specialist" => 6,
                "Intern" => 7,
                _ => 0
            };
        } 
*/