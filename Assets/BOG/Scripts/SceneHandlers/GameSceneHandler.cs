using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BOG
{
    /// <summary>
    /// Responsible for handling all boaard/grid related functionality in game scene
    /// </summary>
    public class GameSceneHandler : BaseSceneHandler
    {
        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private Color backgroundColor;
        [SerializeField] private float blockFallSpeed;
        [SerializeField] private float horizontalSpacing;
        [SerializeField] private float verticalSpacing;
        [SerializeField] private GamePools gamePools;
        [SerializeField] private GameUi gameUi;
        [SerializeField] private Transform levelLocation;
        [SerializeField] private GameObject goalPrefab;
        [SerializeField] private List<AudioClip> gameSounds;
        [SerializeField] private GameObject backgroundTiles;

        [HideInInspector]
        private LevelData levelData;
        internal LevelData LevelData { get { return levelData; } }
        [HideInInspector]
        internal List<GameObject> tileEntities = new List<GameObject>();
        [HideInInspector]
        internal List<Vector2> tilePositions = new List<Vector2>();
        internal readonly List<GameObject> blockers = new List<GameObject>();

        private Coroutine countdownCoroutine;
        private int generatedCollectables;
        private int neededCollectables;
        private float blockWidth;
        private float blockHeight;
        private Camera mainCamera;
        private bool gameStarted;
        private bool gameFinished;
        private bool applyingPenalty;
        private float accTime;
        private const float timeBetweenMatchSuggestions = 5.0f;
        private const float endGamePopupDelay = 0.75f;
        private int currentLimit;
        private readonly GameState gameState = new GameState();
        internal GameStateData CurrentGameStateData { get; private set; }

        private void Start()
        {
            mainCamera = Camera.main;
            string loadPath = "Levels/Level" + GameManager.Instance.lastSelectedLevel;
            levelData = FileUtils.LoadJsonFile<LevelData>(loadPath);
            ResetLevelData();
            CreateBackgroundTiles();
            SoundManager.Instance.AddSounds(gameSounds);
            UpdateGameStateData(levelData.limit);
        }

        private void OnDestroy()
        {
            SoundManager.Instance.RemoveSounds(gameSounds);
        }

        private void Update()
        {
            if (!gameStarted || gameFinished)
            {
                return;
            }

            if (currentPopups.Count > 0)
            {
                return;
            }

            accTime += Time.deltaTime;
            if (accTime >= timeBetweenMatchSuggestions) //used after what amount of time, highlight must be showing
            {
                accTime = 0.0f;
            }

            if (Input.GetMouseButtonDown(0))
            {
                accTime = 0.0f;

                var hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Block"))
                {
                    var hitIdx = tileEntities.FindIndex(x => x == hit.collider.gameObject);
                    var hitBlock = hit.collider.gameObject.GetComponent<TileEntity>();
                    if (blockers[hitIdx] != null)
                    {
                        return;
                    }
                    DestroyBlock(hitBlock);
                }
            }
        }

        /// <summary>
        /// Destroys the specified block.
        /// </summary>
        /// <param name="blockToDestroy">The block to destroy.</param>
        private void DestroyBlock(TileEntity blockToDestroy)
        {
            var blockIdx = tileEntities.FindIndex(x => x == blockToDestroy.gameObject);
            var blocksToDestroy = new List<GameObject>();
            GetMatches(blockToDestroy.gameObject, blocksToDestroy);
            if (blocksToDestroy.Count > 0)
            {
                foreach (var block in blocksToDestroy)
                {
                    var idx = tileEntities.FindIndex(x => x == block);
                    DestroyTileEntity(block.GetComponent<TileEntity>(), idx);
                }
                PerformMove(true);
                UpdateGameStateData(currentLimit);
                StartCoroutine(ApplyGravityAsync());
            }
            else
            {
                blockToDestroy.GetComponent<Animator>().SetTrigger("NoMatches");
                SoundManager.Instance.PlaySound("CubePressError");
                ApplyPenalty();
            }
        }

        /// <summary>
        /// Calculates the matches of the specified block.
        /// </summary>
        /// <param name="go">The block to check.</param>
        /// <param name="matchedTiles">A list containing the matched tiles.</param>
        private void GetMatches(GameObject go, List<GameObject> matchedTiles)
        {
            var idx = tileEntities.FindIndex(x => x == go);
            var i = idx % levelData.width;
            var j = idx / levelData.width;

            var topTile = new TileDef(i, j - 1);
            var bottomTile = new TileDef(i, j + 1);
            var leftTile = new TileDef(i - 1, j);
            var rightTile = new TileDef(i + 1, j);
            var surroundingTiles = new List<TileDef> { topTile, bottomTile, leftTile, rightTile };

            var hasMatch = false;
            foreach (var surroundingTile in surroundingTiles)
            {
                if (IsValidTileEntity(surroundingTile))
                {
                    var tileIndex = (levelData.width * surroundingTile.y) + surroundingTile.x;
                    var tile = tileEntities[tileIndex];
                    if (tile != null)
                    {
                        var block = tile.GetComponent<Block>();
                        if (block != null && block.type == go.GetComponent<Block>().type)
                        {
                            hasMatch = true;
                        }
                    }
                }
            }

            if (!hasMatch)
            {
                return;
            }

            if (!matchedTiles.Contains(go))
            {
                matchedTiles.Add(go);
            }
            //for multiple chains
            foreach (var surroundingTile in surroundingTiles)
            {
                if (IsValidTileEntity(surroundingTile))
                {
                    var tileIndex = (levelData.width * surroundingTile.y) + surroundingTile.x;
                    var tile = tileEntities[tileIndex];
                    if (tile != null)
                    {
                        var block = tile.GetComponent<Block>();
                        if (block != null && block.type == go.GetComponent<Block>().type &&
                            !matchedTiles.Contains(tile))
                        {
                            GetMatches(tile, matchedTiles);
                        }
                    }
                }
            }
        }

        private bool IsValidTileEntity(TileDef tileEntity)
        {
            return tileEntity.x >= 0 && tileEntity.x < levelData.width &&
                   tileEntity.y >= 0 && tileEntity.y < levelData.height;
        }

        /// <summary>
        /// Destroys the specified tile entity.
        /// </summary>
        /// <param name="tileEntity">The tile entity to destroy.</param>
        /// <param name="tileIndex">The index of the tile entity to destroy.</param>
        /// <param name="playSound">True if the destruction sound should be played; false otherwise.</param>
        /// <returns>The score obtained by the destruction of the tile entity.</returns>
        private void DestroyTileEntity(TileEntity tileEntity, int tileIndex, bool playSound = true)
        {
            var block = tileEntity.GetComponent<Block>();
            if (block != null)
            {
                gameState.collectedBlocks[block.type] += 1;
            }

            var particles = gamePools.GetParticles(tileEntity);
            if (particles != null)
            {
                particles.transform.position = tileEntity.transform.position;
                particles.GetComponent<TileParticles>().fragmentParticles.Play();
            }

            if (block != null)
            {
                if (playSound)
                {
                    SoundManager.Instance.PlaySound("CubePress");
                }
            }

            tileEntity.Explode();
            tileEntities[tileIndex] = null;
            tileEntity.GetComponent<PooledObject>().pool.ReturnObject(tileEntity.gameObject);
        }

        /// <summary>
        /// Updates the state of the game based on the last move performed by the player.
        /// </summary>
        /// <param name="updateLimits">True if the current limits of the level should be updated; false otherwise.</param>
        private void PerformMove(bool updateLimits)
        {
            if (levelData.limitType == LimitType.Moves && updateLimits)
            {
                --currentLimit;
                if (currentLimit < 0)
                {
                    currentLimit = 0;
                }
                UpdateLimitText();
            }
            gameUi.goalUi.UpdateGoals(gameState);
        }

        private void UpdateGameStateData(int currentLimit)
        {
            CurrentGameStateData = new GameStateData
            {
                levelId = levelData.id,
                limitType = levelData.limitType,
                limit = currentLimit,
                penalty = levelData.penalty,
                collectedBlocksDic = gameState.collectedBlocks,
            };
        }

        internal void LoadSavedGameState(GameStateData loadedGameStateData)
        {
            levelData.limitType = loadedGameStateData.limitType;
            currentLimit = loadedGameStateData.limit;
            UpdateLimitText();
            loadedGameStateData.collectedBlocksDic  = BlockConversionUtils.ListToDicConversion(loadedGameStateData.collectedBlocksList);
            var loadedGameState = new GameState
            {
                collectedBlocks = loadedGameStateData.collectedBlocksDic,
            };
            gameState.collectedBlocks = loadedGameState.collectedBlocks;
            gameUi.goalUi.UpdateGoals(loadedGameState);
        }

        private IEnumerator ApplyGravityAsync()
        {
            yield return new WaitForSeconds(0.1f);
            ApplyGravity();
            yield return new WaitForSeconds(0.5f);
            CheckForCollectables();
            CheckEndGame();
        }

        private void ApplyGravity()
        {
            for (var i = 0; i < levelData.width; i++)
            {
                for (var j = levelData.height - 1; j >= 0; j--)
                {
                    var tileIndex = i + (j * levelData.width);
                    if (tileEntities[tileIndex] == null ||
                        IsEmptyBlock(tileEntities[tileIndex].GetComponent<TileEntity>()))
                    {
                        continue;
                    }

                    // Find bottom.
                    var bottom = -1;
                    for (var k = j; k < levelData.height; k++)
                    {
                        var idx = i + (k * levelData.width);
                        if (tileEntities[idx] == null)
                        {
                            bottom = k;
                        }
                    }

                    if (bottom != -1)
                    {
                        var tile = tileEntities[tileIndex];
                        if (tile != null)
                        {
                            var numTilesToFall = bottom - j;
                            tileEntities[tileIndex + (numTilesToFall * levelData.width)] = tileEntities[tileIndex];
                            var tween = LeanTween.move(tile,
                                tilePositions[tileIndex + levelData.width * numTilesToFall],
                                blockFallSpeed);
                            tween.setEase(LeanTweenType.easeInQuad);
                            tween.setOnComplete(() =>
                            {
                                if (tile.activeSelf)
                                {
                                    tile.GetComponent<Animator>().SetTrigger("Falling");
                                }
                            });
                            tileEntities[tileIndex] = null;
                        }
                    }
                }
            }

            for (var i = 0; i < levelData.width; i++)
            {
                var numEmpties = 0;
                for (var j = 0; j < levelData.height; j++)
                {
                    var idx = i + (j * levelData.width);
                    if (tileEntities[idx] == null)
                    {
                        numEmpties += 1;
                    }
                }

                if (numEmpties > 0)
                {
                    for (var j = 0; j < levelData.height; j++)
                    {
                        var tileIndex = i + (j * levelData.width);
                        var isEmptyTile = false;
                        if (tileEntities[tileIndex] != null)
                        {
                            var blockTile = tileEntities[tileIndex].GetComponent<Block>();
                            if (blockTile != null)
                            {
                                isEmptyTile = blockTile.type == BlockType.Empty;
                            }
                        }
                        if (tileEntities[tileIndex] == null && !isEmptyTile)
                        {
                            var tile = CreateNewBlock();
                            var pos = tilePositions[i];
                            pos.y = tilePositions[i].y + (numEmpties * (blockHeight + verticalSpacing));
                            --numEmpties;
                            tile.transform.position = pos;
                            var tween = LeanTween.move(tile,
                                tilePositions[tileIndex],
                                blockFallSpeed);
                            tween.setEase(LeanTweenType.easeInQuad);
                            tween.setOnComplete(() =>
                            {
                                if (tile.activeSelf)
                                {
                                    tile.GetComponent<Animator>().SetTrigger("Falling");
                                }
                            });
                            tileEntities[tileIndex] = tile;
                        }
                        if (tileEntities[tileIndex] != null)
                        {
                            tileEntities[tileIndex].GetComponent<SpriteRenderer>().sortingOrder = levelData.height - j;
                        }
                    }
                }
            }
        }

        private bool IsEmptyBlock(TileEntity tileEntity)
        {
            var block = tileEntity as Block;
            return block != null && block.type == BlockType.Empty;
        }

        private void CheckForCollectables()
        {
            var collectablesToDestroy = new List<Block>();
            for (var i = 0; i < levelData.width; i++)
            {
                Block bottom = null;
                var tileIndex = 0;
                for (var j = levelData.height - 1; j >= 0; j--)
                {
                    tileIndex = i + (j * levelData.width);
                    if (tileEntities[tileIndex] == null)
                    {
                        continue;
                    }
                    var block = tileEntities[tileIndex].GetComponent<Block>();
                    if (block != null)
                    {
                        if (block.type == BlockType.Empty)
                        {
                            continue;
                        }
                        bottom = block;
                    }
                    break;
                }

                if (bottom != null && bottom.type == BlockType.Collectable)
                {
                    collectablesToDestroy.Add(bottom);
                    tileEntities[tileIndex] = null;
                }
            }

            if (collectablesToDestroy.Count > 0)
            {
                foreach (var tile in collectablesToDestroy)
                {
                    gameState.collectedBlocks[tile.type] += 1;

                    tile.Explode();
                    tile.GetComponent<PooledObject>().pool.ReturnObject(tile.gameObject);
                }

                PerformMove(false);
                StartCoroutine(ApplyGravityAsync());
            }
        }

        private GameObject CreateNewBlock()
        {
            return CreateBlock(gamePools.GetTileEntity(levelData, new BlockTile { type = BlockType.RandomBlock }).gameObject);
        }

        private GameObject CreateBlock(GameObject go)
        {
            go.GetComponent<TileEntity>().Spawn();
            return go;
        }

        private void ApplyPenalty()
        {
            if (applyingPenalty || levelData.penalty <= 0)
            {
                return;
            }
            applyingPenalty = true;
            StartCoroutine(AnimateLimitDown(levelData.penalty));
        }

        private IEnumerator AnimateLimitDown(int penalty)
        {
            var endValue = currentLimit - penalty;

            while (currentLimit > 0 && currentLimit != endValue)
            {
                currentLimit -= 1;
                UpdateLimitText();
                yield return new WaitForSeconds(0.1f);
            }
            applyingPenalty = false;
        }

        private void UpdateLimitText()
        {
            if (levelData.limitType == LimitType.Moves)
            {
                gameUi.limitText.text = currentLimit.ToString();
            }
        }

        private void CreateBackgroundTiles()
        {
            for (var j = 0; j < levelData.height; j++)
            {
                for (var i = 0; i < levelData.width; i++)
                {
                    var tileIndex = i + (j * levelData.width);
                    var tile = levelData.tiles[tileIndex];
                    var blockTile = tile as BlockTile;
                    if (blockTile != null && blockTile.type == BlockType.Empty)
                    {
                        continue;
                    }

                    var go = new GameObject("Background");
                    go.transform.parent = backgroundTiles.transform;
                    var sprite = go.AddComponent<SpriteRenderer>();
                    sprite.sprite = backgroundSprite;
                    sprite.color = backgroundColor;
                    sprite.sortingLayerName = "Game";
                    sprite.sortingOrder = -2;
                    sprite.transform.position = tileEntities[tileIndex].transform.position;
                }
            }
        }

        public void RestartGame()
        {
            ResetLevelData();
        }

        /// <summary>
        /// Resets the level data. This is particularly useful when replaying a level.
        /// </summary>
        public void ResetLevelData()
        {
            gameStarted = false;
            gameFinished = false;

            generatedCollectables = 0;
            neededCollectables = 0;
            foreach (var goal in levelData.goals)
            {
                if (goal is CollectBlockGoal)
                {
                    var blockGoal = goal as CollectBlockGoal;
                    if (blockGoal.blockType == BlockType.Collectable)
                    {
                        neededCollectables += blockGoal.amount;
                    }
                }
            }

            gameState.Reset();

            gameUi.limitText.text = "";

            foreach (var pool in gamePools.GetComponentsInChildren<ObjectPool>())
            {
                pool.Reset();
            }
            tileEntities.Clear();
            blockers.Clear();
            tilePositions.Clear();

            for (var j = 0; j < levelData.height; j++)
            {
                for (var i = 0; i < levelData.width; i++)
                {
                    var tileIndex = i + (j * levelData.width);
                    var tileToGet = gamePools.GetTileEntity(levelData, levelData.tiles[tileIndex]);
                    var tile = CreateBlock(tileToGet.gameObject);
                    var spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    blockWidth = spriteRenderer.bounds.size.x;
                    blockHeight = spriteRenderer.bounds.size.y;
                    tile.transform.position = new Vector2(i * (blockWidth + horizontalSpacing),
                        -j * (blockHeight + verticalSpacing));
                    tileEntities.Add(tile);
                    spriteRenderer.sortingOrder = levelData.height - j;

                    var block = tile.GetComponent<Block>();
                    if (block != null && block.type == BlockType.Collectable)
                    {
                        generatedCollectables += 1;
                    }
                }
            }
            var totalWidth = (levelData.width - 1) * (blockWidth + horizontalSpacing);
            var totalHeight = (levelData.height - 1) * (blockHeight + verticalSpacing);
            foreach (var block in tileEntities)
            {
                var newPos = block.transform.position;
                newPos.x -= totalWidth / 2;
                newPos.y += totalHeight / 2;
                newPos.y += levelLocation.position.y;
                block.transform.position = newPos;
                tilePositions.Add(newPos);
            }

            for (var j = 0; j < levelData.height; j++)
            {
                for (var i = 0; i < levelData.width; i++)
                {
                    var tileIndex = i + (j * levelData.width);
                    blockers.Add(null);
                }
            }

            OpenPopup<LevelGoalsPopup>("Popups/LevelGoalsPopupPrefab", popup => popup.SetGoals(levelData.goals));
        }

        public void StartGame()
        {
            currentLimit = levelData.limit;
            UpdateLimitText();
            if (levelData.limitType == LimitType.Moves)
            {
                gameUi.limitTitleText.text = "Moves";
            }

            gameUi.SetGoals(levelData.goals, goalPrefab);

            gameStarted = true;
        }

        private void EndGame()
        {
            gameFinished = true;
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
            }
        }

        /// <summary>
        /// Checks if the game has finished.
        /// </summary>
        private void CheckEndGame()
        {
            if (gameFinished)
            {
                return;
            }

            var goalsComplete = true;
            foreach (var goal in levelData.goals)
            {
                if (!goal.IsComplete(gameState))
                {
                    goalsComplete = false;
                    break;
                }
            }

            if (currentLimit == 0)
            {
                EndGame();
            }

            if (goalsComplete)
            {
                EndGame();

                var nextLevel = PlayerPrefs.GetInt("next_level");
                if (nextLevel == 0)
                {
                    nextLevel = 1;
                }
                if (levelData.id == nextLevel)
                {
                    PlayerPrefs.SetInt("next_level", levelData.id + 1);
                    GameManager.Instance.unlockedNextLevel = true;
                }
                else
                {
                    GameManager.Instance.unlockedNextLevel = false;
                }
                StartCoroutine(OpenWinPopupAsync());
            }
            else
            {
                if (gameFinished)
                {
                    OpenLosePopup();
                }
            }
        }

        private IEnumerator OpenWinPopupAsync()
        {
            yield return new WaitForSeconds(endGamePopupDelay);
            OpenWinPopup();
        }

        private void OpenWinPopup()
        {
            OpenPopup<WinPopup>("Popups/WinPopupPrefab", popup =>
            {
                popup.SetLevel(levelData.id);
                popup.SetGoals(gameUi.goalUi.group.gameObject);
            });
        }

        public void OpenLosePopup()
        {
            OpenPopup<LosePopup>("Popups/LosePopupPrefab", popup =>
            {
                popup.SetLevel(levelData.id);
                popup.SetGoals(gameUi.goalUi.group.gameObject);
            });
        }

        /// <summary>
        /// Continues the current game with additional moves/time.
        /// </summary>
        public void Continue()
        {
            gameFinished = false;
        }
    }
}
