using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Act2_Arboles
{
    public class Node
    {
        public string Datos { get; set; }
        public Node Izq { get; set; }
        public Node Der { get; set; }

        public Node(string data)
        {
            Datos = data;
            Izq = null;
            Der = null;
        }
    }
    public class EvaluadorExpresion : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string MensajeUsuario { get; set; }
        public List<string> Elementos { get; set; } = new List<string>();
        char[] operadoresAritmeticos = new char[] { '+', '-', '*', '/', '%' };
        char[] operadoresRelacionales = new char[] { '<', '>', '=', '!' };
        char[] operadoresLogicos = new char[] { '&', '|', '!' };

        public Node Raiz { get; set; }
        public EvaluadorExpresion()
        {
            Raiz = null;
        }

        public void InsertarDato(char dato)
        {
            if (Raiz == null)
            {
                Raiz = new Node(dato);
            }
            else
            {

            }
        }
        // 2+2*4/2
        public List<string> Separar(string mensaje)
        {
            List<string> elementos = new List<string>();
            string numero = "";
            foreach(var m in mensaje)
            {
                if (char.IsDigit(m)) {
                    numero += m;//Agregamos el numero al string
                }
                else
                {
                    if (numero != "") {
                        elementos.Add(numero);
                        numero = "";
                    }
                    if(m!=' ')//Significa que es un operador o parentesis
                    {
                        elementos.Add(m.ToString());
                    }
                }
            }
            if (numero != "")
            {
                elementos.Add(numero);
                numero = "";
            }
            return elementos;
        }   
    }
}
