using BepInEx;
using BepInEx.Logging;
using RWCustom;
using System.Security.Permissions;
using UnityEngine;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace PCCodeStuff;

[BepInPlugin("alduris.pc", "PC Code Stuff", "0.1.0")]
sealed class Plugin : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    bool IsInit;

    public static PlacedObject.Type PCPlayerSensitiveLightSource;

    public void OnEnable()
    {
        Logger = base.Logger;
        On.RainWorld.OnModsInit += OnModsInit;
    }

    private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (IsInit) return;
        IsInit = true;

        PCPlayerSensitiveLightSource = new(nameof(PCPlayerSensitiveLightSource), true);

        On.Room.Loaded += Room_Loaded;
        On.PlacedObject.GenerateEmptyData += PlacedObject_GenerateEmptyData;
        On.DevInterface.ObjectsPage.CreateObjRep += ObjectsPage_CreateObjRep;

        Logger.LogDebug("The caverns are pipelining.");
    }

    private void Room_Loaded(On.Room.orig_Loaded orig, Room self)
    {
        orig(self);
        foreach (var obj in self.roomSettings.placedObjects)
        {
            if (obj.type == PCPlayerSensitiveLightSource)
            {
                var data = obj.data as PlayerSensitiveLightSourceData;
                self.AddObject(new PlayerSensitiveLightSource(obj.pos, data.Rad, data.DetectRad, data.minStrength, data.maxStrength, data.fadeSpeed, data.colorType.index - 2) { colorDirty = true, po = obj });
            }
        }
    }

    private void PlacedObject_GenerateEmptyData(On.PlacedObject.orig_GenerateEmptyData orig, PlacedObject self)
    {
        if (self.type == PCPlayerSensitiveLightSource)
        {
            self.data = new PlayerSensitiveLightSourceData(self);
        }
        else
        {
            orig(self);
        }
    }

    private void ObjectsPage_CreateObjRep(On.DevInterface.ObjectsPage.orig_CreateObjRep orig, DevInterface.ObjectsPage self, PlacedObject.Type tp, PlacedObject pObj)
    {
        if (pObj == null)
        {
            pObj = new PlacedObject(tp, null)
            {
                pos = self.owner.room.game.cameras[0].pos + Vector2.Lerp(self.owner.mousePos, new Vector2(-683f, 384f), 0.25f) + Custom.DegToVec(Random.value * 360f) * 0.2f
            };
            self.RoomSettings.placedObjects.Add(pObj);
        }

        if (tp == PCPlayerSensitiveLightSource)
        {
            var rep = new PlayerSensitiveLightSourceRepresentation(self.owner, tp.ToString() + "_Rep", self, pObj, tp.ToString());
            self.tempNodes.Add(rep);
            self.subNodes.Add(rep);
        }
        else
        {
            orig(self, tp, pObj);
        }
    }
}
