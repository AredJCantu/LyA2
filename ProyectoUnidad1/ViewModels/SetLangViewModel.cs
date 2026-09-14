using CommunityToolkit.Mvvm.Input;
using ProyectoUnidad1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace ProyectoUnidad1.ViewModels
{
    public enum DataType
    {
        set,
        elem,
        boolean
    }
    public class SetLangViewModel:INotifyPropertyChanged
    {

        public string? RawText { get; set; }
        public ICommand AnalyzeCommand { get; set; }
        public ObservableCollection<ErroresModel> ErrorList { get; set; } = new ObservableCollection<ErroresModel>();
        private readonly Dictionary<string, SimboloModel> tablaSimbolos = new Dictionary<string, SimboloModel>();
        List<int> breakLines = new List<int>();

        public event PropertyChangedEventHandler? PropertyChanged;
        public SetLangViewModel()
        {
            AnalyzeCommand = new RelayCommand<string>(AnalyzeCode);
        }

        public IReadOnlyDictionary<string, SimboloModel> TablaSimbolos => tablaSimbolos;

        // parte 1 2 y 3
        // 2 tabla de simbolos
        public bool ExisteVariable(string name) { return tablaSimbolos.ContainsKey(name); }
        public SimboloModel ObtenerVariable(string name)
        {
            tablaSimbolos.TryGetValue(name, out var simbolo);
            return simbolo;
        }
        // identificar declaracion + construir la tabla
        // el parser debe llamar este metodo cada que se encuentre una declaracion
        public bool DeclararVariable(string name, DataType type, int line, bool inicializada = false)
        {
            if (ExisteVariable(name))
            {
                int lineaOriginal = tablaSimbolos[name].Line;
                AddError("Declaración Duplicada", $"La variable '{name}' ya había sido declarada antes (línea {lineaOriginal}).", line);
                return false;
            }
            tablaSimbolos[name] = new SimboloModel(name, type, line, inicializada);
            return true;
        }
        public void MarcarInicializada(string nombre)
        {
            if (tablaSimbolos.TryGetValue(nombre, out var simbolo))
            {
                simbolo.Inicializada = true;
            }
        }// 3 verifica que una variable ya exista en la tabla antes de usarse
        public bool ValidarVariableDeclarada(string nombre, int line)
        {
            if (!ExisteVariable(nombre))
            {
                AddError("Variable No Declarada", $"La variable '{nombre}' se utiliza sin haber sido declarada antes.", line);
                return false;
            }
            return true;
        }// igual que ValidarVariableDeclarada pero para revisar varios identificadores de una
        public void ValidarUsoDeVariables(IEnumerable<string> identificadores, int line)
        {
            foreach (var nombre in identificadores)
            {
                ValidarVariableDeclarada(nombre, line);
            }
        }// a partir del tipo de dato ya guardado en la tabla obtiene el DataType de una variable
        public DataType? ObtenerTipoVariable(string nombre)
        {
            if (tablaSimbolos.TryGetValue(nombre, out var simbolo))
            {
                return (DataType?)simbolo.Type;
            }
            return null;
        }
        // Esto es la parte 7,8 y 9
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

        //Leer datos de la view
        private void AnalyzeCode(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                AddError("Entrada Vacía", "No se proporcionó ningún código para analizar.", 0);
                return;
            }
            var statementMatches = Regex.Matches(code, @";");
            int startIndex = 0;

            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == '\n') breakLines.Add(i);
            }


            foreach (Match item in statementMatches)
            {
                int indexMatch = item.Index;
                string subcadena = code.Substring(startIndex, indexMatch - startIndex).Trim();
                int actualLine = breakLines.Count(x => x < indexMatch) + 1;
                ProcessingTokens(subcadena, actualLine);
                startIndex = item.Index + 1;
                Console.WriteLine(subcadena);
            }

        }
        void ProcessingTokens(string token, int line)
        {
            var regexTokens = @"^\s*(set|elem|bool)\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*=\s*(.*)$";
            var match = Regex.Match(token, regexTokens);
            if (match.Success)
            {
                string tipo = match.Groups[1].Value.ToLower();
                string nombre = match.Groups[2].Value;
                string value = match.Groups[3].Value.Trim();
                Console.WriteLine(tipo);
                Console.WriteLine(nombre);
                Console.WriteLine(value);
                DataType type = tipo == "set" ? DataType.set : tipo == "elem" ? DataType.elem : DataType.boolean;
                //Validar duplicados
                if (DeclararVariable(nombre, type, line, true)){
                    //Validar operacion
                    string regexOperation = @"^(.*)(U,^,//,IN,SUBSET)(.*)$";
                    var valueMatch = Regex.Match(value, regexOperation);
                    if (valueMatch.Success)
                    {
                        string strLeft=valueMatch.Groups[1].Value.Trim();
                        string op = valueMatch.Groups[2].Value.Trim();
                        string strRight = valueMatch.Groups[3].Value.Trim();
                        DataType left=strLeft
                        ValidarOperadores(op, left, right,line);
                    }
                    else
                    {
                        
                    }
                }
            }
            else
            {

            }

        }

    }
}
