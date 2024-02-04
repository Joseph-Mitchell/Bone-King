using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Sprite
    {
        private Texture2D texture;
        public Vector2 position;
        protected float layer;

        public Sprite(Texture2D texture, int x, int y, float layer)
        {
            this.texture = texture;
            position = new Vector2(x, y);
            this.layer = layer;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, layer);
        }
    }
}
