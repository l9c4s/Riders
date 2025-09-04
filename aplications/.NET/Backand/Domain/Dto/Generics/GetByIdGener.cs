using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Generics
{
    public class GetByIdGenerics
    {
        [Required]
        public string Id { get; set; }
    }
}