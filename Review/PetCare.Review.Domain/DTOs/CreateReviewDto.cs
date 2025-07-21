namespace PetCare.Review.Application.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CreateReviewDto
    {
        [Required]
        public Guid ReservationId { get; set; }

        [Required]
        public Guid AuthorId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "El puntaje debe estar entre 1 y 5.")]
        public int Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "El comentario no debe exceder 1000 caracteres.")]
        public string Comment { get; set; } = string.Empty;
    }
}
