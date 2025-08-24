using UnityEngine;

namespace MHSTools.UniversalEnumEditor.Samples
{
    /// <summary>
    /// Sample script demonstrating basic enum definitions.
    /// This script can be used with the Universal Enum Editor tool.
    /// </summary>
    public class BasicEnums : MonoBehaviour
    {
        [Header("Basic Enum Examples")]
        
        /// <summary>
        /// A simple enum for basic directions
        /// </summary>
        public enum Direction
        {
            North,
            South,
            East,
            West
        }

        /// <summary>
        /// An enum for basic colors
        /// </summary>
        public enum Color
        {
            Red,
            Green,
            Blue,
            Yellow,
            Black,
            White
        }

        /// <summary>
        /// An enum for basic shapes
        /// </summary>
        public enum Shape
        {
            Circle,
            Square,
            Triangle,
            Rectangle
        }

        // Example usage
        [SerializeField] private Direction currentDirection = Direction.North;
        [SerializeField] private Color currentColor = Color.Red;
        [SerializeField] private Shape currentShape = Shape.Circle;

        private void Start()
        {
            Debug.Log($"Current Direction: {currentDirection}");
            Debug.Log($"Current Color: {currentColor}");
            Debug.Log($"Current Shape: {currentShape}");
        }

        /// <summary>
        /// Example method showing enum usage
        /// </summary>
        public void ChangeDirection(Direction newDirection)
        {
            currentDirection = newDirection;
            Debug.Log($"Direction changed to: {currentDirection}");
        }
    }
}
