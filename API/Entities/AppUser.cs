using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; } // collection of my photos
        public ICollection<UserLike> LikedByUsers { get; set; } // collection of users who have liked me (currently loggedIn user)
        public ICollection<UserLike> LikedUsers { get; set; }   // collection of users who i have liked
        public ICollection<Message> MessagesSend { get; set; } // colleciton of messages that i have send
        public ICollection<Message> MessagesReceived { get; set; } // collection of message that i have received
        public ICollection<AppUserRole> UserRoles { get; set; } // many - many relation with approles
    }
}