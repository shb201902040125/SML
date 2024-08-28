using Microsoft.Xna.Framework;
using SML.Common;
using Terraria;

namespace SML.Physics
{
    public static class PhysicsHelper
    {
        public static Collider GetCollider(this Player player)
        {
            Vector2 center = player.Center;
            Collider collider = new()
            {
                BindTarget = player,
                Vectors =
                [
                    player.Hitbox.TopLeft(),
                player.Hitbox.TopRight(),
                player.Hitbox.BottomRight(),
                player.Hitbox.BottomLeft()
                ],
                Transform = Matrix.CreateTranslation(-center.X, -center.Y, 0) *
                            Matrix.CreateRotationZ(player.fullRotation) *
                            Matrix.CreateTranslation(center.X, center.Y, 0)
            };
            return collider;
        }
        public static Collider GetCollider(this Item item)
        {
            if (Main.item[item.whoAmI] != item)
            {
                return new()
                {
                    BindTarget = item,
                    Vectors =
                    [
                        item.position
                    ],
                    Transform = Matrix.Identity
                };
            }
            Vector2 center = item.position + new Vector2(item.width / 2, item.height / 2);
            return new()
            {
                BindTarget = item,
                Vectors =
                [
                    item.position,
                item.position + new Vector2(item.width, 0),
                item.position + new Vector2(item.width, item.height),
                item.position + new Vector2(0, item.height)
                ],
                Transform = Matrix.Identity
            };
        }
        public static Collider GetCollider(this NPC npc)
        {
            Vector2 center = npc.Center;
            Collider collider = new()
            {
                BindTarget = npc,
                Vectors =
                [
                    npc.Hitbox.TopLeft(),
                npc.Hitbox.TopRight(),
                npc.Hitbox.BottomRight(),
                npc.Hitbox.BottomLeft()
                ],
                Transform = Matrix.CreateTranslation(-center.X, -center.Y, 0) *
                            Matrix.CreateRotationZ(npc.rotation) *
                            Matrix.CreateTranslation(center.X, center.Y, 0)
            };
            return collider;
        }
        public static Collider GetCollider(this Projectile projectile)
        {
            Vector2 center = projectile.Center;
            Collider collider = new()
            {
                BindTarget = projectile,
                Vectors =
                [
                    projectile.Hitbox.TopLeft(),
                projectile.Hitbox.TopRight(),
                projectile.Hitbox.BottomRight(),
                projectile.Hitbox.BottomLeft()
                ],
                Transform = Matrix.CreateTranslation(-center.X, -center.Y, 0) *
                            Matrix.CreateRotationZ(projectile.rotation) *
                            Matrix.CreateTranslation(center.X, center.Y, 0)
            };
            return collider;
        }
        public static Collider GetCollider(this Tile tile)
        {
            int hashCode = tile.GetHashCode();
            int y = hashCode / Main.tile.Width;
            int x = hashCode % Main.tile.Width;
            Vector2 leftTop = new Vector2(x, y) * 16;
            switch (tile.Slope)
            {
                default:
                case Terraria.ID.SlopeType.Solid:
                    {
                        return tile.IsHalfBlock
                            ? new()
                            {
                                BindTarget = tile,
                                Vectors =
                                [
                                    leftTop + new Vector2(0, 8),
                                    leftTop + new Vector2(16, 8),
                                    leftTop + new Vector2(16, 16),
                                    leftTop + new Vector2(0, 16)
                                ],
                                Transform = Matrix.Identity
                            }
                            : new()
                            {
                                BindTarget = tile,
                                Vectors =
                            [
                                leftTop,
                                leftTop + new Vector2(16, 0),
                                leftTop + new Vector2(16, 16),
                                leftTop + new Vector2(0, 16)
                            ],
                                Transform = Matrix.Identity
                            };
                    }
                //左下，没有右上
                case Terraria.ID.SlopeType.SlopeDownLeft:
                    {
                        return new()
                        {
                            BindTarget = tile,
                            Vectors =
                            [
                                leftTop,
                                leftTop + new Vector2(16, 16),
                                leftTop + new Vector2(0, 16)
                            ],
                            Transform = Matrix.Identity
                        };
                    }
                //右下，没有左上
                case Terraria.ID.SlopeType.SlopeDownRight:
                    {
                        return new()
                        {
                            BindTarget = tile,
                            Vectors =
                            [
                                leftTop + new Vector2(16, 0),
                                leftTop + new Vector2(16, 16),
                                leftTop + new Vector2(0, 16)
                            ],
                            Transform = Matrix.Identity
                        };
                    }
                //左上，没有右下
                case Terraria.ID.SlopeType.SlopeUpLeft:
                    {
                        return new()
                        {
                            BindTarget = tile,
                            Vectors =
                            [
                                leftTop,
                                leftTop + new Vector2(16, 0),
                                leftTop + new Vector2(0, 16)
                            ],
                            Transform = Matrix.Identity
                        };
                    }
                //右上，没有左下
                case Terraria.ID.SlopeType.SlopeUpRight:
                    {
                        return new()
                        {
                            BindTarget = tile,
                            Vectors =
                            [
                                leftTop,
                                leftTop + new Vector2(16, 0),
                                leftTop + new Vector2(16, 16)
                            ],
                            Transform = Matrix.Identity
                        };
                    }
            }
        }
        public static CacheBlock<Collider> ScanTileColliders(int x, int y, int width, int height)
        {
            CacheBlock<Collider> cacheBlock = new(width * height);
            cacheBlock.Rent(width * height, out CacheBlock<Collider>.Interface @interface);
            using (@interface)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        @interface[i + (j * width)] = Main.tile[x + i, y + j].GetCollider();
                    }
                }
            }
            return cacheBlock;
        }
    }

}