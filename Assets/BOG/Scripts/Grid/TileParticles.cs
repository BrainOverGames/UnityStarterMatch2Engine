using UnityEngine;
using UnityEngine.Assertions;

namespace BOG
{
    /// <summary>
    /// Particle FX component for a tile in a grid
    /// </summary>
    public class TileParticles : MonoBehaviour
    {
        public ParticleSystem fragmentParticles;

        private void Awake()
        {
            Assert.IsNotNull(fragmentParticles);
        }
    }
}
