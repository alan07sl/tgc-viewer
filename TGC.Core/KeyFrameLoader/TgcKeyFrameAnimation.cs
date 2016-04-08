using TGC.Core.Geometries;

namespace TGC.Core.KeyFrameLoader
{
    /// <summary>
    ///     Animaci�n de una malla animada por KeyFrames
    /// </summary>
    public class TgcKeyFrameAnimation
    {
        public TgcKeyFrameAnimation(TgcKeyFrameAnimationData data, TgcBoundingBox boundingBox)
        {
            Data = data;
            BoundingBox = boundingBox;
        }

        /// <summary>
        ///     BoundingBox de la animaci�n
        /// </summary>
        public TgcBoundingBox BoundingBox { get; }

        /// <summary>
        ///     Datos de v�rtices de la animaci�n
        /// </summary>
        public TgcKeyFrameAnimationData Data { get; }
    }
}