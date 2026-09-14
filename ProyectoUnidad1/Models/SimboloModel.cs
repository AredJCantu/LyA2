using System;
using System.Collections.Generic;
using System.Text;
using ProyectoUnidad1.ViewModels;

namespace ProyectoUnidad1.Models
{
    public class SimboloModel
    {
        public string Name { get; set; }
        public DataType Type { get; set; }
        public int Line { get; set; }
        public bool Inicializada { get; set; }
        public SimboloModel(string name, DataType type, int line, bool inicializada = false)
        {
            Name = name;

            Type = type;

            Line = line;

            Inicializada = inicializada;

        }
    }
}
