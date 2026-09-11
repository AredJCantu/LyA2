using ProyectoUnidad1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoUnidad1.ViewModels
{
    public enum DataType
    {
        set,
        elem,
        boolean
    }
    public class SetLangViewModel
    {

        // Esto es la parte 7,8 y 9
        public ObservableCollection<ErroresModel> ErrorList { get; set; } = new ObservableCollection<ErroresModel>();

        private void AddError(string type, string description, int line)
        {
            int errorNumber = ErrorList.Count + 1;
            ErrorList.Add(new ErroresModel(errorNumber, type, description, line));
        }

        public bool ValidarOperadores(string op, DataType leftType, DataType rightType, int line)
        {
            if (op == "U" || op == "^" || op == "\\")
            {
                if (leftType != DataType.set || rightType != DataType.set)
                {
                    AddError("Tipo de Dato", $"El operador {op} necesita que ambos operandos sean conjuntos (set). Se obtuvo: {leftType} y {rightType}.", line);
                    return false;
                }
                return true;
            }
            else if (op == "IN")
            {
                bool leftValid = leftType == DataType.elem;
                bool rightValid = rightType == DataType.set;

                if (!leftValid || !rightValid)
                {
                    AddError("Tipo de Dato", $"El operador IN requiere 'elem' a la izquierda y 'set' a la derecha. Se obtuvo: {leftType} e {rightType}.", line);
                    return false;
                }
                return true;
            }
            else if (op == "SUBSET")
            {
                if (leftType != DataType.set || rightType != DataType.set)
                {
                    AddError("Tipo de Dato", $"El operador SUBSET necesita que ambos operandos sean conjuntos (set). Se obtuvo: {leftType} y {rightType}.", line);
                    return false;
                }
                return true;
            }
            else
            {
                AddError("Operador Desconocido", $"El operador '{op}' no es válido.", line);
                return false;
            }
        }

        public void ValidarSetContent(List<string> elements, int line)
        {
            foreach (var x in elements)
            {
                string s = x.Trim();

                if (!int.TryParse(s, out _))
                {
                    AddError("Tipo Incompatible", $"El elemento '{s}' dentro del set no es un entero válido.", line);
                }
            }
        }
    }
}
