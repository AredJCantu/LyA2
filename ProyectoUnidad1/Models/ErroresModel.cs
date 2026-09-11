using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoUnidad1.Models
{
    public class ErroresModel
    {
        public int Number { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Line { get; set; }

        public ErroresModel(int number, string type, string description, int line)
        {
            Number = number;
            Type = type;
            Description = description;
            Line = line;
        }
    }
}
