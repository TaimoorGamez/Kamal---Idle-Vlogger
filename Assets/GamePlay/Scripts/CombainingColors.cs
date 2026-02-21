using UnityEngine;

namespace Core.GamePlay.Coloring
{
    public class CombainingColors : MonoBehaviour
    {
        public Texture2D[] coloredParts; // Array of all colored part textures
        public Texture2D detailImage; // The 512x512 detail image
        public RenderTexture renderTexture; // The RenderTexture for final result

        public void CombineAndSave()
        {
            // Create a temporary 512x512 texture for combining
            Texture2D finalTexture = new Texture2D(512, 512);

            // Set render texture as active
            RenderTexture.active = renderTexture;

            // Combine all parts
            foreach (var part in coloredParts)
            {
                // Example: calculate position and size of each part
                // Use appropriate scaling and positioning logic for your parts
                Vector2 position = GetPartPosition(part); // Calculate position of part on 512x512 canvas
                finalTexture.SetPixels((int)position.x, (int)position.y, part.width, part.height, part.GetPixels());
            }

            // Add the detail layer on top
            finalTexture.SetPixels(0, 0, 512, 512, detailImage.GetPixels());

            // Apply final texture and save it
            finalTexture.Apply();

            // Read from the render texture and save as a PNG
            SaveFinalImage(finalTexture);
        }

        private void SaveFinalImage(Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/FinalImage.png", bytes);
        }

        private Vector2 GetPartPosition(Texture2D part)
        {
            // Calculate part position based on part's original scale and transform
            return new Vector2(0, 0); // Example position logic
        }
    }
}
