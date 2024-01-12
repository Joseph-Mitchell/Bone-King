using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Axe
    {
        Texture2D texture;
        Vector2 position;
        public Rectangle collision;

        public bool activc;

        public Axe (Texture2D texture, int x, int y)
        {
            this.texture = texture;
            position = new Vector2(x, y);
            collision = new Rectangle(x, y, 18, 20);
        }

        public void Draw(SpriteBatch sb)
        {
            if (activc)
            {
                sb.Draw(texture, position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
            }
        }
#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D texture)
        {
            if (activc)
            {
                sb.Draw(texture, collision, null, Color.Yellow, 0, Vector2.Zero, SpriteEffects.None, 1);
            }
        }
#endif
    }
}
