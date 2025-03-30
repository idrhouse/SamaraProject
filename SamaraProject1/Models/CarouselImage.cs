using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace SamaraProject1.Models
{
    public class CarouselImage
    {
        [Key]
        public int IdCarouselImage { get; set; }

        [Required]
        [StringLength(255)]
        public string Url { get; set; }

        [StringLength(255)]
        public string Alt { get; set; }

        public int Order { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}