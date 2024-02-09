using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Button
    {
        Sprite sprite;

        public ButtonEffect effect;

        public bool hovered, active;

        public Button(int x, int y, ButtonEffect effect)
        {
            active = true;
            sprite = new Sprite(new Vector2(x, y), 1)
            {
                centerOrigin = true
            };
            this.effect = effect;
        }

        public void Load(Texture2D texture)
        {
            sprite.Load(texture);
        }

        public void Update() 
        {
            if (!active)
                sprite.color = Color.White * 0.5f;
            else if (hovered)
                sprite.color = Color.White;
            else
                sprite.color = Color.Gray;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
