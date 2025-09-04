using System;

namespace Domain.Entities
{
    /// <summary>
    /// Representa a permissão explícita de um usuário (Sharer)
    /// para que outro usuário (Observer) possa ver sua localização.
    /// </summary>
    public class LocationShare
    {
        public Guid Id { get; set; }

        /// <summary>
        /// O ID do usuário que ESTÁ COMPARTILHANDO sua localização.
        /// </summary>
        public Guid SharerId { get; set; }
        public virtual User Sharer { get; set; }

        /// <summary>
        /// O ID do usuário que PODE VER a localização.
        /// </summary>
        public Guid ObserverId { get; set; }
        public virtual User Observer { get; set; }

        /// <summary>
        /// Indica se o compartilhamento está atualmente ativo.
        /// </summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}