using System.IO;
using UnityEngine;

namespace ShapezShifter.Textures
{
    public static class FileTextureLoader
    {
        public static Texture2D LoadTexture(string path)
        {
            byte[] data = File.ReadAllBytes(path);

            Texture2D texture = new(width: 2, height: 2);
            texture.LoadImage(data);
            return texture;
        }

        public static Sprite LoadTextureAsSprite(string path, out Texture2D texture)
        {
            byte[] data = File.ReadAllBytes(path);

            texture = new Texture2D(width: 2, height: 2);
            texture.LoadImage(data);
            return Sprite.Create(
                texture: texture,
                rect: new Rect(x: 0, y: 0, width: texture.width, height: texture.height),
                pivot: new Vector2(x: 0.5f, y: 0.5f));
        }
    }
}
