using System;
using System.Collections.Generic;

namespace MySuperStats.Contracts.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime CreateDateTime { get; set; }

        public virtual PlayerResponse Player { get; set; }
        public virtual ICollection<MatchGroupUserResponse> MatchGroupUsers { get; set; }

    }
}
