using System.Collections.Generic;
using System.Linq;
using Assets.CustomAssets.Scripts.Anmation;
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player.Behaviour;
using UnityEngine;
using Cubiquity;

namespace Assets.CustomAssets.Scripts {
    public class RayExplorer : MonoBehaviour {
        public int maxDistance = 7;
        public int minDistance = 2;
        public Material groundGrave;
        public Material groundHeap;
        public Terrain terrain;

        public GameObject voxelTerrain;
        private ClickToCarveTerrainVolume clickToCarve;

        private TerrainData terrainData;
        private float[,] heightMap;
        private RaycastHit hit;
        private Ray ray;
        private float time_created = 0f;
        private const float startDelay = .25f;
        private const int mask = ~(1<<9 + 1<<8);

        public void Awake() {
            terrainData = terrain.terrainData;
        }

        public void Start () {
            heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
            time_created = Time.time;

            clickToCarve = voxelTerrain.GetComponent<ClickToCarveTerrainVolume>();
        }
        
        public void OnEnable() {
            time_created = Time.time;
        }

        private GameObject impacted;

        private void terrainAction() {
            if (hit.distance < minDistance) return;
            Vector3 p = terrain.gameObject.transform.InverseTransformPoint(hit.point);
            Vector2 vertex = new Vector2((p.x / terrainData.size.x) * terrainData.heightmapResolution, (p.z / terrainData.size.z) * terrainData.heightmapResolution);
            //Debug.Log("Res = " + terrainData.heightmapResolution + " width: " + terrainData.size.x + ", " + terrainData.size.z + " pos: " + (int)(p.x) + " " + (int)p.z + " vertex: " + (int)vertex.x + " " + (int) vertex.y);
            const int area_w = 4;
            const int area_h = 4;
            float[,] h = new float[area_w, area_h];
            for (int i = 0; i < area_w; ++i) {
                for (int j = 0; j < area_h; ++j) {
                    h[j, i] = 0.2f;//heightMap[(int)(vertex.y)+i, (int)(vertex.x)+j] - terrainData.heightmapResolution;
                                   //heightMap[(int)(vertex.y) + i, (int)(vertex.x) + j] = h[j, i];
                }
            }
            terrainData.SetHeights((int)(vertex.x) - area_w / 2, (int)(vertex.y) - area_h / 2, h);
            createHollowEntity(area_w+1, 2, area_h + 1);

        }

        private void groundGraveAction() {
            Player.Player.getInstance().behaviour = new DigBehaviour(gameObject, impacted);
        }

        private void groundHeapAction() {
            Debug.LogWarning("TODO: DIGBEHAVIOUR INVERSE");
            /*
            impacted.transform.localScale -= Vector3.one * .25f;
            impacted.transform.parent.GetChild(0).transform.localPosition += Vector3.up * .25f;
            */
        }

        private void coffinAction() {
            Player.Player.getInstance().behaviour = new CoffinDragBehaviour(gameObject, impacted);
        }

        public void Update () {
            if (Time.time - time_created < startDelay) return;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            PickSurfaceResult pickResult;
            bool hashit = Picking.PickSurface(voxelTerrain.GetComponent<TerrainVolume>(), ray, maxDistance, out pickResult);
            if (hashit) {
                UIUtils.infoInteractive.text = "dig terrain!";
            }
            if (Physics.Raycast(ray, out hit, maxDistance, mask)) {
                impacted = hit.collider.gameObject;
                Debug.DrawRay(Player.Player.getInstance().eyeSight.position, hit.point - this.gameObject.transform.position, Color.red);
                showInfoMsg(impacted);
                if (GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                    if (impacted.tag == "groundHeap") {
                        groundHeapAction();
                    }
                    else if (impacted.tag == "coffin")
                        coffinAction();
                    else {
                        if (hashit && !Player.Player.getInstance().digNewHolesDisabled) {
                            clickToCarve.doAction();
                            createHollowEntity(clickToCarve.rangeX + 1, clickToCarve.rangeY + 1, clickToCarve.rangeZ + 1);
                        }
                    }
                }
            }
            else {
                UIUtils.infoInteractive.text = "";
            }
        }

        private void createHollowEntity(int sizeX, int sizeY, int sizeZ) {
            GameObject parent = new GameObject("Grave");
            BoxCollider bc = parent.AddComponent<BoxCollider>();
            Vector3 v = new Vector3(sizeX*1.5f, sizeY*1.5f, sizeZ*1.5f);
            bc.size = v;
            bc.gameObject.layer = 9;
            bc.enabled = false;

            parent.transform.position = hit.point - new Vector3(sizeX/2f, 0, sizeZ/2f);
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            GameObject heap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            heap.tag = "groundHeap";
            plane.tag = "groundGrave";
            plane.transform.parent = parent.transform;
            plane.transform.localPosition = -Vector3.up * .25f;

            GameObject triggerDisableDigging = new GameObject("Disable digging area");
            SphereCollider sc = triggerDisableDigging.AddComponent<SphereCollider>();
            sc.radius = Mathf.Max(sizeX, sizeZ) * 2f;
            sc.isTrigger = true;
            triggerDisableDigging.transform.parent = parent.transform;
            triggerDisableDigging.transform.localPosition = Vector3.zero;
            triggerDisableDigging.AddComponent<trigger_disable_digging>();

            GameObject triggerThrowCoffin = new GameObject("Trigger throw coffin");
            bc = triggerThrowCoffin.AddComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.size = v * 1.25f;
            triggerThrowCoffin.transform.parent = parent.transform;
            triggerThrowCoffin.transform.localPosition = Vector3.zero;
            trigger_hollow_behaviours t = triggerThrowCoffin.AddComponent<trigger_hollow_behaviours>();
            t.curve = AnimationUtils.createThrowCoffinCurve();
            t.node1 = parent.transform;
            t.groundFloor = plane;

            heap.transform.parent = parent.transform;
            heap.transform.localPosition = Vector3.zero + sizeX / 2f * Vector3.right + Vector3.up;
            MeshRenderer mr = plane.GetComponent<MeshRenderer>();
            mr.material = groundGrave;

        }

        private void showInfoMsg(GameObject impacted) {

            if (impacted.tag == "terrain") {
                UIUtils.infoInteractive.text = "dig terrain!";
                Debug.Log("terrain!");
            }
            else if (impacted.tag == "groundGrave")
                UIUtils.infoInteractive.text = "dig plane!";
            else if (impacted.tag == "groundHeap")
                UIUtils.infoInteractive.text = "undig!";
            else if (impacted.tag == "coffin")
                UIUtils.infoInteractive.text = "drag coffin!";
            else {
                UIUtils.infoInteractive.text = "";
            }
                        
        }

    }
}
