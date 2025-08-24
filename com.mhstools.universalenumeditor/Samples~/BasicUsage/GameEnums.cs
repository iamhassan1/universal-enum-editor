using UnityEngine;

namespace MHSTools.UniversalEnumEditor.Samples
{
    /// <summary>
    /// Sample script demonstrating game-related enum definitions.
    /// Shows more complex enum scenarios and best practices.
    /// </summary>
    public class GameEnums : MonoBehaviour
    {
        [Header("Game State Enums")]
        
        /// <summary>
        /// Main game states
        /// </summary>
        public enum GameState
        {
            MainMenu,
            Loading,
            Playing,
            Paused,
            GameOver,
            Victory,
            Settings
        }

        /// <summary>
        /// Player states during gameplay
        /// </summary>
        public enum PlayerState
        {
            Idle,
            Walking,
            Running,
            Jumping,
            Falling,
            Attacking,
            Defending,
            Dead
        }

        /// <summary>
        /// Item types in the game
        /// </summary>
        public enum ItemType
        {
            Weapon,
            Armor,
            Consumable,
            Key,
            Currency,
            Quest,
            Material
        }

        /// <summary>
        /// Difficulty levels
        /// </summary>
        public enum Difficulty
        {
            Easy,
            Normal,
            Hard,
            Expert,
            Nightmare
        }

        /// <summary>
        /// Audio categories for sound management
        /// </summary>
        public enum AudioCategory
        {
            Music,
            SFX,
            Voice,
            Ambient,
            UI
        }

        // Example usage
        [SerializeField] private GameState currentGameState = GameState.MainMenu;
        [SerializeField] private PlayerState currentPlayerState = PlayerState.Idle;
        [SerializeField] private ItemType selectedItemType = ItemType.Weapon;
        [SerializeField] private Difficulty currentDifficulty = Difficulty.Normal;
        [SerializeField] private AudioCategory currentAudioCategory = AudioCategory.Music;

        private void Start()
        {
            LogCurrentStates();
        }

        private void LogCurrentStates()
        {
            Debug.Log($"Game State: {currentGameState}");
            Debug.Log($"Player State: {currentPlayerState}");
            Debug.Log($"Selected Item: {selectedItemType}");
            Debug.Log($"Difficulty: {currentDifficulty}");
            Debug.Log($"Audio Category: {currentAudioCategory}");
        }

        /// <summary>
        /// Example method showing enum state changes
        /// </summary>
        public void ChangeGameState(GameState newState)
        {
            currentGameState = newState;
            Debug.Log($"Game state changed to: {currentGameState}");
            
            // Example of enum-based logic
            switch (currentGameState)
            {
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.GameOver:
                    // Handle game over logic
                    break;
            }
        }

        /// <summary>
        /// Example method showing enum validation
        /// </summary>
        public bool IsValidItemType(ItemType itemType)
        {
            return itemType != ItemType.None; // Assuming None is not a valid option
        }
    }
}
