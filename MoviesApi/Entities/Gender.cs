﻿using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Entities;

public class Gender
{
    public int Id { get; set; }
    [Required] [StringLength(40)] public string Name { get; set; }
}