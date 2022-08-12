using System.ComponentModel.DataAnnotations;

namespace eryaz.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}