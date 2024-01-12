using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Scores
    {
        Texture2D texture;
        Vector2 position;

        float frameTimer;
        public bool active;

        const float ACTIVETIME = 60;
        public Scores(Texture2D texture, int x, int y)
        {
            this.texture = texture;
            position = new Vector2(x, y);

            active = true;
            frameTimer = ACTIVETIME;
        }

        public void Update()
        {
            if (frameTimer <= 0)
            {
                active = false;
            }
            else
            {
                frameTimer -= 1;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, null, Color.White, 0, new Vector2(texture.Bounds.Center.X, texture.Bounds.Center.Y), 1, SpriteEffects.None, 1);
        }
    }
}
