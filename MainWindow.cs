using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK; //La matematica
using OpenTK.Graphics.OpenGL;
using gl = OpenTK.Graphics.OpenGL.GL;
using CGUNS.Shaders;
using CGUNS.Cameras;
using CGUNS.Primitives;
using CGUNS.Primitives;
using CGUNS.Doubly_Conected_Edge_List;
namespace CGUNS
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            //invento
          
        }
        private int  primeraVez;
        private ShaderProgram sProgram; //Nuestro programa de shaders.
        private Cube myCube; //Nuestro objeto a dibujar.
        private SphericalCamera myCamera;  //Camara
        private Rectangle viewport; //Viewport a utilizar (Porcion del glControl en donde voy a dibujar).
        private Ejes ejes_globales; // Ejes de referencia globales
        private Ejes ejes_locales; // Ejes de referencia laocales al cubo
         private List<Matrix4> matrices;
        private escena mundo;
      //  private ShaderProgram sProgram;

        private int transformaciones = 0;

        private void glControl3_Load(object sender, EventArgs e)
        {

            logContextInfo(); //Mostramos info de contexto.
            SetupShaders(); //Creamos los shaders y el programa de shader
            matrices = new List<Matrix4>();
            primeraVez = 1;
            myCube = new Cube(0.1f); //Construimos los objetos que voy a dibujar.
            ejes_globales = new Ejes(10.0f);
            ejes_locales = new Ejes(0.2f);
            mundo = new escena(sProgram);
            
            mundo.iniciarEscena();
            
            myCube.Build(sProgram); //Construyo los buffers OpenGL que voy a usar.
            ejes_globales.Build(sProgram);
            ejes_locales.Build(sProgram);

            myCamera = new SphericalCamera(); //Creo una camara.
           
            gl.ClearColor(Color.Black); //Configuro el Color de borrado.
            gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); //De cada poligono solo dibujo las lineas de contorno (wireframe).
        }

        private void glControl3_Paint(object sender, PaintEventArgs e)
        {
           
            Matrix4 modelMatrix = Matrix4.Identity; //Por ahora usamos la identidad.
            Matrix4 viewMatrix = myCamera.getViewMatrix();
            Matrix4 projMatrix = myCamera.getProjectionMatrix();
            Matrix4 mvMatrix = Matrix4.Mult(viewMatrix, modelMatrix);
            Vector4 figColor = new Vector4(1.0f, 1.0f, 0.0f, 1.0f); // amarillo
            Matrix4 rotacion;
            Matrix4 traslacion;
            Matrix4 mmat=Matrix4.Identity;
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); //Borramos el contenido del glControl.
            int i=0;
            gl.Viewport(viewport); //Especificamos en que parte del glControl queremos dibujar..
            //gl.Enable(EnableCap.DepthTest);
            sProgram.Activate(); //Activamos el programa de shaders
            //seteamos los valores uniformes.
            sProgram.SetUniformValue("figureColor", figColor);
            sProgram.SetUniformValue("projMat", projMatrix);
            sProgram.SetUniformValue("mvMat", mvMatrix);

            //Dibujamos los ejes de referencia.
            ejes_globales.Dibujar(sProgram);

            // Transformaciones que vamos a utilizar
            modelMatrix = Matrix4.Identity; // Guardamos la matriz identidad en la variable modelMatrix.
            rotacion = Matrix4.CreateRotationY(45.0f); // Rotacion de 45 grados respecto al eje-y.
            traslacion = Matrix4.CreateTranslation(0.5f, 0.0f, 0.0f); // Traslacion de 0.5 unidades sobre el eje-x.

            // Esto es solo un ejemplo de como podemos crear la trasalcion de manera manual.
            // CUIDAD QUE PARA UTILIZARLA HAY QUE TRASPONERLA! (Recordar lo que se explica en la clase).
            Matrix4 traslacion_a_mano = new Matrix4
               (1.0f, 0.0f, 0.0f, 0.5f, 
                0.0f, 1.0f, 0.0f, 0.0f, 
                0.0f, 0.0f, 1.0f, 0.0f, 
                0.0f, 0.0f, 0.0f, 1.0f);

            // Interacciones para aplicar las transformaciones.
          int cantMatrices=matrices.Count;
            switch (transformaciones)
            {
                case 0:
                    //No hago nada, se dibujo el cubo en su posición inicial.
                    break;

                //Probamos que pasa cuando acumulamos transformaciones de IZQUIERDA a DERECHA
                case 1:
                    
                        modelMatrix = Matrix4.Mult(rotacion, modelMatrix); //( rotacion * model )
                       for (int j = 0; j < matrices.Count; j++)
                        {
                            matrices[j] = Matrix4.Mult(rotacion, matrices[j]);
                        }
                        i = 0;
                        primeraVez = 0;
                        break;
                    
                case 2:
                    modelMatrix = Matrix4.Mult(rotacion, modelMatrix);
                    modelMatrix = Matrix4.Mult(traslacion, modelMatrix); //( traslacion * ( rotacion * model ) )
                     for (int j = 0; j < matrices.Count; j++)
                        {
                            matrices[j] = Matrix4.Mult(rotacion, matrices[j]);
                            matrices[j] = Matrix4.Mult(traslacion, matrices[j]);
                        }
                        i = 0;
                        primeraVez = 0;
                    break;

                case 3:
                    modelMatrix = Matrix4.Mult(traslacion, modelMatrix); //( traslacion * model )
                     for (int j = 0; j < matrices.Count; j++)
                        {
                            matrices[j] = Matrix4.Mult(traslacion, matrices[j]);
                            
                        }
                        i = 0;
                        primeraVez = 0;
                    break;

                case 4:
                    modelMatrix = Matrix4.Mult(traslacion, modelMatrix); 
                    modelMatrix = Matrix4.Mult(rotacion, modelMatrix); //( rotacion * ( traslacion * model ) )
                     for (int j = 0; j < matrices.Count; j++)
                        {
                            matrices[j] = Matrix4.Mult(traslacion, matrices[j]);
                            matrices[j] = Matrix4.Mult(rotacion, matrices[j]);
                            
                        }
                        i = 0;
                        primeraVez = 0;
                    break;

                //Probamos que pasa cuando acumulamos transformaciones de DERECHA a IZQUIERDA
                case 5:
                    modelMatrix = Matrix4.Mult(modelMatrix, rotacion); //( model * rotacion )
                    break;

                case 6:
                    modelMatrix = Matrix4.Mult(modelMatrix, rotacion);
                    modelMatrix = Matrix4.Mult(modelMatrix, traslacion); //( model * ( rotacion * traslacion ) )
                    break;

                case 7:
                    modelMatrix = Matrix4.Mult(modelMatrix, traslacion); //( model * traslacion )
                    break;

                case 8:
                    modelMatrix = Matrix4.Mult(modelMatrix, traslacion);
                    modelMatrix = Matrix4.Mult(modelMatrix, rotacion); //( model * ( traslacion * rotacion ) )
                    break;
           
            
            }
            List<ObjetoGrafico> lista = mundo.getObjetos();
        
            DoublyConectedEdgeList l;
           //i = 0;

            if (matrices.Count == 0)
            {
                for (int j = 0; j < lista.Count; j++)
                {
                    matrices.Add(new Matrix4());
                    matrices[j] = Matrix4.Identity;
                }
                primeraVez = 1;
            }
            else primeraVez = 0;
          
                foreach (ObjetoGrafico o in lista)
                {
                    l = o.getEstructura();
                    Vector3 v = l.getTraslacion();
                    Vector3 r = l.getRotacion();
                    traslacion = Matrix4.CreateTranslation(v.X, v.Y, v.Z);

                    //  modelMatrix = Matrix4.Identity; 
                    Matrix4 rotacionx = Matrix4.Identity;
                    Matrix4 rotaciony = Matrix4.Identity;
                    Matrix4 rotacionz = Matrix4.Identity;

                    if (r.X != 0f)
                    {
                        Console.WriteLine(" x" + r.X);

                        //  rotacionx = Matrix4.CreateRotationX((float)((r[0] * Math.PI) / (180)));
                        rotacionx = Matrix4.CreateRotationX(((float)r[0] * (float)Math.PI) / (180));
                        Console.WriteLine("larotacion es: en x");
                        Console.WriteLine(rotacionx.ToString());
                    }
                    if (r.Y != 0f)
                    {
                        Console.WriteLine(" y" + r.Y);
                        rotaciony = Matrix4.CreateRotationY((float)((float)(r[1] * Math.PI) / (180)));
                        Console.WriteLine("la rotacion es: en y");
                        Console.WriteLine(rotaciony.ToString());
                    }
                    if (r.Z != 0f)
                    {
                        Console.WriteLine(" z " + r.Z);
                        rotacionz = Matrix4.CreateRotationZ(((float)((float)(r[2] * Math.PI) / (180))));
                        Console.WriteLine("la rotacion es: en z");
                        Console.WriteLine(rotacionz.ToString());
                    }
                    //multiplicacion de matrices:prinmero rotacion y luego traslacion.. se lee de izquierda a a derecha en opentk
                    //matematicamente se lee de derecha a izquierda


                    //   modelMatrix = Matrix4.Mult(modelMatrix, rotacion);
                    //  modelMatrix = Matrix4.Mult(modelMatrix, traslacion); //( model * ( rotacion * traslacion ) )

                    if (primeraVez == 1)
                    {
                        matrices[i] = Matrix4.Mult(matrices[i], rotacionx);
                        matrices[i] = Matrix4.Mult(matrices[i], rotaciony);
                        matrices[i] = Matrix4.Mult(matrices[i], rotacionz);
                        matrices[i] = Matrix4.Mult(matrices[i], Matrix4.CreateTranslation(v.X, v.Y, v.Z));
                    }
                    mvMatrix = Matrix4.Mult(matrices[i], viewMatrix);


                    figColor = new Vector4(l.getColor(), 1.0f);
                    sProgram.SetUniformValue("figureColor", figColor);
                    sProgram.SetUniformValue("mvMat", mvMatrix);
                    o.Dibujar(sProgram);

                    i++;
                }
            //}//fin de bloque por primera vez
           
                //Dibujamos el cubo.
      //    modelMatrix = Matrix4.Identity;
           sProgram.SetUniformValue("figureColor", figColor); //Seteamos color amarillo al shader de fragmentos.
            mvMatrix = Matrix4.Mult(modelMatrix, viewMatrix);
            sProgram.SetUniformValue("mvMat", mvMatrix);
            myCube.Dibujar(sProgram);
            ejes_locales.Dibujar(sProgram);

            sProgram.Deactivate(); //Desactivamos el programa de shader.

            glControl3.SwapBuffers(); //Intercambiamos buffers frontal y trasero, para evitar flickering.
        }

        private void glControl3_Resize(object sender, EventArgs e)
        {   //Actualizamos el viewport para que dibuje en el centro de la pantalla.
            Size size = glControl3.Size;
            if (size.Width < size.Height)
            {
                viewport.X = 0;
                viewport.Y = (size.Height - size.Width) / 2;
                viewport.Width = size.Width;
                viewport.Height = size.Width;
            }
            else
            {
                viewport.X = (size.Width - size.Height) / 2;
                viewport.Y = 0;
                viewport.Width = size.Height;
                viewport.Height = size.Height;
            }
            glControl3.Invalidate(); //Invalidamos el glControl para que se redibuje.(llama al metodo Paint)
        }

        private void glControl3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void glControl3_KeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.S:
                    myCamera.Abajo();
                    break;

                case Keys.Up:
                case Keys.W:
                    myCamera.Arriba();
                    break;

                case Keys.Right:
                case Keys.D:
                    myCamera.Derecha();
                    break;

                case Keys.Left:
                case Keys.A:
                    myCamera.Izquierda();
                    break;

                case Keys.Add:
                    myCamera.Acercar(0.5f);
                    break;

                case Keys.Subtract:
                    myCamera.Alejar(0.5f);
                    break;
                    //inventados//________________________________---
                case Keys.O:
                    myCamera.setUp(new Vector3(2, 0, 0)); //cambia la orientacion
                    break;
                case Keys.T:
                    myCamera.setTarget(new Vector3(2,0,0));
                    break;
                case Keys.C:
                    myCamera.setCampoVisionPerspectiva(50,1);
                    break;

                case Keys.P:
                    myCamera.setProfundidadPerspectiva(0.1f, 200f);
                    break;
                    //--------------------------------------------
                 //Teclas para el ejemplo de transformaciones
                case Keys.D0:
                case Keys.NumPad0:
                    transformaciones = 0;
                    break;

                case Keys.D1:
                case Keys.NumPad1:
                    transformaciones = 1;
                    break;

                case Keys.D2:
                case Keys.NumPad2:
                    transformaciones = 2;
                    break;

                case Keys.D3:
                case Keys.NumPad3:
                    transformaciones = 3;
                    break;

                case Keys.D4:
                case Keys.NumPad4:
                    transformaciones = 4;
                    break;

                case Keys.D5:
                case Keys.NumPad5:
                    transformaciones = 5;
                    break;

                case Keys.D6:
                case Keys.NumPad6:
                    transformaciones = 6;
                    break;

                case Keys.D7:
                case Keys.NumPad7:
                    transformaciones = 7;
                    break;

                case Keys.D8:
                case Keys.NumPad8:
                    transformaciones = 8;
                    break;
               
            }

            glControl3.Invalidate(); //Notar que renderizamos para CUALQUIER tecla que sea presionada.
        }

        private void SetupShaders()
        {
            //Lo hago con mis clases, que encapsulan el manejo de shaders.
            //1. Creamos los shaders, a partir de archivos.
            String vShaderFile = "files/shaders/vshader1.glsl";
            String fShaderFile = "files/shaders/fshader1.glsl";
            Shader vShader = new Shader(ShaderType.VertexShader, vShaderFile);
            Shader fShader = new Shader(ShaderType.FragmentShader, fShaderFile);
            //2. Los compilamos
            vShader.Compile();
            fShader.Compile();
            //3. Creamos el Programa de shader con ambos.
            sProgram = new ShaderProgram();
            sProgram.AddShader(vShader);
            sProgram.AddShader(fShader);
            //4. Construimos (linkeamos) el programa.
            sProgram.Build();
            //5. Ya podemos eliminar los shaders compilados. (Si no los vamos a usar en otro programa)
            vShader.Delete();
            fShader.Delete();
        }

        private void logContextInfo()
        {
            String version, renderer, shaderVer, vendor;//, extensions;
            version = gl.GetString(StringName.Version);
            renderer = gl.GetString(StringName.Renderer);
            shaderVer = gl.GetString(StringName.ShadingLanguageVersion);
            vendor = gl.GetString(StringName.Vendor);
            //extensions = gl.GetString(StringName.Extensions);
            log("========= CONTEXT INFORMATION =========");
            log("Renderer:       {0}", renderer);
            log("Vendor:         {0}", vendor);
            log("OpenGL version: {0}", version);
            log("GLSL version:   {0}", shaderVer);
            //log("Extensions:" + extensions);
            log("===== END OF CONTEXT INFORMATION =====");

        }
        private void log(String format, params Object[] args)
        {
            System.Diagnostics.Debug.WriteLine(String.Format(format, args), "[CGUNS]");
        }


    }
}
