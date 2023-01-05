using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace BOG
{
	/// <summary>
	/// Responsible to generate pools of game objects/prefabs
	/// </summary>
	public class GamePools : MonoBehaviour
	{
		public ObjectPool block1Pool;
		public ObjectPool block2Pool;
		public ObjectPool block3Pool;
		public ObjectPool block4Pool;
		public ObjectPool block5Pool;
		public ObjectPool block6Pool;
		public ObjectPool emptyTilePool;

        public ObjectPool blockParticlesPool;

        private readonly List<ObjectPool> blockPools = new List<ObjectPool>();

		private void Awake()
		{
			Assert.IsNotNull(block1Pool);
			Assert.IsNotNull(block2Pool);
			Assert.IsNotNull(block3Pool);
			Assert.IsNotNull(block4Pool);
			Assert.IsNotNull(block5Pool);
			Assert.IsNotNull(block6Pool);
			Assert.IsNotNull(emptyTilePool);
            Assert.IsNotNull(blockParticlesPool);

            blockPools.Add(block1Pool);
			blockPools.Add(block2Pool);
			blockPools.Add(block3Pool);
			blockPools.Add(block4Pool);
			blockPools.Add(block5Pool);
			blockPools.Add(block6Pool);
		}

		public TileEntity GetTileEntity(LevelData level, LevelTile tile)
		{
			if (tile is BlockTile)
			{
				var blockTile = (BlockTile)tile;
				switch (blockTile.type)
				{
					case BlockType.Block1:
						return block1Pool.GetObject().GetComponent<TileEntity>();

					case BlockType.Block2:
						return block2Pool.GetObject().GetComponent<TileEntity>();

					case BlockType.Block3:
						return block3Pool.GetObject().GetComponent<TileEntity>();

					case BlockType.Block4:
						return block4Pool.GetObject().GetComponent<TileEntity>();

					case BlockType.Block5:
						return block5Pool.GetObject().GetComponent<TileEntity>();

					case BlockType.Block6:
						return block6Pool.GetObject().GetComponent<TileEntity>();

					case BlockType.RandomBlock:
						{
							var randomIdx = Random.Range(0, level.availableColors.Count);
							return blockPools[(int)level.availableColors[randomIdx]].GetObject().GetComponent<TileEntity>();
						}

					case BlockType.Empty:
						return emptyTilePool.GetObject().GetComponent<TileEntity>();
				}
			}
			return null;
		}

        public GameObject GetParticles(TileEntity tileEntity)
        {
            GameObject particles = null;
            var block = tileEntity as Block;
			if (block != null)
				particles = blockParticlesPool.GetObject();
			return particles;
        }
    }
}
