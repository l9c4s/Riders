using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Locations;

public class UpdateLocationDto
{
    [Required(ErrorMessage = "Latitude is required.")]
    public double Latitude { get; set; }

    [Required(ErrorMessage = "Longitude is required.")]
    public double Longitude { get; set; }
}

