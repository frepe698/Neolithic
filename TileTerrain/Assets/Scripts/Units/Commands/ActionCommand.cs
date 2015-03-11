using UnityEngine;
using System.Collections;

public class ActionCommand : Command {

    private WarpObject warpObject;
    private bool hasWarped;
    private Vector2 warpPosition;

    public ActionCommand (Unit unit, WarpObject warpObject)
        : base(unit)
	{
        this.warpObject = warpObject;
        this.destination = this.warpPosition = warpObject.get2DPos();
	}

    public override void start()
    {
        unit.setPath(destination);
    }

    public override void update()
    {
        if (Vector2.Distance(unit.get2DPos(), destination) < warpObject.getActionRadius())
        {

            unit.setMoving(false);
            WarpObject wObject = World.tileMap.getTile(new Vector2i(warpObject.get2DPos())).getTileObject() as WarpObject;
            if (wObject != null)
            {
                unit.warp(warpObject.getDestination());

                //unit.setAnimationRestart(unit.getAttackAnim(resObject.getDamageType()), unit.getAttackSpeed());
            }
            setCompleted();



        }
    }
}
