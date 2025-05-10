using System.ComponentModel.DataAnnotations;

namespace RateMe.Models.DtoModels
{
    class User
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public required string Email { get; set; }
        [MaxLength(200)]
        public required string Password { get; set; }
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(200)]
        public string Surname { get; set; } = string.Empty;
    }
}
