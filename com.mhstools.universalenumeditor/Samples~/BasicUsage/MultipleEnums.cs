using UnityEngine;

namespace MHSTools.UniversalEnumEditor.Samples
{
    /// <summary>
    /// Sample script demonstrating multiple enum definitions in a single file.
    /// This shows how the Universal Enum Editor handles scripts with multiple enums.
    /// </summary>
    public class MultipleEnums : MonoBehaviour
    {
        [Header("Character System Enums")]
        
        /// <summary>
        /// Character classes available in the game
        /// </summary>
        public enum CharacterClass
        {
            Warrior,
            Mage,
            Archer,
            Rogue,
            Cleric,
            Paladin,
            Druid,
            Warlock
        }

        /// <summary>
        /// Character races
        /// </summary>
        public enum CharacterRace
        {
            Human,
            Elf,
            Dwarf,
            Orc,
            Gnome,
            Halfling,
            Dragonborn,
            Tiefling
        }

        [Header("Combat System Enums")]
        
        /// <summary>
        /// Types of damage
        /// </summary>
        public enum DamageType
        {
            Physical,
            Fire,
            Ice,
            Lightning,
            Poison,
            Holy,
            Dark,
            Arcane
        }

        /// <summary>
        /// Combat actions
        /// </summary>
        public enum CombatAction
        {
            Attack,
            Defend,
            Cast,
            Use,
            Move,
            Wait,
            Flee
        }

        [Header("Quest System Enums")]
        
        /// <summary>
        /// Quest types
        /// </summary>
        public enum QuestType
        {
            Main,
            Side,
            Daily,
            Weekly,
            Event,
            Hidden,
            Tutorial
        }

        /// <summary>
        /// Quest status
        /// </summary>
        public enum QuestStatus
        {
            NotStarted,
            InProgress,
            Completed,
            Failed,
            Abandoned
        }

        [Header("UI System Enums")]
        
        /// <summary>
        /// UI panel types
        /// </summary>
        public enum UIPanelType
        {
            MainMenu,
            HUD,
            Inventory,
            Character,
            Skills,
            Map,
            Settings,
            Pause
        }

        /// <summary>
        /// Button states
        /// </summary>
        public enum ButtonState
        {
            Normal,
            Hovered,
            Pressed,
            Disabled,
            Selected
        }

        // Example usage
        [SerializeField] private CharacterClass playerClass = CharacterClass.Warrior;
        [SerializeField] private CharacterRace playerRace = CharacterRace.Human;
        [SerializeField] private DamageType preferredDamageType = DamageType.Physical;
        [SerializeField] private QuestType currentQuestType = QuestType.Main;
        [SerializeField] private UIPanelType activePanel = UIPanelType.HUD;

        private void Start()
        {
            LogAllEnums();
        }

        private void LogAllEnums()
        {
            Debug.Log($"Player Class: {playerClass}");
            Debug.Log($"Player Race: {playerRace}");
            Debug.Log($"Preferred Damage: {preferredDamageType}");
            Debug.Log($"Current Quest: {currentQuestType}");
            Debug.Log($"Active Panel: {activePanel}");
        }

        /// <summary>
        /// Example method showing enum combinations
        /// </summary>
        public void CreateCharacter(CharacterClass characterClass, CharacterRace characterRace)
        {
            playerClass = characterClass;
            playerRace = characterRace;
            
            Debug.Log($"Created {characterRace} {characterClass}");
            
            // Example of enum-based logic
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    preferredDamageType = DamageType.Physical;
                    break;
                case CharacterClass.Mage:
                    preferredDamageType = DamageType.Arcane;
                    break;
                case CharacterClass.Cleric:
                    preferredDamageType = DamageType.Holy;
                    break;
            }
        }

        /// <summary>
        /// Example method showing enum validation
        /// </summary>
        public bool CanUseAction(CombatAction action, CharacterClass characterClass)
        {
            switch (action)
            {
                case CombatAction.Cast:
                    return characterClass == CharacterClass.Mage || 
                           characterClass == CharacterClass.Cleric || 
                           characterClass == CharacterClass.Warlock;
                case CombatAction.Attack:
                    return true; // All classes can attack
                default:
                    return true;
            }
        }
    }
}
