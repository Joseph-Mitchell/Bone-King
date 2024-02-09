using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Scores
    {
        Sprite sprite;

        float frameTimer;
        public bool active;

        const float ACTIVETIME = 60;
        public Scores(Texture2D texture, int x, int y)
        {
            sprite = new Sprite(new Vector2(x, y), 1)
            {
                centerOrigin = true
            };

            active = true;
            frameTimer = ACTIVETIME;

            sprite.Load(texture);
        }

        public void Update()
        {
            if (frameTimer > 0)
                frameTimer -= 1;
            else
                active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
