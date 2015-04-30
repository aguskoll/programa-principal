using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

using OpenTK;
using CGUNS.Doubly_Conected_Edge_List;
namespace CGUNS
{
    public class ObjFileParser
    {
        #region ATRIBUTOS

        private const String VERTEX = "v";
       
        private const String FACE = "f";
        private static String[] OBJ = { "o", "g" };
        private const Char COMMENT = '#';
        private static Char[] SEPARATOR = { ' ' };
        private static Char[] FACE_SEPARATOR = { '/' };
        
        #endregion

        public ObjFileParser(DoublyConectedEdgeList mesh, String fileName)
        {
            String line;
            String[] lineSplit;
            StreamReader file = new StreamReader(fileName);
            line = file.ReadLine();
            while (line != null)
            {
                line = line.Trim(); //Saco espacios en blanco.
                if ((line.Length != 0) && (!line[0].Equals(COMMENT)))
                { //Si no es comentario
                    line = line.Replace('.', ',');
                    lineSplit = line.Split(SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                    if (lineSplit[0].Equals(VERTEX) )
                    {
                        //  System.Console.WriteLine("vertices:/n ");
                        //    mostrarContenido(lineSplit);
                        CrearVertice(mesh, lineSplit);
                    }
                    else
                        if (lineSplit[0].Equals(FACE))
                        {
                            CrearCara(mesh, lineSplit);
                            //   System.Console.WriteLine("caras :/n ");
                            //  mostrarContenido(lineSplit);
                        }
                }

                line = file.ReadLine();
            }
            file.Close();
            Console.WriteLine("Estoy en el parser ");
            List<Vertex> vertices = mesh.getVertices();
            for (int i = 0; i < vertices.Count; i++)
            {
                Console.WriteLine("vertice  " + i + vertices[i].getCoordenadas().ToString());
            }
        }

        private void CrearVertice(DoublyConectedEdgeList mesh, string[] lineSplit)
        {
            Vector3 vertice = new Vector3();
            vertice[0] = float.Parse(lineSplit[1], NumberStyles.Number);
            vertice[1] = float.Parse(lineSplit[2], NumberStyles.Number);
            vertice[2] = float.Parse(lineSplit[3], NumberStyles.Number);
            mesh.CrearVertice(vertice);
            //Para control
                Console.Write("vertice creado: ");
               Console.WriteLine(vertice);
        }

      
        private void CrearCara(DoublyConectedEdgeList mesh, string[] lineSplit)
        {
            List<int> vertices = new List<int>();
       
            for (int j = 1; j < lineSplit.Length; j++)
            {
                vertices.Add(int.Parse(lineSplit[j]) - 1);
            }
          //  Console.WriteLine(" ya cree los vertices ahora los uno");
            int cara = mesh.crearCara(vertices);
            
            //prueba
            //creo edge
            int e01 = mesh.crearLado(vertices[0], vertices[1]);
            mesh.getLados()[0].setCaraIncidente(cara);
            int v0 = vertices[0];
            mesh.getVertices()[v0].setEdgeIncidente(e01);
            mesh.getCaras()[cara].setHalfEdgeIncidente(e01);
            int edgeInicial = e01;
            int n = vertices.Count;
         //   Console.WriteLine("creando cara {0}", cara);
            int i = 1;
            for ( i = 1; i < n; i++)       //Se unen  las aristas para formar la cara.
            {
                int e12 = mesh.crearLado(vertices[i], vertices[(i + 1) % n]);

                mesh.getLados()[e12].setCaraIncidente(cara);
                mesh.getVertices()[vertices[i]].setEdgeIncidente(e12);
                mesh.getLados()[e01].setEdgeSiguiente(e12);
                mesh.getLados()[e12].setEdgePrevio(e01);
                e01 = e12;
                //Console.WriteLine("Aca llego2");
            }
          //  Console.WriteLine("salio del while");
            mesh.getLados()[e01].setEdgeSiguiente(edgeInicial);
            //Console.WriteLine("setio previo");
            mesh.getLados()[edgeInicial].setEdgePrevio(e01);
        }

        public void mostrarContenido(String[] lineSplit)
        {
           Console.WriteLine("Cant elementos arrelo:");
            Console.WriteLine(lineSplit.Length);
            for (int i = 0; i < lineSplit.Length; i++)
            {
                string element = lineSplit[i];
                Console.Write(element);
                Console.Write(" ");
            }
            Console.WriteLine();
        }
    }
}
