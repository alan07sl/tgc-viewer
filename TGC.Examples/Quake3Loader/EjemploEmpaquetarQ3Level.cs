using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TGC.Core._2D;
using TGC.Core.Example;
using TGC.Util;

namespace TGC.Examples.Quake3Loader
{
    /// <summary>
    ///     Ejemplo EjemploEmpaquetarQ3Level
    ///     Unidades Involucradas:
    ///     # Unidad 7 - Optimizaci�n - BSP y PVS
    ///     Ver primero ejemplo EjemploLoadQ3Level
    ///     Permite empaquetar un escenario de Quake 3 hecho con el editor GtkRadiant.
    ///     Genera como salida una carpeta con todo el contenido m�nimo requerido para el escenario especificado.
    ///     Este ejemplo no posee salida gr�fica.
    ///     Autor: Martin Giachetti, Mat�as Leone
    /// </summary>
    public class EjemploEmpaquetarQ3Level : TgcExample
    {
        private string currentFile;
        private string quake3MediaPath;

        public override string getCategory()
        {
            return "Quake3";
        }

        public override string getName()
        {
            return "Pack level";
        }

        public override string getDescription()
        {
            return "Utilidad para empaquetar escenarios de Quake 3 hechos con el editor GtkRadiant.";
        }

        public override void init()
        {
            //Path de recursos del Quake 3 original (descomprimir archivo pak0.pk3 de la carpeta de instalaci�n del Quake 3, renombrar a .zip)
            quake3MediaPath = "C:\\Program Files\\Quake III Arena\\baseq3\\pak0\\";

            //Modifier para abrir archivo
            currentFile = "C:\\Program Files\\Quake III Arena\\baseq3\\maps\\prueba.bsp";
            GuiController.Instance.Modifiers.addFile("BspFile", currentFile, ".Niveles Quake 3|*.bsp");
        }

        public override void render(float elapsedTime)
        {
            //Ver si se seleccion� alguno nivel a empaquetar
            var selectedFile = (string)GuiController.Instance.Modifiers["BspFile"];
            if (selectedFile != currentFile)
            {
                currentFile = selectedFile;

                //Cargar nivel
                var loader = new BspLoader();
                var bspMap = loader.loadBsp(currentFile, quake3MediaPath);

                //Empaquetar
                var info = new FileInfo(currentFile);
                var fileName = info.Name.Substring(0, info.Name.IndexOf('.'));
                var outputDir = info.DirectoryName + "\\" + fileName;

                loader.packLevel(bspMap, quake3MediaPath, outputDir);

                //Librer recursos
                bspMap.dispose();

                MessageBox.Show(GuiController.Instance.MainForm, "Empaquetado almacenado en: " + outputDir,
                    "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            TgcDrawText.Instance.drawText(
                "Este ejemplo no posee salida gr�fica. verificar c�digo y sus comentarios.", 5, 50,
                Color.Yellow);
            
        }

        public override void close()
        {
        }
    }
}