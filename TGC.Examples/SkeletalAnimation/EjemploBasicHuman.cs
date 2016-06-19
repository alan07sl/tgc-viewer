using Microsoft.DirectX;
using System;
using System.Drawing;
using System.IO;
using TGC.Core;
using TGC.Core.Camara;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.SkeletalAnimation;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;

namespace TGC.Examples.SkeletalAnimation
{
    /// <summary>
    ///     Ejemplo BasicHuman:
    ///     Unidades Involucradas:
    ///     # Unidad 5 - Animaci�n - Skeletal Animation
    ///     Utiliza el esqueleto gen�rico BasicHuman provisto por la c�tedra para
    ///     animar varios modelos distintos mediante animaci�n esquel�tica.
    ///     El esqueleto posee una lista de animaciones default que pueden ser reutilizadas
    ///     para varios modelos distintos que se hayan acoplado a estos huesos.
    ///     Autor: Leandro Barbagallo, Mat�as Leone
    /// </summary>
    public class BasicHuman : TgcExample
    {
        private string[] animationsPath;
        private TgcSkeletalBoneAttach attachment;
        private Color currentColor;
        private string mediaPath;
        private TgcSkeletalMesh mesh;
        private string selectedAnim;
        private string selectedMesh;
        private bool showAttachment;

        public BasicHuman(string mediaDir, string shadersDir, TgcUserVars userVars, TgcModifiers modifiers,
            TgcAxisLines axisLines, TgcCamera camara)
            : base(mediaDir, shadersDir, userVars, modifiers, axisLines, camara)
        {
            Category = "SkeletalAnimation";
            Name = "BasicHuman";
            Description =
                "Utiliza el esqueleto gen�rico BasicHuman provisto por la c�tedra para animar varios modelos distintos mediante animaci�n esquel�tica.";
        }

        public override void Init()
        {
            //Path para carpeta de texturas de la malla
            mediaPath = MediaDir + "SkeletalAnimations\\BasicHuman\\";

            //Cargar dinamicamente todos los Mesh animados que haya en el directorio
            var dir = new DirectoryInfo(mediaPath);
            var meshFiles = dir.GetFiles("*-TgcSkeletalMesh.xml", SearchOption.TopDirectoryOnly);
            var meshList = new string[meshFiles.Length];
            for (var i = 0; i < meshFiles.Length; i++)
            {
                var name = meshFiles[i].Name.Replace("-TgcSkeletalMesh.xml", "");
                meshList[i] = name;
            }

            //Cargar dinamicamente todas las animaciones que haya en el directorio "Animations"
            var dirAnim = new DirectoryInfo(mediaPath + "Animations\\");
            var animFiles = dirAnim.GetFiles("*-TgcSkeletalAnim.xml", SearchOption.TopDirectoryOnly);
            var animationList = new string[animFiles.Length];
            animationsPath = new string[animFiles.Length];
            for (var i = 0; i < animFiles.Length; i++)
            {
                var name = animFiles[i].Name.Replace("-TgcSkeletalAnim.xml", "");
                animationList[i] = name;
                animationsPath[i] = animFiles[i].FullName;
            }

            //Cargar mesh inicial
            selectedAnim = animationList[0];
            changeMesh(meshList[0]);

            //Modifier para elegir modelo
            Modifiers.addInterval("mesh", meshList, 0);

            //Agregar combo para elegir animacion
            Modifiers.addInterval("animation", animationList, 0);

            //Modifier para especificar si la animaci�n se anima con loop
            var animateWithLoop = true;
            Modifiers.addBoolean("loop", "Loop anim:", animateWithLoop);

            //Modifier para renderizar el esqueleto
            var renderSkeleton = false;
            Modifiers.addBoolean("renderSkeleton", "Show skeleton:", renderSkeleton);

            //Modifier para FrameRate
            Modifiers.addFloat("frameRate", 0, 100, 30);

            //Modifier para color
            currentColor = Color.White;
            Modifiers.addColor("Color", currentColor);

            //Modifier para BoundingBox
            Modifiers.addBoolean("BoundingBox", "BoundingBox:", false);

            //Modifier para habilitar attachment
            showAttachment = false;
            Modifiers.addBoolean("Attachment", "Attachment:", showAttachment);
        }

