using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Textures;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;

namespace TGC.Examples.SkeletalAnimation
{
    /// <summary>
    ///     Ejemplo EjemploMeshInstance:
    ///     Unidades Involucradas:
    ///     # Unidad 5 - Animaci�n - Skeletal Animation
    ///     # Unidad 7 - T�cnicas de Optimizaci�n - Instancias de Modelos
    ///     Muestra como crear instancias de modelos animados con Skeletal Animation.
    ///     Al crear instancias de un �nico modelo original se reutiliza toda su informaci�n
    ///     gr�fica (animaciones, v�rtices, texturas, etc.)
    ///     Autor: Leandro Barbagallo, Mat�as Leone
    /// </summary>
    public class EjemploMeshInstance : TgcExample
    {
        private List<TgcSkeletalMesh> instances;
        private TgcSkeletalMesh original;
        private TgcBox suelo;

        public EjemploMeshInstance(string mediaDir, string shadersDir, TgcUserVars userVars, TgcModifiers modifiers,
            TgcAxisLines axisLines, TgcCamera camara)
            : base(mediaDir, shadersDir, userVars, modifiers, axisLines, camara)
        {
            Category = "SkeletalAnimation";
            Name = "MeshInstance";
            Description = "Muestra como crear instancias de modelos animados con Skeletal Animation.";
        }

        public override void Init()
        {
            //Crear suelo
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device,
                MediaDir + "Texturas\\Quake\\TexturePack2\\rock_floor1.jpg");
            suelo = TgcBox.fromSize(new Vector3(500, 0, 500), new Vector3(2000, 0, 2000), pisoTexture);

            //Cargar malla original
            var loader = new TgcSkeletalLoader();
            var pathMesh = MediaDir + "SkeletalAnimations\\Robot\\Robot-TgcSkeletalMesh.xml";
            var mediaPath = MediaDir + "SkeletalAnimations\\Robot\\";
            original = loader.loadMeshFromFile(pathMesh, mediaPath);

            //Agregar animaci�n a original
            loader.loadAnimationFromFile(original, mediaPath + "Patear-TgcSkeletalAnim.xml");

            //Agregar attachment a original
            var attachment = new TgcSkeletalBoneAttach();
            var attachmentBox = TgcBox.fromSize(new Vector3(3, 60, 3), Color.Green);
            attachment.Mesh = attachmentBox.toMesh("attachment");
            attachment.Bone = original.getBoneByName("Bip01 L Hand");
            attachment.Offset = Matrix.Translation(10, -40, 0);
            attachment.updateValues();
            original.Attachments.Add(attachment);

            //Crear 9 instancias mas de este modelo, pero sin volver a cargar el modelo entero cada vez
            float offset = 200;
            var cantInstancias = 4;
            instances = new List<TgcSkeletalMesh>();
            for (var i = 0; i < cantInstancias; i++)
            {
                var instance = original.createMeshInstance(original.Name + i);

                instance.move(i * offset, 0, 0);
                instances.Add(instance);
            }

            //Especificar la animaci�n actual para todos los modelos
            original.playAnimation("Patear");
            foreach (var instance in instances)
            {
                instance.playAnimation("Patear");
            }

            //Camara en primera persona
            Camara = new TgcFpsCamera();
            ((TgcFpsCamera)Camara).MovementSpeed = 400;
            ((TgcFpsCamera)Camara).JumpSpeed = 400;
            Camara.setCamera(new Vector3(293.201f, 291.0797f, -604.6647f),
                new Vector3(299.1028f, -63.9185f, 330.1836f));
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void Render()
        {
            IniciarEscena();
            base.Render();

            //Renderizar suelo
            suelo.render();

            //Renderizar original e instancias
            original.animateAndRender(ElapsedTime);
            foreach (var instance in instances)
            {
                instance.animateAndRender(ElapsedTime);
            }

            FinalizarEscena();
        }

        public override void Close()
        {
            base.Close();

            suelo.dispose();

            //Al hacer dispose del original, se hace dispose autom�ticamente de todas las instancias
            original.dispose();
        }
    }
}