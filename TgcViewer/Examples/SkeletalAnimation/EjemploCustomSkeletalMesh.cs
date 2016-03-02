using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.SkeletalAnimation;
using TGC.Viewer;

namespace TGC.Examples.SkeletalAnimation
{
    /// <summary>
    ///     Ejemplo EjemploCustomSkeletalMesh:
    ///     Unidades Involucradas:
    ///     # Unidad 5 - Animaci�n - Skeletal Animation
    ///     Muestra como extender la clase TgcSkeletalMesh para agregarle comportamiento personalizado.
    ///     En este ejemplo se redefine el m�todo executeRender() de TgcSkeletalMesh para renderizar
    ///     el modelo en Wireframe.
    ///     Autor: Leandro Barbagallo, Mat�as Leone
    /// </summary>
    public class EjemploCustomSkeletalMesh : TgcExample
    {
        private MyCustomMesh mesh;

        public override string getCategory()
        {
            return "SkeletalAnimation";
        }

        public override string getName()
        {
            return "CustomMesh";
        }

        public override string getDescription()
        {
            return
                "Muestra como extender la clase TgcSkeletalMesh para agregarle comportamiento personalizado. En este ejemplo se renderiza en Wireframe";
        }

        public override void init()
        {
            //Crear loader
            var loader = new TgcSkeletalLoader();

            //Configurar MeshFactory customizado
            loader.MeshFactory = new MyCustomMeshFactory();

            //Cargar modelo con una animaci�n
            var pathMesh = GuiController.Instance.ExamplesMediaDir +
                           "SkeletalAnimations\\BasicHuman\\WomanJeans-TgcSkeletalMesh.xml";
            string[] animationsPath =
            {
                GuiController.Instance.ExamplesMediaDir +
                "SkeletalAnimations\\BasicHuman\\Animations\\Push-TgcSkeletalAnim.xml"
            };
            mesh = (MyCustomMesh)loader.loadMeshAndAnimationsFromFile(pathMesh, animationsPath);

            //Ejecutar animacion
            mesh.playAnimation("Push");

            //Centrar camara rotacional respecto a este mesh
            GuiController.Instance.RotCamera.targetObject(mesh.BoundingBox);
        }

        public override void render(float elapsedTime)
        {
            mesh.animateAndRender(elapsedTime);
        }

        public override void close()
        {
            mesh.dispose();
        }
    }

    /// <summary>
    ///     Mesh customizado de ejemplo. Lo �nico que altera es que renderiza en Wireframe en lugar de renderizar en modo
    ///     solido.
    ///     Extiende de TgcSkeletalMesh. Implementa la interfaz IRenderQueueElement para poder redefinir el metodo
    ///     executeRender().
    ///     Tiene que tener el mismo constructor que tiene la clase TgcSkeletalMesh
    /// </summary>
    public class MyCustomMesh : TgcSkeletalMesh
    {
        /// <summary>
        ///     Primer constructor de TgcSkeletalMesh.
        ///     No se hace nada, solo se llama al constructor del padre.
        /// </summary>
        public MyCustomMesh(Mesh mesh, string name, MeshRenderType renderType, TgcSkeletalBone[] bones)
            : base(mesh, name, renderType, bones)
        {
        }

        /// <summary>
        ///     Se redefine tal cual, para que llame a nuestro render
        /// </summary>
        public new void animateAndRender(float elapsedTime)
        {
            if (!enabled)
                return;

            updateAnimation(elapsedTime);
            render();
        }

        /// <summary>
        ///     Se redefine este m�todo para customizar el renderizado de este modelo.
        ///     Se agrega la palabra "new" al m�todo para indiciar que est� redefinido.
        /// </summary>
        public new void render()
        {
            //Cambiamos a modo WireFrame
            D3DDevice.Instance.Device.RenderState.FillMode = FillMode.WireFrame;

            //Llamamos al metodo original del padre
            base.render();

            //Restrablecemos modo solido
            D3DDevice.Instance.Device.RenderState.FillMode = FillMode.Solid;
        }
    }

    /// <summary>
    ///     Factory customizado que crea una instancia de la clase MyCustomMesh.
    ///     Debe implementar la interfaz TgcSceneLoader.IMeshFactory
    ///     En el m�todo se crea una instancia de MyCustomMesh.
    /// </summary>
    public class MyCustomMeshFactory : TgcSkeletalLoader.IMeshFactory
    {
        public TgcSkeletalMesh createNewMesh(Mesh d3dMesh, string meshName, TgcSkeletalMesh.MeshRenderType renderType,
            TgcSkeletalBone[] bones)
        {
            return new MyCustomMesh(d3dMesh, meshName, renderType, bones);
        }
    }
}