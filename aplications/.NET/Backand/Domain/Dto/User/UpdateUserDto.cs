using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.User;

public class UpdateUserDto
{

        [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 50 caracteres.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail em formato inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", 
            ErrorMessage = "A senha deve conter pelo menos 8 caracteres, incluindo 1 letra maiúscula, 1 letra minúscula, 1 número e 1 caractere especial.")]
        public string Password { get; set; } = string.Empty;
}
