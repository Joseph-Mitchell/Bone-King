using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Sprite
    {
        private Texture2D texture;
        public Vector2 position;
        public Color color;
        protected float layer;
        public bool centerOrigin;

        public Sprite(int x, int y, float layer)
        {       
            position = new Vector2(x, y);
            this.layer = layer;

            color = Color.White;
        }

        public void Load(Texture2D texture)
        {
            this.texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (centerOrigin)
                spriteBatch.Draw(texture, position, null, color, 0, new Vector2(texture.Width/2, texture.Height/2), 1, SpriteEffects.None, layer);
            else
                spriteBatch.Draw(texture, position, null, color, 0, Vector2.Zero, 1, SpriteEffects.None, layer);
        }
    }
}
