using TGC.Core.SceneLoader;

namespace TGC.Examples.Optimization.Octree
{
    /// <summary>
    ///     Nodo del �rbol Octree
    /// </summary>
    internal class OctreeNode
    {
        public OctreeNode[] children;
        public TgcMesh[] models;

        public bool isLeaf()
        {
            return children == null;
        }
    }
}