using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    public class Sprite
    {
        private Texture2D texture;
        public Vector2 position;
        public Color color;
        public float rotation;
        protected float layer;
        public bool centerOrigin;

        public Sprite(Vector2 position, float layer)
        {
            this.position = position;
            this.layer = layer;

            rotation = 0;
            centerOrigin = false;

            color = Color.White;
        }

        public virtual void Load(Texture2D texture)
        {
            this.texture = texture;
        }

        public void Update(Vector2 position)
        {
            this.position = position;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (centerOrigin)
                spriteBatch.Draw(texture, position, null, color, rotation, new Vector2(texture.Width/2, texture.Height/2), 1, SpriteEffects.None, layer);
            else
                spriteBatch.Draw(texture, position, null, color, rotation, Vector2.Zero, 1, SpriteEffects.None, layer);
        }
    }
}
