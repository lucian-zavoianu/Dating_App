using System;

namespace DatingApp.API.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public bool IsApproved { get; set; }

        // This enables cascade delete in the case where
        // if a user will be deleted, his/her photos will
        // also be deleted and not orphaned, as if it would
        // be the case with the EF automatically generated
        // UserId (when nothing is specified and IsMain
        // would be the last property)
        public virtual User User { get; set; }
        public int UserId { get; set; }
    }
}