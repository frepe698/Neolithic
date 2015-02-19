
public class DamageType {

	public const int COMBAT = 0;
	public const int TREE = 1;
	public const int STONE = 2;

    public enum dtype
    {
        Combat, Tree, Stone
    }

    public static dtype intToDamageType(int type)
    {
        switch (type)
        {
            case COMBAT:
                return dtype.Combat;
            case TREE:
                return dtype.Tree;
            case STONE:
                return dtype.Stone;
            default:
                return dtype.Combat;
        }
    }

    public static int damageTypeToInt(dtype type)
    {
        switch (type)
        {
            case dtype.Combat:
                return COMBAT;
            case dtype.Tree:
                return TREE;
            case dtype.Stone:
                return STONE;
            default:
                return COMBAT;
        }
    }
}
