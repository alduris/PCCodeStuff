using System;
using DevInterface;
using RWCustom;
using UnityEngine;

namespace PCCodeStuff
{
    public class PlayerSensitiveLightSourceRepresentation : PlacedObjectRepresentation
    {
        protected PlayerSensitiveLightSource light;

        protected Handle radHandle;
        protected FSprite radCircle;
        protected FSprite radLine;
        
        protected Handle detectRadHandle;
        protected FSprite detectRadCircle;
        protected FSprite detectRadLine;

        protected PSLSControlPanel panel;
        protected FSprite panelLine;
        protected PlayerSensitiveLightSourceData data;

        public PlayerSensitiveLightSourceRepresentation(DevUI owner, string IDstring, DevUINode parentNode, PlacedObject pObj, string name) : base(owner, IDstring, parentNode, pObj, name)
        {
            data = pObj.data as PlayerSensitiveLightSourceData;

            // Initialize UI stuff
            radHandle = new Handle(owner, "Rad_Handle", this, new Vector2(0f, 100f));
            detectRadHandle = new Handle(owner, "DetRad_Handle", this, new Vector2(0f, 160f));
            subNodes.Add(radHandle);
            subNodes.Add(detectRadHandle);

            radCircle = new FSprite("Futile_White", true)
            {
                shader = owner.room.game.rainWorld.Shaders["VectorCircle"]
            };
            radLine = new FSprite("pixel", true)
            {
                anchorY = 0f
            };
            detectRadCircle = new FSprite("Futile_White", true)
            {
                shader = owner.room.game.rainWorld.Shaders["VectorCircle"]
            };
            detectRadLine = new FSprite("pixel", true)
            {
                anchorY = 0f
            };
            panelLine = new FSprite("pixel", true)
            {
                anchorY = 0f
            };
            fSprites.Add(radCircle);
            fSprites.Add(radLine);
            fSprites.Add(detectRadCircle);
            fSprites.Add(detectRadLine);
            fSprites.Add(panelLine);

            panel = new PSLSControlPanel(owner, "PSLS_Panel", this, data.panelPos);
            subNodes.Add(panel);

            // Find our light source
            foreach (var obj in owner.room.updateList)
            {
                if (obj is PlayerSensitiveLightSource psls && psls.po == pObj)
                {
                    light = psls;
                    break;
                }
            }

            if (light == null)
            {
                light = new PlayerSensitiveLightSource(pos, radHandle.pos.magnitude, detectRadHandle.pos.magnitude, 0f, 1f, -1);
                owner.room.AddObject(light);
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            // Update sprites
            radCircle.SetPosition(absPos + new Vector2(0.01f, 0.01f));
            radCircle.scale = radHandle.pos.magnitude / 8f;
            radCircle.alpha = 2f / radHandle.pos.magnitude;

            detectRadCircle.SetPosition(absPos + new Vector2(0.01f, 0.01f));
            detectRadCircle.scale = detectRadHandle.pos.magnitude / 8f;
            detectRadCircle.alpha = 1f / detectRadHandle.pos.magnitude;

            radLine.SetPosition(absPos + new Vector2(0.01f, 0.01f));
            radLine.scaleY = radHandle.pos.magnitude;
            radLine.rotation = Custom.AimFromOneVectorToAnother(absPos, radHandle.absPos);

            detectRadLine.SetPosition(absPos + new Vector2(0.01f, 0.01f));
            detectRadLine.scaleY = detectRadHandle.pos.magnitude;
            detectRadLine.rotation = Custom.AimFromOneVectorToAnother(absPos, detectRadHandle.absPos);
            
            panelLine.SetPosition(absPos + new Vector2(0.01f, 0.01f));
            panelLine.scaleY = radHandle.pos.magnitude;
            panelLine.rotation = Custom.AimFromOneVectorToAnother(absPos, panel.absPos);

            // Update data
            data.radHandlePos = radHandle.pos;
            data.detRadHandlePos = detectRadHandle.pos;
            data.panelPos = panel.pos;

            // Update light
            light.pos = pObj.pos;
            light.rad = data.Rad;
            light.detectRad = data.DetRad;
            light.effectColor = data.colorType.index - 1;
            light.minStrength = data.minStrength;
            light.maxStrength = data.maxStrength;
            light.Flat = data.flat;
        }

        public class PSLSControlPanel : Panel, IDevUISignals
        {
            public PSLSControlPanel(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos) : base(owner, IDstring, parentNode, pos, new Vector2(250f, 90f), "Player Sensitive Light Source")
            {
                // todo: add the controls here
            }

            public void Signal(DevUISignalType type, DevUINode sender, string message)
            {
                throw new NotImplementedException();
            }
        }
    }
}
