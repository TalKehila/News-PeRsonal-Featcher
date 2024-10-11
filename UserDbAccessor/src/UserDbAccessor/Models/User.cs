

namespace UserDbAccessor.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Preferences { get; set; }
        public string CommunicationChannel { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}, FullName: {FullName}, Email: {Email}, Phone: {Phone}, Preferences: {Preferences}, CommunicationChannel: {CommunicationChannel}";
        }
    }
}