        public override void Update()
        {
            base.PreUpdate();
        }

        /// <summary>
        ///     Cargar una nueva malla
        /// </summary>
        private void changeMesh(string meshName)
        {
            if (selectedMesh == null || selectedMesh != meshName)
            {
                if (mesh != null)
                {
                    mesh.dispose();
                    mesh = null;
                }

                selectedMesh = meshName;

                //Cargar mesh y animaciones
                var loader = new TgcSkeletalLoader();
                mesh = loader.loadMeshAndAnimationsFromFile(mediaPath + selectedMesh + "-TgcSkeletalMesh.xml", mediaPath,
                    animationsPath);

                //Crear esqueleto a modo Debug
                mesh.buildSkletonMesh();

                //Elegir animacion inicial
                mesh.playAnimation(selectedAnim, true);

                //Crear caja como modelo de Attachment del hueos "Bip01 L Hand"
                attachment = new TgcSkeletalBoneAttach();
                var attachmentBox = TgcBox.fromSize(new Vector3(2, 40, 2), Color.Red);
                attachment.Mesh = attachmentBox.toMesh("attachment");
                attachment.Bone = mesh.getBoneByName("Bip01 L Hand");
                attachment.Offset = Matrix.Translation(3, -15, 0);
                attachment.updateValues();

                //Configurar camara
                Camara = new TgcRotationalCamera(mesh.BoundingBox.calculateBoxCenter(), mesh.BoundingBox.calculateBoxRadius() * 2);
            }
        }

        private void changeAnimation(string animation)
        {
            if (selectedAnim != animation)
            {
                selectedAnim = animation;
                mesh.playAnimation(selectedAnim, true);
            }
        }

        public override void Render()
        {
            base.PreRender();
            

            //Ver si cambio la malla
            var meshPath = (string)Modifiers.getValue("mesh");
            changeMesh(meshPath);

            //Ver si cambio la animacion
            var anim = (string)Modifiers.getValue("animation");
            changeAnimation(anim);

            //Ver si rendeizamos el esqueleto
            var renderSkeleton = (bool)Modifiers.getValue("renderSkeleton");

            //Ver si cambio el color
            var selectedColor = (Color)Modifiers.getValue("Color");
            if (currentColor == null || currentColor != selectedColor)
            {
                currentColor = selectedColor;
                mesh.setColor(currentColor);
            }

            //Agregar o quitar Attachment
            var showAttachmentFlag = (bool)Modifiers["Attachment"];
            if (showAttachment != showAttachmentFlag)
            {
                showAttachment = showAttachmentFlag;
                if (showAttachment)
                {
                    //Al agregar el attachment, el modelo se encarga de renderizarlo en forma autom�tica
                    attachment.Mesh.Enabled = true;
                    mesh.Attachments.Add(attachment);
                }
                else
                {
                    attachment.Mesh.Enabled = false;
                    mesh.Attachments.Remove(attachment);
                }
            }

            //Actualizar animacion
            mesh.updateAnimation(ElapsedTime);

            //Solo malla o esqueleto, depende lo seleccionado
            mesh.RenderSkeleton = renderSkeleton;
            mesh.render();

            //Se puede renderizar todo mucho mas simple (sin esqueleto) de la siguiente forma:
            //mesh.animateAndRender();

            //BoundingBox
            var showBB = (bool)Modifiers["BoundingBox"];
            if (showBB)
            {
                mesh.BoundingBox.render();
            }

            PostRender();
        }

        public override void Dispose()
        {
            

            //La malla tambi�n hace dispose del attachment
            mesh.dispose();
            mesh = null;
            selectedMesh = null;
        }
    }
}